using _7DTDManager.Interfaces;
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
        static List<string> loadedDLLs = new List<string>();

        public static object lockObject = new Object();

        static LineManager()
        {
            
        }

        public static void Init(IServerConnection serverConnection)
        {
            lock (lockObject)
            {
                RegisterLineHandlers(System.Reflection.Assembly.GetExecutingAssembly(), serverConnection);
            }
        }

        public static void LoadLineHandlers(IServerConnection serverConnection)
        {
            logger.Info("Loading LineHandlers ...");
            String path = System.IO.Path.Combine(Program.ApplicationDirectory, "ext");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (var file in Directory.EnumerateFiles(path, "*.dll"))
            {
                try
                {
                    if (loadedDLLs.Contains(file))
                        continue;
                    Assembly x = Assembly.LoadFile(file);
                    /*
                    if (x.FullName.Contains("PublicKeyToken=null"))
                    {
                        logger.Warn("Assembly {0} has no strong name. Loading skipped....", x.FullName);
                        x = null;
                        continue;
                    }*/
                    logger.Info("Loading Commands from {0}", x.FullName);
                    RegisterLineHandlers(x,serverConnection);
                    loadedDLLs.Add(file);

                }
                catch (Exception ex)
                {
                    logger.Error("Error loading {0}: {1}", file, ex.ToString());
                }
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
                    logger.Info("Loading LineHandler {0} from {1}", t.FullName, x.FullName);
                    IServerLineHandler ex = Activator.CreateInstance(t) as IServerLineHandler;
                    ILogger l = LogManager.GetLogger(t.ToString(), typeof(ExtensionLogger)) as ILogger;
                    ex.Init(serverConnection,l);
                    allHandlers.Add(ex);                                        
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
