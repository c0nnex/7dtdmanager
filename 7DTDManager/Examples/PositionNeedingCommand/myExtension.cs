using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionNeedingCommand
{
    public class myExtension : IExtension
    {
        public string Name
        {
            get { return "PositionNeedingCommand"; }
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
            // We Don't need any Initialization
        }
    }
}
