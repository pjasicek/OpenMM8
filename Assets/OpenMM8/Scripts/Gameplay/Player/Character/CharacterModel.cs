using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    class CharacterModel
    {
        public int CharacterAvatarId;
        public int PartyIndex;

        public string Name;
        public Class Class;
        public int Experience;
        public int SkillPoints;
        public int CurrHitPoints;
        public int CurrSpellPoints;
        public Condition Condition;

        public string QuickSpellName = "";

        public CharacterStats DefaultStats;
        public CharacterStats BonusStats;
        public List<Skill> Skills = new List<Skill>();
        public Dictionary<SkillType, int> SkillBonuses = new Dictionary<SkillType, int>();
        public Inventory Inventory;
        public List<Award> Awards = new List<Award>();
        public List<Spell> Spells = new List<Spell>();
    }
}
