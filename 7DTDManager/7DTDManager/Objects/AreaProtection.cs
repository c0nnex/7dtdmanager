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


        public bool IsExpired
        {
            get { throw new NotImplementedException(); }
        }

        public int AreaProtectionID
        {
            get { throw new NotImplementedException(); }
            set {  }
        }

        IExposedList<IAreaProtectionEvent> IAreaProtection.RecordedEvents
        {
            get { throw new NotImplementedException(); }
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
