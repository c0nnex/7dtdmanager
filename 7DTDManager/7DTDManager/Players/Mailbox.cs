using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _7DTDManager.Players
{
    [Serializable]
    public class Mailbox : IMailbox
    {
        public List<MailMessage> Mails = new List<MailMessage>();

        public void AddMail(IPlayer from, string message)
        {
            Mails.Add(new MailMessage { FromSteamID = from.SteamID, When = DateTime.Now, Message = message });
        }

        public void RemoveMail(IMailMessage mail)
        {
            Mails.Remove(mail as MailMessage);
        }

        public void Clear()
        {
            Mails.Clear();
        }

        [XmlIgnore]
        IReadOnlyList<IMailMessage> IMailbox.Mails
        {
            get { return Mails as IReadOnlyList<IMailMessage>; }
        }
    }

    public class MailMessage : IMailMessage
    {
        public string FromSteamID { get; set; }
        public DateTime When { get; set; }
        public string Message { get; set; }

    }
}
