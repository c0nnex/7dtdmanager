using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Objects
{
    class AreaProtection : AreaDefiniton
    {
        public string OwnedBy { get;set; }
        public List<string> Friends { get; set; }

        public DateTime Expires { get; set; }

        public List<string> RecordedEvents { get; set; }
    }
}
