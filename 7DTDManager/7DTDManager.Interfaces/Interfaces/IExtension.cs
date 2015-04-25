using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public interface IExtension
    {
        string Name { get; }
        string Author { get; }
        string WebSite { get; }
        string Contact { get; }
        string Version { get; }

        void InitializeExtension(IServerConnection server,ILogger logger);
    }
}
