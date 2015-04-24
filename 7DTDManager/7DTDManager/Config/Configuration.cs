using _7DTDManager.Interfaces;
using _7DTDManager.ShopSystem;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace _7DTDManager.Config
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
        public double CoinPercentageOnKill { get; set; }
        public double BountyFactor { get; set; }
        public double MinimumDistanceForPayday { get; set; }

        public string MOTD { get; set; }

        public int PollInterval { get; set; }
        public int PositionInterval { get; set; }
        public int PaydayInterval { get; set; }
        public double PositionTrackingRangeFactor { get; set; }
        public int MaxPingAccepted { get; set; }
        public int BanAfterKicks { get; set; }
        public string BanDuration { get; set; }

        public bool EnableProtection { get; set; }
        public int nextAreaProtectionID { get; set;}
        public int ProtectionPriceOneDay { get; set; }
        public int ProtectionPriceOneWeek { get; set; }
        public int ProtectionPriceOneMonth { get; set; }

        [XmlArrayItem(ElementName="Admin")]
        public AdminList Admins { get; set; }

        public CommandConfigurationList Commands { get; set; }
        public List<ShopSystem.Shop> Shops { get; set; }

        [XmlIgnore]
        public bool IsNewConfiguration = false;
        [XmlIgnore]
        public bool IsDirty { get { return _IsDirty; } set { _IsDirty = value; } } private bool _IsDirty = false;
        [XmlIgnore]
        public Dictionary<string, ShopHandler> ShopHandlers = new Dictionary<string, ShopHandler>();
        [XmlIgnore]
        public static HashSet<string> AllKnownItems = new HashSet<string>();
        [XmlIgnore]
        IReadOnlyList<IShop> IConfiguration.Shops { get { return Shops as IReadOnlyList<IShop>; } }
        [XmlIgnore]
        IAdminList IConfiguration.Admins
        {
            get { return Admins as IAdminList; }
        }
        [XmlIgnore]
        public Shop GlobalShop = null;

        public Configuration()
        {
            ServerHost = "81.169.234.52";
            ServerTelnetPort = 8081;
            ServerPassword = "supersecret";
            CoinsPerMinute = 1;
            CoinsPerZombiekill = 5;
            CoinLossPerDeath = 100;
            CoinPercentageOnKill = 5.0;
            BountyFactor = 2.0;
            MOTD = "Place your MOTD here";
            PollInterval = 30000; // every 30 seconds default polling
            PositionInterval = 2000; // every 2 seconds if someone is tracked
            PaydayInterval = 1; // Every minute
            PositionTrackingRangeFactor = 10.0; // Start tracking 10x away from target-extend 
            MaxPingAccepted = 250; // Higher ping that this will be kicked
            BanAfterKicks = 5;
            BanDuration = "1 year";
            MinimumDistanceForPayday = 10.0;
            ProtectionPriceOneDay = 50;
            ProtectionPriceOneWeek = 300;
            ProtectionPriceOneMonth = 1000;
            EnableProtection = true;
            Admins = new AdminList();
            Commands = new CommandConfigurationList();
            Shops = new List<ShopSystem.Shop>();
            ShopHandlers.Add("DefaultShopHandler", new DefaultShopHandler());
            ShopHandlers.Add("ProtectionShopHandler", new ProtectionShopHandler());
        }

        public void UpdateDefaults()
        {
            bool needSave = false;
            if (Admins.Count == 0)
            {
                logger.Info("Adding default admin");    
                Admins.AddAdmin("76561198003534614", 100);
                needSave = true;
            }
            GlobalShop = (from s in Shops where s.ShopID == 0 select s).FirstOrDefault();
            if ( (GlobalShop != null) && !EnableProtection)
            {
                logger.Info("Removing global shop (EnableProtection = false)");
                Shops.Remove(GlobalShop);
                GlobalShop = null;
                needSave = true;
            }
            else
                if ((GlobalShop == null) && EnableProtection)
                {
                    logger.Info("Adding global shop");
                    GlobalShop = new Shop
                    {
                        ShopName = "Serverwide Shop",
                        ShopRestocks = false,
                        ShopID = 0,
                    };
                    GlobalShop.RegisterItem(new ShopItem
                        {
                            ItemName = "One RL Day Protection",
                            SellPrice = ProtectionPriceOneDay,
                            StockAmount = 100,
                            RestockAmount = -1,
                            BuyPrice = 1,
                            HandlerName = "ProtectionShopHandler"
                        });
                    GlobalShop.RegisterItem(new ShopItem
                    {
                        ItemName = "One RL week Protection",
                        SellPrice = ProtectionPriceOneWeek,
                        StockAmount = 100,
                        RestockAmount = -1,
                        BuyPrice = 7, 
                        HandlerName = "ProtectionShopHandler"
                    });
                    GlobalShop.RegisterItem(new ShopItem
                    {
                        ItemName = "One RL Month Protection",
                        SellPrice = ProtectionPriceOneMonth,
                        StockAmount = 100,
                        RestockAmount = -1,
                        BuyPrice = 30,
                        HandlerName = "ProtectionShopHandler"
                    });
                    Shops.Add(GlobalShop);
                    needSave = true;
                }
            if (GlobalShop != null)
                GlobalShop.GlobalShop = true;
            if (needSave)
                Save(true);
        }

        public void Init()
        {            
            foreach (var item in Shops)
            {
                item.Init();
            }
        }

        public void Deinit()
        {
            foreach (var item in Shops)
            {
                item.Deinit();
            }
        }


        public int RegisterShop(ShopSystem.Shop shop)
        {
            var findShop = (from s in Shops select s.ShopID);
            int lastShop = 0;
            if ( (findShop != null) && (findShop.Count() > 0))
                lastShop = findShop.Max();

            shop.ShopID = lastShop + 1;
            Shops.Add(shop);
            Save(true);
            return shop.ShopID;
        }

        public int NewAreaProtectionID()
        {
            IsDirty = true;
            return ++nextAreaProtectionID;
        }

        public static Configuration Load()
        {
            try
            {
                /*XmlSerializer<Configuration> serializer = new XmlSerializer<Configuration>(
                   new XmlSerializationOptions(null, Encoding.UTF8, null, true).DisableRedact(), null);
                 */
                XmlSerializer serializer = new XmlSerializer(typeof(Configuration));
                StreamReader reader = new StreamReader("config.xml");
                Configuration c = (Configuration)serializer.Deserialize(reader);
                reader.Close();
                c.Save(true); // Make sure updated configs are written to the xml              
                return c;
            }
            catch (Exception ex)
            {
                logger.Warn(ex.Message);
                logger.Info("Problem loading configuration, creating default one");               
                Configuration c = new Configuration();              
                c.Save(true);
                c.IsNewConfiguration = true;
                return c;
            }
        }

        public void Save(bool force = false)
        {
            if (!IsDirty && !force)
                return;
            try
            {
                /*XmlSerializer<Configuration> serializer = new XmlSerializer<Configuration>(
                    new XmlSerializationOptions(null, Encoding.UTF8, null, true).DisableRedact(), null);
                */
                XmlSerializer serializer = new XmlSerializer(typeof(Configuration)); 
                StreamWriter writer = new StreamWriter("config.xml");
                serializer.Serialize(writer, this);
                writer.Close();
                IsDirty = false;
            }
            catch (Exception ex)
            {
                logger.Info("Problem saving configuration");
                logger.Info(ex.ToString());                
            }
        }

        
    }

   
}
