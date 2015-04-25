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
            CommandHelp = "R:Cmd.Help.Help";
            CommandName = "R:Cmd.Help.Command";
        }
       
        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {                 
            if ( args.Length == 1)
            {
                ListCommands(server,p);
                return true;
            }
            string command = args[1];
            ICommand cmd = CommandManager.FindCommand(command);
            if (cmd == null)
            {
                // Check for Info
                string info = Localizer.GetPlayerLocalization(p, "Info." + command);
                if (!String.IsNullOrEmpty(info))
                {
                    p.Message(info);
                    return true;
                }

                p.Error(MESSAGES.ERR_NOSUCHCOMAND, command);
                return true;
            }           
            if (cmd.CommandLevel > p.AdminLevel)
            {
                p.Error(MESSAGES.ERR_NOSUCHCOMAND, command);
                return true;
            }
            p.Message("R:Cmd.Help.HelpCommand", p.Localize(cmd.CommandName));
            p.Message(cmd.CommandHelp);
            if (!String.IsNullOrEmpty(cmd.CommandUsage))
                p.Message("R:Cmd.Help.HelpUsage", p.Localize(cmd.CommandUsage));
            if (cmd.CommandCost > 0 )
                p.Message("R:Cmd.Help.HelpCost", cmd.CommandCost);
            if (cmd.CommandCoolDown > 0)
                p.Message("R:Cmd.Help.HelpCooldown", cmd.CommandCoolDown);
            if (cmd.CommandAliases.Length > 0)
                p.Message("R:Cmd.Help.HelpAliases", String.Join(", ", cmd.CommandAliases));
            return true;
        }

        private void ListCommands(IServerConnection server, IPlayer p)
        {
            IEnumerable<ICommand> cmds = CommandManager.AllCommands.Values;     

            string s = "R:Cmd.Help.Available";
            SortedSet<string> set = new SortedSet<string>();
            foreach (var cmd in cmds)
            {
                if ((cmd.CommandLevel > 0) || (cmd is InfoCommand))
                    continue;

                set.Add(Localizer.Localize(p, cmd.CommandName));
            }
            if (set.Count > 0)
                p.Message(s, String.Join(", ", set.ToArray()));
            if (p.IsAdmin)
            {
                s = "R:Cmd.Help.AvailableAdmin";
                set = new SortedSet<string>();
                foreach (var cmd in cmds)
                {
                    if ((cmd.CommandLevel > p.AdminLevel) || (cmd is InfoCommand) || (cmd.CommandLevel == 0))
                        continue;
                    set.Add(Localizer.Localize(p, cmd.CommandName));
                }
                if (set.Count > 0)
                    p.Error(s, String.Join(", ", set.ToArray()));
            }
            s = "R:Cmd.Help.AvailableInfo";
            set = new SortedSet<string>();
            foreach (var key in Localizer.GetSubkeysStartingWith(p,"Info."))
            {                
                set.Add(key);
            }
            if (set.Count > 0)
                p.Message(s, String.Join(", ", set.ToArray()));
        }
       
    }
}
