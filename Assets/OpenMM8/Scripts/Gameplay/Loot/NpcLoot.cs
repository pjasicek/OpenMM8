using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Assets.OpenMM8.Scripts.Gameplay.Items;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    [System.Serializable]
    public class Loot
    {
        public int GoldAmount = 0;
        public Item Item = null;
    }
}
