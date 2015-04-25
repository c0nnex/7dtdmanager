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
        ICalloutManager CalloutManager { get; }
        IConfiguration Configuration { get; }
        IPositionManager PositionManager { get; }
        IShop             GlobalShop { get; }

        bool IsConnected { get; }
        bool CommandsDisabled { get; set; }
        string SavePath { get; }

        void PrivateMessage(IPlayer p, string msg, params object[] args);
        void PublicMessage(string msg, params object[] args);
        void Execute(string cmd, params object[] args);

        IPosition CreatePosition(string pos);
        IAreaDefiniton CreateArea(IPlayer p,IPosition pos, double size = 10.0);
    }
}
