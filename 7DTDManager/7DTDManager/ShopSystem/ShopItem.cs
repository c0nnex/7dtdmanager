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
                    CalloutCallback(null);
                }
                
                if ( ( NextRestock == DateTime.MaxValue) || (NextRestock > (DateTime.Now + RestockDelay)))
                {
                    NextRestock = DateTime.Now + RestockDelay;
                }
                logger.Info("{0}: Start restocking {1}  Delay = {2}", Shop.ShopName,ItemName, RestockDelay.ToString());
                CalloutManager.RegisterCallout(new RestockCallout { When = NextRestock, Callback = this, Persistent = true });
            }
        }

        public void StopRestocking()
        {
            logger.Info("{0}: Stop restocking {1} ", Shop.ShopName, ItemName);
            CalloutManager.UnregisterCallouts(this);
        }

        public void CalloutCallback(ICallout c)
        {
            if (RestockAmount <= 0 )
            {
                return;
            }
            if (StockAmount < MaxStock)
            {
                StockAmount += RestockAmount;
                Program.Config.IsDirty = true;
                logger.Info("{0} restocking {1} new Stock {2}",Shop.ShopName, ItemName, StockAmount);
            }
            else
            {
                logger.Debug("{0} restocking {1}: max stock reached", Shop.ShopName, ItemName, StockAmount);
            }

            NextRestock = DateTime.Now + RestockDelay;
            if ( c != null )
                c.When = NextRestock; // Update callout
        }
    }

    public class RestockCallout : ICallout
    {
        public override void Execute()
        {
            if (Callback != null)
                Callback.CalloutCallback(this);
        }
    }
}
