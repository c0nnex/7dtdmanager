using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.ShopSystem
{
    public class DefaultShopHandler : ShopHandler
    {
        public DefaultShopHandler()
        {
            HandlerName = "DefaultShopHandler";
        }

        public override bool ItemBought(Interfaces.IServerConnection server, Interfaces.IPlayer buyer, ShopItem shopItem, int amount, int price)
        {
            server.Execute("give {0} {1} {2}", buyer.EntityID, shopItem.ItemName, amount);
            buyer.AddCoins((-1) * price, String.Format("{0} {1} shop {2}", amount, shopItem.ItemName, shopItem.Shop.ShopName));

            buyer.Confirm("You bought {0} {1} for {2} coins.", amount, shopItem.ItemName, price);
            buyer.Message("Your items have been placed next to you.");
            shopItem.StockAmount -= amount;
            shopItem.TotalSold += amount;
            shopItem.Shop.TotalDeals++;
            shopItem.Shop.TotalSales += price;
            shopItem.Shop.TotalRevenue += price; //TODO: Economy Factor!
            shopItem.Shop.TotalCustomers++;
            Program.Config.Save();

            return true;
        }

        public override bool ItemSold(Interfaces.IServerConnection server, Interfaces.IPlayer seller, ShopItem item, int amount, int price)
        {
            throw new NotImplementedException();
        }

        public override int EvaluateBuy(Interfaces.IServerConnection server, Interfaces.IPlayer buyer, ShopItem item, int amount)
        {
            //TODO: SKillcheck
            return item.SellPrice * amount;
        }

        public override int EvaluateSell(Interfaces.IServerConnection server, Interfaces.IPlayer seller, ShopItem item, int amount)
        {
            throw new NotImplementedException();
        }
    }
}
