using System;
using System.Collections.Generic;
namespace _7DTDManager.Interfaces
{
    public delegate void PlayerMovedDelegate(object sender, PlayerMovementEventArgs e);
    public delegate void PlayerPositionUpdateDelegate(object sender, PlayerPositionUpdateEventArgs e);

    public interface IPlayer
    {
        string Name { get; }
        string SteamID { get; }
        string EntityID { get; }
        string IPAddress { get; }
        string Language { get;  }
        int Age { get; }
        int Ping { get; }
        

        int zCoins { get; }
        int Bounty { get; }
        int BloodCoins { get; }
        int BountyCollected { get; }
        int Spent { get; }
        double DistanceTravelled { get; }

        int ZombieKills { get; }
        int Deaths { get; }
        int PlayerKills { get; }

        bool IsAdmin { get; }
        int AdminLevel { get; }
        bool IsOnline { get; }
        int PingKicks { get;  }

        IPosition CurrentPosition { get; }
        IPosition HomePosition { get; }
        
        DateTime FirstLogin { get; }        
        DateTime LastLogin { get; }
        DateTime LastPayday { get; }

        IExposedList<IPlayer> Friends { get; }
        IMailbox Mailbox { get; }
        IExposedList<IAreaDefiniton> LandProtections { get; }
        IPlayer ProxyPlayer { get; set; }
        IPlayer ExecuteAs { get; set; }

        event PlayerMovedDelegate PlayerMoved;

        /// <summary>
        /// Raised when the PlayerPosition was updated. 
        /// Note: This is a onetime event! After it was raised the Eventdelegate will be removed from the list!
        /// </summary>
        event PlayerPositionUpdateDelegate PlayerPositionUpdated;

        void Login();
        void Logout();
        void Recalc();
        int AddCoins(int howmuch, string why = "unknown");
        void UpdateStats(int deaths, int zombies, int players, int ping);
        void UpdatePosition(string pos);
        void UpdateHomePosition(string pos);
        void UpdateHomePosition(IPosition newHome);
        void Message(string msg, params object[] args);
        void Error(string msg, params object[] args);
        void Confirm(string msg, params object[] args);

        bool CanExecute(ICommand cmd);
        int GetCoolDown(ICommand cmd);
        void SetCoolDown(ICommand cmd);
        void ClearCooldowns();
       
        void AddBounty(int howmuch, string why);
        void CollectBounty(int howmuch, string why);
        void AddBloodCoins(int howmuch, string why);
        void ClearBounty();

        bool IsFriendOf(IPlayer other);

        void SetCurrentShop(IShop whichShop);
        IShop GetCurrentShop();

        void ClearPingKicks();
        void Dirty();
        void SetIPAddress(string ip);
        void SetLanguage(string lang);
        string Localize(string key, params object[] args);
    }

    public class PlayerMovementEventArgs : EventArgs
    {
        public IPosition OldPosition;
        public IPosition NewPosition;       
    }

    public class PlayerPositionUpdateEventArgs : EventArgs
    {
        public IPosition NewPosition;
    }

    public interface IMailMessage
    {
        string FromSteamID { get;  }
        DateTime When { get;  }
        string Message { get;  }
    }

    public interface IMailbox
    {
        IReadOnlyList<IMailMessage> Mails { get; }
        void AddMail(IPlayer from, string message);
        void RemoveMail(IMailMessage mail);
        void Clear();
    }
}
