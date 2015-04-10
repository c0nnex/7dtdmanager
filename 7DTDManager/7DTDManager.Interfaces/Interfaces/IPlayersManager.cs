using System;
using System.Collections.Generic;
namespace _7DTDManager.Interfaces
{
    public interface IPlayersManager
    {
        IReadOnlyList<IPlayer> Players { get; }

        IPlayer AddPlayer(string name, string steamid, string entityid);
        IPlayer FindPlayerByName(string name, bool onlyonline = true);
        void Save(bool force=false);
    }
}
