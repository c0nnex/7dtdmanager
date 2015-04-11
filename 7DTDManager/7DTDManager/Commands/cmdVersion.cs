using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdVersion : PublicCommandBase
    {
        public cmdVersion()
        {
            CommandName = "version";
            CommandHelp = "Show version of the command handler";
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            p.Message("7DTDManager Version {0} https://github.com/c0nnex/7dtdmanager", System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString());
            return true;
        }
    }
}
