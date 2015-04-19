using System;
using System.Collections.Generic;
namespace _7DTDManager.Interfaces
{
    public interface IPlayersManager
    {
        IReadOnlyList<IPlayer> Players { get; }

        event EventHandler PlayerLogin;
        event EventHandler PlayerLogout;

        IPlayer AddPlayer(string name, string steamid, string entityid);
        IPlayer FindPlayerByName(string name, bool onlyonline = true);
        IPlayer FindPlayerBySteamID(string id);
        void Save(bool force=false);
    }
}
