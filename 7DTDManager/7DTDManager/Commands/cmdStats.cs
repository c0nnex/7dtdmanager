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
            CommandHelp = "Show your stats.";
            CommandName = "stats";
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            IPlayer targetPlayer = p;
            if (p.IsAdmin && (args.Length >= 2))
            {
                IPlayer target = server.AllPlayers.FindPlayerByNameOrID(args[1],false);
                if ((target == null))
                {
                    p.Message("Targetplayer '{0}' was not found.", args[1]);
                    return false;
                }
                targetPlayer = target;
            }
            if (targetPlayer != p)
                p.Message("Stats for {0}:", targetPlayer.Name);

            TimeSpan t = new TimeSpan(0, targetPlayer.Age, 0);
            p.Message("Age: {0} Coins: {1}", t.ToDaysHoursMinutesString(), targetPlayer.zCoins);
            p.Message("Bounties collected: {0} coins Bloodmoney collected: {1} coins", targetPlayer.BountyCollected, targetPlayer.BloodCoins);
            if (targetPlayer.DistanceTravelled > 0)
                p.Message("You travelled {0} km so far.", (int)(targetPlayer.DistanceTravelled / 1000.0));
            if (targetPlayer.Bounty > 0)
                p.Error("Bounty on your head: {0} coins", targetPlayer.Bounty);
            if ((targetPlayer == p) && (targetPlayer.Mailbox.Mails.Count > 0))
                p.Confirm("You have {0} unread mails.", targetPlayer.Mailbox.Mails.Count);
            return true;
        }
        
    }
}
