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
    public class cmdTransfer : PublicCommandBase
    {
        static Regex rgTransfer = new Regex("transfer (?<coins>[0-9]+) coins to (?<name>.*)");

        public cmdTransfer()
        {
            CommandName = "transfer";
            CommandHelp = "Transfer coins to another player. usage: /transfer [howmany] coins to [tragetname]";
            CommandCost = 10;
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
                    p.Message("usage: /transfer [howmany] coins to [tragetname]");
                    return false;
                }
                target = server.AllPlayers.FindPlayerByName(groups["name"].Value);
                if ( (target == null) || (!target.IsOnline) )
                {
                    p.Message("Targetplayer '{0}' was not found or is not online.", groups["name"].Value);
                    return false;
                }
                if ( p.zCoins < (howmany + CommandCost) )
                {
                    p.Message("You don't have enough coins in your wallet.");
                    return false;
                }

                p.AddCoins((-1) * howmany, "transfer to " + target.Name);
                p.Message("You transferred {0} coins to {1}.", howmany, target.Name);
                target.AddCoins(howmany, "transfer from " + p.Name);
                target.Message("{0} transferred {1} coins to your wallet.", p.Name, howmany);
                return true;
            }
            return false;
        }
    }
}
