using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands.AdminCommands
{
    public class cmdReload : AdminCommandBase
    {        
        public cmdReload()
        {
            CommandName = "reload";
            CommandHelp = "Reload configuration";
            CommandUsage = "/reload";
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            try
            {
                Log.Info("reloading config");
                Config.Configuration reloadedConfig = Config.Configuration.Load();
                Program.Config = reloadedConfig;               
                p.Message("Configuration reloaded.");
            }
            catch ( Exception ex)
            {
                p.Error("Error reloading config: {0}", ex.Message);
                Log.Error("Error reloading config");
                Log.Error(ex.ToString());
            }
            return true;
        }
    }
}
