using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
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
            TimeSpan t = new TimeSpan(0, p.Age, 0);
            p.Message("Age: {0} Coins: {1}", String.Format("{0} days {1} hours {2} minutes",t.Days,t.Hours,t.Minutes), p.zCoins);
            p.Message("Bounties collected: {0} coins Bloodmoney collected: {1} coins", p.BountyCollected, p.BloodCoins);
            if (p.Bounty > 0)
                p.Error("Bounty on your head: {0} coins", p.Bounty);
            return true;
        }
        
    }
}
