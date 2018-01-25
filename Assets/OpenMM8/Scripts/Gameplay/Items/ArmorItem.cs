using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay.Items
{
    class ArmorItem : BaseItem
    {
        public ArmorItem(ref ItemData itemData) : base(ref itemData)
        {
            
        }

        public override ItemInteractResult InteractWithDoll(Character player)
        {
            return ItemInteractResult.Invalid;
        }
    }
}
