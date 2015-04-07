using _7DTDManager.Commands;
using _7DTDManager.Interfaces;
using NLog;
using NLog.Config;
using NLog.Targets;
using NLog.Targets.Wrappers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _7DTDManager
{
    class Program
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        public static String ApplicationDirectory
        {
            get { return _ApplicationDirectory ?? (_ApplicationDirectory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location)); }
        } private static String _ApplicationDirectory;

        public static Manager server;
        public static Configuration config;

        static Player ServerPlayer = new ServerPlayer { Name = "ServerPlayer" };

        static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

            ConfigureLogging();
            config = Configuration.Load();
            if (config == null)
            {
                Console.WriteLine("Configuration not found. A default-config has been created for you. Please change to your needs and restart the application.");
                return;
            }

            server = new Manager();

            server.ConnectToServer();
            while (1 == 1)
            {
                server.Process();
                if (Console.KeyAvailable)
                {
                    string cline = Console.ReadLine();
                    if (cline == "exit")
                    {
                        server.allPlayers.Save();
                        LogManager.Flush();
                        return;
                    }
                    else
                    {
                        string[] largs = cline.ToLowerInvariant().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        if (!CommandManager.AllCommands.ContainsKey(largs[0]))
                        {
                            Console.WriteLine("Unknown command");
                            continue;
                        }
                        ICommand cmd = CommandManager.AllCommands[largs[0]];
                        bool res = cmd.Execute(server, ServerPlayer, largs);
                    }
                }
            }
        }

        private static void ConfigureLogging()
        {
            bool bBuiltIn = false;
            string logPath = Path.Combine(ApplicationDirectory, "logs");

            Directory.CreateDirectory(logPath);

           
            try { File.Delete(Path.Combine(logPath, "7dtdmanager.log.20")); }
            catch { }
            for (int i = 19; i >= 0; i--)
            {
                try { File.Move(Path.Combine(logPath, "7dtdmanager.log." + i.ToString()), Path.Combine(logPath, "7dtdmanager.log." + (i + 1).ToString())); }
                catch { }

            }
            try { File.Move(Path.Combine(logPath, "7dtdmanager.log"), Path.Combine(logPath, "7dtdmanager.log.0")); }
            catch { }

            if (!File.Exists(Path.Combine(ApplicationDirectory, "nlog.config")))
            {
                LoggingConfiguration config = new LoggingConfiguration();


                FileTarget fileTarget = new FileTarget();

                config.AddTarget("file", fileTarget);
                fileTarget.FileName = "${basedir}/logs/7dtdmanager.log";
                fileTarget.Encoding = Encoding.UTF8;
                fileTarget.Layout = "${longdate}|${level:uppercase=true}|${threadid}|${logger}|${message}";

               // AsyncTargetWrapper asyncFileTarget = new AsyncTargetWrapper(fileTarget, 5000, AsyncTargetWrapperOverflowAction.Discard);

                NLogViewerTarget nlogTarget = new NLogViewerTarget();
                nlogTarget.Address = "udp://127.0.0.1:9999";

                config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, fileTarget));
                config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, new ConsoleTarget()));
               // config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, nlogTarget));
                //config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, nlogTarget));
                LogManager.Configuration = config;
                bBuiltIn = true;
            }
            LogManager.EnableLogging();
            logger = LogManager.GetCurrentClassLogger();
            logger.Info(">>> START 7dtdmanager Version {0}", Assembly.GetEntryAssembly().FullName);
            logger.Info("Using {0} configuration", (bBuiltIn ? "Builtin" : "Configfile"));
        }
    }
}
