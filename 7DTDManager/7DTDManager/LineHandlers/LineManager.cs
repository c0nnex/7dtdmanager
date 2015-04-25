using _7DTDManager.Interfaces;
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
    public static class LineManager
    {
        static Logger logger = LogManager.GetCurrentClassLogger();

        static List<IServerLineHandler> allHandlers = new List<IServerLineHandler>();       

        public static object lockObject = new Object();
        static bool WasInitialized = false;

        static LineManager()
        {
            
        }

        public static void Init(IServerConnection serverConnection)
        {
            lock (lockObject)
            {
                if (!WasInitialized)
                {
                    RegisterLineHandlers(System.Reflection.Assembly.GetExecutingAssembly(), serverConnection);
                    LoadLineHandlers(serverConnection);
                }
                WasInitialized = true;
            }
        }

        public static void LoadLineHandlers(IServerConnection serverConnection)
        {
            logger.Info("Loading LineHandlers ...");           
            foreach (var assembly in ExtensionManager.AllExtensions)
            {                                  
                    RegisterLineHandlers(assembly,serverConnection);                                   
            }
        }

        public static void RegisterLineHandlers(Assembly x, IServerConnection serverConnection)
        {
            foreach (var t in x.GetExportedTypes())
            {
                if (t.GetInterfaces().Contains(typeof(IServerLineHandler)))
                {
                    if (t.IsAbstract)
                        continue;
                    try
                    {
                        logger.Info("Loading LineHandler {0} from {1}", t.FullName, x.FullName);
                        IServerLineHandler ex = Activator.CreateInstance(t) as IServerLineHandler;
                        ILogger l = LogManager.GetLogger(t.ToString(), typeof(ExtensionLogger)) as ILogger;
                        ex.Init(serverConnection, l);
                        allHandlers.Add(ex);
                    }
                    catch (Exception ex)
                    {
                        logger.Error("Error loading {0}: {1}", t.ToString(), ex.ToString());
                    }
                }
            }
        }

        public static void ProcessLine(IServerConnection serverConnection, string currentLine)
        {
            lock (lockObject)
            {
                logger.Trace("Processing: {0}", currentLine);
                // First High prio
                foreach (var item in (from h in allHandlers where h.PriorityProcess == true select h).ToArray())
                {
                    if (item.ProcessLine(serverConnection, currentLine) && item.Exclusive)
                        return;
                }
                foreach (var item in (from h in allHandlers where h.PriorityProcess == false select h).ToArray())
                {
                    if (item.ProcessLine(serverConnection, currentLine) && item.Exclusive)
                        return;
                }
            }
        }
    }
}
