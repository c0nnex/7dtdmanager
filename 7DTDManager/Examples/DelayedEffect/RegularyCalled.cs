using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelayedEffect
{
    public class RegularyCalled : ICalloutCallback
    {
        public bool CalloutCallback(ICallout c, IServerConnection serverConnection)
        {

            // lets have some fun and pester some random player
            var onlinePlayers = (from player in serverConnection.AllPlayers.Players where player.IsOnline select player).ToArray();
            if ( ( onlinePlayers != null ) && (onlinePlayers.Length > 0) )
            {
                var rnd = new Random();

                var victim = onlinePlayers[rnd.Next(onlinePlayers.Length)];

                serverConnection.CalloutManager.AddCallout(myExtension.Instance, new Example2EffectCallout(victim), TimeSpan.FromSeconds(10), false);
            }

            // return value is ignored since this is a persistent callout. See Example2RemoveCalloutCommand on how to remove it
            return false;
        }
    }
}
