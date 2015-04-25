using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager
{
    public static class Extensions
    {        

        public static string ToString(this TimeSpan t,IPlayer p)
        {
            StringBuilder sb = new StringBuilder();
            if (t.Days > 0)
                sb.Append( p.Localize("R:TimeSpan.Days",t.Days)+" ");
            if (t.Hours > 0)
                sb.Append(p.Localize("R:TimeSpan.Hours", t.Hours) + " ");
            sb.Append(p.Localize( "R:TimeSpan.Minutes", t.Minutes));

            return sb.ToString();
        }
       
        public static string ToStringShort(this TimeSpan t,IPlayer p)
        {
            StringBuilder sb = new StringBuilder();
            if (t.Days > 0)
                sb.Append(p.Localize( "R:TimeSpan.ShortDays", t.Days) + " ");
            if (t.Hours > 0)
                sb.Append(p.Localize( "R:TimeSpan.ShortHours", t.Hours) + " ");
            sb.Append(p.Localize( "R:TimeSpan.ShortMinutes", t.Minutes));
            return sb.ToString();
        }

        public static string ToDistanceString(this double dist,IPlayer p, string head,bool inside)
        {
            if (dist >= 1000.0)
                return String.Format("{0} ({1} km)", head, (int)(dist / 1000.0));
            else
            {
                if (inside)
                    return String.Format("[00FF00]{0} ("+p.Localize("R:Here")+")[FFFFFF]", head, (int)dist);
                else
                    return String.Format("{0} ({1} m)", head, (int)dist);
            }
        }
    }
}
