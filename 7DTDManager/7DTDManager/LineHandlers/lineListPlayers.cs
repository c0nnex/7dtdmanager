using _7DTDManager.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _7DTDManager.LineHandlers
{
    public class lineListPlayers : BaseLineHandler
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        static Regex rgLPLine = new Regex("^[0-9]+. id=(?<enityid>[0-9]+), (?<name>.*), pos=\\((?<pos>.*)\\), rot=.*, deaths=(?<deaths>[0-9]+), zombies=(?<zombies>[0-9]+), players=(?<players>[0-9]+), score=.*, steamid=(?<steamid>[0-9]+), ip=(?<ip>.*), ping=(?<ping>[0-9]+)");

        public override bool ProcessLine(IServerConnection serverConnection, string currentLine)
        {
            if (rgLPLine.IsMatch(currentLine))
            {
                Match match = rgLPLine.Match(currentLine);
                GroupCollection groups = match.Groups;
                if (Convert.ToInt32(groups["zombies"].Value) == 0)
                {
                    // logger.Warn("LP PARSE ERROR: Zombies = 0 ");
                    // logger.Warn(currentLine);
                }
                IPlayer p = serverConnection.AllPlayers.AddPlayer(groups["name"].Value, groups["steamid"].Value, groups["enityid"].Value);
                p.Login();
                p.SetIPAddress(groups["ip"].Value);
                p.UpdateStats(Convert.ToInt32(groups["deaths"].Value), Convert.ToInt32(groups["zombies"].Value), Convert.ToInt32(groups["players"].Value), Convert.ToInt32(groups["ping"].Value));
                p.UpdatePosition(groups["pos"].Value);
                
                //logger.Info("LP line {0} {1}", p.Name,p.EntityID);
                return true;
            }
            return false;
        }

        
        
    }
}
