using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public abstract class ICallout
    {
        public IPlayer Who { get; set; }
        public DateTime When { get; set; }
        public bool Done { get; set; }
        public bool Persistent { get; set; }
        public object Callback { get;set; }

        public abstract void Execute();

        public ICallout()
        {

        }

        public ICallout( IPlayer who, object callback, DateTime when )
        {
            Who = who;
            Callback = callback;
            When = when;
        }
    }
}
