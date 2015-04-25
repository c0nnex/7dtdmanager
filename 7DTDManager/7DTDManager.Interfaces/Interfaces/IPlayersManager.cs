using System;
using System.Collections.Generic;
namespace _7DTDManager.Interfaces
{
    public interface IPlayersManager
    {
        IReadOnlyList<IPlayer> Players { get; }

        event EventHandler PlayerLogin;
        event EventHandler PlayerLogout;
        event EventHandler PlayerSave;
        event AreaEventDelegate AreaInitialize;
       

        IPlayer AddPlayer(string name, string steamid, string entityid);
        IPlayer FindPlayerByNameOrID(string name, bool onlyonline = true);
        IPlayer FindPlayerBySteamID(string id);
        void Save(bool force=false);
    }
}
