using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdBounty: PublicCommandBase
    {        

        public cmdBounty()
        {
            CommandRegex = "(?<coins>[0-9]+) (?<name>.*)";
            CommandName = "R:Cmd.Bounty.Command";
            CommandHelp = "R:Cmd.Bounty.Help";
            CommandUsage = "R:Cmd.Bounty.Usage";
            CommandCost = 100;
            CommandCoolDown = 10;
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            string currentLine = String.Join(" ", args, 1, args.Length - 1);
            GroupCollection groups = CommandMatch(p, currentLine);
            if (groups == null)
            {
                p.Error(CommandUsage);
                return false;
            }
            int howmany = 0;
            IPlayer target = null;

            if (!Int32.TryParse(groups["coins"].Value, out howmany))
            {
                p.Message(CommandUsage);
                return false;
            }
            target = server.AllPlayers.FindPlayerByNameOrID(groups["name"].Value, false);
            if ((target == null))
            {
                p.Message("R:Error.TargetNotFound", groups["name"].Value);
                return false;
            }
            if (p.zCoins < (howmany + CommandCost))
            {
                p.Message("R:Error.NotEnoughCoins");
                return false;
            }

            p.AddCoins((-1) * howmany, "bounty on " + target.Name);
            p.Message("R:Cmd.Bounty.NewBounty", howmany, target.Name);
            target.AddBounty(howmany, "bounty by " + p.Name);
            return true;
        }
            
        
    }
}
