using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public interface IServerLineHandler
    {
        void Init(IServerConnection serverConnection, ILogger logger);
        bool PriorityProcess { get; }
        bool Exclusive { get; }
        bool ProcessLine(IServerConnection serverConnection, string currentLine);
    }
}
