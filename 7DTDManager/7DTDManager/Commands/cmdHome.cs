using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdHome : PublicCommandBase
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        public cmdHome()
        {
            CommandCost = 150;
            CommandCoolDown = 60;
            CommandHelp = "Teleports you to your home set with /sethome";
            CommandName = "home";
        }
         

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            if (!p.HomePosition.IsValid())
            {
                p.Message("No homeposition for you recorded. set it with /sethome");
                return false;
            }
            server.Execute("tele {0} {1}", p.EntityID, p.HomePosition.ToCommandString());
            // HACK: Teleport fix?
            Thread.Sleep(500);
            logger.Debug("Second try");
            server.Execute("tele {0} {1}", p.EntityID, p.HomePosition.ToCommandString());
            return true;
        }
        
    }
}
