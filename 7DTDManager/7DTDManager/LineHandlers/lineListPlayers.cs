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
        static Regex rgLPEnd = new Regex("Total of (?<numplayers>[0-9]+) in the game");

        private List<IPlayer> found = new List<IPlayer>();
        bool IsFirst = true;
        int countPlayers = 0;

        public override bool ProcessLine(IServerConnection serverConnection, string currentLine)
        {
            if (rgLPLine.IsMatch(currentLine))
            {
                if (IsFirst)
                {
                    countPlayers = 0;
                    found = new List<IPlayer>();
                    IsFirst = false;
                    PriorityProcess = true;
                }
                countPlayers++;
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
                found.Add(p);
                //logger.Info("LP line {0} {1}", p.Name,p.EntityID);
                return true;
            }
            if (rgLPEnd.IsMatch(currentLine))
            {
                IsFirst = true;
                Match match = rgLPEnd.Match(currentLine);
                GroupCollection groups = match.Groups;

                PriorityProcess = false;
                IsFirst = true;
                int targetNum = Int32.Parse(groups["numplayers"].Value);
                if (countPlayers != targetNum)
                {
                    logger.Warn("Number mismatch in ListPlayers! {0} != {1}", targetNum, countPlayers);
                }
                CleanupPlayers(serverConnection);
                logger.Info("ListPlayers Parsing done.");       
            }
            return false;
        }

        private void CleanupPlayers(IServerConnection server)
        {
            foreach (var checkPlayer in server.AllPlayers.Players)
            {
                IPlayer pOnline = (from p in found where p.SteamID == checkPlayer.SteamID select p).FirstOrDefault();
                if ((pOnline == null) && (checkPlayer.IsOnline))
                {
                    logger.Warn("Logout for {0} missed. Fixing.", checkPlayer.Name);
                    checkPlayer.Logout();
                }
            }
        }
        
    }
}
