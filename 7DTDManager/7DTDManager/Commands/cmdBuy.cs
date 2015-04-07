using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtdManager.Commands
{
    public class cmdBuy : PublicCommandBase
    {
        public cmdBuy()
        {
            CommandHelp = "Lets you buy an item from the shop.";
            CommandName = "buy";
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            p.Message("The shop is currently closed.");
            return true;
        }
    }
}
