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
       
        public cmdTransfer()
        {
            CommandName = "R:Cmd.Transfer.Command";
            CommandHelp = "R:Cmd.Transfer.Help";
            CommandUsage = "R:Cmd.Transfer.Usage";
            CommandCost = 10;
            CommandCoolDown = 10;
            CommandRegex = "(?<coins>[0-9]+) (?<name>.*)";
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
            target = server.AllPlayers.FindPlayerByNameOrID(groups["name"].Value);
            if ((target == null) || (!target.IsOnline))
            {
                p.Message(MESSAGES.ERR_TARGETNOTFOUND, groups["name"].Value);
                return false;
            }
            if (p.zCoins < (howmany + CommandCost))
            {
                p.Message(MESSAGES.ERR_NOTENOUGHCOINS);
                return false;
            }

            p.AddCoins((-1) * howmany, "transfer to " + target.Name);
            p.Confirm("R:Cmd.Transfer.SenderMsg", howmany, target.Name);
            target.AddCoins(howmany, "transfer from " + p.Name);
            target.Confirm("R:Cmd.Transfer.ReceiverMsg", p.Name, howmany);
            return true;

        }
    }
}
