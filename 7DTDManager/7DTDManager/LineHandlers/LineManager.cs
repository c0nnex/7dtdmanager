using _7DTDManager.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public static class LineManager
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        static List<IServerLineHandler> allHandlers = new List<IServerLineHandler>();



        static LineManager()
        {
            
        }

        public static void Init()
        {
            RegisterCommandHandlers(System.Reflection.Assembly.GetExecutingAssembly());
        }

        public static void RegisterCommandHandlers(Assembly x)
        {
            foreach (var t in x.GetExportedTypes())
            {
                if (t.GetInterfaces().Contains(typeof(IServerLineHandler)))
                {
                    if (t.IsAbstract)
                        continue;
                    logger.Info("Loading Command {0} from {1}", t.FullName, x.FullName);
                    IServerLineHandler ex = Activator.CreateInstance(t) as IServerLineHandler;

                    allHandlers.Add(ex);                    
                    
                }
            }
        }

        public static void ProcessLine(IServerConnection serverConnection, string currentLine)
        {
            foreach (var item in allHandlers)
            {
                if (item.ProcessLine(serverConnection, currentLine))
                    return;
            }
        }
    }
}
