using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7DTDManager.Objects;

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
            if (server.AllExtensions.Count > 0)
            {
                p.Message("7DTDmanager Extensions:");

                foreach (var ex in server.AllExtensions)
                {
                    p.Message("Extension {0} Author {1} WebSite {2} Version {3}", ex.Name.Green(), ex.Author.Green(), ex.WebSite.Url(), ex.Version.Green());
                }
            }
            return true;
        }
    }
}
