﻿using _7DTDManager.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _7dtdManager
{
    [Serializable]
    [XmlRootAttribute("Config", Namespace = "http://fsgs.com/7DTD", IsNullable = false)]
    public class Configuration : IConfiguration
    {
        static Logger logger = LogManager.GetCurrentClassLogger();


        public string ServerHost { get; set; }
        public Int32 ServerTelnetPort { get; set; }
        public string ServerPassword { get; set; }

        public int CoinsPerMinute { get; set; }
        public int CoinsPerZombiekill { get; set; }

        public Configuration()
        {
            ServerHost = "81.169.234.52";
            ServerTelnetPort = 8081;
            ServerPassword = "supersecret";
            CoinsPerMinute = 1;
            CoinsPerZombiekill = 5;
        }

        public static Configuration Load()
        {
            try
            {

                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                StreamReader reader = new StreamReader("config.xml");
                Configuration c = (Configuration)serializer.Deserialize(reader);
                reader.Close();

                return c;
            }
            catch (Exception ex)
            {
                logger.Info("Problem loading configuration, creating default one");
                logger.Info(ex.ToString());
                Configuration c = new Configuration();
                c.Save();
                return null;
            }
        }

        public void Save()
        {
            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                StreamWriter writer = new StreamWriter("config.xml");
                serializer.Serialize(writer, this);
                writer.Close();                
            }
            catch (Exception ex)
            {
                logger.Info("Problem saving configuration");
                logger.Info(ex.ToString());                
            }
        }
    }
}
