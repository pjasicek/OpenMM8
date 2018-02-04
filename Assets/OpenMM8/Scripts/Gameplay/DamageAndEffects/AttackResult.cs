using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class AttackResult
    {
        public AttackResultType Type = AttackResultType.None;
        public string HitObjectName = "";
        public int DamageDealt = 0;
    }
}
