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
        public cmdHome()
        {
            CommandCost = 150;
            CommandCoolDown = 60;
            CommandHelp = "R:Cmd.Home.Help";
            CommandName = "R:Cmd.Home.Command";
        }
         

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            if (!p.HomePosition.IsValid)
            {
                p.Message("R:Cmd.Home.NoHome");
                return false;
            }
            server.Execute("tele {0} {1}", p.EntityID, p.HomePosition.ToCommandString());
            // HACK: Teleport fix?
            Thread.Sleep(2000);            
            server.Execute("tele {0} {1}", p.EntityID, p.HomePosition.ToCommandString());
            return true;
        }
        
    }
}
