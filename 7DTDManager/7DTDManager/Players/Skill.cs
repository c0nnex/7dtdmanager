using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _7DTDManager.Players
{
    public class Skill
    {
        public string SKillName { get; set; }
        public int SkillLevel { get; set; }
        public DateTime NextAdvance { get; set;}
    }
}
