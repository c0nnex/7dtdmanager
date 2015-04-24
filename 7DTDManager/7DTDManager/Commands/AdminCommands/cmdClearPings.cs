using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdClearPings : AdminCommandBase
    {
        public cmdClearPings()
        {
            CommandName = "clearpings";
            CommandHelp = "Clear Pingkicks of a player.";
            CommandLevel = 100;
            CommandUsage = "/clearpings <playername|entityid|steamid>|all";
            CommandArgs = 1;
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            if ( args.Length < 2)
            {
                return false;
            }
            if (args[1] == "all")
            {
                foreach (var p1 in server.AllPlayers.Players)
                {
                    p1.ClearPingKicks();
                }
                server.AllPlayers.Save(true);
                p.Confirm("All Pingkicks cleared");
                return true;
            }
            IPlayer target = server.AllPlayers.FindPlayerByNameOrID(args[1]);
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
