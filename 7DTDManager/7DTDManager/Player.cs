using _7DTDManager.Interfaces;
using _7DTDManager.Objects;
using _7DTDManager.Players;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _7DTDManager
{
    [Serializable]
    [XmlRoot(ElementName="Players")]
    public class PlayersManager : IPlayersManager 
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        public List<Player> players = new List<Player>();
        private bool IsDirty = false;

        public Player FindPlayerByName(string name,bool onlyonline = true)
        {            
            // Try exact match
            var exactMatch = (from item in players where (String.Compare(item.Name.ToLowerInvariant(), name.ToLowerInvariant(), true) == 0) && (!onlyonline || (item.IsOnline)) select item).FirstOrDefault();
            if (exactMatch != null)
                return exactMatch;

            var laxMatch = from item in players where (item.Name.ToLowerInvariant().Contains(name.ToLowerInvariant()) && (!onlyonline || (item.IsOnline))) select item;
            if (laxMatch != null)
            {
                if (laxMatch.Count() == 1)
                    return laxMatch.First();
            }
            // more than one match! Don't return any!
            return null;
        }

        public Player AddPlayer(string name, string steamid, string entityid)
        {
            foreach (var item in players)
            {
                if (item.SteamID == steamid)
                {
                    item.EntityID = entityid;
                    item.Name = name; // Update name, maybe he changed it?!
                    return item;
                }
            }
            Player p = new Player { Name = name, SteamID = steamid, EntityID = entityid, FirstLogin = DateTime.Now };            
            players.Add(p);
            p.Changed += Player_Changed;
            return p;
        }

        void Player_Changed(object sender, EventArgs e)
        {
            IsDirty = true;
        }

        public void Payday()
        {
            foreach (var item in players)
            {
                if (item.IsOnline)
                    item.Payday();
            }
        }

        public static string ProfilePath { get { return Path.Combine(Program.ApplicationDirectory, "save"); } }

        public static PlayersManager Load()
        {
            try
            {
                Directory.CreateDirectory(ProfilePath);
                XmlSerializer serializer = new XmlSerializer(typeof(PlayersManager));
                StreamReader reader = new StreamReader(Path.Combine(ProfilePath, "players.xml"));
                PlayersManager p = (PlayersManager)serializer.Deserialize(reader);
                reader.Close();
                p.RegisterPlayers();
                return p;
            }
            catch (Exception ex)
            {
                logger.Info("Problem loading players");
                logger.Info(ex.ToString());
                return new PlayersManager();
            }
        }

        private void RegisterPlayers()
        {
            foreach (var item in players)
            {
                item.Changed += Player_Changed;
            }
        }

        public void Save(bool force=false)
        {
            try
            {
                if ( (!IsDirty) && (!force))
                    return;
                IsDirty = false;
                logger.Trace("Saving Players.");
                Directory.CreateDirectory(ProfilePath);

                XmlSerializer serializer = new XmlSerializer(typeof(PlayersManager));

                StreamWriter writer = new StreamWriter(Path.Combine(ProfilePath, "players.xml"));
                serializer.Serialize(writer, this);
                writer.Close();
            }
            catch (Exception ex)
            {
                logger.Info("Problem saving players");
                logger.Info(ex.ToString());
            }

        }

        [XmlIgnore]
        public IReadOnlyList<IPlayer> Players
        {
            get { return players as IReadOnlyList<IPlayer>; }
        }

        IPlayer IPlayersManager.AddPlayer(string name, string steamid, string entityid)
        {
            return AddPlayer(name, steamid, entityid) as IPlayer;
        }

        IPlayer IPlayersManager.FindPlayerByName(string name, bool onlyonline)
        {
            return FindPlayerByName(name) as IPlayer;
        }
    }

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
        public Position CurrentPosition { get; set; }
        public Position HomePosition { get; set; }

        public int Deaths { get; set; }
        public int PlayerKills { get; set; }
        public int ZombieKills { get; set; }
        public int Ping { get; set; }

        private int LastDeaths = 0, LastPlayerKills = 0, LastZombieKills = 0;
        private DateTime LastUpdate = DateTime.Now;

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

        private void OnPlayerMoved(IPosition oldPos,IPosition newPos)
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
                    LastPayday = DateTime.Now;
                    AddCoins(span.Minutes * Program.Config.CoinsPerMinute, "Payday");
                    Age += span.Minutes;
                    OnChanged();
                }
            }
        }

        public virtual void Message(string p, params object[] args)
        {
            if ( IsOnline)
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
            if ( (CurrentPosition != oldPos) && (oldPos.IsValid) && (CurrentPosition.IsValid) )
                OnPlayerMoved(oldPos, CurrentPosition);
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
            Message("[FF0000]"+msg+"[FFFFFF]", args); 
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

    public class ServerPlayer : Player
    {
        public override void Message(string p, params object[] args)
        {
            Console.WriteLine(String.Format(p, args));
        }

        public override bool IsAdmin
        {
            get
            {
                return true;
            }
        }
    }

    [Serializable]
    public sealed class Position : IPosition,IEquatable<Position>
    {
        [XmlAttribute]
        public Double X { get; set; }
        [XmlAttribute]
        public Double Y { get; set; }
        [XmlAttribute]
        public Double Z { get; set; }

        public bool IsValid
        {
            get { return ((X != InvalidPosition.X) && (Y != InvalidPosition.Y) && (Z != InvalidPosition.Z)); }
        }

        public static readonly Position InvalidPosition = new Position { X = Double.MinValue, Y = Double.MinValue, Z = Double.MinValue };

        

        public Position Clone()
        {
            return new Position { X = this.X, Y = this.Y, Z = this.Z };
        }

        public override string ToString()
        {
            return String.Format("({0}, {1}, {2})", X, Y, Z);
        }
        public string ToHumanString()
        {
            return String.Format("{0}{1}, {2}{3}", Math.Abs(Z), Z < 0 ? "S" : "N", Math.Abs(X), X < 0 ? "W" : "E");
        }

        public string ToCommandString()
        {
            return String.Format("{0} {1} {2}", X, Y, Z);
        }


        IPosition IPosition.InvalidPosition
        {
            get { return Position.InvalidPosition as IPosition; }
        }

        IPosition IPosition.Clone()
        {
            return Clone() as IPosition;
        }

        public bool Equals(Position other)
        {
            return ((X == other.X) && (Y == other.Y) && (Z == other.Z));
        }
    }

    // Pesty Code Analyzer ;)
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2237:MarkISerializableTypesWithSerializable")]
    [XmlRoot("dictionary")]    
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, IXmlSerializable
    {

        #region IXmlSerializable Members

        public System.Xml.Schema.XmlSchema GetSchema()
        {

            return null;

        }



        public void ReadXml(System.Xml.XmlReader reader)
        {

            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));

            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));



            bool wasEmpty = reader.IsEmptyElement;

            reader.Read();



            if (wasEmpty)

                return;



            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {

                reader.ReadStartElement("item");



                reader.ReadStartElement("key");

                TKey key = (TKey)keySerializer.Deserialize(reader);

                reader.ReadEndElement();



                reader.ReadStartElement("value");

                TValue value = (TValue)valueSerializer.Deserialize(reader);

                reader.ReadEndElement();



                this.Add(key, value);



                reader.ReadEndElement();

                reader.MoveToContent();

            }

            reader.ReadEndElement();

        }



        public void WriteXml(System.Xml.XmlWriter writer)
        {

            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));

            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));



            foreach (TKey key in this.Keys)
            {

                writer.WriteStartElement("item");



                writer.WriteStartElement("key");

                keySerializer.Serialize(writer, key);

                writer.WriteEndElement();



                writer.WriteStartElement("value");

                TValue value = this[key];

                valueSerializer.Serialize(writer, value);

                writer.WriteEndElement();



                writer.WriteEndElement();

            }

        }

        #endregion

    }
}
