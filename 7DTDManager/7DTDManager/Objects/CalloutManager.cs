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
        static List<Callout> AllCallouts;
        static DateTime nextCallout = DateTime.MaxValue;
        static DateTime lastCallout = DateTime.Now;

        public static DateTime NextCallout
        {
            get { return nextCallout; }
        }

        public static void RegisterCallout(Callout callout)
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
            List<Callout> newList = new List<Callout>();
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
                if (item.Target == p)
                    item.SetDone();
            }
            UpdateCallouts();
        }
    }
}
