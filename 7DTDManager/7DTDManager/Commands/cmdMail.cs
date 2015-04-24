using _7DTDManager.Interfaces;
using _7DTDManager.Interfaces.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Commands
{
    public class cmdMail : PublicCommandBase
    {
        public cmdMail()
        {
            CommandHelp = "Read/write a mail";
            CommandName = "mail";
            CommandCost = 0;
            CommandUsage = "/mail read or /mail [targetplayer] [message]";
            CommandArgs = 0;
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            if (args.Length == 1)
            {
                if (p.Mailbox.Mails.Count > 0)
                    p.Confirm("You have {0} unread mail(s). Use '/mail read' to read them.", p.Mailbox.Mails.Count);
                else
                    p.Confirm("You have no unread mail.");
                return true;
            }
            if (args[1] == "read")
            {
                if (p.Mailbox.Mails.Count <= 0)
                {
                    p.Error("No unread mail.");
                    return true;
                }
                IMailMessage mail = p.Mailbox.Mails[0];
                p.Mailbox.RemoveMail(mail);
                p.Message("From: {0}", server.AllPlayers.FindPlayerBySteamID(mail.FromSteamID).Name);
                p.Message("Date: {0} {1}", mail.When.ToShortDateString(), mail.When.ToShortTimeString());
                p.Message("Text: {0}", mail.Message);
                return true;
            }
            IPlayer targetPlayer = server.AllPlayers.FindPlayerByNameOrID(args[1],false);
            if ((targetPlayer == null))
            {
                p.Message("Targetplayer '{0}' was not found.", args[1]);
                return false;
            }
            if (args.Length < 3)
            {
                p.Error(CommandUsage);
                return true;
            }
            string restCmd = String.Join(" ", args, 2, args.Length - 2);
            targetPlayer.Mailbox.AddMail(p, restCmd);
            p.Confirm("Mail sent to '{0}'.", targetPlayer.Name);
            return true;
        }
        
    }

    
}
