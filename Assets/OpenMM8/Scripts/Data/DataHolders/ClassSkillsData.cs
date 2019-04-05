using Assets.OpenMM8.Scripts.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Data
{
    public class ClassSkillsData : DbData<Class>
    {
        // int Id = Class
        public Dictionary<SkillType, SkillMastery> SkillTypeToSkillMasteryMap 
            = new Dictionary<SkillType, SkillMastery>();
    }
}
