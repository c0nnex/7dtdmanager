using _7DTDManager.Commands;
using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _7DTDManager.LineHandlers
{
    public class lineServerCommand : IServerLineHandler
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        static Regex rgGMSG = new Regex(".*INF GMSG: (?<name>.*): /(?<msg>.*)");

        public bool ProcessLine(IServerConnection serverConnection, string currentLine)
        {
            if (rgGMSG.IsMatch(currentLine))
            {
                Match match = rgGMSG.Match(currentLine);
                GroupCollection groups = match.Groups;
                string msg = groups["msg"].Value;
                string[] args = groups["msg"].Value.ToLowerInvariant().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                string name = groups["name"].Value;
                string command = (args.Length > 0) ? args[0] : "unknown";

                IPlayer p = null;
                if (name != "Server")
                    p = serverConnection.AllPlayers.FindPlayerByName(name);
                else
                    p = Program.ServerPlayer;

                if (p == null)
                {
                    logger.Info("Servercommand {0} Player {1} NOT FOUND", msg, name);
                    return true;
                }
                logger.Info("Servercommand {0} Player {1}", msg, name);
                if (!CommandManager.AllCommands.ContainsKey(command))
                {
                    p.Message("Unknown Command");
                    return true;
                }
                if ((serverConnection != null) && serverConnection.CommandsDisabled && !p.IsAdmin)
                {
                    p.Message("Commands are currently disabled.");
                    return true;
                }
                if (!Program.Config.Commands.IsEnabled(command))
                {
                    p.Message("Command '{0}' is currently disabled.", command);
                    return true;
                }
                ICommand cmd = CommandManager.AllCommands[command];
                if (cmd.CommandLevel > p.AdminLevel)
                {
                    p.Message("Unknown Command");
                    return true;
                }
                if (cmd is InfoCommand)
                {
                    p.Message("Unknown Command");
                    return true;
                }
                if (args.Length < cmd.CommandArgs + 1)
                {
                    p.Error("Usage: " + cmd.CommandUsage);
                    return true;
                }
                bool bCoolDown = false;
                if ( (cmd.CommandCoolDown > 0) && !p.IsAdmin)
                {
                    if (!p.CanExecute(cmd))
                    {

                        p.Message("You will need to wait another {0} Minutes before you can use this command again.", p.GetCoolDown(cmd));
                        return true;
                    }
                    bCoolDown = true;
                }

                if (((cmd.CommandCost > 0) && (p.zCoins < cmd.CommandCost)) && (!p.IsAdmin))
                {
                    p.Message("Not enough coins ({0}) for this command.", cmd.CommandCost);
                    return true;
                }
                if (p.IsAdmin)
                {
                    cmd.AdminExecute(serverConnection, p, args);
                }
                else
                {
                    if (cmd.Execute(serverConnection, p, args))
                    {
                        p.AddCoins((-1) * cmd.CommandCost, "CommandCost " + command);
                        if (bCoolDown)
                            p.SetCoolDown(cmd);
                    }
                }
                return true;
            }
            return false;
        }

        public bool PriorityProcess
        {
            get { return false; }
        }        

        public void Init(IServerConnection serverConnection)
        {
            
        }
    }
}
