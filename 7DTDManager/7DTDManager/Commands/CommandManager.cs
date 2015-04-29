
using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using _7DTDManager.Localize;
using _7DTDManager.Objects;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
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
            RegisterCommandHandlers(System.Reflection.Assembly.GetExecutingAssembly());
            LoadCommands();
        }

        public static ICommand FindCommand(string command)
        {
            command = command.ToLowerInvariant();
            if (AllCommands.ContainsKey(command))
                return AllCommands[command];
            foreach (var cmd in AllCommands.Values)
            {
                if (cmd.Handles(command))
                    return cmd;                
            }

            return null;
        }
        public static void LoadCommands()
        {
            logger.Info("Loading Commands ...");
            foreach (var assembly in ExtensionManager.AllExtensions)
            {
                RegisterCommandHandlers(assembly);
            }                    
        }

        public static void RegisterCommandHandlers(Assembly x)
        {
            foreach (var t in x.GetExportedTypes())
            {
                if (t.GetInterfaces().Contains(typeof(ICommand)))
                {
                    if (t.IsAbstract)
                        continue;
                    try
                    {
                        logger.Info("Loading Command {0} from {1}", t.FullName, x.GetName().Name);
                        ICommand ex = Activator.CreateInstance(t) as ICommand;

                        allCommands[MessageLocalizer.GetDefaultLocalization(ex.CommandName)] = ex;
                        ILogger l = LogManager.GetLogger(t.ToString(), typeof(ExtensionLogger)) as ILogger;
                        ex.Init(l, MessageLocalizer.Instance);
                        if (!Program.Config.Commands.ContainsCommand(MessageLocalizer.GetDefaultLocalization(ex.CommandName)))
                        {
                            Program.Config.Commands.Add(new Config.CommandConfiguration(ex));
                        }
                        else
                            Program.Config.Commands.UpdateCommand(ex);
                        foreach (var item in ex.CommandAliases)
                        {
                            if (!String.IsNullOrEmpty(item))
                                allCommands[item.ToLowerInvariant()] = ex;
                        }
                        Program.Config.Save(true);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error loading {0}: {1}", t.ToString(), ex.ToString());
                    }
                }
            }
        }
        
    }

    
}
