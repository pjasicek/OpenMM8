using Assets.OpenMM8.Scripts.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Data
{
    public enum StartingSkillAvailability
    {
        HasByDefault,
        CanLearn,
        None
    }

    public class ClassStartingSkillsData : DbData<CharacterClass>
    {
        public Dictionary<SkillType, StartingSkillAvailability> SkillAvailabilityMap =
            new Dictionary<SkillType, StartingSkillAvailability>();
    }
}
