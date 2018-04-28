using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class AttackResult
    {
        public AttackResultType Type = AttackResultType.None;
        public int DamageDealt = 0;
        public string VictimName = "";
        public GameObject Victim;
    }
}
