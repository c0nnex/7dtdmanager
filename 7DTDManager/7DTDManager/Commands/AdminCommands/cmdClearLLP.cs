using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.AdminCommands
{
    public class cmdClearLLP : AdminCommandBase
    {
        public cmdClearLLP()
        {
            CommandName = "clearllp";
            CommandHelp = "Clear LLP of a player.";
            CommandLevel = 100;
            CommandUsage = "/clearllp <playername|entityid|steamid>|all";
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
                    p1.LandProtections.Clear();
                }
                server.AllPlayers.Save(true);
                p.Confirm("All LLP cleared");
                return true;
            }
            IPlayer target = server.AllPlayers.FindPlayerByNameOrID(args[1]);
            if ((target == null) || (!target.IsOnline))
            {
                p.Message("Targetplayer '{0}' was not found or is not online.", args[1]);
                return false;
            }
            target.LandProtections.Clear();
            p.Message("You cleared {0}'s LLP.",  target.Name);
           
            return true;
        }
    }
}
