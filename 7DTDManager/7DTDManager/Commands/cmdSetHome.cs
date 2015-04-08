using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdSetHome : PublicCommandBase
    {
        public cmdSetHome()
        {
            CommandCost = 60;
            CommandTimeLimit = 30;
            CommandHelp = "Set the position you will teleport to using /home";
            CommandName = "sethome";
        }


        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            if (!p.CurrentPosition.IsValid())
            {
                p.Message("No valid position for you recorded. Wait a little please.");
                return false;
            }
            p.UpdateHomePosition(p.CurrentPosition);
            p.Message("Homeposition set to {0}", p.HomePosition.ToHumanString());
            return true;
        }


    }
}
