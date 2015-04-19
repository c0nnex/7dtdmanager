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
            RegisterLineHandlers(System.Reflection.Assembly.GetExecutingAssembly());
        }

        public static void RegisterLineHandlers(Assembly x)
        {
            foreach (var t in x.GetExportedTypes())
            {
                if (t.GetInterfaces().Contains(typeof(IServerLineHandler)))
                {
                    if (t.IsAbstract)
                        continue;
                    logger.Info("Loading LineHandler {0} from {1}", t.FullName, x.FullName);
                    IServerLineHandler ex = Activator.CreateInstance(t) as IServerLineHandler;

                    allHandlers.Add(ex);                    
                    
                }
            }
        }

        public static void ProcessLine(IServerConnection serverConnection, string currentLine)
        {
            logger.Trace("Processing: {0}", currentLine);
            // First High prio
            foreach (var item in (from h in allHandlers where h.PriorityProcess select h))
            {
                if (item.ProcessLine(serverConnection, currentLine))
                    return;
            }
            foreach (var item in (from h in allHandlers where !h.PriorityProcess select h))
            {
                if (item.ProcessLine(serverConnection, currentLine))
                    return;
            }
        }
    }
}
