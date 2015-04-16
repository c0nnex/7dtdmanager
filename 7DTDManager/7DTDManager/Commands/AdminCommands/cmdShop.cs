using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using _7DTDManager.Objects;
using _7DTDManager.ShopSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdShop : AdminCommandBase
    {
        public cmdShop()
        {
            CommandHelp = "Shop edit functions";
            CommandName = "shop";
            CommandLevel = 100;
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            string subCmd = args[0];
            string restCmd = String.Join(" ",args).Substring(subCmd.Length + 1);

            Shop newShop = new Shop { ShopName = restCmd, ShopPosition = new AreaDefiniton(p.CurrentPosition) };

            Program.Config.Shops.Add(newShop);
            Program.Config.Save();
            newShop.Init();
            return true;
        }
    }
}
