using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class CharacterStats
    {
        public int MaxHitPoints;
        public int MaxSpellPoints;
        public int ArmorClass;

        public int Age;
        public int Level;

        public Dictionary<CharAttribute, int> Attributes = new Dictionary<CharAttribute, int>();
        public Dictionary<SpellElement, int> Resistances = new Dictionary<SpellElement, int>();
        public Dictionary<SkillType, int> Skills = new Dictionary<SkillType, int>();

        public CharacterStats()
        {
            foreach (CharAttribute attr in Enum.GetValues(typeof(CharAttribute)))
            {
                Attributes.Add(attr, 0);
            }

            foreach (SpellElement elem in Enum.GetValues(typeof(SpellElement)))
            {
                Resistances.Add(elem, 0);
            }

            foreach (SkillType skillType in Enum.GetValues(typeof(SkillType)))
            {
                Skills.Add(skillType, 0);
            }
        }
    }
}
