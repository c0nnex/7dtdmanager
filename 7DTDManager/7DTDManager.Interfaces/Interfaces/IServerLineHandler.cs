using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public interface IServerLineHandler
    {
        void Init(IServerConnection serverConnection);
        bool PriorityProcess { get; }
        bool ProcessLine(IServerConnection serverConnection, string currentLine);
    }
}
