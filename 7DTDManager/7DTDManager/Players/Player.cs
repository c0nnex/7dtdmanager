using _7DTDManager.Interfaces;
using _7DTDManager.Objects;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _7DTDManager.Players
{
    public delegate void PlayerChanged(object sender, EventArgs e);

    [Serializable]
    public class Player : IPlayer
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        [XmlAttribute]
        public string SteamID { get; set; }
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string EntityID { get; set; }

        public DateTime FirstLogin { get; set; }
        public DateTime LastLogin { get; set; }
        public DateTime LastPayday { get; set; }

        public Int32 zCoins { get; set; }
        public Int32 Bounty { get; set; }
        public Int32 BountyCollected { get; set; }
        public Int32 BloodCoins { get; set; }
        public Int32 Age { get; set; }
        public Int32 Spent { get; set; }
        public double DistanceTravelled { get; set; }
        public Position CurrentPosition { get; set; }
        public Position HomePosition { get; set; }

        public string WebPassword { get; set; }

        public int Deaths { get; set; }
        public int PlayerKills { get; set; }
        public int ZombieKills { get; set; }
        public int Ping { get; set; }

        // ShopSystem Stuff
        public List<AreaProtection> AreaProtections { get; set; }

        private int LastDeaths = 0, LastPlayerKills = 0, LastZombieKills = 0;
        private DateTime LastUpdate = DateTime.Now;
        private Position LastPaydayPosition { get; set; }

        [XmlIgnore]
        public bool IsOnline { get; set; }

        public event PlayerChanged Changed;
        public event PlayerMovedDelegate PlayerMoved;

        private void OnChanged()
        {
            PlayerChanged handler = Changed;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void OnPlayerMoved(IPosition oldPos, IPosition newPos)
        {
            PlayerMovedDelegate handler = PlayerMoved;
            if (handler != null)
                handler(this, new PlayerMovementEventArgs { OldPosition = oldPos, NewPosition = newPos });
        }

        public CoolDownList CommandCoolDowns = new CoolDownList();

        public Player()
        {
            Deaths = 0;
            ZombieKills = 0;
            PlayerKills = 0;
            Ping = 0;
            CurrentPosition = Position.InvalidPosition;
            HomePosition = Position.InvalidPosition;
            LastPaydayPosition = Position.InvalidPosition;
            IsOnline = false;
        }

        public Int32 AddCoins(int howmany, string why = "unknown")
        {
            if (howmany == 0)
                return zCoins;
            if (howmany < 0)
                Spent += (-1) * howmany;
            zCoins += howmany;
            if (zCoins < 0)
                zCoins = 0;
            logger.Info("{0} coins Change [{3}]: {1} (new {2})", Name, howmany, zCoins, why);
            OnChanged();
            return zCoins;
        }

        public void Login()
        {
            if (!IsOnline)
            {
                logger.Info("{0} is now online (ZK {1}) Coins: {2}", Name, ZombieKills, zCoins);
                LastLogin = DateTime.Now;
                LastPayday = DateTime.Now;
                IsOnline = true;
                LastDeaths = Deaths;
                LastPlayerKills = PlayerKills;
                LastZombieKills = ZombieKills;
                CurrentPosition = Position.InvalidPosition;
                if (!String.IsNullOrEmpty(Program.Config.MOTD))
                    CalloutManager.RegisterCallout(new Callout(this, CalloutType.Error, Program.Config.MOTD));
                CalloutManager.RegisterCallout(new Callout(this, new TimeSpan(0, 0, 90), CalloutType.Error, Program.HELLO));
                OnChanged();
            }
        }

        public void Logout()
        {
            Payday();
            logger.Info("{0} is now offline", Name);
            IsOnline = false;
            CalloutManager.UnregisterCalloutsForPlayer(this);
            OnChanged();
        }


        internal void Payday()
        {
            if (IsOnline)
            {
                TimeSpan span = DateTime.Now - LastPayday;
                TimeSpan heartbeat = DateTime.Now - LastUpdate;
                if (heartbeat.TotalMinutes > 5)
                {
                    logger.Info("{0} last heartbeath {1} Minutes ago. Setting offline", Name, heartbeat.Minutes);
                    IsOnline = false;
                    Logout();
                    return;
                }

                if (span.Minutes > 0)
                {
                    double distance = 0.0;

                    if ((LastPaydayPosition != Position.InvalidPosition) && (CurrentPosition != Position.InvalidPosition))
                    {
                        distance = CurrentPosition.Distance(LastPaydayPosition);
                        if (distance < Program.Config.MinimumDistanceForPayday)
                        {
                            logger.Info("{0} NO payday Distance: {1} / {2}", Name, distance, Program.Config.MinimumDistanceForPayday);
                            LastPaydayPosition = CurrentPosition;
                            LastPayday = DateTime.Now;
                            return;
                        }
                    }
                    LastPayday = DateTime.Now;
                    LastPaydayPosition = CurrentPosition;
                    AddCoins(span.Minutes * Program.Config.CoinsPerMinute, String.Format("Payday Dst: {0}", distance));
                    Age += span.Minutes;
                    OnChanged();
                }
            }
        }

        public virtual void Message(string p, params object[] args)
        {
            if (IsOnline)
                Program.Server.PrivateMessage(this, String.Format(p, args));
        }

        public void UpdateStats(int deaths, int zombies, int players, int ping)
        {
            LastUpdate = DateTime.Now;
            Deaths = deaths; LastDeaths = deaths;
            if (zombies > 0)
            {
                ZombieKills = zombies;
                logger.Debug("Update {0}: ZK {1} LZK {2} D {3} LD {4} P {5} LP {6} Ping {7}", Name, ZombieKills, LastZombieKills, Deaths, LastDeaths, PlayerKills, LastPlayerKills, Ping);
                if (ZombieKills > LastZombieKills)
                {
                    AddCoins((ZombieKills - LastZombieKills) * Program.Config.CoinsPerZombiekill, "Zombiekills");

                }
                LastZombieKills = ZombieKills;
            }
            if (PlayerKills > LastPlayerKills)
            {
                // TODO: PlayerKills Bounty
            }
            LastPlayerKills = PlayerKills;
            Ping = ping;
            OnChanged();
        }

        public void UpdatePosition(string pos)
        {
            // logger.Debug("UpdatePosition {0}: {1}", Name, pos);

            string[] p = pos.Split(new char[] { ',' });
            Position oldPos = CurrentPosition;
            CurrentPosition = new Position
            {
                X = Convert.ToDouble(p[0].Trim().ToLowerInvariant()),
                Y = Convert.ToDouble(p[1].Trim().ToLowerInvariant()),
                Z = Convert.ToDouble(p[2].Trim().ToLowerInvariant())
            };

            OnChanged();
            if ((CurrentPosition != oldPos) && (oldPos.IsValid) && (CurrentPosition.IsValid))
            {
                DistanceTravelled += CurrentPosition.Distance(oldPos);
                OnPlayerMoved(oldPos, CurrentPosition);
            }
        }

        public void UpdateHomePosition(string pos)
        {
            // logger.Debug("UpdateHomePosition {0}: {1}", Name, pos);
            string[] p = pos.Split(new char[] { ',' });
            HomePosition = new Position
            {
                X = Convert.ToDouble(p[0].Trim().ToLowerInvariant()),
                Y = Convert.ToDouble(p[1].Trim().ToLowerInvariant()),
                Z = Convert.ToDouble(p[2].Trim().ToLowerInvariant())
            };
            OnChanged();
        }

        public void UpdateHomePosition(IPosition newHome)
        {
            HomePosition = new Position { X = newHome.X, Y = newHome.Y, Z = newHome.Z };

        }

        [XmlIgnore]
        public virtual bool IsAdmin
        {
            get { return Program.Config.Admins.IsAdmin(SteamID); }
        }

        [XmlIgnore]
        public int AdminLevel
        {
            get { return Program.Config.Admins.AdminLevel(SteamID); }
        }

        public void Recalc()
        {
            zCoins = 0;
            AddCoins(Age, "Age");
            AddCoins(ZombieKills * Program.Config.CoinsPerZombiekill, "ZombieKills");
            LastZombieKills = ZombieKills;
        }

        public bool CanExecute(ICommand cmd)
        {
            return GetCoolDown(cmd) == 0;
        }

        public int GetCoolDown(ICommand cmd)
        {
            int timePassed = -1;

            if (cmd.cmdCoolDown <= 0)
                return 0;

            if (CommandCoolDowns.ContainsCommand(cmd.cmd))
            {
                timePassed = Age - CommandCoolDowns[cmd.cmd];
                return (timePassed > cmd.cmdCoolDown) ? 0 : cmd.cmdCoolDown - timePassed;
            }
            return 0;
        }

        public void SetCoolDown(ICommand cmd)
        {
            if (cmd.cmdCoolDown <= 0)
                return;
            CommandCoolDowns[cmd.cmd] = Age;
        }

        [XmlIgnore]
        IPosition IPlayer.CurrentPosition
        {
            get { return CurrentPosition as IPosition; }
        }

        [XmlIgnore]
        IPosition IPlayer.HomePosition
        {
            get { return HomePosition as IPosition; }
        }


        public void ClearCooldowns()
        {
            CommandCoolDowns.Clear();
        }


        public void Error(string msg, params object[] args)
        {
            Message("[FF0000]" + msg + "[FFFFFF]", args);
        }

        public void AddBounty(int howmuch, string why)
        {
            if (howmuch == 0)
                return;
            Bounty += howmuch;
            if (Bounty < 0)
                Bounty = 0;
            logger.Info("{0} Bounty Change [{3}]: {1} (new {2})", Name, howmuch, Bounty, why);
            Message("A bounty of {0} coins has been set on your head. Total bounty: {1}", howmuch, Bounty);
            OnChanged();
        }


        public void CollectBounty(int howmuch, string why)
        {
            BountyCollected += howmuch;
            AddCoins(howmuch, why);
        }

        public void AddBloodCoins(int howmuch, string why)
        {
            BloodCoins += howmuch;
            AddCoins(howmuch, why);
        }

        public void ClearBounty()
        {
            Bounty = 0;
            OnChanged();
        }



    }
}
