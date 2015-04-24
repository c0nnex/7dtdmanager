﻿using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.AdminCommands
{
    public class cmdClearCooldown : AdminCommandBase
    {
        public cmdClearCooldown()
        {
            CommandName = "clearcooldown";
            CommandHelp = "Clear cooldowns of a player.";
            CommandLevel = 1;
            CommandAliases = new string[] { "ccd" };
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            if ( args.Length < 2)
            {
                return false;
            }
            IPlayer target = server.AllPlayers.FindPlayerByNameOrID(args[1]);
            if ((target == null) || (!target.IsOnline))
            {
                p.Message("Targetplayer '{0}' was not found or is not online.", args[1]);
                return false;
            }
            target.ClearCooldowns();
            p.Message("You cleared {0}'s cooldowns.",  target.Name);
            target.Message("{0} cleared your cooldowns.", p.Name);
            server.AllPlayers.Save();
            return true;
        }
    }
}
