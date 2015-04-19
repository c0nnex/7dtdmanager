using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public abstract class ICallout
    {
     
        public DateTime When { get; set; }
        public bool Done { get; set; }
        public bool Persistent { get; set; }
        public ICalloutCallback Callback { get;set; }
        public object Owner { get; set; }

        public abstract void Execute();
       
    }

    public interface ICalloutCallback
    {
        void CalloutCallback(ICallout c);
    }
}
