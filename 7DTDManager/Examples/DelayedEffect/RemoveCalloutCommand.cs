using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DelayedEffect
{
    /*
     * To Add this command to 7DTDManager, compile it and place DLL in "ext" Folder.
    */

    public class RemoveCalloutCommand : PublicCommandBase
    {
        public RemoveCalloutCommand()
        {
            CommandName = "ex2remove";
            CommandHelp = "Example 2: remove the 10 minutes persitent callout";
            CommandCoolDown = 0;
            CommandCost = 0;
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            // Posibility 1: Remove all Callouts belonging to "myExtension"
            server.CalloutManager.RemoveAllCalloutsFor(myExtension.Instance);

            // Posibility 2: Just remove my specfically created callout
            server.CalloutManager.RemoveAllCalloutsFor(myExtension.Instance.myEvery10MinutesCalled);

            return true; // Command Sucessfully executed, Deduct Costs and set Cooldowns
        }

       
    }
}
