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
    public class CommandConfigurationList : List<CommandConfiguration>
    {
        public bool ContainsCommand(string command)
        {
            var t = (from cmds in this where cmds.Command.ToLowerInvariant() == command.ToLowerInvariant() select cmds).FirstOrDefault();
            return t != null;
        }

        public CommandConfiguration this[string key]
        {
            get
            {
                var t = (from cmds in this where cmds.Command.ToLowerInvariant() == key.ToLowerInvariant() select cmds).FirstOrDefault();
                return t;
            }

            set 
            { 
                var t = (from cmds in this where cmds.Command.ToLowerInvariant() == key.ToLowerInvariant() select cmds).FirstOrDefault();
                if (t == null)
                {
                    this.Add(value);
                    return;
                }
                this.Remove(t);
                this.Add(value);
            }
        }

        public void AddCommand(CommandConfiguration c)
        {
            this[c.Command] = c; // Makes sure we have no duplicates
        }

        public bool IsEnabled(string command)
        {
            CommandConfiguration c = this[command.ToLowerInvariant()];
            if (c == null)
                return true;
            return c.Enabled;
        }

        internal void UpdateCommand(ICommand cmd)
        {
            CommandConfiguration config = this[cmd.cmd];
            cmd.cmdCoolDown = config.CoolDown;
            cmd.cmdCost = config.Cost;
            cmd.cmdLevel = config.Level;
        }
    }

    [Serializable]
    [XmlRoot(ElementName = "Command")]
    public class CommandConfiguration
    {
        [XmlAttribute]
        public string Command { get; set; }
        [XmlAttribute]
        public int Cost { get; set; }
        [XmlAttribute]
        public int CoolDown { get; set; }
        [XmlAttribute]
        public bool Enabled { get; set; }
        [XmlAttribute]
        public int Level { get; set; }

        public CommandConfiguration()
        {
            Enabled = false;
            Level = 0;
        }

        public CommandConfiguration(ICommand cmd) : this()
        {
            Command = cmd.cmd.ToLowerInvariant();
            Cost = cmd.cmdCost;
            CoolDown = cmd.cmdCoolDown;
            Enabled = true;
            Level = cmd.cmdLevel;
        }
    }
}
