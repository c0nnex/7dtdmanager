﻿using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.ShopSystem
{
    public class ShopItem : IShopItem
    {
        public ShopItem()
        {
            HandlerName = "DefaultShopHandler";
        }

        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public int SellPrice { get;set; }
        public int BuyPrice { get; set; }
        public int StockAmount { get; set; }
        public int RestockAmount { get; set; }
        public int RestockDelay { get; set; }
        public int MaxStock { get; set; }
        public int TotalSold { get; set; }
        public double EconomyFactor { get; set; }
        public int LevelRequired { get; set; }
        
        public DateTime NextRestock { get; set; }
        public string HandlerName { get; set; }
    }
}