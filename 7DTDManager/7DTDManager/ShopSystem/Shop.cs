using _7DTDManager.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.ShopSystem
{
    class Shop
    {
        public int ShopID { get; set; }
        public string ShopName { get; set; }
        public AreaDefiniton ShopPosition { get; set; }
        
        public int TotalSales { get; set; }
        public int TotalRevenue { get; set; }
        public int TotalDeals { get; set; }
        public int TotalCustomers { get; set; }
        public double EconomyFactor { get; set; }
        
        public bool IsOpen { get; set; }
        public bool HasOpeningHours { get; set; }
        public int ShopOpensAt { get; set; }
        public int ShopClosesAt { get; set; }
        public bool ShopRestocks { get;set; }

        public List<ShopItem> ShopItems { get; set; }
    }
}
