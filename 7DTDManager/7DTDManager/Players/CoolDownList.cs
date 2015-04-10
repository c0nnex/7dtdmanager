using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _7DTDManager.Players
{
    [Serializable]    
    public class CoolDownList : List<CommandCoolDown>
    {
        public bool ContainsCommand(string command)
        {
            var t = (from cmds in this where cmds.Command.ToLowerInvariant() == command.ToLowerInvariant() select cmds).FirstOrDefault();
            return t != null;
        }

        public int this[string key]
        {
            get
            {
                var t = (from cmds in this where cmds.Command.ToLowerInvariant() == key.ToLowerInvariant() select cmds).FirstOrDefault();
                if (t == null)
                    return -1;
                return t.LastUsedAge;
            }

            set 
            { 
                var t = (from cmds in this where cmds.Command.ToLowerInvariant() == key.ToLowerInvariant() select cmds).FirstOrDefault();
                if (t == null)
                {
                    this.Add(new CommandCoolDown(key.ToLowerInvariant(), value));
                    return;
                }
                t.LastUsedAge = value;
            }
        }
    }

    [Serializable]
    public class CommandCoolDown
    {
        [XmlAttribute]
        public string Command { get; set; }
        [XmlAttribute]
        public int LastUsedAge { get; set; }

        public CommandCoolDown()
        {

        }

        public CommandCoolDown(string command, int lastusedage) : this()
        {
            Command = command;
            LastUsedAge = lastusedage;
        }
    }
}
