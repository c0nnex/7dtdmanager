using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces.Commands
{
    public abstract class AdminCommandBase : PublicCommandBase
    {
        public AdminCommandBase()
        {
            CommandLevel = 1;
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args) { return false;}

    }
}
