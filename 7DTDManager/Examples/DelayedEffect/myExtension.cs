using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelayedEffect
{
    public class myExtension : IExtension
    {
        internal static myExtension Instance;

        internal RegularyCalled myEvery10MinutesCalled;

        public string Name
        {
            get { return "DelayedEffect"; }
        }

        public string Author
        {
            get { return "c0nnex"; }
        }

        public string WebSite
        {
            get { return "https://github.com/c0nnex/7dtdmanager"; }
        }

        public string Contact
        {
            get { return "Github"; }
        }

        public string Version
        {
            get { return "1.0"; }
        }

        public void InitializeExtension(IServerConnection server, ILogger logger)
        {
            Instance = this;
            myEvery10MinutesCalled = new RegularyCalled();
            // Create a callout that will be called every 10 Minutes UNTIL WE CANCEL IT!
            server.CalloutManager.AddCallout(this, myEvery10MinutesCalled, TimeSpan.FromMinutes(10), true);
        }
    }
}
