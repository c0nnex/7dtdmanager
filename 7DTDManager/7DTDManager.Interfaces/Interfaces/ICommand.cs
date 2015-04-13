using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public interface ICommand
    {
        int cmdCost { get; set; }
        int cmdCoolDown { get; set; }
        string cmdHelp { get; set; }
        string cmd { get; set; }
        int cmdLevel { get; set; }
        bool InfoOnly { get; set; }

        bool Execute(IServerConnection server, IPlayer p, params string[] args);
        
    }
}
