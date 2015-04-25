using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdLanguage : PublicCommandBase
    {
        public cmdLanguage()
        {            
            CommandHelp = "R:Cmd.Language.Help";
            CommandName = "R:Cmd.Language.Command";
        }


        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            if (args.Length == 1)
            {
                p.Message("R:Cmd.Language.Available", String.Join(", ", Localizer.AvailableLanguages));
                p.Confirm("R:Cmd.Language.Current", p.Language);
                return true;
            }
            string lang = args[1].ToLowerInvariant();
            if (! Localizer.AvailableLanguages.Contains(lang))
            {
                p.Error("R:Cmd.Language.Unknown",lang);
                return false;
            }
            p.SetLanguage(lang);          
            p.Confirm("R:Cmd.Language.Current", p.Language);
            return true;
        }


    }
}
