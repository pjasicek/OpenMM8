using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class CharacterData
    {
        public CharacterType CharacterType = CharacterType.None;

        //public int CharacterAvatarId;
        //public int PartyIndex;

        public string Name;
        public Class Class;
        public int Experience;
        public int SkillPoints;
        public int CurrHitPoints;
        public int CurrSpellPoints;
        public Condition Condition;

        public string QuickSpellName = "";

        public CharacterStats DefaultStats = new CharacterStats();
        public CharacterStats BonusStats = new CharacterStats();
        public Dictionary<SkillType, int> Skills = new Dictionary<SkillType, int>();
        public Dictionary<SkillType, int> SkillBonuses = new Dictionary<SkillType, int>();
        public List<Award> Awards = new List<Award>();
        public List<Spell> Spells = new List<Spell>();
    }
}
