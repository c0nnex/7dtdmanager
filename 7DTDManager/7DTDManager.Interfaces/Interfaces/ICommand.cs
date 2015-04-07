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
        int cmdTimelimit { get; }
        string cmdHelp { get;  }
        string cmd { get; }
        bool AdminOnly { get; }
        bool InfoOnly { get; }

        bool Execute(IServerConnection server, IPlayer p, params string[] args);
        
    }
}
