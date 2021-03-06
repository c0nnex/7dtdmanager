﻿using _7DTDManager.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _7DTDManager.LineHandlers
{
    public class lineListLandProtection : BaseLineHandler,ICalloutCallback
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        static Regex rgLLPStart = new Regex("Player \"(?<name>.*) \\((?<steamid>[0-9]+)\\)\" owns (?<keystones>[0-9]+) keystones.*");
        static Regex rgLLPLine = new Regex("   \\((?<pos>.*)\\)");
        static Regex rgLLPEnd = new Regex("Total of (?<numstones>[0-9]+) keystones in the game");

        private IPlayer currentPlayer = null;
        private List<IAreaDefiniton> found = null;
        bool IsFirst = true;
        
        int countStones = 0;

        public override bool ProcessLine(IServerConnection serverConnection, string currentLine)
        {
            if (rgLLPStart.IsMatch(currentLine)) // Start of ListLandProtections deteced
            {
                PriorityProcess = true;

                if (IsFirst)
                {
                    countStones = 0;
                    found = new List<IAreaDefiniton>();  
                }
                
                Match match = rgLLPStart.Match(currentLine);
                GroupCollection groups = match.Groups;

                IPlayer p = serverConnection.AllPlayers.FindPlayerBySteamID(groups["steamid"].Value);
                currentPlayer = p;
                
                if (p != null)
                {
                    logger.Debug("Parsing LandProtections of {0}", p.Name);                                      
                }
                else
                {
                    logger.Debug("Player {0} unknown! {1}", groups["name"].Value,currentLine);
                }
                IsFirst = false;
                return true;
            }
            if (rgLLPLine.IsMatch(currentLine))
            {
                Match match = rgLLPLine.Match(currentLine);
                GroupCollection groups = match.Groups;
                IPosition newLP = serverConnection.CreatePosition(groups["pos"].Value);
                if ( (newLP != null) && (currentPlayer != null))
                {
                    
                    IAreaDefiniton protection = (from p in currentPlayer.LandProtections.Items where p.Center.Equals(newLP) select p).FirstOrDefault();
                    if (protection == null)
                    {
                        protection = serverConnection.CreateArea(currentPlayer,newLP,10.0);
                        currentPlayer.LandProtections.Add(protection);                        
                    }
                    found.Add(protection);
                    countStones++;
                }
                return true;
            }
            if (rgLLPEnd.IsMatch(currentLine))
            {
                Match match = rgLLPEnd.Match(currentLine);
                GroupCollection groups = match.Groups;

                PriorityProcess = false;
                IsFirst = true;
                int targetNum = Int32.Parse(groups["numstones"].Value);
                if (countStones != targetNum)
                {
                    logger.Warn("Number mismatch in ListLandProtection! {0} != {1}",targetNum,countStones);
                }
                logger.Info("LandProtection Parsing done.");                
                CleanupProtections(serverConnection);
                serverConnection.AllPlayers.Save(true);
                return true;
            }
            return false;
        }

        private void CleanupProtections(IServerConnection server)
        {
            foreach (var checkPlayer in server.AllPlayers.Players)
            {
                IAreaDefiniton[] oldList = checkPlayer.LandProtections.Items.ToArray();
                foreach (var item in oldList)
                {
                    IAreaDefiniton protection = (from p in found where p.Center == item.Center select p).FirstOrDefault();
                    if (protection == null)
                    {
                        logger.Info("KeyStone {0} ({1}) destroyed ({2})", item.Identifier, item.Center.ToString(), checkPlayer.Name);
                        item.OnDestroy();
                        checkPlayer.LandProtections.Remove(item); //TODO: Does not remove item?
                    }
                }
                
            }
        }
       

        public bool CalloutCallback(ICallout c, IServerConnection serverConnection)
        {
            serverConnection.Execute("llp");
            return true;
        }

        public override void Init(IServerConnection serverConnection, ILogger logger) 
        {
            base.Init(serverConnection, logger);
            // Update LandProtections every 5 Minutes
            serverConnection.CalloutManager.AddCallout(this, this, new TimeSpan(0, 5, 0), true);
            serverConnection.Execute("llp");
        }
    }
}
