using _7DTDManager.Commands;
using _7DTDManager.Interfaces;
using _7DTDManager.Localize;
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
        public string Name { get { return _name; } set { _name = value;} } private string _name;
        [XmlAttribute]
        public string EntityID { get; set; }

        public string IPAddress { get; set; }
        public string Language { get; set; }
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
        public int PingKicks { get; set; }
        
        // ShopSystem Stuff       
        public List<string> Friends { get; set; }
        public Mailbox Mailbox { get; set; }
        public ExposedList<AreaDefiniton, IAreaDefiniton> LandProtections {get;set;}

        private int LastDeaths = 0, LastPlayerKills = 0, LastZombieKills = 0;
        private DateTime LastUpdate = DateTime.Now;
        private Position LastPaydayPosition { get; set; }
        private IShop currentShop { get; set; }
        private PingTracker PingTracker = new PingTracker();
        
        [XmlIgnore]
        public bool IsOnline { get; set; }
        [XmlIgnore]
        public IPlayer ProxyPlayer { get; set; }
        [XmlIgnore]
        public IPlayer ExecuteAs { get; set; }

        public event PlayerChanged Changed;
        public event PlayerMovedDelegate PlayerMoved;
        public event PlayerPositionUpdateDelegate PlayerPositionUpdated;

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
            PlayersManager.Instance.OnPlayerMoved(this, oldPos, newPos);
        }

        private void OnPlayerPositionUpdated(IPosition newPos)
        {
            PlayerPositionUpdateDelegate handler = PlayerPositionUpdated;
            if (handler != null)
                handler(this, new PlayerPositionUpdateEventArgs { NewPosition = newPos });
            this.ClearEventInvocations("PlayerPositionUpdated");
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
            Friends = new List<string>();          
            Mailbox = new Mailbox();
            LandProtections = new ExposedList<AreaDefiniton, IAreaDefiniton>();
            ProxyPlayer = null;
            ExecuteAs = null;
            Language = "english";
        }

        /// <summary>
        /// Housekeeping stuff called when the player is loaded
        /// Independent from if the player is online or not
        /// </summary>
        public void Init()
        {
            foreach (var protection in LandProtections.Items)
            {
                PlayersManager.Instance.OnAreaInitialize(protection,this);                
            }
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
                PingTracker.Clear();
                logger.Info("{0} is now online (ZK {1}) Coins: {2}", Name, ZombieKills, zCoins);
                LastLogin = DateTime.Now;
                LastPayday = DateTime.Now;
                IsOnline = true;
                LastDeaths = Deaths;
                LastPlayerKills = PlayerKills;
                LastZombieKills = ZombieKills;
                CurrentPosition = Position.InvalidPosition;
                if (!String.IsNullOrEmpty(Program.Config.MOTD))
                    CalloutManagerImpl.RegisterCallout(new MessageCallout(this, CalloutType.Error, Program.Config.MOTD));
                CalloutManagerImpl.RegisterCallout(new MessageCallout(this, new TimeSpan(0, 0, 90), CalloutType.Error, Program.HELLO));
                OnChanged();
                PlayersManager.Instance.OnPlayerLogin(this);
                if (Mailbox.Mails.Count > 0)
                    Confirm("R:Mail.Inbox", Mailbox.Mails.Count);
            }
        }

        public void Logout()
        {
            Payday();
            logger.Info("{0} is now offline", Name);
            IsOnline = false;
            CalloutManagerImpl.UnregisterCallouts(this);
            OnChanged();
            PlayersManager.Instance.OnPlayerLogout(this);
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
            if (ping > 0)
            {
                PingTracker.AddPing(ping);
                logger.Trace("{0} Avg Ping: {1}", Name, PingTracker.AveragePing);
                if (!IsAdmin && (Program.Config.MaxPingAccepted > 0) && (PingTracker.AveragePing > Program.Config.MaxPingAccepted))
                {
                    PingKicks++;
                    if ((Program.Config.BanAfterKicks > 0) && (PingKicks > Program.Config.BanAfterKicks))
                    {
                        Program.Server.Execute("ban add {0} {1} \"Pinglimit exceeded\"", SteamID, Program.Config.BanDuration);
                        logger.Warn("{0} banned: ping limit exceeded (Avg: {1})",Name,PingTracker.AveragePing);
                        PingKicks = 0;
                        OnChanged();
                        return;
                    }
                    Program.Server.Execute("kick {0} \"Pinglimit exceeded\"", SteamID);
                    logger.Warn("{0} kicked: ping limit exceeded (Avg: {1})",Name,PingTracker.AveragePing);
                }
            }
            OnChanged();
        }

        public void UpdatePosition(string pos)
        {
            // logger.Debug("UpdatePosition {0}: {1}", Name, pos);
            
            Position oldPos = CurrentPosition;
            CurrentPosition = Position.FromString(pos) as Position;

            if ( CurrentPosition.IsValid )
            {
                OnPlayerPositionUpdated(CurrentPosition);
            }
            OnChanged();
            if ((CurrentPosition != oldPos) && (oldPos.IsValid) && (CurrentPosition.IsValid))
            {
                DistanceTravelled += CurrentPosition.Distance(oldPos);
                OnPlayerMoved(oldPos, CurrentPosition);
            }
        }

        public void UpdateHomePosition(string pos)
        {         
            HomePosition = Position.FromString(pos) as Position;
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
        public virtual int AdminLevel
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

            if (cmd.CommandCoolDown <= 0)
                return 0;

            if (CommandCoolDowns.ContainsCommand(cmd.CommandName))
            {
                timePassed = Age - CommandCoolDowns[cmd.CommandName];
                return (timePassed > cmd.CommandCoolDown) ? 0 : cmd.CommandCoolDown - timePassed;
            }
            return 0;
        }

        public void SetCoolDown(ICommand cmd)
        {
            if (cmd.CommandCoolDown <= 0)
                return;
            CommandCoolDowns[cmd.CommandName] = Age;
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

        public virtual void Message(string p, params object[] args)
        {
            string msg = MessageLocalizer.Localize(this, p, args);
            if ( (ProxyPlayer != null) && ( (ProxyPlayer.IsOnline) || (ProxyPlayer == Program.ServerPlayer)) && (ProxyPlayer != this))
                ProxyPlayer.Message(msg);
            if (IsOnline)
                Program.Server.PrivateMessage(this,msg);
        }

        public void Error(string msg, params object[] args)
        {
            Message("[FF0000]" +  MessageLocalizer.Localize(this,msg,args) + "[FFFFFF]");
        }

        public void Confirm(string msg, params object[] args)
        {
            Message("[00FF00]" +  MessageLocalizer.Localize(this,msg,args) + "[FFFFFF]");
        }

        public void AddBounty(int howmuch, string why)
        {
            if (howmuch == 0)
                return;
            Bounty += howmuch;
            if (Bounty < 0)
                Bounty = 0;
            logger.Info("{0} Bounty Change [{3}]: {1} (new {2})", Name, howmuch, Bounty, why);
            if ( howmuch > 0 )
                Message("R:Bounty.AddedBounty", howmuch, Bounty);
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

        public bool IsFriendOf(IPlayer other)
        {
            return false;
        }

        public void SetCurrentShop(IShop whichShop)
        {
            currentShop = whichShop;
        }

        public IShop GetCurrentShop()
        {
            return currentShop;
        }

        public void CalloutCallback(ICallout c)
        {
            
        }
        
        [XmlIgnore]
        IExposedList<IPlayer> IPlayer.Friends
        {
            get { throw new NotImplementedException(); }
        }

        [XmlIgnore]
        IMailbox IPlayer.Mailbox
        {
            get { return Mailbox; }
        }

       

        [XmlIgnore]
        IExposedList<IAreaDefiniton> IPlayer.LandProtections
        {
            get { return LandProtections as IExposedList<IAreaDefiniton>; }
          
        }



        public void ClearPingKicks()
        {
            if (PingKicks > 0)
            {
                logger.Info("{0}: PingKicks ({1}) cleared", Name, PingKicks);
                PingKicks = 0;
                OnChanged();
            }
        }

        public void Dirty()
        {
            OnChanged();
        }


        public void SetIPAddress(string ip)
        {
            IPAddress = ip;
            OnChanged();
        }

        public void SetLanguage(string lang)
        {
            Language = lang;
            OnChanged();
        }

        public string Localize(string key,params object[] args)
        {
            return MessageLocalizer.Localize(this, key,args);
        }
    }

    public class PingTracker
    {
        List<int> Pings;

        public PingTracker()
        {
            Pings = new List<int>();
        }

        public int AveragePing
        {
            get
            {
                if (Pings.Count <= 3)
                    return 0;

                int avg = 0;

                for (int i = 0; i < Pings.Count; i++)
                {
                    avg += Pings[i];
                }
                return (int)(avg / Pings.Count);
            }
        }

        public void AddPing(int value)
        {
            Pings.Add(value);
            while (Pings.Count > 10)
                Pings.RemoveAt(0);
        }

        public void Clear()
        {
            Pings.Clear();
        }
    }

    
}
