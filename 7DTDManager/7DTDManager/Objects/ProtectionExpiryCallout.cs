using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _7DTDManager.Objects
{
    public class ProtectionExpiryCallout : ICallout
    {
        public ProtectionExpiryCallout(IPlayer who, object callback, DateTime when) : base(who,callback,when)
        {}

        public override void Execute()
        {
            throw new NotImplementedException();
        }
    }
}
