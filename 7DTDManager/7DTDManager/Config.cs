using _7DTDManager.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _7DTDManager
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
        public int CoinLossPerDeath { get; set; }

        [XmlArrayItem(ElementName="SteamID")]
        public List<string> Admins { get; set; }

        public Configuration()
        {
            ServerHost = "81.169.234.52";
            ServerTelnetPort = 8081;
            ServerPassword = "supersecret";
            CoinsPerMinute = 1;
            CoinsPerZombiekill = 5;
            CoinLossPerDeath = 100;
        }

        public static Configuration Load()
        {
            try
            {

                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                StreamReader reader = new StreamReader("config.xml");
                Configuration c = (Configuration)serializer.Deserialize(reader);
                reader.Close();
                c.Save(); // Make sure updated configs are written to the xml
                return c;
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                logger.Info("Problem loading configuration, creating default one");               
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

    [Serializable]
    [XmlRoot(ElementName="Command")]
    public class CommandConfiguration
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public int Cost { get; set; }
        [XmlAttribute]
        public int  CoolDown { get; set; }
        [XmlAttribute]
        public bool Enabled { get; set; }
    }
}
