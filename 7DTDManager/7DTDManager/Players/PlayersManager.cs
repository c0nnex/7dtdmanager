using _7DTDManager.Interfaces;
using _7DTDManager.Objects;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;


namespace _7DTDManager.Players
{
    [Serializable]
    [XmlRoot(ElementName = "Players")]
    public class PlayersManager : Singleton<PlayersManager>,IPlayersManager,ISingleton
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        public List<Player> players { get; set;}
        
        private bool IsDirty = false;
        
        [XmlIgnore]
        public int OnlinePlayers = 0;

        public event PlayerMovedDelegate PlayerMoved;
        public event EventHandler PlayerLogin;
        public event EventHandler PlayerLogout;
        public event EventHandler PlayerSave;
        public event AreaEventDelegate AreaInitialize;

        public PlayersManager() : base()
        {
            players = new List<Player>();
        }

        public IPlayer FindPlayerBySteamID(string id)
        {
            var exactMatch = (from item in players where item.SteamID == id select item).FirstOrDefault();         
            return exactMatch;
        }

        public Player FindPlayerByNameOrID(string name, bool onlyonline = true)
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
            var entityMatch = (from item in players where ((item.EntityID == name) && (!onlyonline || (item.IsOnline))) select item).FirstOrDefault();
            if (entityMatch != null)
                return entityMatch;
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
            p.Init();
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
                logger.Info("Loading Players....");
                //XmlSerializer<PlayersManager> serializer = new XmlSerializer<PlayersManager>(new XmlSerializationOptions(null, Encoding.UTF8, null, true).DisableRedact(),null);
                XmlSerializer serializer = new XmlSerializer(typeof(PlayersManager));
                StreamReader reader = new StreamReader(Path.Combine(ProfilePath, "players.xml"));
                PlayersManager p = (PlayersManager)serializer.Deserialize(reader);
                reader.Close();
                if (p.players == null)
                    Instance.players = new List<Player>();
                else
                    Instance.players = p.players;               
                return Instance;
            }
            catch (Exception ex)
            {
                if (!(ex is IOException))
                {
                    logger.Error("Problem loading players");
                    logger.Error(ex.ToString());
                }
                Instance.players = new List<Player>();
                return Instance;
            }
        }

        public void RegisterPlayers()
        {
            if (players != null)
            {
                foreach (var item in players)
                {
                    item.Init();
                    item.Changed += Player_Changed;
                }
            }
        }

        public void Save(bool force = false)
        {
            try
            {
                if ((!IsDirty) && (!force))
                    return;
                IsDirty = false;
                logger.Trace("Saving Players...");
                Directory.CreateDirectory(ProfilePath);
                OnPlayerSave();
               /* XmlSerializer<PlayersManager> serializer = new XmlSerializer<PlayersManager>(
                    new XmlSerializationOptions(null, Encoding.UTF8, null, true).DisableRedact(), null);
                */
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


        public void OnPlayerLogin(IPlayer p)
        {
            OnlinePlayers++;
            EventHandler e = PlayerLogin;
            if (e != null)
                e(p, EventArgs.Empty);
        }

        public void OnPlayerLogout(IPlayer p)
        {
            OnlinePlayers--;
            EventHandler e = PlayerLogout;
            if (e != null)
                e(p, EventArgs.Empty);
        }

        public void OnPlayerMoved(IPlayer p, IPosition oldPos, IPosition newPos)
        {
            PlayerMovedDelegate handler = PlayerMoved;
            if (handler != null)
                handler(p, new PlayerMovementEventArgs { OldPosition = oldPos, NewPosition = newPos });
        }

        public void OnAreaInitialize(IAreaDefiniton area,IPlayer p)
        {
            try
            {
                logger.Info("AreaInitialize {0} {1}", area.Identifier, p.SteamID);
                AreaEventDelegate handler = AreaInitialize;
                if (handler != null)
                    handler(area, new AreaEventArgs(p, AreaEventType.Init));
            }
            catch (Exception ex)
            {
                logger.Error("OnAreaInitialize: {0}", ex.ToString());
            }
        }

        public void OnPlayerSave()
        {
            EventHandler handler = PlayerSave;
            if (handler != null)
                handler(this, EventArgs.Empty);
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

        IPlayer IPlayersManager.FindPlayerByNameOrID(string name, bool onlyonline)
        {
            return FindPlayerByNameOrID(name, onlyonline) as IPlayer;
        }

        void ISingleton.InitInstance()
        {
           
        }
    }
}
