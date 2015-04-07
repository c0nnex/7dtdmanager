
using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public static class CommandManager
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        static Dictionary<string, ICommand> allCommands = new Dictionary<string, ICommand>();

        public static IReadOnlyDictionary<string, ICommand> AllCommands
        {
            get { return allCommands as IReadOnlyDictionary<string, ICommand>; }
        }

        static CommandManager()
        {
            
        }

        public static void Init()
        {
            allCommands["coins"] = new InfoCommand("You will get 1 coin for 1 minute playtime (not idletime!) and 5 coins per zombie slain.");

            RegisterCommandHandlers(System.Reflection.Assembly.GetExecutingAssembly());
        }

        public static void RegisterCommandHandlers(Assembly x)
        {
            foreach (var t in x.GetExportedTypes())
            {
                if (t.GetInterfaces().Contains(typeof(ICommand)))
                {
                    if (t.IsAbstract)
                        continue;
                    logger.Info("Loading Command {0} from {1}", t.FullName, x.FullName);
                    ICommand ex = Activator.CreateInstance(t) as ICommand;

                    allCommands[ex.cmd] = ex;
                    
                }
            }
        }
        
    }
}
