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
        static Regex rgTransfer = new Regex("bounty (?<coins>[0-9]+) coins on (?<name>.*)");

        public cmdBounty()
        {
            CommandName = "bounty";
            CommandHelp = "Set a bounty on another players head. usage: /bounty [howmany] coins on [tragetname]";
            CommandCost = 100;
            CommandCoolDown = 10;
        }
        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            string currentLine = String.Join(" ", args);
            if (rgTransfer.IsMatch(currentLine))
            {
                Match match = rgTransfer.Match(currentLine);
                GroupCollection groups = match.Groups;

                int howmany = 0;
                IPlayer target = null;

                if ( !Int32.TryParse(groups["coins"].Value, out howmany))
                {
                    p.Message("usage: /bounty [howmany] coins on [targetname]");
                    return false;
                }
                target = server.AllPlayers.FindPlayerByName(groups["name"].Value,false);
                if ( (target == null)  )
                {
                    p.Message("Targetplayer '{0}' was not found.", groups["name"].Value);
                    return false;
                }
                if ( p.zCoins < (howmany + CommandCost) )
                {
                    p.Message("You don't have enough coins in your wallet.");
                    return false;
                }

                p.AddCoins((-1) * howmany, "bounty on " + target.Name);
                p.Message("You set a bounty of {0} coins on {1}'s head.", howmany, target.Name);
                target.AddBounty(howmany, "bounty by " + p.Name);                
                return true;
            }
            p.Message("usage: /bounty [howmany] coins on [targetname]");
            return false;
        }
    }
}
