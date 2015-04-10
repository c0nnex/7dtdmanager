using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces.Commands
{
    public abstract class AdminCommandBase : ICommand
    {
        public int CommandCost = 0, CommandCoolDown = 0;
        public string CommandHelp = "No help available", CommandName = "noname";
        public bool CommandIsInfo = false;

        
        public int cmdCost
        {
            get { return CommandCost; }
        }

        public int cmdCoolDown
        {
            get { return CommandCoolDown; }
        }

        public string cmdHelp
        {
            get { return CommandHelp; }
        }
        
        public bool AdminOnly
        {
            get { return true; }
        }

        public string cmd
        {
            get { return CommandName; }
        }

        public bool InfoOnly
        {
            get { return CommandIsInfo; }
        }

        public virtual bool Execute(IServerConnection server, IPlayer p, params string[] args) { return false;}

    }
}
