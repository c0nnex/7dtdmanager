using System;
using System.Collections.Generic;
namespace _7DTDManager.Interfaces
{
    public interface IConfiguration
    {
        string ServerHost { get; }
        int ServerTelnetPort { get; }

        int CoinsPerMinute { get; }
        int CoinsPerZombiekill { get; }
        int CoinLossPerDeath { get; }
        double CoinPercentageOnKill { get; }
        
        int BanAfterKicks { get; }
        string BanDuration { get; }
        double BountyFactor { get; }

        bool IsDirty { get; set; }
        int MaxPingAccepted { get; }
        double MinimumDistanceForPayday { get; }
        string MOTD { get; set; }
        int PaydayInterval { get; }
        int PollInterval { get; }
        int PositionInterval { get; }
        double PositionTrackingRangeFactor { get; }

        IAdminList Admins { get; }
        IReadOnlyList<IShop> Shops { get; }

        void Save(bool force = false);
    }

    public interface IAdminList : IReadOnlyList<IAdminEntry>
    {
        void AddAdmin(string steamid, int adminlevel);
        void RemoveAdmin(string steamid);
        bool IsAdmin(string steamid);
    }

    public interface IAdminEntry
    {
        int AdminLevel { get; set; }
        string SteamID { get;  }
    }
}
