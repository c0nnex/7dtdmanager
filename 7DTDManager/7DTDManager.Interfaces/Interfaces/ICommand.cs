using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public interface ICommand
    {
        int cmdCost { get;}
        int cmdCoolDown { get; }
        string cmdHelp { get;  }
        string cmd { get; }
        int cmdLevel { get; }
        bool InfoOnly { get; }

        bool Execute(IServerConnection server, IPlayer p, params string[] args);
        
    }
}
