using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _7DTDManager.Interfaces
{
    public interface IMessageLocalizer
    {
        Dictionary<string, Regex> CreateLocalizedCommandExpressions(string baseRegex);
        IReadOnlyList<string> CreateLocalizedCommandNames(string commandKey);
        string Localize(IPlayer p, string key, params object[] args);
        string GetPlayerLocalization(IPlayer p, string key);
        IReadOnlyList<string> GetSubkeysStartingWith(IPlayer p, string key);

        IReadOnlyList<string> AvailableLanguages { get; }
    }
}
