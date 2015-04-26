using System;
using System.Collections.Generic;
namespace _7DTDManager.Interfaces
{
    public interface IShopItem
    {
        int ItemID { get; }
        IShop Shop { get; }
        int BuyPrice { get;set; }
        double EconomyFactor { get;set; }
        string HandlerName { get;set; }
        string ItemName { get;set; }
        int LevelRequired { get;set; }
        DateTime NextRestock { get;set; }
        int RestockAmount { get;set; }
        TimeSpan RestockDelay { get;set; }
        int SellPrice { get;set; }
        int StockAmount { get;set; }
        int TotalSold { get;set; }

        IShopHandler ShopHandler { get; }
    }

    public interface IShop
    {
        int ShopID { get; }
        double EconomyFactor { get;set; }
        bool HasOpeningHours { get;set; }
        
        int ShopClosesAt { get;set; }
        IReadOnlyList<IShopItem> ShopItems { get; }
        string ShopName { get;set; }
        int ShopOpensAt { get;set; }
        IAreaDefiniton ShopPosition { get; }
        bool ShopRestocks { get;set; }
        int TotalCustomers { get;set; }
        int TotalDeals { get;set; }
        int TotalRevenue { get;set; }
        int TotalSales { get;set; }

        bool IsOpen();
        IShopHandler GetShopHandler();
    }

    
}
