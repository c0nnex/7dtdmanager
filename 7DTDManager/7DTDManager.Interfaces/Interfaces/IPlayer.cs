using System;
namespace _7DTDManager.Interfaces
{
    public interface IPlayer
    {
        string Name { get; }
        string SteamID { get; }
        string EntityID { get; }
        int Age { get; }
        int Ping { get; }
        

        int zCoins { get; }
        int Spent { get; }
       
        int ZombieKills { get; }
        int Deaths { get; }
        int PlayerKills { get; }

        bool IsAdmin { get; }
        int AdminLevel { get; }
        bool IsOnline { get; }

        
        IPosition CurrentPosition { get; }
        IPosition HomePosition { get; }
        
        DateTime FirstLogin { get; }        
        DateTime LastLogin { get; }
        DateTime LastPayday { get; }

        void Login();
        void Logout();
        void Recalc();
        int AddCoins(int howmany, string why = "unknown");
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

        
    }
}
