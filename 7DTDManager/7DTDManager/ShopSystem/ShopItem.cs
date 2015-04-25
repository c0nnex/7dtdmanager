using _7DTDManager.Interfaces;
using _7DTDManager.Objects;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _7DTDManager.ShopSystem
{
    [Serializable]
    public class ShopItem : IShopItem, ICalloutCallback
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public ShopItem()
        {
            HandlerName = "DefaultShopHandler";
            NextRestock = DateTime.MaxValue;
        }

        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int SellPrice { get;set; }
        public int BuyPrice { get; set; }
        public int StockAmount { get; set; }
        public int RestockAmount { get; set; }
        
        [XmlIgnore]
        public TimeSpan RestockDelay { 
            get; 
            set;  
        }

        [XmlElement(ElementName="RestockDelay")]
        public long _RestockDelay { 
            get { return (long)RestockDelay.TotalSeconds; } 
            set { RestockDelay = TimeSpan.FromSeconds(value); } 
        }
        
        public int MaxStock { get; set; }
        public int TotalSold { get; set; }
        public double EconomyFactor { get; set; }
        public int LevelRequired { get; set; }
        
        public DateTime NextRestock 
        { 
            get; 
            set; 
        }

        public string HandlerName { get; set; }

        [XmlIgnore]
        public Shop Shop { get; set; } 

        public void StartRestocking()
        {
            if (RestockAmount > 0 )
            {
                if (NextRestock < DateTime.Now )
                {
                    CalloutCallback(null,null);
                }
                
                if ( ( NextRestock == DateTime.MaxValue) || (NextRestock > (DateTime.Now + RestockDelay)))
                {
                    NextRestock = DateTime.Now + RestockDelay;
                }
                logger.Info("{0}: Start restocking {1}  Delay = {2}", Shop.ShopName,ItemName, RestockDelay.ToString());
                CalloutManagerImpl.Instance.AddCallout(this, RestockDelay, true);
              
            }
        }

        public void StopRestocking()
        {
            logger.Info("{0}: Stop restocking {1} ", Shop.ShopName, ItemName);
            CalloutManagerImpl.UnregisterCallouts(this);
        }

        public bool CalloutCallback(ICallout c,IServerConnection serverConnection)
        {
            if (RestockAmount <= 0 )
            {
                return false;
            }
            if (StockAmount < MaxStock)
            {
                StockAmount += RestockAmount;
                StockAmount = Math.Min(MaxStock, StockAmount);
                Program.Config.IsDirty = true;
                logger.Info("{0} restocking {1} new Stock {2}",Shop.ShopName, ItemName, StockAmount);
            }
            else
            {
                logger.Debug("{0} restocking {1}: max stock reached", Shop.ShopName, ItemName, StockAmount);
            }
            return true;
        }


        IShop IShopItem.Shop
        {
            get { return Shop; }
        }
    }

    
}
