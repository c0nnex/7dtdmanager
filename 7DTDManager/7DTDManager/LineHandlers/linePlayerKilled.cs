using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _7DTDManager.LineHandlers
{
    public class linePlayerKilled : IServerLineHandler
    {
        static Regex rgDeath = new Regex(".*INF GMSG: Player (?<killer>.*) eliminated Player (?<victim>.*)");

        public bool ProcessLine(IServerConnection serverConnection, string currentLine)
        {
            if (rgDeath.IsMatch(currentLine))
            {
                Match match = rgDeath.Match(currentLine);
                GroupCollection groups = match.Groups;

                IPlayer killer = serverConnection.AllPlayers.FindPlayerByName(groups["killer"].Value);
                IPlayer victim = serverConnection.AllPlayers.FindPlayerByName(groups["victim"].Value);
                if ((killer == null) || (victim == null))
                    return false;
                if (killer == victim) 
                {
                    killer.AddCoins(-100, "Death");
                    killer.Message("You lost 100 coins.");                    
                }
                else
                {
                    if (victim.Bounty != 0)
                    {
                        killer.CollectBounty(victim.Bounty, "Bounty on " + victim.Name);
                        killer.Message("You collected the bounty of {0} coins set on {1}'s head.", victim.Bounty, victim.Name);
                        victim.AddBounty((-1) * victim.Bounty, "collected by " + killer.Name);
                        victim.Message("The bounty on your head has been cleared.");
                    }
                    else
                    {
                        if ( (Program.Config.CoinPercentageOnKill > 0 ) && (victim.zCoins > 100))
                        {
                            int howmuch = (int)((victim.zCoins * Program.Config.CoinPercentageOnKill) / 100.0);
                            if (howmuch > 0)
                            {
                                killer.AddBloodCoins(howmuch, "Killed " + victim.Name);
                                killer.Message("You took {0} coins from the dead body of {1}", howmuch, victim.Name);
                                victim.AddCoins((-1) * howmuch, "Killed by " + killer.Name);
                                victim.Message("{0} took {1} coins from your dead body.", killer.Name, howmuch);

                                if (Program.Config.BountyFactor > 0)
                                    killer.AddBounty((int)(howmuch * Program.Config.BountyFactor), "MDK " + victim.Name);

                            }
                        }
                    }
                }
                serverConnection.Execute("lp");
                return true;
            }
            return false;
        }
    }
}
