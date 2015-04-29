using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelayedEffect
{
    public class Example2EffectCallout : ICalloutCallback
    {
        private IPlayer targetPlayer;
        private int TimesCalled = 0;

        public Example2EffectCallout(IPlayer target)
        {
            targetPlayer = target;
        }

        /// <summary>
        /// The programmed timeout elapsed and we shall execute what ever we need to
        /// </summary>
        /// <param name="c">The reference to the ICallout executing</param>
        /// <param name="serverConnection">Connection to the Server</param>
        /// <returns></returns>
        public bool CalloutCallback(ICallout c, IServerConnection serverConnection)
        {
            TimesCalled++;

            switch (TimesCalled)
            {
                case 1:
                    targetPlayer.Message("Toc...");
                    break;
                case 2:
                    targetPlayer.Message("Toc... Toc...");
                    break;
                default:
                    targetPlayer.Error("GOT YOU! YOU WILL DIE HAHAHAHAHHAHA!");
                    break;
            }

            if (TimesCalled < 3)
            {
                // As long we have not been called 3 Times, we tell the CalloutManager to repeat the callout
                return true;
            }
            else
            {
                // We are done now, so we tell the manager to no longer call us
                return false;
            }
        }
    }
}
