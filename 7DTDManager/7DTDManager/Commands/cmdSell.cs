using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdSell : PublicCommandBase
    {
        public cmdSell()
        {
            CommandHelp = "Lets you sell an item to the shop.";
            CommandName = "sell";
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            p.Message("The command is currently disabled.");
            return true;
        }
    }
}
