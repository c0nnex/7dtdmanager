using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _7DTDManager.Objects
{
    public class ProtectionExpiryCallout : BasicCallout
    {
        public ProtectionExpiryCallout()
        {
            Delay = new TimeSpan(1, 0, 0);
            Persistent = true;
        }
      
    }
}
