using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _7DTDManager.LineHandlers
{
    public class linePlayerLogout : BaseLineHandler
    {
        static Regex rgPlayerLeave = new Regex(".*disconnected:.*, EntityID=(?<entityid>[0-9]+), PlayerID='(?<steamid>[0-9]+)',.*, PlayerName='(?<name>.*)'.*");
        public override bool ProcessLine(IServerConnection serverConnection, string currentLine)
        {
            if (rgPlayerLeave.IsMatch(currentLine))
            {
                Match match = rgPlayerLeave.Match(currentLine);
                GroupCollection groups = match.Groups;

                IPlayer p = serverConnection.AllPlayers.AddPlayer(groups["name"].Value, groups["steamid"].Value, groups["entityid"].Value);
                if (p.IsOnline)
                {                    
                    p.Logout();
                }
                serverConnection.Execute("lp");
                return true;
            }
            return false;
        }

        

    }
}
