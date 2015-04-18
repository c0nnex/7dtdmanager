using System;
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

        
        bool Execute(IServerConnection server, IPlayer p, params string[] args);
        
    }
}
