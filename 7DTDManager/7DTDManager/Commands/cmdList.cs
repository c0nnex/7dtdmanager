using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using _7DTDManager.ShopSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdList : PublicCommandBase
    {
        public cmdList()
        {
            CommandArgs = 0;
            CommandName = "list";
            CommandHelp = "Shows the items in stock in a shop.";
            CommandUsage = "/list [page]";
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {       
            Shop shop = (from s in Program.Config.Shops where s.ShopPosition.IsInside(p.CurrentPosition) select s).FirstOrDefault();
            if (shop == null)
            {
                p.Error("You are not inside a shop area. see /shops for a list of shops.");
                return false;
            }
            if (!shop.IsOpen())
            {
                p.Error("The shop is currently closed.");
                return false;
            }
            
            int startitem = 0;
            if ( args.Length > 1)
            {
                if ( !Int32.TryParse(args[1],out startitem))
                {
                    p.Error(CommandUsage);
                    return false;
                }
                startitem--;
            }
            p.Message(String.Format("{0,3} {1,-15} {2,5} {3,5}", "#", "name", "price", "stock").Replace(" ","_"));

            for (int i = startitem*5; i < (startitem+1)*5; i++)
            {
                if ( i < shop.ShopItems.Count )
                {
                    ShopItem item = shop.ShopItems[i];
                    int price = Program.Config.ShopHandlers[item.HandlerName].EvaluateBuy(server, p, item, 1);
                    p.Message(String.Format("{0,3} {1,-15} {2,5} {3,5}", item.ItemID, item.ItemName, price, item.StockAmount).Replace(" ", "_"));
                }
            }
            p.Message("--- Page {0} of {1} ----", startitem + 1, ((shop.ShopItems.Count+5) / 5));
            return true;
        }

        public override bool AdminExecute(IServerConnection server, IPlayer p, params string[] args)
        {
            foreach (var shop in Program.Config.Shops)
            {
                p.Confirm(shop.ShopName);

                p.Message(String.Format("{0,3} {1,-15} {2,5} {3,5}", "#", "name", "price", "stock").Replace(" ", "_"));

                for (int i = 0; i < shop.ShopItems.Count; i++)
                {
                    if (i < shop.ShopItems.Count)
                    {
                        ShopItem item = shop.ShopItems[i];
                        int price = Program.Config.ShopHandlers[item.HandlerName].EvaluateBuy(server, p, item, 1);
                        p.Message(String.Format("{0,3} {1,-15} {2,5} {3,5}", item.ItemID, item.ItemName, price, item.StockAmount).Replace(" ", "_"));
                    }
                }
                p.Message("");
            }
            return true;
        }
    }
}
