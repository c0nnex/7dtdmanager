using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Objects
{
    public class AreaProtection : AreaDefiniton
    {
        public string OwnedBy { get;set; }
        public DateTime Expires { get; set; }

        public List<string> RecordedEvents { get; set; }

       
    }
}
