using _7DTDManager.Interfaces;
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
    public class PlayersManager : IPlayersManager
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        public List<Player> players = new List<Player>();
        private bool IsDirty = false;

        public Player FindPlayerByName(string name, bool onlyonline = true)
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

        public void Save(bool force = false)
        {
            try
            {
                if ((!IsDirty) && (!force))
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
            return FindPlayerByName(name, onlyonline) as IPlayer;
        }
    }
}
