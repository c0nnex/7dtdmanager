using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdSetHome : PublicCommandBase
    {
        public cmdSetHome()
        {
            CommandCost = 60;
            CommandCoolDown = 30;
            CommandHelp = "R:Cmd.SetHome.Help";
            CommandName = "R:Cmd.SetHome.Command";
        }


        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            if (!p.CurrentPosition.IsValid)
            {
                p.Error("R:Error.NoPosition");
                return false;
            }
            p.UpdateHomePosition(p.CurrentPosition);
            p.Message("R:Cmd.SetHome.NewHome", p.HomePosition.ToHumanString());
            return true;
        }


    }
}
