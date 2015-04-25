using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdWallet : PublicCommandBase
    {
        public cmdWallet()
        {
            CommandHelp = "R:Cmd.Wallet.Help";
            CommandName = "R:Cmd.Wallet.Command";
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            p.Confirm("R:Cmd.Wallet.ResultMsg", p.zCoins);
            return true;
        }
        
    }
}
