using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Objects
{
    public class CalloutManager
    {
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
                    item.Execute();
                }
            }
            Housekeeping();

            nextCallout = DateTime.MaxValue;

            foreach (var item in AllCallouts)
            {
                if (item.When < nextCallout)
                    nextCallout = item.When;
            }
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

        public static void UnregisterCalloutsForPlayer(IPlayer p)
        {
            foreach (var item in AllCallouts)
            {
                if ( (item.Who == p) && (!item.Persistent))
                    item.Done = true;
            }
            UpdateCallouts();
        }
    }
}
