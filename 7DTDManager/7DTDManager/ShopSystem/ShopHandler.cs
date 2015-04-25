using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.ShopSystem
{
    public abstract class ShopHandler
    {
        public string HandlerName { get; set; }
        public string Error { get; set; }
        public abstract bool ItemBought(Interfaces.IServerConnection server, Interfaces.IPlayer buyer, IShopItem shopItem, int amount, int price);
        public abstract bool ItemSold(Interfaces.IServerConnection server, Interfaces.IPlayer buyer, IShopItem shopItem, int amount, int price);
        public abstract int EvaluateBuy(IServerConnection server, IPlayer buyer, IShopItem item, int amount);
        public abstract int EvaluateSell(IServerConnection server, IPlayer seller, IShopItem item, int amount);

    }
}
