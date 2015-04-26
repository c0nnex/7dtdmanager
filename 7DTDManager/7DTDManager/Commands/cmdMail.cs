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
            CommandHelp = "R:Cmd.Mail.Help";
            CommandName = "R:Cmd.Mail.Command";
            CommandCost = 0;
            CommandUsage = "R:Cmd.Mail.Usage";
            CommandArgs = 0;
        }

        public override bool Execute(IServerConnection server, IPlayer p, params string[] args)
        {
            if (args.Length == 1)
            {
                if (p.Mailbox.Mails.Count > 0)
                    p.Confirm("R:Mail.Inbox", p.Mailbox.Mails.Count);
                else
                    p.Confirm("R:Mail.InboxEmpty");
                return true;
            }
            if (args[1] == p.Localize("R:read"))
            {
                if (p.Mailbox.Mails.Count <= 0)
                {
                    p.Error("R:Mail.InboxEmpty");
                    return true;
                }
                IMailMessage mail = p.Mailbox.Mails[0];
                p.Mailbox.RemoveMail(mail);
                p.Message("R:Mail.From", server.AllPlayers.FindPlayerBySteamID(mail.FromSteamID).Name);
                p.Message("R:Mail.When", mail.When.ToShortDateString(), mail.When.ToShortTimeString());
                p.Message("R:Mail.Text", mail.Message);
                return true;
            }
            IPlayer targetPlayer = server.AllPlayers.FindPlayerByNameOrID(args[1],false);
            if ((targetPlayer == null))
            {
                p.Message("R:Error.TargetNotFound", args[1]);
                return false;
            }
            if (args.Length < 3)
            {
                p.Error(CommandUsage);
                return true;
            }
            string restCmd = String.Join(" ", args, 2, args.Length - 2);
            targetPlayer.Mailbox.AddMail(p, restCmd);
            p.Confirm("R:Cmd.Mail.Sent", targetPlayer.Name);
            return true;
        }
        
    }

    
}
