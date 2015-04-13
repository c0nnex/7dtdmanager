using System;
namespace _7DTDManager.Interfaces
{
    public delegate void PlayerMovedDelegate(object sender, PlayerMovementEventArgs e);

    public interface IPlayer
    {
        string Name { get; }
        string SteamID { get; }
        string EntityID { get; }
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

        event PlayerMovedDelegate PlayerMoved;

        IPosition CurrentPosition { get; }
        IPosition HomePosition { get; }
        
        DateTime FirstLogin { get; }        
        DateTime LastLogin { get; }
        DateTime LastPayday { get; }

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

        bool CanExecute(ICommand cmd);
        int GetCoolDown(ICommand cmd);
        void SetCoolDown(ICommand cmd);
        void ClearCooldowns();
       
        void AddBounty(int howmuch, string why);
        void CollectBounty(int howmuch, string why);
        void AddBloodCoins(int howmuch, string why);


        void ClearBounty();
    }

    public class PlayerMovementEventArgs : EventArgs
    {
        public IPosition OldPosition;
        public IPosition NewPosition;
        //TODO: Movement args
    }
}
