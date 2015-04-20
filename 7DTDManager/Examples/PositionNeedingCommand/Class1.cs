using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PositionNeedingCommand
{
    /*
     * To Add this command to 7DTDManager, compile it and place DLL in "ext" Folder.
    */

    public class Class1 : PublicCommandBase
    {
        public Class1()
        {
            CommandName = "ex1";
            CommandHelp = "Example 1: Command needing most current position.";
            CommandCoolDown = 0;
            CommandCost = 0;
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            // OK, so we want to execute this command on the most recent position of a player
            
            // Subscribe to the PositionUpdate. Note: This is a ONETIME event!
            p.PlayerPositionUpdated += p_PlayerPositionUpdated;

            // Now have the server execute the lp command to update the positions
            server.Execute("lp");

            return true; // Command Sucessfully executed, Deduct Costs and set Cooldowns
        }

        /// <summary>
        /// After lp-Command is parsed, the position of all players will be updated and
        /// this event raised for the player we subscribed to above
        /// </summary>
        /// <param name="sender">IPlayer of the player which position changed. We subscribed to this earlier</param>
        /// <param name="e">The new Position of the player</param>
        void p_PlayerPositionUpdated(object sender, PlayerPositionUpdateEventArgs e)
        {
            
            IPlayer p = sender as IPlayer;

            Log.Debug("Positionupdate received for {0} Position: {1}", p.Name,p.CurrentPosition.ToHumanString());

            // Now here we can process the real command, like setting the home
            // No need to care for unsubscribiung from the event. The Manager does that for us

        }

    }
}
