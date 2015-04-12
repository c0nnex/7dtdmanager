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

                IPlayer p = serverConnection.AllPlayers.FindPlayerByName(name);
                if (p == null)
                {
                    logger.Info("Servercommand {0} Player {1} NOT FOUND", msg, name);
                    return true;
                }
                logger.Info("Servercommand {0} Player {1}", msg, name);
                if (!CommandManager.AllCommands.ContainsKey(args[0]))
                {
                    p.Message("Unknown Command");
                    return true;
                }
                if ( serverConnection.CommandsDisabled && !p.IsAdmin )
                {
                    p.Message("Commands are currently disabled.");
                    return true;
                }
                if ( !Program.Config.Commands.IsEnabled(args[0]))
                {
                    p.Message("Command '{0}' is currently disabled.",args[0]);
                    return true;
                }
                ICommand cmd = CommandManager.AllCommands[args[0]];
                if  (cmd.cmdLevel > p.AdminLevel )
                {
                    p.Message("Unknown Command");
                    return true;
                }
                if (cmd is InfoCommand)
                {
                    p.Message("Unknown Command");
                    return true;
                }

                bool bCoolDown = false;
                if (cmd.cmdCoolDown > 0)
                {
                    if (!p.CanExecute(cmd))
                    {

                        p.Message("You will need to wait another {0} Minutes before you can use this command again.", p.GetCoolDown(cmd));
                        if (!p.IsAdmin)
                            return true;
                    }
                    bCoolDown = true;
                }
                if (((cmd.cmdCost > 0) && (p.zCoins < cmd.cmdCost)))
                {
                    p.Message("Not enough coins ({0}) for this command.", cmd.cmdCost);
                    if (!p.IsAdmin) 
                        return true;
                }
                if (cmd.Execute(serverConnection, p,args))
                {
                    p.AddCoins((-1) * cmd.cmdCost,"CommandCost "+args[0]);
                    if (bCoolDown)
                        p.SetCoolDown(cmd);
                }
                return true;
            }
            return false;
        }
    }
}
