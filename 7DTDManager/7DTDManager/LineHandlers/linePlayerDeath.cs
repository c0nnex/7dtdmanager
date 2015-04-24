using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _7DTDManager.LineHandlers
{
    public class linePlayerDeath : BaseLineHandler
    {
        static Regex rgDeath = new Regex(".*INF GMSG: Player (?<name>.*) died");

        public override bool ProcessLine(IServerConnection serverConnection, string currentLine)
        {
            if (rgDeath.IsMatch(currentLine))
            {
                Match match = rgDeath.Match(currentLine);
                GroupCollection groups = match.Groups;

                IPlayer p = serverConnection.AllPlayers.FindPlayerByNameOrID(groups["name"].Value);
                if (p != null) 
                {
                    p.AddCoins(-100, "Death");
                    p.Message("You lost 100 coins.");
                }
                serverConnection.Execute("lp");
                return true;
            }
            return false;
        }

        

    }
}
