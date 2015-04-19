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
    public class lineListLandProtection : IServerLineHandler,ICalloutCallback
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        static Regex rgLLPStart = new Regex("Player \"(?<name>.*) \\((?<steamid>[0-9]+)\\)\" owns (?<keystones>[0-9]+) keystones.*");
        static Regex rgLLPLine = new Regex("   \\((?<pos>.*)\\)");
        static Regex rgLLPEnd = new Regex("Total of (?<numstones>[0-9]+) keystones in the game");

        private IPlayer currentPlayer = null;
        bool IsFirst = true;
        int countStones = 0;

        public bool ProcessLine(IServerConnection serverConnection, string currentLine)
        {
            if (rgLLPStart.IsMatch(currentLine)) // Start of ListLandProtections deteced
            {
                PriorityProcess = true;

                if (IsFirst)
                    countStones = 0;
                Match match = rgLLPStart.Match(currentLine);
                GroupCollection groups = match.Groups;

                IPlayer p = serverConnection.AllPlayers.FindPlayerBySteamID(groups["steamid"].Value);
                currentPlayer = p;
                if (p != null)
                {
                    logger.Debug("Parsing LandProtections of {0}", p.Name);                
                }                
                return true;
            }
            if (rgLLPLine.IsMatch(currentLine))
            {
                Match match = rgLLPStart.Match(currentLine);
                GroupCollection groups = match.Groups;
                IPosition newLP = serverConnection.CreatePosition(groups["pos"].Value);
                if ( (newLP != null) && (currentPlayer != null))
                {
                    currentPlayer.LandProtections.Add(newLP);
                    countStones++;
                }
                return true;
            }
            if (rgLLPEnd.IsMatch(currentLine))
            {
                Match match = rgLLPStart.Match(currentLine);
                GroupCollection groups = match.Groups;

                PriorityProcess = false;
                IsFirst = true;
                int targetNum = Int32.Parse(groups["numstones"].Value);
                if (countStones != targetNum)
                {
                    logger.Warn("Number mismatch in ListLandProtection!");
                }
                return true;
            }
            return false;
        }

        public bool PriorityProcess
        {
            get;
            set;
        }

       

        public void CalloutCallback(ICallout c, IServerConnection serverConnection)
        {
            serverConnection.Execute("llp");
        }

        public void Init(IServerConnection serverConnection)
        {
            
        }
    }
}
