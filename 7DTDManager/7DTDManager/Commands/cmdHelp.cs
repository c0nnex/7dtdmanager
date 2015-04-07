﻿using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7dtdManager.Commands
{
    public class cmdHelp : PublicCommandBase
    {
        public cmdHelp()
        {
            CommandHelp = "Show help about commands. Usage: /help or /help [command]";
            CommandName = "help";
        }
       
        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            IReadOnlyDictionary<string, ICommand> cmds = CommandManager.AllCommands;
            ICommand cmd;

            if ( args.Length == 1)
            {
                string s = "Available commands: ";
                foreach (var item in cmds.Keys)
                {
                    cmd = cmds[item];
                    if ( (cmd.AdminOnly && !p.IsAdmin) || (cmd is InfoCommand))
                        continue;
                    s += " /" + item + ",";
                }
                p.Message(s);

                s = "Available Help-Information: ";
                foreach (var item in cmds.Keys)
                {
                    cmd = cmds[item];
                    if ((cmd.AdminOnly && !p.IsAdmin) || !(cmd is InfoCommand))
                        continue;
                    s += " " + item + ",";
                }
                p.Message(s);
                return true;
            }
            if (!cmds.ContainsKey(args[1]))
            {
                p.Message("No such command '{0}'", args[1]);
                return true;
            }
            cmd = cmds[args[1]];
            if (cmd.AdminOnly && !p.IsAdmin)
            {
                p.Message("No such command '{0}'", args[1]);
                return true;
            }

            p.Message(cmd.cmdHelp);
            if (cmd.cmdCost > 0 ) 
                p.Message("Cost: {0} coins.", cmd.cmdCost);
            if (cmd.cmdTimelimit > 0)
                p.Message("Cooldown: {0} minutes.", cmd.cmdTimelimit);
            return true;
        }
       
    }
}
