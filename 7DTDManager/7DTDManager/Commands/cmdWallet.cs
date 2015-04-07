using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtdManager.Commands
{
    public class cmdWallet : PublicCommandBase
    {
        public cmdWallet()
        {
            CommandHelp = "Show your current coin balance.";
            CommandName = "wallet";
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            p.Message("You have {0} coins.", p.zCoins);
            return true;
        }
        
    }
}
