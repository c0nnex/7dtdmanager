using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Objects
{
    public enum CalloutType
    {
        Message,
        Error
    }

    public class Callout
    {
        public IPlayer Target { get; set; }
        public DateTime When { get; set; }
        public CalloutType What { get; set; }
        public String Message { get; set; }
        public bool Done { get; set; }

        public Callout()
        {
            Done = false;
        }

        public Callout(IPlayer target,TimeSpan when, CalloutType what, string msg) : this()
        {
            Target = target;
            When = DateTime.Now + when;
            What = what;
            Message = msg;
        }

        public virtual void Execute()
        {
            Done = true;
        }

        public virtual void SetDone(bool done = true)
        {
            Done = done;
        }
    }

}
