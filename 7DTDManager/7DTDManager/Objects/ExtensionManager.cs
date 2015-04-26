using _7DTDManager.Commands;
using _7DTDManager.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Objects
{
    public static class ExtensionManager
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        public static List<Assembly> AllExtensions = new List<Assembly>();
        public static List<IExtension> AllExtensionInterfaces = new List<IExtension>();

        static List<string> loadedDLLs = new List<string>();

        public static void LoadExtensions(IServerConnection server)
        {
            logger.Info("Loading Extensions ...");
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

                    logger.Info("Checking {0}", x.FullName);
                    if (x.FullName.Contains("PublicKeyToken=null"))
                    {
                        logger.Warn("No strong name. Loading skipped....");
                        x = null;
                        continue;
                    }
                    foreach (var t in x.GetExportedTypes())
                    {
                        if (t.GetInterfaces().Contains(typeof(IExtension)))
                        {
                            if (t.IsAbstract)
                                continue;
                            logger.Info("Loading Extension {0} from {1}", t.FullName, x.FullName);
                            IExtension ex = Activator.CreateInstance(t) as IExtension;
                            ILogger l = LogManager.GetLogger(t.ToString(), typeof(ExtensionLogger)) as ILogger;
                            ex.InitializeExtension(server, l);
                            logger.Info("Extension {0} Author {1} WebSite {2} Version {3}", ex.Name, ex.Author, ex.WebSite, ex.Version);
                            AllExtensions.Add(x);
                            AllExtensionInterfaces.Add(ex);
                            loadedDLLs.Add(file);
                            break;
                        }
                    }                                      
                }
                catch (Exception ex)
                {
                    logger.Error("Error loading {0}: {1}", file, ex.ToString());
                }
            }
        }
    }

    public class ExtensionLogger : Logger, ILogger
    {

    }
}
