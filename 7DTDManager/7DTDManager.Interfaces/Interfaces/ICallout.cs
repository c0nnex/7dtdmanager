using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public interface ICallout
    {

        DateTime When { get; }
        TimeSpan Delay { get; }
        bool Done { get; }
        bool Persistent { get; }
        ICalloutCallback Callback { get; }
        object Owner { get; }

        void Execute(IServerConnection serverConnection);

    }

    public interface ICalloutCallback
    {
        void CalloutCallback(ICallout c, IServerConnection serverConnection);
    }

    public interface ICalloutManager
    {
        ICallout AddCallout(ICalloutCallback owner, TimeSpan delay, bool persistant);
        void RemoveCallout(ICallout callout);
        void RemoveAllCalloutsFor(object owner);
    }

   
}
