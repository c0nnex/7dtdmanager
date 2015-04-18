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

        public abstract void ItemBought(IServerConnection server, IPlayer buyer,ShopItem item, int amount);
        public abstract void ItemSold(IServerConnection server, IPlayer seller, ShopItem item, int amount);
        public abstract int EvaluateBuy(IServerConnection server, IPlayer buyer, ShopItem item, int amount);
        public abstract int EvaluateSell(IServerConnection server, IPlayer seller, ShopItem item, int amount);

    }
}
