using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces.Commands
{
    public abstract class PublicCommandBase : ICommand
    {
        protected ILogger Log = null;
        protected IMessageLocalizer Localizer = null;

        private int _CommandCost = 0, _CommandCoolDown = 0, _CommandLevel = 0, _CommandArgs = 0;
        private string _CommandHelp = "No help available", _CommandName = "noname", _CommandUsage = "";
        private string _ComandRegex;
        private bool _CommandIsInfo = false;
        private string[] _CommandAliases = new string[] { };
        private Regex _CommandExpression = null;
        private IReadOnlyList<string> _CommandNames = null;

        public string CommandRegex
        {
            get { return _ComandRegex;  }
            set { _ComandRegex = value;  }
        }
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

        public void Init(ILogger logger,IMessageLocalizer localizer)
        {
            Localizer = localizer;
            Log = logger;
            if (!String.IsNullOrEmpty(CommandRegex))
                _CommandExpression = new Regex(CommandRegex, RegexOptions.CultureInvariant);
            if ( Localizer != null)
             _CommandNames = Localizer.CreateLocalizedCommandNames(CommandName);
        }

        public bool Handles(string command)
        {
            command = command.ToLowerInvariant();
            if (_CommandNames == null)
                return command == CommandName;
            if ((CommandAliases != null) && (CommandAliases.Contains(command)))
                return true;
            return _CommandNames.Contains(command);
        }

        public GroupCollection CommandMatch(IPlayer p, string cmdLine)
        {
            if (!_CommandExpression.IsMatch(cmdLine))
                return null;
            return _CommandExpression.Match(cmdLine).Groups;
        }
    }
}
