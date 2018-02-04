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

        public Dictionary<Attribute, int> Attributes = new Dictionary<Attribute, int>();
        public Dictionary<SpellElement, int> Resistances = new Dictionary<SpellElement, int>();
    }
}
