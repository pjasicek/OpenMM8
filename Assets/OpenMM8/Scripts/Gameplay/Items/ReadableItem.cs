using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay.Items
{
    class ReadableItem : BaseItem
    {
        public ReadableItem(ref ItemData itemData) : base(ref itemData)
        {
            
        }

        public override ItemInteractResult InteractWithDoll(Character player)
        {
            return ItemInteractResult.Invalid;
        }
    }
}
