using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _7DTDManager.Objects
{
    [Serializable]
    public class AreaProtection : AreaDefiniton, ICalloutCallback
    {
        [XmlIgnore]
        public IPlayer Owner { get; set; }
        public string OwnedBy { get;set; }
        public DateTime Expires { get; set; }

        public List<string> RecordedEvents { get; set; }
        
        public void CalloutCallback(ICallout c, IServerConnection serverConnection)
        {
            
        }
    }
}
