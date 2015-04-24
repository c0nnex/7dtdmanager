using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdProxy : AdminCommandBase
    {
        public cmdProxy()
        {
            CommandName = "proxy";
            CommandHelp = "proxy a player.";
            CommandLevel = 100;
            CommandUsage = "/proxy <playername|entityid|steamid>|all";
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
                    p1.ProxyPlayer = p;
                }                
                p.Confirm("All Players proxyed");
                return true;
            }
            if (args[1] == "none")
            {
                foreach (var p1 in server.AllPlayers.Players)
                {
                    p1.ProxyPlayer = null;
                }
                p.Confirm("All Players unproxyed");
                return true;
            }
            IPlayer target = server.AllPlayers.FindPlayerByNameOrID(args[1],false);
            if ((target == null))
            {
                p.Message("Targetplayer '{0}' was not found.", args[1]);
                return false;
            }
            target.ProxyPlayer = p;
            p.ExecuteAs = target;
            p.Message("Proxying {0}",  target.Name);
            return true;
        }
    }
}
