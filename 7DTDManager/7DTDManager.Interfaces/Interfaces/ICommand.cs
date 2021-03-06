﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public interface ICommand
    {
        int CommandCost { get; set; }
        int CommandCoolDown { get; set; }
        string CommandHelp { get; set; }
        string CommandName { get; set; }
        int CommandLevel { get; set; }
        bool IsInfoCommand { get; set; }
        int CommandArgs { get; set; }
        string CommandUsage { get; set; }
        string[] CommandAliases { get; set; }

        void Init(ILogger logger,IMessageLocalizer localizer);
        bool Handles(string command);
        bool Execute(IServerConnection server, IPlayer p, params string[] args);
        bool AdminExecute(IServerConnection server, IPlayer p, params string[] args);
    }
}
