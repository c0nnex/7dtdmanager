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

        public static string ToHoursMinutesString(this TimeSpan t)
        {
            return String.Format("{1} hours {2} minutes", t.Hours + t.Days*24, t.Minutes);
        }
        public static string ToDaysHoursMinutesString(this TimeSpan t)
        {
            return String.Format("{0} days {1} hours {2} minutes", t.Days, t.Hours, t.Minutes);
        }
        public static string ToDaysHoursMinutesStringShort(this TimeSpan t)
        {
            return String.Format("{0}d {1}h {2}m", t.Days, t.Hours, t.Minutes);
        }
        public static string ToDistanceString(this double dist,string head,bool inside)
        {
            if (dist >= 1000.0)
                return String.Format("{0} ({1} km)", head, (int)(dist / 1000.0));
            else
            {
                if (inside)
                    return String.Format("[00FF00]{0} (HERE)[FFFFFF]", head, (int)dist);
                else
                    return String.Format("{0} ({1} m)", head, (int)dist);
            }
        }
    }
}
