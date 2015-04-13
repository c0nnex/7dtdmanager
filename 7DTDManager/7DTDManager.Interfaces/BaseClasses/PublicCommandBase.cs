using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces.Commands
{
    public abstract class PublicCommandBase : ICommand
    {
        public int CommandCost=0, CommandCoolDown = 0, CommandLevel = 0;
        public string CommandHelp="No help available",CommandName = "noname";
        public bool CommandIsInfo = false;

        public int cmdCost
        {
            get { return CommandCost; }
            set { CommandCost = value; }
        }

        public int cmdCoolDown
        {
            get { return CommandCoolDown; }
            set { CommandCoolDown = value; }
        }

        public string cmdHelp
        {
            get { return CommandHelp; }
            set { CommandHelp = value; }
        }
        
        public int cmdLevel
        {
            get { return CommandLevel; }
            set { CommandLevel = value; }
        }

        public string cmd
        {
            get { return CommandName; }
            set { CommandName = value; }
        }

        public bool InfoOnly
        {
            get { return CommandIsInfo; }
            set { CommandIsInfo = value; }
        }

        public virtual bool Execute(IServerConnection server, IPlayer p, params string[] args) { return false;}



        
    }
}
