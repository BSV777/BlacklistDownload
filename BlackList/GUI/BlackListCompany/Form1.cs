using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading;
using System.Diagnostics;
using System.ServiceModel;




namespace BlackListCompany
{
    public partial class Form1 : Form
    {
        String code;

        public Form1()
        {
            InitializeComponent();
        }

        private void btParse_Click(object sender, EventArgs e)
        {
            //String requestFile;
//            String signatureFile;

            Byte[] registerZipArchive;

            RegisterDump dump;
            //String resultComment;
            //String code;
            Settings options;

            
         
        options = new Settings();

        registerZipArchive = null;

            if (ParseRegisterDump.Parse(out dump, registerZipArchive, options.NameEventLog))
        
            {
                //EventLog.WriteEntry(options.NameEventLog, "База разобранна успешно", EventLogEntryType.Information, 100, 006);

                tb1.AppendText("Правила преобразованы\n");

                
                
                if (FilterL7RouterOS.AddFilterL7(options.ip, options.username, options.password, dump, options.SRCAddress, options.NameEventLog))
                {
                   // EventLog.WriteEntry(options.NameEventLog, "Правила добавлены успешно", EventLogEntryType.Information, 100, 007);
                }
                 
            }                        

        }

        private void button1_Click(object sender, EventArgs e)
        {
 
        }

        private void btMakeXML_Click(object sender, EventArgs e)
        {
            try
            {
                string MyFileName = @"Company.XML";
                string dtstr;

                FileStream fs = new FileStream(MyFileName, FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fs,Encoding.GetEncoding("Windows-1251"));

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
                tb1.AppendText("Создан Company.XML\n");
                
            }
            catch (Exception)
            {
             
            }
        }

        private void btSubscribe_Click(object sender, EventArgs e)
        {
            //Boolean ret = true;

            String OpenSSLPath = @"C:\OpenSSL\bin";
            String RequestPath = @"Company.XML";
            String SignRequestPath = @"Company.XML.sig";
            String KeyPEMPath = @"key.pem";

            try
            {

                Process cmdProcess = new Process();

                /*
                 * Строку ниже можно убрать 
                 * если переменная среды PATH
                 * имеет путь до OpenSSL
                 */
                cmdProcess.StartInfo.WorkingDirectory = OpenSSLPath;
                cmdProcess.StartInfo.FileName = "openssl.exe";
                cmdProcess.StartInfo.Arguments = String.Format("smime -sign -in {0} -out {1} -signer {2} -outform DER", RequestPath, SignRequestPath, KeyPEMPath);

                cmdProcess.StartInfo.CreateNoWindow = true;
                cmdProcess.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                cmdProcess.Start();
                tb1.AppendText("Запущен OpenSSL\n");
                //Спасибо dimzon541
                if (!cmdProcess.WaitForExit(5000))
                {
                    cmdProcess.Kill();
                    //ret = false;
                }
                tb1.AppendText("Завершен OpenSSL\n");
            }
            catch (Exception)
            {
                //ret = false;
            }

            //return ret;
        }

        private void btUpload_Click(object sender, EventArgs e)
        {
            String requestFile = @"Company.XML";
            String signatureFile = @"Company.XML.sig";
            //Byte[] registerZipArchive;
            //RegisterDump dump;
            String resultComment;
            //String code;

            Int64 ldd = ZapretSOAPServices.LastDumpDate();
            Int64 LastDumpDate = 0;


            DateTime LastDumpDt = (new DateTime(1970, 1, 1, 0, 0, 0, 0)).AddSeconds(ldd / 1000);
            tb1.AppendText("getLastDumpDate (дата последнего дампа):\n");
            tb1.AppendText(LastDumpDt.ToString("    yyyy-MM-dd HH:mm:ss") + "\n");
            tb1.AppendText("\n");


            if (ldd != LastDumpDate)
            {
                LastDumpDate = ldd;
                tb1.AppendText("Отправляю файлы :\n");
                tb1.AppendText("    " + requestFile + " и " + signatureFile + "\n");
                tb1.AppendText("\n");
                
                    if (ZapretSOAPServices.SendRequest(out resultComment, out code, File.ReadAllBytes(requestFile), File.ReadAllBytes(signatureFile)))
                    {
                        tb1.AppendText("Ответ сервера на SendRequest:\n");
                        tb1.AppendText("    resultComment: " + resultComment + "\n");
                        tb1.AppendText("    code: " + code + "\n");
                        tb1.AppendText("\n");

                        //File.Delete(requestFile);
                        //File.Delete(signatureFile);
                        
/*
                        while (!ZapretSOAPServices.GetResult(out resultComment, out registerZipArchive, code))
                        {
                            tb1.AppendText("Ответ сервера на GetResult:\n");
                            tb1.AppendText("    resultComment: " + resultComment + "\n");
                            

                            if (resultComment != "запрос обрабатывается")
                            {
                             
                                //return;
                            }
                            Thread.Sleep(300000);
                            
                        }
 * */
                    }            
            }
        }

        private void btDownload_Click(object sender, EventArgs e)
        {

            Byte[] registerZipArchive;
            String registerZipArchivePath = @"register.zip";

            String resultComment;

            tb1.AppendText("Проверяем статус по коду: " + code + "\n");

            ZapretSOAPServices.GetResult(out resultComment, out registerZipArchive, code);
                        
            tb1.AppendText("Ответ сервера на GetResult:\n");
            tb1.AppendText("    resultComment: " + resultComment + "\n");
            tb1.AppendText("\n");
            if (resultComment != "запрос обрабатывается")
            {

                File.WriteAllBytes(registerZipArchivePath, registerZipArchive);
            }


        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {              
                MK mikrotik = new MK("192.168.88.1");
                string mk_output;
                Int64 rulenumber = 0;

                if (mikrotik.Login("admin", ""))
                {
                    string MyFileName = @"rules.txt";
                    string line = "";
                    using (StreamReader sr = new StreamReader(MyFileName))
                    {
                        while ((line = sr.ReadLine()) != null)
                        {
                            mikrotik.Send(line + "\n");
                            mk_output = mikrotik.Read().ToString();
                            tb1.AppendText("Загрузка правила " + rulenumber + " в Микротик: " + mk_output + "\n");
                            rulenumber++;
                        }
                    }
                }
                mikrotik.Close();
            }
            catch (Exception)
            {
            }
        }     
    }

    public class ParseRegisterDump
    {
        public static Boolean Parse(out RegisterDump Register, Byte[] registerZipArchive, String NameEventLog)
        {
            Boolean ret = true;
            Register = new RegisterDump();
            //String registerZipArchivePath = @"register.zip";
            //String UnZIPPath = @"";
            String dumpfile = @"dump.xml";
            //String signdumpfile = @"dump.xml.sig";

            
            
            try
            {
                //File.WriteAllBytes(registerZipArchivePath, registerZipArchive);

                //ZipFile.ExtractToDirectory(registerZipArchivePath, UnZIPPath);

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

                //Directory.Delete(UnZIPPath, true);
                //File.Delete(registerZipArchivePath);
            }
            catch (Exception)
            {
                //EventLog.WriteEntry(NameEventLog, "Ошибка парсера: " + error.Message, EventLogEntryType.Error, 200, 003);
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


    public class Settings
    {
        public String operatorName { get; set; }
        public String inn { get; set; }
        public String ogrn { get; set; }
        public String email { get; set; }

        public String OpenSSLPath { get; set; }
        public String KeyPEM { get; set; }

        public String ip { get; set; }
        public String username { get; set; } 
        public String password { get; set; }

        public String SRCAddress { get; set; }

        public Int64 LastDumpDate { get; set; }

        public String NameEventLog { get; set; }
    }

    public class FilterL7RouterOS
    {
        public static Boolean AddFilterL7(String ip, String username, String password, RegisterDump dump, String SRCAddress, String NameEventLog)
        {
            Boolean ret = true;


            
            try
            {
            
               
                string MyFileName = @"rules.txt";

                File.Delete(MyFileName);

                FileStream fs = new FileStream(MyFileName, FileMode.OpenOrCreate);
                StreamWriter sw = new StreamWriter(fs);
                
                
             
              //MK mikrotik = new MK(IPAddress.Parse(ip).ToString());



                //if (mikrotik.Login(username, password))
                //{
                string s;
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
                        for (Int32 i = 0; i < item.domain.Count; i++ )
                        {

                        s = @"/ip firewall layer7-protocol add comment=register name=" + item.id + "_" + i + @" regexp=""^.+(" + item.domain[i] + @").*\$""";
                        sw.WriteLine(s);
                        s = @"/ip firewall filter add comment=register action=drop chain=forward disabled=no dst-port=80 layer7-protocol=" + item.id + "_" + i + " protocol=tcp src-address= out-interface=ether1-gateway";
                        sw.WriteLine(s);
                        
                        }
                    }
                //}
            sw.Close(); 
            fs.Close();
                //mikrotik.Close();
            }
            catch (Exception)
            {
                //EventLog.WriteEntry(NameEventLog, "Ошибка добавления правил: " + error.Message, EventLogEntryType.Error, 200, 004);
                ret = false;
            }
    
            return ret;
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


}
