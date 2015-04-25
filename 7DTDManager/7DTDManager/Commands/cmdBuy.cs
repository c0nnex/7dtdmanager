using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using _7DTDManager.ShopSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdBuy : PublicCommandBase
    {
        public cmdBuy()
        {
            CommandHelp = "R:Cmd.Buy.Help";
            CommandName = "R:Cmd.Buy.Command";
            CommandArgs = 2;
            CommandUsage = "R:Cmd.Buy.Usage";
            CommandRegex = "(?<amount>[0-9]+) #(?<itemid>[0-9]+)";
        }
        
        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            string cmd = String.Join(" ", args, 1, args.Length - 1);
            IShop shop = p.GetCurrentShop();
            if ( shop == null )
            {
               p.Error("R:Shop.NotInside");
               return false;
            }
            if (!shop.IsOpen())
            {
                p.Error("R:Shop.Closed",shop.ShopOpensAt,shop.ShopClosesAt);
                return false;
            }

            GroupCollection groups = CommandMatch(p, cmd);
            if (groups == null)
            {
                p.Error(CommandUsage);
                return false;
            }
            int amount = Convert.ToInt32(groups["amount"].Value);
            int itemid = Convert.ToInt32(groups["itemid"].Value);

            IShopItem shopItem = (from item in shop.ShopItems where item.ItemID == itemid select item).FirstOrDefault();
            if (shopItem == null)
            {
                p.Error("R:Shop.OutOfStock");
                return false;
            }
            if (shopItem.StockAmount < amount)
            {
                p.Error("R:Shop.ShortStock", shopItem.StockAmount, shopItem.ItemName);
                return false;
            }
            int price = Program.Config.ShopHandlers[shopItem.HandlerName].EvaluateBuy(server, p, shopItem, amount);
            if (price > p.zCoins)
            {
                p.Error("R:Shop.OutOfCoins", price);
                return false;
            }
            return Program.Config.ShopHandlers[shopItem.HandlerName].ItemBought(server, p, shopItem, amount, price);           
        }
    }
}
