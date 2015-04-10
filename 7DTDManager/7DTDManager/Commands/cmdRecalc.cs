using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdRecalc : AdminCommandBase
    {
        public cmdRecalc()
        {
            CommandName = "recalc";
            CommandHelp = "Recalculate coins";
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            foreach (IPlayer p1 in server.AllPlayers.Players)
            {
                p1.Recalc();             
            }
            server.AllPlayers.Save();
            if (p != null )
                p.Message("Coins recalcuclated");
            return true;
        }
    }
}
