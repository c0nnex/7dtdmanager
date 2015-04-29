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

    public class DelayedEffectCommand : PublicCommandBase
    {
        public DelayedEffectCommand()
        {
            CommandName = "ex2";
            CommandHelp = "Example 2: Command sending a message to player after a delay.";
            CommandCoolDown = 0;
            CommandCost = 0;
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            // Add an Callput to the Manager.
            // Example2EffectCallout.CalloutCallback will be called after 10 seconds, since persitance is false
            // the callout will be removed if callback returns false. 
            // Setting presistence to true means, the callout will be repeated every Timespand forever. Be careful with
            // this option!
            // NOTE: We give the targetplayer as OWNER, so the callout will be voided automatically voided if the player logs out!
            // and we can access the player via ICallout.Owner in the Callout itself. Nifty Nifty!
            server.CalloutManager.AddCallout(p, new Example2EffectCallout(p), TimeSpan.FromSeconds(10), false);

            return true; // Command Sucessfully executed, Deduct Costs and set Cooldowns
        }

       
    }
}
