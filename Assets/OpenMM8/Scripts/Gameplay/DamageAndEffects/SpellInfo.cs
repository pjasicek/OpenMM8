using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class SpellInfo
    {
        public string SpellName = "";
        public SkillMastery SpellMastery = 0;
        public int SpellLevel = 0;
        CharacterClass PreferredClass = CharacterClass.None;

        public SpellInfo(string def)
        {
            string[] defs = def.ToLower().Split(',');
            if (defs.Length == 3)
            {
                SpellName = defs[0].ToLower();
                switch (defs[1])
                {
                    case "n": SpellMastery = SkillMastery.Normal; break;
                    case "e": SpellMastery = SkillMastery.Expert; break;
                    case "m": SpellMastery = SkillMastery.Master; break;
                    case "gm": SpellMastery = SkillMastery.Grandmaster; break;
                    default: SpellMastery = SkillMastery.Normal; break;
                }
                SpellLevel = int.Parse(defs[2]);
            }
        }
    }
}
