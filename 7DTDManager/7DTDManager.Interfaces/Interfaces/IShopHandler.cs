using _7DTDManager.Interfaces;
using System;

namespace _7DTDManager.Interfaces
{
    public interface IShopHandler
    {
        string HandlerName { get;  }
        string Error { get; }
        int EvaluateBuy(IServerConnection server, IPlayer buyer, IShopItem item, int amount);
        int EvaluateSell(IServerConnection server, IPlayer seller, IShopItem item, int amount);
       
        bool ItemBought(IServerConnection server, IPlayer buyer, IShopItem shopItem, int amount, int price);
        bool ItemSold(IServerConnection server, IPlayer buyer, IShopItem shopItem, int amount, int price);
    }
}
