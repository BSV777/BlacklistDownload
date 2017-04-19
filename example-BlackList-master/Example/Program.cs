using BlackList;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Example
{
    class Program
    {
        static void Main(string[] args)
        {
            Settings options;
            
            try
            {
                Console.WriteLine("Грузим конфиг: " + args[0]);
                
                Console.WriteLine("1 "); 
                
                using (Stream stream = new FileStream(args[0], FileMode.Open))
                {
                
                    Console.WriteLine("2 "); 
                    
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    Console.WriteLine("3 ");

                    options = new Settings();
                    //options = (Settings)serializer.Deserialize(stream);
                    Console.WriteLine("4 ");     
                    

                }

                
                

                try
                {
                    Console.WriteLine("Проверяем наличие журнала: " + options.NameEventLog);
                    if (!EventLog.SourceExists(options.NameEventLog))
                    {                        
                        EventLog.CreateEventSource(options.NameEventLog, options.NameEventLog);
                        Console.WriteLine("Создаем журнал: " + options.NameEventLog);
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Нужны прова администратора на создание журнала");
                    ProcessStartInfo processInfo = new ProcessStartInfo();
                    processInfo.Verb = "runas";
                    processInfo.FileName = Assembly.GetExecutingAssembly().Location;
                    processInfo.Arguments = args[0];
                    try
                    {
                        Process.Start(processInfo);
                        return;
                    }
                    catch (Win32Exception)
                    {
                        return;
                    }
                }

                Console.WriteLine("Процесс пошел...");
                options = GetRegister.Start(options);

                Console.WriteLine("Сохраняем кофиг");
                using (Stream writer = new FileStream(args[0], FileMode.Create))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(Settings));
                    serializer.Serialize(writer, options);
                }

                Console.ReadKey();
            }
            catch (Exception)
            {
                
            }
        }

        static Settings temp()
        {
            Settings options = new Settings();

            options.operatorName = @"ООО 'КРТ'";
//            options.inn = @"0123456789";
            //options.ogrn = @"9876543210";
            //options.email = @"sysadmin@krt.ru";

            //options.NameEventLog = "Zapret";

            //options.OpenSSLPath = @"c:\OpenSSL-Win64\bin\";
//            #region options.KeyPEM = "keykeykey";
            //options.KeyPEM = @"keykeykey";
            //#endregion

            //options.LastDumpDate = 0;

            //options.ip = IPAddress.Parse(@"10.40.10.254").ToString();
            //options.username = @"admin";
            //options.password = @"krt2344004";
            //options.SRCAddress = @"10.40.10.0/24";

            return options;
        }
    }
}
