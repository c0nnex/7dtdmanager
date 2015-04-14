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

        public abstract void ItemBought(IPlayer buyer,int amount);
        public abstract void ItemSold(IPlayer seller, int amount);
        public abstract int EvaluateBuy(IPlayer buyer, int amount);
        public abstract int EvaluateSell(IPlayer seller, int amount);

    }
}
