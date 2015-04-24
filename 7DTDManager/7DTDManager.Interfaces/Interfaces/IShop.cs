using System;
using System.Collections.Generic;
namespace _7DTDManager.Interfaces
{
    public interface IShopItem
    {
        int BuyPrice { get; }
        double EconomyFactor { get; }
        string HandlerName { get; }
        string ItemName { get; }
        int LevelRequired { get; }
        DateTime NextRestock { get; }
        int RestockAmount { get; }
        TimeSpan RestockDelay { get; }
        int SellPrice { get; }
        int StockAmount { get; }
        int TotalSold { get; }
    }

    public interface IShop
    {
        double EconomyFactor { get; }
        bool HasOpeningHours { get; }
        
        int ShopClosesAt { get; }
        IReadOnlyList<IShopItem> ShopItems { get; }
        string ShopName { get; }
        int ShopOpensAt { get; }
        IAreaDefiniton ShopPosition { get; }
        bool ShopRestocks { get; }
        int TotalCustomers { get; }
        int TotalDeals { get; }
        int TotalRevenue { get; }
        int TotalSales { get; }

        bool IsOpen();
    }

    
}
