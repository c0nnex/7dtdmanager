using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces.Commands
{  
    public class InfoCommand : PublicCommandBase
    {
        public InfoCommand(string help)
        {
            CommandHelp = help;
            CommandIsInfo = true;
        }

    }
}
