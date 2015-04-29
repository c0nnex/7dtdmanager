using _7DTDManager.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Objects
{
    public class CalloutManagerImpl : ICalloutManager
    {
        internal static Logger logger = LogManager.GetCurrentClassLogger();

        public static CalloutManagerImpl Instance = new CalloutManagerImpl();

        static List<ICallout> AllCallouts = new List<ICallout>();
        static DateTime nextCallout = DateTime.MaxValue;
        static DateTime lastCallout = DateTime.Now;

        public static DateTime NextCallout
        {
            get { return nextCallout; }
        }

        public static void RegisterCallout(ICallout callout)
        {
            AllCallouts.Add(callout);
            UpdateCallouts();
        }

        public static void UpdateCallouts()
        {
            DateTime now = DateTime.Now;

            // First execute callouts
            foreach (var item in AllCallouts)
            {
                if (item.When <= now )
                {
                    item.Execute(Program.Server);
                }
            }
            Housekeeping();

            nextCallout = DateTime.MaxValue;

            foreach (var item in AllCallouts)
            {
                if (item.When < nextCallout)
                    nextCallout = item.When;
            }
            logger.Debug("Next callout in {0}", (nextCallout - DateTime.Now));
        }

        public static void Housekeeping()
        {
            List<ICallout> newList = new List<ICallout>();
            foreach (var item in AllCallouts)
            {
                if (!item.Done)
                    newList.Add(item);
            }
            AllCallouts = newList;
        }

        public static void UnregisterCallouts(object p)
        {
            List<ICallout> newList = new List<ICallout>();
            foreach (var item in AllCallouts)
            {
                if ((item.Callback != p) && (item.Owner != p))
                    newList.Add(item);
            }
            AllCallouts = newList;
            UpdateCallouts();
        }

        public ICallout AddCallout(object owner, ICalloutCallback callback, TimeSpan delay, bool persistant)
        {
            BasicCallout c = new BasicCallout();
            c.When = DateTime.Now + delay;
            c.Callback = callback;
            c.Owner = owner;
            c.Persistent = persistant;
            c.Delay = delay;
            CalloutManagerImpl.RegisterCallout(c);
            return c;
        }

        public void RemoveCallout(ICallout callout)
        {
            CalloutManagerImpl.AllCallouts.Remove(callout);
            UpdateCallouts();
        }

        public void RemoveAllCalloutsFor(object owner)
        {
            CalloutManagerImpl.UnregisterCallouts(owner);
        }

    }

    public class BasicCallout : ICallout
    {
        public DateTime When { get; set; }
        public TimeSpan Delay { get; set; }
        public bool Done { get; set; }
        public bool Persistent { get; set; }
        public ICalloutCallback Callback { get; set; }
        public object Owner { get; set; }

        public virtual void Execute(IServerConnection serverConnection)
        {
            try
            {
                if (Callback != null)
                {
                    bool bRepeat = Callback.CalloutCallback(this, serverConnection);
                    if (!Persistent && !bRepeat)
                        Done = true;
                    else
                        When = DateTime.Now + Delay;
                }
            }
            catch (Exception ex)
            {
                CalloutManagerImpl.logger.Error("Error in calloutcallback {0}: {1}", Callback.ToString(), ex.Message);
            }
        }
    }
    
}
