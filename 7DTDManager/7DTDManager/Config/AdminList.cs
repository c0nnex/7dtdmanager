using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _7DTDManager.Config
{
    [Serializable]
    public class AdminList : List<AdminEntry>
    {
        public bool IsAdmin(string steamid)
        {
            var t = (from cmds in this where cmds.SteamID.ToLowerInvariant() == steamid.ToLowerInvariant() select cmds).FirstOrDefault();
            return t != null;
        }
        
        public void AddAdmin(string steamid, int adminlevel)
        {
            this.Add(new AdminEntry(steamid, adminlevel));
        }

        internal int AdminLevel(string steamid)
        {
            var t = (from cmds in this where cmds.SteamID.ToLowerInvariant() == steamid.ToLowerInvariant() select cmds).FirstOrDefault();
            return (t != null ? t.AdminLevel : 0);
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "Admin")]
    public class AdminEntry
    {
        [XmlAttribute]
        public string SteamID { get; set; }
        [XmlAttribute]
        public int AdminLevel { get; set; }
        

        public AdminEntry()
        {
            AdminLevel = 0;
        }

        public AdminEntry(string steamid, int level)
            : this()
        {
            SteamID = steamid;
            AdminLevel = level;
        }
    }
}
