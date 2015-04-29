using _7DTDManager.Commands;
using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.AdminCommands
{
    public class cmdClearCooldown : AdminCommandBase
    {
        public cmdClearCooldown()
        {
            CommandName = "clearcooldown";
            CommandHelp = "Clear cooldowns of a player.";
            CommandLevel = 1;
            CommandAliases = new string[] { "ccd" };
            CommandUsage = "clearcooldown <all|targetplayer> [command]";
            CommandArgs = 1;
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            ICommand targetCommand = null;
            if ( args.Length < 2)
            {
                return false;
            }
            if ( args.Length == 3)
            {
                targetCommand = CommandManager.FindCommand(args[2]); // TODO: Use Interface instead --> expose CommandManager
                if ( targetCommand == null )
                {
                    p.Error(MESSAGES.ERR_NOSUCHCOMAND, args[2]);
                    return false;
                }
            }
            if ( args[1] == "all" )
            {
                foreach (var curPlayer in server.AllPlayers.Players)
                {
                    curPlayer.ClearCooldowns(targetCommand);
                    if (curPlayer.IsOnline)
                    {
                        curPlayer.Message("R:Cmd.ClearCooldown.Message", p.Name, targetCommand != null? curPlayer.Localize(targetCommand.CommandName) : curPlayer.Localize(MESSAGES.WORD_ALL) );
                    }
                }
                p.Confirm("You cleared all players cooldown for: {0}",targetCommand == null ? "all" : args[2]);
                return true;

            }
            IPlayer target = server.AllPlayers.FindPlayerByNameOrID(args[1]);
            if((target == null) )
            {
                p.Message(MESSAGES.ERR_TARGETNOTFOUND, args[1]);
                return false;
            }
            target.ClearCooldowns(targetCommand);
            p.Confirm("You cleared '{0}'s cooldown for: {1}",target.Name,targetCommand == null ? "all" : args[2]);
            target.Message("R:Cmd.ClearCooldown.Message", p.Name, targetCommand != null? target.Localize(targetCommand.CommandName) : target.Localize(MESSAGES.WORD_ALL) );
            server.AllPlayers.Save();
            return true;
        }
    }
}
