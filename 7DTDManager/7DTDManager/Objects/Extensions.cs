using _7DTDManager.Interfaces;
using _7DTDManager.Localize;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Objects
{
    public static class Extensions
    {
        public static void ClearEventInvocations(this object obj, string eventName)
        {
            var fi = obj.GetType().GetEventField(eventName);
            if (fi == null) return;
            fi.SetValue(obj, null);
        }

        private static FieldInfo GetEventField(this Type type, string eventName)
        {
            FieldInfo field = null;
            while (type != null)
            {
                /* Find events defined as field */
                field = type.GetField(eventName, BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null && (field.FieldType == typeof(MulticastDelegate) || field.FieldType.IsSubclassOf(typeof(MulticastDelegate))))
                    break;

                /* Find events defined as property { add; remove; } */
                field = type.GetField("EVENT_" + eventName.ToUpper(), BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic);
                if (field != null)
                    break;
                type = type.BaseType;
            }
            return field;
        }

        public static string ToString(this TimeSpan t,IPlayer p)
        {
            StringBuilder sb = new StringBuilder();
            if (t.Days > 0)
                sb.Append( MessageLocalizer.Localize(p,"R:TimeSpan.Days",t.Days)+" ");
            if (t.Hours > 0)
                sb.Append(MessageLocalizer.Localize(p, "R:TimeSpan.Hours", t.Hours) + " ");
            sb.Append(MessageLocalizer.Localize(p, "R:TimeSpan.Minutes", t.Minutes));

            return sb.ToString();
        }
       
        public static string ToStringShort(this TimeSpan t,IPlayer p)
        {
            StringBuilder sb = new StringBuilder();
            if (t.Days > 0)
                sb.Append(MessageLocalizer.Localize(p, "R:TimeSpan.ShortDays", t.Days) + " ");
            if (t.Hours > 0)
                sb.Append(MessageLocalizer.Localize(p, "R:TimeSpan.ShortHours", t.Hours) + " ");
            sb.Append(MessageLocalizer.Localize(p, "R:TimeSpan.ShortMinutes", t.Minutes));
            return sb.ToString();
        }

        public static string ToDistanceString(this double dist,IPlayer p, string head,bool inside)
        {
            if (dist >= 1000.0)
                return String.Format("{0} ({1} km)", head, (int)(dist / 1000.0));
            else
            {
                if (inside)
                    return String.Format("[00FF00]{0} ("+MessageLocalizer.Localize(p, "R:Here")+")[FFFFFF]", head, (int)dist);
                else
                    return String.Format("{0} ({1} m)", head, (int)dist);
            }
        }
    }
}
