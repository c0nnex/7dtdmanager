using _7DTDManager.Interfaces;
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
    public class Players : IPlayers 
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        public List<Player> players = new List<Player>();

        public Player FindPlayerByName(string name,bool onlyonline = true)
        {
            // Try exact match
            foreach (var item in players)
            {
                if ( (String.Compare(item.Name, name, true) == 0) && ( !onlyonline || (item.IsOnline) ) )
                    return item;
            }
            List<Player> foundPlayers = new List<Player>();
            foreach (var item in players)
            {
                if (item.Name.ToLowerInvariant().Contains(name.ToLowerInvariant()) && (!onlyonline || (item.IsOnline)))
                    foundPlayers.Add(item);
            }
            if (foundPlayers.Count == 1)
                return foundPlayers[0];
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
                    return item;
                }
            }
            Player p = new Player { Name = name, SteamID = steamid, EntityID = entityid, FirstLogin = DateTime.Now };
            players.Add(p);
            return p;
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

        public static Players Load()
        {
            try
            {
                Directory.CreateDirectory(ProfilePath);
                XmlSerializer serializer = new XmlSerializer(typeof(Players));
                StreamReader reader = new StreamReader(Path.Combine(ProfilePath, "players.xml"));
                Players p = (Players)serializer.Deserialize(reader);
                reader.Close();

                return p;
            }
            catch (Exception ex)
            {
                logger.Info("Problem loading players");
                logger.Info(ex.ToString());
                return new Players();
            }
        }

        public void Save()
        {
            try
            {
                Directory.CreateDirectory(ProfilePath);


                XmlSerializer serializer = new XmlSerializer(typeof(Players));

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


        public IReadOnlyList<IPlayer> AllPlayers
        {
            get { return players as IReadOnlyList<IPlayer>; }
        }

        IPlayer IPlayers.AddPlayer(string name, string steamid, string entityid)
        {
            return AddPlayer(name, steamid, entityid) as IPlayer;
        }

        IPlayer IPlayers.FindPlayerByName(string name, bool onlyonline)
        {
            return FindPlayerByName(name) as IPlayer;
        }
    }

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

        public SerializableDictionary<string, int> cmdHistory = new SerializableDictionary<string, int>();

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
            }
        }

        public void Logout()
        {
            Payday();
            logger.Info("{0} is now offline", Name);
            IsOnline = false;
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
                    AddCoins(span.Minutes * Program.config.CoinsPerMinute, "Payday");
                    Age += span.Minutes;
                }
            }
        }

        public virtual void Message(string p, params object[] args)
        {
            Program.server.PrivateMessage(this, String.Format(p, args));
        }

        public void UpdateStats(int deaths, int zombies, int players, int ping)
        {
            LastUpdate = DateTime.Now;
            Deaths = deaths; LastDeaths = deaths;
            if (zombies > 0)
            {
                ZombieKills = zombies;
                logger.Info("Update {0}: ZK {1} LZK {2} D {3} LD {4} P {5} LP {6} Ping {7}", Name, ZombieKills, LastZombieKills, Deaths, LastDeaths, PlayerKills, LastPlayerKills, Ping);
                if (ZombieKills > LastZombieKills)
                {
                    AddCoins((ZombieKills - LastZombieKills) * Program.config.CoinsPerZombiekill, "Zombiekills");

                }
                LastZombieKills = ZombieKills;
            }
            if (PlayerKills > LastPlayerKills)
            {
                // TODO: PlayerKills Bounty
            }
            LastPlayerKills = PlayerKills;
            Ping = ping;
        }

        public void UpdatePosition(string pos)
        {
            string[] p = pos.Split(new char[] { ',' });
            CurrentPosition = new Position
            {
                X = (int)Convert.ToDouble(p[0].Trim().ToLowerInvariant()),
                Y = (int)Convert.ToDouble(p[1].Trim().ToLowerInvariant()),
                Z = (int)Convert.ToDouble(p[2].Trim().ToLowerInvariant())
            };
        }

        public void UpdateHomePosition(string pos)
        {
            string[] p = pos.Split(new char[] { ',' });
            HomePosition = new Position
            {
                X = (int)Convert.ToDouble(p[0].Trim().ToLowerInvariant()),
                Y = (int)Convert.ToDouble(p[1].Trim().ToLowerInvariant()),
                Z = (int)Convert.ToDouble(p[2].Trim().ToLowerInvariant())
            };
        }

        public virtual bool IsAdmin
        {
            get { return SteamID == "76561198003534614"; }
        }


        public void Recalc()
        {
            zCoins = 0;
            AddCoins(Age, "Age");
            AddCoins(ZombieKills * 5, "ZombieKills");
            LastZombieKills = ZombieKills;
        }

        public bool CanExecute(ICommand cmd)
        {
            return GetCoolDown(cmd) == 0;
        }

        public int GetCoolDown(ICommand cmd)
        {
            int timePassed = -1;

            if (cmd.cmdTimelimit <= 0)
                return 0;

            if (cmdHistory.ContainsKey(cmd.cmd))
            {
                timePassed = Age - cmdHistory[cmd.cmd];
                return (timePassed > cmd.cmdTimelimit) ? 0 : cmd.cmdTimelimit - timePassed;
            }
            return 0;
        }

        public void SetCoolDown(ICommand cmd)
        {
            if (cmd.cmdTimelimit <= 0)
                return;
            cmdHistory[cmd.cmd] = Age;
        }


        IPosition IPlayer.CurrentPosition
        {
            get { return CurrentPosition as IPosition; }
        }

        IPosition IPlayer.HomePosition
        {
            get { return HomePosition as IPosition; }
        }


        public void ClearCooldowns()
        {
            cmdHistory.Clear();
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
    public class Position : IPosition
    {
        [XmlAttribute]
        public Double X { get; set; }
        [XmlAttribute]
        public Double Y { get; set; }
        [XmlAttribute]
        public Double Z { get; set; }

        public bool IsValid()
        {
            return ((X != InvalidPosition.X) && (Y != InvalidPosition.Y) && (Z != InvalidPosition.Z));
        }

        public static readonly Position InvalidPosition = new Position { X = Int32.MinValue, Y = Int32.MinValue, Z = Int32.MaxValue };

        

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
            return String.Format("{0} {1} {2}", X, Y + 1, Z);
        }


        IPosition IPosition.InvalidPosition
        {
            get { return InvalidPosition as IPosition; }
        }

        IPosition IPosition.Clone()
        {
            return Clone() as IPosition;
        }
    }

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
