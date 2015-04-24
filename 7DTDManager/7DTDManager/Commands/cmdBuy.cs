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
            CommandHelp = "Lets you buy an item from the shop.";
            CommandName = "buy";
            CommandArgs = 2;
            CommandUsage = "/buy <amount> #<itemid>. (/buy 5 #1) See /list for itemids";
        }

        Regex rgBuy = new Regex("(?<amount>[0-9]+) #(?<itemid>[0-9]+)");

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            string cmd = String.Join(" ", args, 1, args.Length - 1);
            Shop shop = (from s in Program.Config.Shops where s.ShopPosition.IsInside(p.CurrentPosition) select s).FirstOrDefault();
            if ( shop == null )
            {
               p.Error("You are not inside a shop area. see /shops for a list of shops.");
               return false;
            }
            if (!shop.IsOpen())
            {
                p.Error("The shop is currently closed.");
                return false;
            }
            if ( !rgBuy.IsMatch(cmd))
            {
                p.Error(CommandUsage);
                return false;
            }
            return ExecuteBuy(server, p, cmd, shop,false);
        }

        public bool ExecuteBuy(IServerConnection server, IPlayer p,string cmd,Shop shop,bool silent)
        {
            Match match = rgBuy.Match(cmd);
            GroupCollection groups = match.Groups;

            int amount = Convert.ToInt32(groups["amount"].Value);
            int itemid = Convert.ToInt32(groups["itemid"].Value);

            ShopItem shopItem = (from item in shop.ShopItems where item.ItemID == itemid select item).FirstOrDefault();
            if (shopItem == null)
            {
                p.Error("No such item in stock. Sorry.");
                return false;
            }
            if (shopItem.StockAmount < amount)
            {
                p.Error("There are only {0} {1} in stock.", shopItem.StockAmount, shopItem.ItemName);
                return false;
            }
            int price = Program.Config.ShopHandlers[shopItem.HandlerName].EvaluateBuy(server, p, shopItem, amount);
            if (price > p.zCoins)
            {
                p.Error("You do not have enough coins ({0}) for this transaction.", price);
                return false;
            }
            return Program.Config.ShopHandlers[shopItem.HandlerName].ItemBought(server, p, shopItem, amount, price);           
        }
    }
}
