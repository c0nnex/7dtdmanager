using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdShop : PublicCommandBase
    {
        public cmdShop()
        {
            CommandHelp = "Displays the currently available items in the shop";
            CommandName = "shop";
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            p.Message("The shop is currently closed.");
            return true;
        }
    }
}
