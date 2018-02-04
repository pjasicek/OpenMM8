using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class AttackInfo
    {
        public int MinDamage = 0;
        public int MaxDamage = 0;
        public int AttackMod = 0;
        public int SourceLevel = 0;
        public SpellElement DamageType = SpellElement.None;
        public Class PreferredClass = Class.None;
        public string Missile = "";
    }
}
