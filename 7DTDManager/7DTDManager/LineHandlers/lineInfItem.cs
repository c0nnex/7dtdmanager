using _7DTDManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace _7DTDManager.LineHandlers
{
    public class lineInfItem : BaseLineHandler
    {
        static Regex rgItem = new Regex("INF ITEM: (?<name>.*)");

        public override bool ProcessLine(IServerConnection serverConnection, string currentLine)
        {
            if (rgItem.IsMatch(currentLine))
            {
                Match match = rgItem.Match(currentLine);
                GroupCollection groups = match.Groups;

                Config.Configuration.AllKnownItems.Add(groups["name"].Value.ToLowerInvariant());
                return true;
            }
            return false;
        }

       

    }
}
