using _7DTDManager.Interfaces;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace _7DTDManager.Localize
{

    public class MessageLocalizer : IMessageLocalizer
    {
        static Logger logger = LogManager.GetCurrentClassLogger();
        public static Dictionary<string, TranslationLanguage> Languages = new Dictionary<string, TranslationLanguage>();
        static TranslationLanguage defaultLanguage;
        static TranslationLanguage exportLanguage = null;
        public static MessageLocalizer Instance = new MessageLocalizer();

        public static void Init()
        {
            logger.Info("Loading Languages ...");
            String path = System.IO.Path.Combine(Program.ApplicationDirectory, "lang");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            foreach (var file in Directory.EnumerateFiles(path, "*.xml"))
            {
                try
                {
                    if (Path.GetFileName(file) == "english-default.xml")
                        continue;
                    logger.Info("Loading {0}", file);
                    XmlSerializer serializer = new XmlSerializer(typeof(TranslationLanguage));
                    StreamReader reader = new StreamReader(file);
                    TranslationLanguage c = (TranslationLanguage)serializer.Deserialize(reader);
                    reader.Close();
                    c.Init();
                    Languages[c.Language.ToLowerInvariant()] = c;
                    if (c.Language.ToLowerInvariant() == Program.Config.DefaultLanguage)
                        defaultLanguage = c;
                    logger.Info("Language: {0} ({1} Translations)", c.Language, c.Messages.Count);
                }
                catch (Exception ex)
                {
                    logger.Error("Error loading {0}: {1}", file, ex.ToString());
                }
            }
        }
       
        public static string GetPlayerLocalization(IPlayer p, string key)
        {
            string retVal = String.Empty;
            key = key.ToLowerInvariant();
            if ((p != null) && (Languages.ContainsKey(p.Language)))
                retVal = Languages[p.Language].Translate(key);
            return Program.Config.ReplaceConfigValues(retVal); 
        }

        public static string GetLocalization(IPlayer p, string key)
        {
            string retVal = String.Empty;
            key = key.ToLowerInvariant();
            if ((p != null) && (Languages.ContainsKey(p.Language)))
                retVal = Languages[p.Language].Translate(key);
            if (String.IsNullOrEmpty(retVal))
                retVal = defaultLanguage.Translate(key);
            if (String.IsNullOrEmpty(retVal))
                retVal = "MissingKey " + key;            
            return Program.Config.ReplaceConfigValues(retVal);
        }

        public static List<string> GetSubkeysStartingWith(IPlayer p, string key)
        {
            List<string> retVal = new List<string>();
            TranslationLanguage lang = defaultLanguage;
            int x = key.Length;
            key = key.ToLowerInvariant();
            if ((p != null) && (Languages.ContainsKey(p.Language)))
                lang = Languages[p.Language];
            foreach (var item in lang.dictMessages.Keys)
            {
                if (item.StartsWith(key))
                    retVal.Add(item.Substring(x).ToLowerInvariant());
            }
            return retVal;
        }

        public static string Localize(IPlayer p, string key,params object[] args)
        {
            string tKey = key.ToLowerInvariant();
            if (tKey.StartsWith("r:"))
                key = GetLocalization(p,tKey.Substring(2));
            if ((args != null) && (args.Length > 0))
                return String.Format(key, args);
            return key;
        }

        public static void InitTranslation(string language)
        {
            exportLanguage = new TranslationLanguage { Language = language };

            AddTranslation("Cmd.Bounty.Command", "bounty");
            AddTranslation("Cmd.Bounty.Help", "Set a bounty on another players head. usage: /bounty [howmany] [tragetname]");
            AddTranslation("Cmd.Bounty.Usage", "usage: /bounty [howmany] [targetname]");
            AddTranslation("Cmd.Bounty.NewBounty", "You set a bounty of {0} coins on {1}'s head.");

            AddTranslation("Cmd.Buy.Command", "buy");
            AddTranslation("Cmd.Buy.Help", "Lets you buy an item from the shop.");
            AddTranslation("Cmd.Buy.Usage", "/buy <amount> <itemid#>. (/buy 5 1) See /list for itemids");


            AddTranslation("Cmd.Help.Command", "help");
            AddTranslation("Cmd.Help.Help", "Show help about commands. Usage: /help or /help [command]");
            AddTranslation("Cmd.Help.Available", "Available commands: {0}");
            AddTranslation("Cmd.Help.AvailableAdmin", "Admincommands: {0}");
            AddTranslation("Cmd.Help.AvailableInfo", "Available Information: {0}");
            AddTranslation("Cmd.Help.HelpCommand","Command: {0}");
            AddTranslation("Cmd.Help.HelpUsage","Usage: {0}" );
            AddTranslation("Cmd.Help.HelpCost","Cost: {0} coins" );
            AddTranslation("Cmd.Help.HelpCooldown","Cooldown: {0} minutes" );
            AddTranslation("Cmd.Help.HelpAliases","Aliases: {0}" );

            AddTranslation("Cmd.Home.Command", "home");
            AddTranslation("Cmd.Home.Help", "Teleports you to your home set with /sethome");
            AddTranslation("Cmd.Home.NoHome", "No homeposition for you recorded. set it with /sethome");

            AddTranslation("Cmd.List.Command", "list");
            AddTranslation("Cmd.List.Help", "Shows the items in stock in a shop.");
            AddTranslation("Cmd.List.Usage", "/list [page]");

            AddTranslation("Cmd.Mail.Command", "mail");
            AddTranslation("Cmd.Mail.Help", "Read/write a mail");
            AddTranslation("Cmd.Mail.Usage", "/mail read or /mail [targetplayer] [message]");
            AddTranslation("Cmd.Mail.Sent", "Mail sent to '{0}'.");

            AddTranslation("Cmd.SetHome.Command", "sethome");
            AddTranslation("Cmd.SetHome.Help", "Set the position you will teleport to using /home");
            AddTranslation("Cmd.SetHome.NewHome", "Homeposition set to {0}");

            AddTranslation("Cmd.Shops.Command", "shops"); // 
            AddTranslation("Cmd.Shops.Help", "Show known shops");
            AddTranslation("Cmd.Shops.NoShops", "No shops registered.");
            AddTranslation("Cmd.Shops.KnownShops", "Known shops:");
            
            AddTranslation("Cmd.Stats.Command", "stats");
            AddTranslation("Cmd.Stats.Help", "Show your stats");
            AddTranslation("Cmd.Stats.StatsFor", "Stats for {0}:");
            AddTranslation("Cmd.Stats.AgeCoins", "Age: {0} Coins: {1}");
            AddTranslation("Cmd.Stats.BountyCollected", "Bounties collected: {0} coins Bloodmoney collected: {1} coins");
            AddTranslation("Cmd.Stats.Travel", "You travelled {0} km so far.");
            AddTranslation("Cmd.Stats.Bounty", "Bounty on your head: {0} coins");
          
            AddTranslation("Cmd.Transfer.Command", "transfer");
            AddTranslation("Cmd.Transfer.Help", "Transfer coins to another player. usage: /transfer [howmany] [targetname]");
            AddTranslation("Cmd.Transfer.Usage", "usage: /transfer [howmany] [targetname]");
            AddTranslation("Cmd.Transfer.SenderMsg", "You transferred {0} coins to {1}.");
            AddTranslation("Cmd.Transfer.ReceiverMsg", "{0} transferred {1} coins to your wallet.");

            AddTranslation("Cmd.Wallet.Command", "wallet");
            AddTranslation("Cmd.Wallet.Help", "Show your current coin balance.");
            AddTranslation("Cmd.Wallet.ResultMsg", "You have {0} coins.");

            AddTranslation("Cmd.AddCoins.Command", "addcoins");
            AddTranslation("Cmd.AddCoins.Help", "Give yourself some coins");
            AddTranslation("Cmd.AddCoins.ConfirmMsg", "You added {0} coins to your wallet.");

            AddTranslation("Cmd.Language.Command", "language");
            AddTranslation("Cmd.Language.Help", "Set your language");
            AddTranslation("Cmd.Language.Available", "Available languages: {0}");
            AddTranslation("Cmd.Language.Current", "Current language: {0}");
            AddTranslation("Cmd.Language.Unknown", "Unknown language '{0}'");

            AddTranslation("Protection.ExpireMsg", "Protection for area #{0} will expire in {1}");
            AddTranslation("Protection.Enter", "entered area");
            AddTranslation("Protection.Leave", "left area");
            AddTranslation("Protection.Visit", "visited area");
            AddTranslation("Protection.Destroyed", "{0} {1}: Protection for area #{2} was destroyed.");
            AddTranslation("Protection.Expired", "{0} {1}: Protection for area #{2} expired.");
            AddTranslation("Protection.Stay", "(Stay {0})");
            
            AddTranslation("Cmd.Protection.Command", "protection");
            AddTranslation("Cmd.Protection.Help", "Controls keystone protection");
            AddTranslation("Cmd.Protection.Usage", "/protection show|list|buy|events");
            AddTranslation("Cmd.Protection.YourProtections", "Your protections:");
            AddTranslation("Cmd.Protection.Available", "Available protections:");
            AddTranslation("Cmd.Protection.HelpBuy", "To buy a protection: /{0} {1} <areaid> <amount> <itemid#>");
            AddTranslation("Cmd.Protection.HelpBuy1", "e.g. '/{0} {1} 25 15 1' to protect area 25 for 15 RL Days");
            AddTranslation("Cmd.Protection.NoSuchArea", "No such protection area. See '/{0} {1}'.");
            AddTranslation("Cmd.Protection.Update", "Protection area #{0} has been updated.");
            AddTranslation("Cmd.Protection.UsageEvents", "/{0} {1} <areaid>");
            AddTranslation("Cmd.Protection.NoEvents", "No Events recoreded for area #{0}");
            AddTranslation("Cmd.Protection.Recorded", "Events recoreded for area #{0}:");
            AddTranslation("Cmd.Protection.NoMoreEvents", "No more Events recoreded for area #{0}");
            AddTranslation("Cmd.Protection.MoreEvents", "{0} more events recored for area #{1}");         

            AddTranslation("Here", "HERE");
            AddTranslation("show", "show");
            AddTranslation("page", "page");
            AddTranslation("where", "where");
            AddTranslation("protected", "protected"); 
            AddTranslation("events", "events");
            AddTranslation("list", "list");
            AddTranslation("buy", "buy");
            
            

            AddTranslation("Bounty.AddedBounty", "A bounty of {0} coins has been set on your head. Total bounty: {1}");
            AddTranslation("Mail.Inbox", "You have {0} unread mail(s). Use '/mail read' to read them.");
            AddTranslation("Mail.InboxEmpty", "You have no unread mail.");
            AddTranslation("Mail.From", "From: {0}");
            AddTranslation("Mail.When", "Date: {0} {1}");
            AddTranslation("Mail.Text", "Text: {0}");
            AddTranslation("Shop.OutOfStock", "No such item in stock. Sorry.");
            AddTranslation("Shop.ShortStock", "There are only {0} {1}(s) in stock.");
            AddTranslation("Shop.OutOfCoins", "You do not have enough coins ({0}) for this transaction.");
            AddTranslation("Shop.NotInside", "You are not inside a shop area. see /shops for a list of shops.");
            AddTranslation("Shop.Closed", "The shop is currently closed. Opening hours: {0} - {1}");
            AddTranslation("Shop.NotOutside", "Command not possible inside a shop area.");
            AddTranslation("Shop.Item.Name", "Name");
            AddTranslation("Shop.Item.Price", "Price");
            AddTranslation("Shop.Item.Stock", "Stock");
            AddTranslation("TimeSpan.Days", "{0} Days");
            AddTranslation("TimeSpan.Hours", "{0} Hours");
            AddTranslation("TimeSpan.Minutes", "{0} Minutes");
            AddTranslation("TimeSpan.ShortDays", "{0}d");
            AddTranslation("TimeSpan.ShortHours", "{0}h");
            AddTranslation("TimeSpan.ShortMinutes", "{0}m");
           
            AddTranslation("List.Page", "--- Page {0} of {1} ----");
            AddTranslation("Error.NoSuchCommand", "No such command '{0}'");
            AddTranslation("Error.TargetNotFound", "Targetplayer '{0}' was not found.");
            AddTranslation("Error.NotEnoughCoins", "You don't have enough coins in your wallet.");
            AddTranslation("Error.NoPosition", "No valid position for you recorded. Wait a little please.");
            AddTranslation("Error.NotAllowed", "You are not allowed to use this command.");
            AddTranslation("Error.CommandsDisabled", "Commands are currently disabled.");
            AddTranslation("Error.CommandDisabled", "Command '{0}' is currently disabled.");
            AddTranslation("Error.Cooldown", "You will need to wait another {0} Minutes before you can use this command again.");
            AddTranslation("Error.CommandFailed", "Command failed.");
            AddTranslation("Info.Coins", "You will get {CoinsPerMinute} coin(s) per minute playtime (not idletime!) and {CoinsPerZombiekill} coin(s) per zombie slain.");
            AddTranslation("Info.Death", "You will loose {CoinLossPerDeath} coin(s) if you die (not exterminated by another player!)");
            AddTranslation("Info.Bounties", "When you eliminate another player a bounty will be set on your head");
        }

        public static void AddTranslation(string key,string value)
        {
            if (exportLanguage != null )
                exportLanguage.Add(key, value);
        }

        public static void SaveTranslation()
        {
            if (exportLanguage != null)
            {
                String path = System.IO.Path.Combine(Program.ApplicationDirectory, "lang");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                XmlSerializer serializer = new XmlSerializer(typeof(TranslationLanguage));
                StreamWriter writer = new StreamWriter(Path.Combine(path,exportLanguage.Language+".xml"));
                serializer.Serialize(writer, exportLanguage);
                writer.Close();
            }
        }

        public Dictionary<string, Regex> CreateLocalizedCommandExpressions(string baseRegex)
        {
            throw new NotImplementedException();
        }

        public IReadOnlyList<string> CreateLocalizedCommandNames(string commandKey)
        {
            commandKey = commandKey.ToLowerInvariant();
            if (!commandKey.StartsWith("r:"))
                return null;
            List<string> cmds = new List<string>();
            commandKey = commandKey.Substring(2);
            foreach (var item in Languages)
            {
                string tmp = item.Value.Translate(commandKey);
                if ( !String.IsNullOrEmpty(tmp) && !cmds.Contains(tmp))
                    cmds.Add(tmp);
            }
            return cmds;
        }

        IReadOnlyList<string> IMessageLocalizer.GetSubkeysStartingWith(IPlayer p, string key)
        {
            return MessageLocalizer.GetSubkeysStartingWith(p, key);
        }

        string IMessageLocalizer.Localize(IPlayer p, string key, params object[] args)
        {
            return MessageLocalizer.Localize(p, key, args);
        }
        string IMessageLocalizer.GetPlayerLocalization(IPlayer p, string key)
        {
            return MessageLocalizer.GetPlayerLocalization(p, key);
        }

        IReadOnlyList<string> IMessageLocalizer.AvailableLanguages
        {
            get
            {
                return Languages.Keys.ToList<string>();
            }
        }
    }

    [Serializable]
    [XmlRoot(Namespace="http://fsgs.com/Schemas/7DTD_LANG")]
    public class TranslationLanguage
    {        
        public string Language { get; set; }
        public List<TranslatedMessage> Messages { get; set; }

        [XmlIgnore]
        public Dictionary<string, string> dictMessages = new Dictionary<string,string>();

        public TranslationLanguage()
        {
            Messages = new List<TranslatedMessage>();            
        }

        public void Init()
        {
            foreach (var item in Messages)
            {
                dictMessages[item.Key.ToLowerInvariant()] = item.Value;
            }
        }
        public string Translate(string key)
        {
            if (!dictMessages.ContainsKey(key))
                return String.Empty;
            return dictMessages[key];
        }

        internal void Add(string key, string value)
        {
            dictMessages.Add(key, value);
            Messages.Add(new TranslatedMessage(key, value));
        }
    }

    [Serializable]
    [XmlRoot(Namespace = "http://fsgs.com/Schemas/7DTD_LANG")]
    public class TranslatedMessage
    {
        [XmlAttribute]
        public string Key {get;set;}
        [XmlText]
        public string Value {get;set;}

        public TranslatedMessage()
        {

        }
        public TranslatedMessage(string key,string value) : this()
        {
            Key = key;
            Value = value;
        }
    }
}
