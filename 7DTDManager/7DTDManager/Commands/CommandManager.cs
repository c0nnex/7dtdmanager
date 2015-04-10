
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
            allCommands["coins"] = new InfoCommand(String.Format("You will get {0} coin(s) per minute playtime (not idletime!) and {1} coin(s) per zombie slain.",Program.Config.CoinsPerMinute,Program.Config.CoinsPerZombiekill));
            allCommands["death"] = new InfoCommand(String.Format("You will loose {0} coin(s) if you die (not exterminated by another player!)",Program.Config.CoinLossPerDeath));


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
                    if ( !Program.Config.Commands.ContainsCommand(ex.cmd))
                    {
                        Program.Config.Commands.Add(new Config.CommandConfiguration(ex));
                    }
                    Program.Config.Save();
                }
            }
        }
        
    }
}
