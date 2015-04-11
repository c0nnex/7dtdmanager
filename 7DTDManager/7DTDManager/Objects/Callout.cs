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

    public enum CalloutTriggerType
    {
        Time,
        Movement
    }

    public class Callout
    {
        public IPlayer Who { get; set; }
        public DateTime When { get; set; }
        public CalloutType What { get; set; }
        public CalloutTriggerType Trigger { get;set;}
        public String Message { get; set; }
        public bool Done { get; set; }

        public Callout()
        {
            Done = false;
            Trigger = CalloutTriggerType.Time;
            When = DateTime.MaxValue;
        }

        public Callout(IPlayer target,TimeSpan when, CalloutType what, string msg) : this()
        {
            Who = target;
            When = DateTime.Now + when;
            What = what;
            Message = msg;
            Trigger = CalloutTriggerType.Time;            

        }

        public Callout(IPlayer target, CalloutType what, string msg) : this()
        {
            Who = target;         
            What = what;
            Message = msg;
            Trigger = CalloutTriggerType.Movement;
            target.PlayerMoved += target_PlayerMoved;
        }

        void target_PlayerMoved(object sender, PlayerMovementEventArgs e)
        {
            Execute();
            Who.PlayerMoved -= target_PlayerMoved;
        }

        public virtual void Execute()
        {
            if (Done)
                return;
            if (!Who.IsOnline)
            {
                SetDone();
                return;
            }
            switch (What)
            {
                case CalloutType.Message:
                    Who.Message(Message);
                    break;
                case CalloutType.Error:
                    Who.Error(Message);
                    break;
                default:
                    break;
            }
            Done = true;
        }

        public virtual void SetDone(bool done = true)
        {
            Done = done;
        }
    }

    
}
