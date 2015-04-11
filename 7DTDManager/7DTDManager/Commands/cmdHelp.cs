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
                    if ((cmd.cmdLevel > 0) || (cmd is InfoCommand))
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
                        if ((cmd.cmdLevel <= p.AdminLevel) || (cmd is InfoCommand))
                            continue;
                        s += " /" + item + ",";
                    }
                    p.Error(s);
                }
                s = "Available Help-Information:";
                foreach (var item in cmds.Keys)
                {
                    cmd = cmds[item];
                    if ((cmd.cmdLevel > p.AdminLevel ) || !(cmd is InfoCommand))
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
            if (cmd.cmdLevel > p.AdminLevel)
            {
                p.Message("No such command '{0}'", args[1]);
                return true;
            }

            p.Message(cmd.cmdHelp);
            if (cmd.cmdCost > 0 ) 
                p.Message("Cost: {0} coins.", cmd.cmdCost);
            if (cmd.cmdCoolDown > 0)
                p.Message("Cooldown: {0} minutes.", cmd.cmdCoolDown);
            return true;
        }
       
    }
}
