using Assets.OpenMM8.Scripts.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Data
{
    public class SkillDescriptionData : DbData
    {
        public SkillType SkillType;
        public SkillGroupType SkillGroup;
        public string Name;
        public string Description;
        public string Normal;
        public string Expert;
        public string Master;
        public string GrandMaster;
    }
}
