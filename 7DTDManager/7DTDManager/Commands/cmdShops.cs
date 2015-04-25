using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7DTDManager.Objects;

namespace _7DTDManager.Commands
{
    public class cmdShops : PublicCommandBase
    {
        public cmdShops()
        {
            CommandHelp = "R:Cmd.Shops.Help";
            CommandName = "R:Cmd.Shops.Command";
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            if ( (Program.Config.Shops.Count == 0) || ( (Program.Config.EnableProtection) && (Program.Config.Shops.Count == 1)))
            {
                p.Error("R:Cmd.Shops.NoShops");
                return true;
            }
            bool bFirst = true;
            foreach (var shop in Program.Config.Shops)
            {
                if (shop.GlobalShop)
                    continue;
                
                if (shop.SecretShop)
                {
                    if (!shop.ShopPosition.IsInside(p.CurrentPosition))
                        continue;
                }

                double dist = p.CurrentPosition.Distance(shop.ShopPosition.Center);
                string head = String.Format("#{0} '{1}' {2} ", shop.ShopID, shop.ShopName, shop.ShopPosition.Center.ToHumanString());
                if (bFirst)
                {
                    bFirst = false;
                    p.Message("R:Cmd.Shops.KnownShops");
                }
                p.Message(dist.ToDistanceString(p,head,shop.ShopPosition.IsInside(p.CurrentPosition)));
            }
            if (bFirst)
                p.Error("R:Cmd.Shops.NoShops");
            return true;
        }
    }
}
