using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdShops : PublicCommandBase
    {
        public cmdShops()
        {
            CommandHelp = "Show known shops";
            CommandName = "shops";
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            if ( Program.Config.Shops.Count == 0)
            {
                p.Error("No shops registered.");
                return true;
            }
            p.Message("Known shops:");
            foreach (var shop in Program.Config.Shops)
            {
                
                double dist = p.CurrentPosition.Distance(shop.ShopPosition.Center);
                string head = String.Format("#{0} '{1}' {2} ",shop.ShopID,shop.ShopName,shop.ShopPosition.Center.ToHumanString());

                if (shop.SecretShop)
                {
                    if (!shop.ShopPosition.IsInside(p.CurrentPosition))
                    continue;
                }

                if (dist >= 1000.0)
                    p.Message("{0} ({1} km)", head, (int)(dist / 1000.0));
                else
                {
                    if (shop.ShopPosition.IsInside(p.CurrentPosition))
                        p.Error("{0} (HERE)", head, (int)dist);
                    else
                        p.Message("{0} ({1} m)", head, (int)dist);
                }
            }
            return true;
        }
    }
}
