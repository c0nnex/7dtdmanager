using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using _7DTDManager.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdStats : PublicCommandBase
    {
        public cmdStats()
        {
            CommandHelp = "R:Cmd.Stats.Help";
            CommandName = "R:Cmd.Stats.Command";
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            IPlayer targetPlayer = p;
            if (p.IsAdmin && (args.Length >= 2))
            {
                IPlayer target = server.AllPlayers.FindPlayerByNameOrID(args[1],false);
                if ((target == null))
                {
                    p.Message(MESSAGES.ERR_TARGETNOTFOUND, args[1]);
                    return false;
                }
                targetPlayer = target;
            }
            if (targetPlayer != p)
                p.Message("R:Cmd.Stats.StatsFor", targetPlayer.Name);

            TimeSpan t = TimeSpan.FromMinutes(targetPlayer.Age);
            TimeSpan s = TimeSpan.FromMinutes(targetPlayer.SessionAge);
            p.Message("R:Cmd.Stats.AgeCoins", t.ToString(p), targetPlayer.zCoins);
            p.Message("R:Cmd.Stats.SessionAge", s.ToString(p), targetPlayer.zCoins);
            p.Message("R:Cmd.Stats.BountyCollected", targetPlayer.BountyCollected, targetPlayer.BloodCoins);
            if (targetPlayer.DistanceTravelled > 0)
                p.Message("R:Cmd.Stats.Travel", (int)(targetPlayer.DistanceTravelled / 1000.0));
            if (targetPlayer.Bounty > 0)
                p.Error("R:Cmd.Stats.Bounty", targetPlayer.Bounty);
            if ((targetPlayer == p) && (targetPlayer.Mailbox.Mails.Count > 0))
                p.Confirm("R:Mail.Inbox", targetPlayer.Mailbox.Mails.Count);
            return true;
        }
        
    }
}
