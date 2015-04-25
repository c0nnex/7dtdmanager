using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.AdminCommands
{
    public class cmdAddCoins : AdminCommandBase
    {

        public cmdAddCoins()
        {
            CommandHelp = "R:Cmd.AddCoins.Help";
            CommandName = "R:Cmd.AddCoins.Command";
            CommandLevel = 100;           
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {           
            p.AddCoins(1000, "Cheating");
            p.Confirm("R:Cmd.AddCoins.ConfirmMsg", 1000);
            return true;
        }


        
    }
}
