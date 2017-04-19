using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.ServiceModel;


namespace BlackListCompanyConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Count() == 0)
            {
                Console.WriteLine("Параметры:");
                Console.WriteLine("/s - сохранить файл запроса Company.XML");
                Console.WriteLine("/u - отправить файлы запроса Company.XML и Company.XML.sig");
                Console.WriteLine("В файле BlackListCompanyConsole.code сохраняется код запроса и в BlackListCompanyConsole.log журнал");
                Console.WriteLine("/d - проверить статус по коду из BlackListCompanyConsole.code и сохранить выгрузку в register.zip");
                Console.WriteLine("/p - преобразовать dump.xml в rules.txt");
                Console.WriteLine("/m - загрузить rules.txt в микротик");
            } else
            {
                if (args[0] == "/s") ProcedureS();
                if (args[0] == "/u") ProcedureU();
                if (args[0] == "/d") ProcedureD();
                if (args[0] == "/p") ProcedureP();
                if (args[0] == "/m") ProcedureM();
            }
        }
        public static void Logging(string text)
        {
            string MyFileName = @"BlackListCompanyConsole.log";
            string dtstr;
            FileStream fs = new FileStream(MyFileName, FileMode.Append);
            StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("Windows-1251"));
            dtstr = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            sw.WriteLine(dtstr + "   " + text);
            sw.Close();
            fs.Close();                
        }
        public static void ProcedureS()
        {
            try
            {
                string MyFileName = @"Company.XML";
                string dtstr;
                FileStream fs = new FileStream(MyFileName, FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("Windows-1251"));
                sw.WriteLine("<?xml version=\"1.0\" encoding=\"windows-1251\"?>");
                sw.WriteLine("<request>");
                dtstr = DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss.fffzzz");
                sw.WriteLine("<requestTime>" + dtstr + "</requestTime>");
                sw.WriteLine("<operatorName>Интеллсити</operatorName>");
                sw.WriteLine("<inn>7706179063</inn>");
                sw.WriteLine("<ogrn>1037700109611</ogrn>");
                sw.WriteLine("<email>sysadmin@krt.ru</email>");
                sw.WriteLine("</request>");
                sw.Close();
                fs.Close();                
                Logging("Создан Company.XML");
            }
            catch (Exception)
            {
                Logging("Ошибка при создании Company.XML");
            }
        }

        public static void ProcedureU()
        {
            try
            {
                String requestFile = @"Company.XML";
                String signatureFile = @"Company.XML.sig";
                String resultComment;
                String code;
                Int64 ldd = ZapretSOAPServices.LastDumpDate();
                Int64 LastDumpDate = 0;
                DateTime LastDumpDt = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(ldd / 1000);
                Logging("getLastDumpDate (дата последнего дампа): " + LastDumpDt.ToString("yyyy-MM-dd HH:mm:ss"));
                if (ldd != LastDumpDate)
                {
                    LastDumpDate = ldd;
                    Logging("Отправляю файлы :" + requestFile + " и " + signatureFile + "\n");

                    if (ZapretSOAPServices.SendRequest(out resultComment, out code, File.ReadAllBytes(requestFile), File.ReadAllBytes(signatureFile)))
                    {
                        Logging("Ответ сервера на SendRequest:   resultComment = " + resultComment + ", code = " + code);
                        string MyFileName = @"BlackListCompanyConsole.code";
                        FileStream fs = new FileStream(MyFileName, FileMode.OpenOrCreate);
                        StreamWriter sw = new StreamWriter(fs, Encoding.GetEncoding("Windows-1251"));
                        sw.Write(code);
                        sw.Close();
                        fs.Close();                
                    }
                }          
            }
            catch (Exception)
            {
                Logging("Ошибка при отправке запроса");
            }
        }


        public static void ProcedureD()
        {
            try
            {
                string code = System.IO.File.ReadAllText(@"BlackListCompanyConsole.code");
                Byte[] registerZipArchive;
                String registerZipArchivePath = @"register.zip";
                String resultComment;
                Logging("Проверяем статус по коду: " + code);
                ZapretSOAPServices.GetResult(out resultComment, out registerZipArchive, code);
                Logging("Ответ сервера на GetResult: resultComment = " + resultComment);
                if (resultComment != "запрос обрабатывается")
                {
                    File.WriteAllBytes(registerZipArchivePath, registerZipArchive);
                    Logging("Файл register.zip сохранен");
                }
            }
            catch (Exception)
            {
                Logging("Ошибка при получении ответа от сервера");
            }
        }

        public static void ProcedureP()
        {
            try
            {
                Byte[] registerZipArchive;
                RegisterDump dump;
                registerZipArchive = null;
                if (ParseRegisterDump.Parse(out dump, registerZipArchive))
                {
                    if (FilterL7RouterOS.AddFilterL7(dump))
                    {
                        Logging("Правила преобразованы из dump.xml в rules.txt");
                    }
                }
            }
            catch (Exception)
            {
                Logging("Ошибка при преобразовании из dump.xml в rules.txt");
            }
        }

        public static void ProcedureM()
        {
            try
            {
                MK mikrotik = new MK("192.168.88.1");
                //string mk_output;
                Int64 rulenumber = 0;
                if (mikrotik.Login("admin", ""))
                {
                    Logging("Подключился к Микротик");
                    string MyFileName = @"rules.txt";
                    string line = "";
                    using (StreamReader sr = new StreamReader(MyFileName))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            mikrotik.Send(line, true);
                            Logging("Правило " + line);
                            mikrotik.Read();
                            //Logging("Загрузка правила " + rulenumber + " в Микротик: " + mk_output);
                            rulenumber++;
                        }
                    }
                }
                mikrotik.Close();
            }
            catch (Exception)
            {
                Logging("Ошибка при загрузке в Микротик");
            }
        }


    }

    public class ZapretSOAPServices
    {
        public static Int64 LastDumpDate()
        {
            Int64 lastDumpDate = 0;

            BasicHttpBinding HttpBinding = new BasicHttpBinding();
            HttpBinding.MaxReceivedMessageSize = 65536 * 2;

            using (ChannelFactory<ServiceReference.OperatorRequestPortType> scf = new ChannelFactory<ServiceReference.OperatorRequestPortType>(
                HttpBinding, new EndpointAddress("http://vigruzki.rkn.gov.ru/services/OperatorRequest/")))
            {
                ServiceReference.OperatorRequestPortType channel = scf.CreateChannel();
                ServiceReference.getLastDumpDateResponse glddr = channel.getLastDumpDate(new ServiceReference.getLastDumpDateRequest());
                lastDumpDate = glddr.lastDumpDate;
            }

            return lastDumpDate;
        }

        public static Boolean SendRequest(out String resultComment, out String code, Byte[] requestFile, Byte[] signatureFile)
        {
            Boolean result = false;
            code = null;

            BasicHttpBinding HttpBinding = new BasicHttpBinding();
            HttpBinding.MaxReceivedMessageSize = 65536 * 2;

            using (ChannelFactory<ServiceReference.OperatorRequestPortType> scf = new ChannelFactory<ServiceReference.OperatorRequestPortType>(
                HttpBinding, new EndpointAddress("http://vigruzki.rkn.gov.ru/services/OperatorRequest/")))
            {
                ServiceReference.OperatorRequestPortType channel = scf.CreateChannel();
                ServiceReference.sendRequestRequestBody srrb = new ServiceReference.sendRequestRequestBody();

                srrb.requestFile = requestFile;
                srrb.signatureFile = signatureFile;

                ServiceReference.sendRequestResponse srr = channel.sendRequest(new ServiceReference.sendRequestRequest(srrb));

                resultComment = srr.Body.resultComment;

                if (result = srr.Body.result)
                {
                    code = srr.Body.code;
                }
            }

            return result;
        }

        public static Boolean GetResult(out String resultComment, out Byte[] registerZipArchive, String code)
        {
            Boolean result = false;
            registerZipArchive = null;

            BasicHttpBinding HttpBinding = new BasicHttpBinding();
            HttpBinding.MaxReceivedMessageSize = 65536 * 2;

            using (ChannelFactory<ServiceReference.OperatorRequestPortType> scf = new ChannelFactory<ServiceReference.OperatorRequestPortType>(
                HttpBinding, new EndpointAddress("http://vigruzki.rkn.gov.ru/services/OperatorRequest/")))
            {
                ServiceReference.OperatorRequestPortType channel = scf.CreateChannel();
                ServiceReference.getResultRequestBody grrb = new ServiceReference.getResultRequestBody();

                grrb.code = code;

                ServiceReference.getResultResponse grr = channel.getResult(new ServiceReference.getResultRequest(grrb));

                resultComment = grr.Body.resultComment;

                if (result = grr.Body.result)
                {
                    registerZipArchive = grr.Body.registerZipArchive;
                }
            }

            return result;
        }
    }    


    public class ParseRegisterDump
    {
        public static Boolean Parse(out RegisterDump Register, Byte[] registerZipArchive)
        {
            Boolean ret = true;
            Register = new RegisterDump();
            String dumpfile = @"dump.xml";
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.Load(dumpfile);
                Register.UpdateTime = xmlDoc.GetElementsByTagName("reg:register")[0].Attributes.GetNamedItem("updateTime").InnerText;
                XmlNodeList content = xmlDoc.GetElementsByTagName("content");
                for (int i = 0; i < content.Count; i++)
                {
                    ItemRegisterDump item = new ItemRegisterDump();
                    item.id = content[i].Attributes.GetNamedItem("id").InnerText;
                    item.includeTime = content[i].Attributes.GetNamedItem("includeTime").InnerText;
                    foreach (XmlNode node in content[i].ChildNodes)
                    {
                        switch (node.Name)
                        {
                            case "decision":
                                item.date = node.Attributes.GetNamedItem("date").InnerText;
                                item.number = node.Attributes.GetNamedItem("number").InnerText;
                                item.org = node.Attributes.GetNamedItem("org").InnerText;
                                break;
                            case "url":
                                item.url.Add(node.InnerText);
                                break;
                            case "domain":
                                item.domain.Add(node.InnerText);
                                break;
                            case "ip":
                                item.ip.Add(node.InnerText);
                                break;
                        }
                    }
                    Register.Items.Add(item);
                }
            }
            catch (Exception)
            {
                ret = false;
            }
            return ret;           
        }             
    }


    public class RegisterDump
    {
        /*
         * <reg:register updateTime="2013-07-15T10:05:00+04:00" xmlns:reg="http://rsoc.ru" xmlns:tns="http://rsoc.ru">
         *    <content></content>
         *    <content></content>
         *       ...
         *    <content></content>
         * </reg:register>
         */

        public List<ItemRegisterDump> Items { get; set; }
        public String UpdateTime { get; set; }

        public RegisterDump()
        {
            this.Items = new List<ItemRegisterDump>();
            this.UpdateTime = String.Empty;
        }

        public RegisterDump(String UpdateTime, List<ItemRegisterDump> Items)
        {
            this.Items = Items;
            this.UpdateTime = UpdateTime;
        }
    }

    public class ItemRegisterDump
    {
        /*
         * <content id="60" includeTime="2013-01-12T16:33:38">
         *    <decision date="2013-11-03" number="МИ-6" org="РосКосМопсПопс"/>
         *    <url><![CDATA[http://habrahabr.ru/post/187574/]]></url>
         *    <ip>123.45.67.89</ip>
         * </content>
         * <content id="69" includeTime="2013-05-12T12:43:34">
         *    <decision date="2013-10-02" number="ФБИ" org="СФНК"/>
         *    <domain><![CDATA[chelaxe.ru]]></domain>
         *    <ip>123.45.67.89</ip>
         *    <ip>87.65.43.210</ip>
         * </content>
         */

        public String id { get; set; }
        public String includeTime { get; set; }

        public String date { get; set; }
        public String number { get; set; }
        public String org { get; set; }

        public List<String> url { get; set; }
        public List<String> domain { get; set; }
        public List<String> ip { get; set; }

        public ItemRegisterDump()
        {
            id = String.Empty;
            includeTime = String.Empty;

            date = String.Empty;
            number = String.Empty;
            org = String.Empty;

            url = new List<string>();
            domain = new List<string>();
            ip = new List<string>();
        }
    }



    public class FilterL7RouterOS
    {
        public static Boolean AddFilterL7(RegisterDump dump)
        {
            Boolean ret = true;
            try
            {
                string MyFileName = @"rules.txt";
                string s;
                File.Delete(MyFileName);

                FileStream fs = new FileStream(MyFileName, FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fs);
                s = @"/system script add name=cleaner source=""/ip firewall layer7-protocol remove [find comment=register]\n/ip firewall filter remove [find comment=register]""";
                sw.WriteLine(s);
                s = @"/system script run number=cleaner";
                sw.WriteLine(s);
                
                /* Cleaner
                 * /ip firewall layer7-protocol remove [find comment=register]
                 * /ip firewall filter remove [find comment=register]
                 */

                foreach (ItemRegisterDump item in dump.Items)
                {
                    for (Int32 i = 0; i < item.domain.Count; i++)
                    {
                        s = @"/ip firewall layer7-protocol add comment=register name=" + item.id + "_" + i + @" regexp=""^.+(" + item.domain[i] + @").*\$""";
                        sw.WriteLine(s);
                        s = @"/ip firewall filter add comment=register action=drop chain=forward disabled=no dst-port=80 layer7-protocol=" + item.id + "_" + i + " protocol=tcp src-address= out-interface=ether1-gateway";
                        sw.WriteLine(s);
                    }
                }
                sw.Close();
                fs.Close();
            }
            catch (Exception)
            {
                ret = false;
            }

            return ret;
        }
    }



}
