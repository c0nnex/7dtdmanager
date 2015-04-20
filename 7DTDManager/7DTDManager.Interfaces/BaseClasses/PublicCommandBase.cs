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
        protected ILogger Log = null;

        public int _CommandCost = 0, _CommandCoolDown = 0, _CommandLevel = 0, _CommandArgs = 0;
        public string _CommandHelp = "No help available", _CommandName = "noname", _CommandUsage = "";
        public bool _CommandIsInfo = false;
        public string[] _CommandAliases = new string[] { };

        public int CommandCost
        {
            get { return _CommandCost; }
            set { _CommandCost = value; }
        }

        public int CommandCoolDown
        {
            get { return _CommandCoolDown; }
            set { _CommandCoolDown = value; }
        }

        public string CommandHelp
        {
            get { return _CommandHelp; }
            set { _CommandHelp = value; }
        }

        public int CommandLevel
        {
            get { return _CommandLevel; }
            set { _CommandLevel = value; }
        }

        public string CommandName
        {
            get { return _CommandName; }
            set { _CommandName = value; }
        }

        public bool IsInfoCommand
        {
            get { return _CommandIsInfo; }
            set { _CommandIsInfo = value; }
        }

        public int CommandArgs
        {
            get { return _CommandArgs; }
            set { _CommandArgs = value; }
        }

        public string CommandUsage
        {
            get { return _CommandUsage; }
            set { _CommandUsage = value; }
        }

        public string[] CommandAliases
        {
            get { return _CommandAliases; }
            set { _CommandAliases = value; }
        }

        public PublicCommandBase()
        {

        }

        public virtual bool Execute(IServerConnection server, IPlayer p, params string[] args) { return false; }
        public virtual bool AdminExecute(IServerConnection server, IPlayer p, params string[] args)
        {
            return Execute(server, p, args);
        }

        public void Init(ILogger logger)
        {
            Log = logger;
        }
    }
}
