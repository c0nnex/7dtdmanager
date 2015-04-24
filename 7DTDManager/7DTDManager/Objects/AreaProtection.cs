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
    public class AreaProtection : AreaDefiniton, ICalloutCallback, IAreaProtection
    {
        [XmlIgnore]
        public IPlayer Owner { get; set; }
        public string OwnedBy { get;set; }
        public DateTime Expires { get; set; }

        public List<string> RecordedEvents { get; set; }
        
        public bool CalloutCallback(ICallout c, IServerConnection serverConnection)
        {
            return false;
        }

        [XmlIgnore]
        public bool IsExpired
        {
            get { return false; }
        }

        public int AreaProtectionID { get; set;}

        [XmlIgnore]
        IExposedList<IAreaProtectionEvent> IAreaProtection.RecordedEvents
        {
            get { return null; }
        }

        public void RecordEvent(AreaProtectionEventType eventType)
        {
            throw new NotImplementedException();
        }

        public void Update(TimeSpan extend)
        {
            throw new NotImplementedException();
        }

        public string ToString(IPosition pos)
        {
            throw new NotImplementedException();
        }
    }
}
