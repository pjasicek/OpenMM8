using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    class InventorySystem
    {
        public delegate void ItemEquippedAction(ItemData item);
        public delegate void ItemUneqippedAction(ItemData item);
        public delegate void ItemReplacedAction(ItemData replacedItem, ItemData newItem);

        public event ItemEquippedAction OnItemEquipped;
        public event ItemUneqippedAction OnItemUnequipped;
        public event ItemReplacedAction OnItemReplaced;

        private Inventory Inventory;

        public InventorySystem(ref Inventory inventory)
        {
            Inventory = inventory;
        }

        public bool CanEquipItem(InventoryItem item)
        {
            return false;
        }

        public bool TryEquipItem(InventoryItem item)
        {
            return false;
        }

        public bool CanPickUpItem(InventoryItem item)
        {
            return false;
        }

        public bool TryPickUpItem(InventoryItem item)
        {
            return false;
        }
    }
}
