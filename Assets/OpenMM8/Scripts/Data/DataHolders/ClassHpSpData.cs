using Assets.OpenMM8.Scripts.Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Data
{
    public class ClassHpSpData : DbData<CharacterClass>
    {
        // int Id = Class
        public int HitPointsBase;
        public int HitPointsFactor;
        public int SpellPointsBase;
        public int SpellPointsFactor;
        public bool IsSpellPointsFromIntellect;
        public bool IsSpellPointsFromPersonality;
    }
}
