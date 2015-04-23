using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
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
                string s = "Available commands:";
                foreach (var item in cmds.Keys)
                {
                    cmd = cmds[item];
                    if ((cmd.CommandLevel > 0) || (cmd is InfoCommand))
                        continue;
                    if (cmd.CommandName != item) // hide aliases
                        continue;
                    s += " /" + item + ",";
                }
                p.Message(s);
                if (p.IsAdmin)
                {
                    s = "Admin commands:";
                    foreach (var item in cmds.Keys)
                    {
                        cmd = cmds[item];
                        if ((cmd.CommandLevel > p.AdminLevel) || (cmd is InfoCommand) || (cmd.CommandLevel == 0))
                            continue;
                        if (cmd.CommandName != item) // hide aliases
                            continue;
                        s += " /" + item + ",";
                    }
                    p.Error(s);
                }
                s = "Available Help-Information:";
                foreach (var item in cmds.Keys)
                {
                    cmd = cmds[item];
                    if ((cmd.CommandLevel > p.AdminLevel ) || !(cmd is InfoCommand))
                        continue;
                    if (cmd.CommandName != item) // hide aliases
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
            if (cmd.CommandLevel > p.AdminLevel)
            {
                p.Message("No such command '{0}'", args[1]);
                return true;
            }
            p.Message("Command '{0}':", cmd.CommandName);
            p.Message(cmd.CommandHelp);
            if (!String.IsNullOrEmpty(CommandUsage))
                p.Message("Usage: {0}",cmd.CommandUsage);
            if (cmd.CommandCost > 0 ) 
                p.Message("Cost: {0} coins.", cmd.CommandCost);
            if (cmd.CommandCoolDown > 0)
                p.Message("Cooldown: {0} minutes.", cmd.CommandCoolDown);
            if (cmd.CommandAliases.Length > 0)
                p.Message("Aliases: {0}", String.Join(", ", cmd.CommandAliases));
            return true;
        }
       
    }
}
