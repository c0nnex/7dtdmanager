using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtdManager.Commands
{
    public class cmdAddCoins : AdminCommandBase
    {

        public cmdAddCoins()
        {
            CommandHelp = "Add coins to a player";
            CommandName = "addcoins";
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            if (!p.IsAdmin)
            {
                p.Message("Nice try!");
                return true;
            }
            p.AddCoins(1000, "Cheating");
            return true;
        }


        
    }
}
