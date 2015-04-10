using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public interface IServerConnection
    {

        IPlayersManager AllPlayers { get; }
        bool IsConnected { get; }
        bool CommandsDisabled { get; set; }

        void PrivateMessage(IPlayer p, string msg, params object[] args);
        void PublicMessage(string msg, params object[] args);
        void Execute(string cmd, params object[] args);
    }
}
