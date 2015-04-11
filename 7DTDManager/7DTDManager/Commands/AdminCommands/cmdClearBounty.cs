using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdClearBounty : AdminCommandBase
    {
        public cmdClearBounty()
        {
            CommandName = "clearbounty";
            CommandHelp = "Clear bounty of a player.";
            CommandLevel = 100;
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            if ( args.Length < 2)
            {
                return false;
            }
            IPlayer target = server.AllPlayers.FindPlayerByName(args[1]);
            if ((target == null) || (!target.IsOnline))
            {
                p.Message("Targetplayer '{0}' was not found or is not online.", args[1]);
                return false;
            }
            target.ClearBounty();
            p.Message("You cleared {0}'s bounty.",  target.Name);
            target.Message("{0} cleared your bounty.", p.Name);            
            return true;
        }
    }
}
