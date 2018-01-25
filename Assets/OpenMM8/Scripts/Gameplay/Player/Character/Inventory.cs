using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    class InventoryItem
    {
        public ItemData Item;
        public Vector2Int SlotPosition;
        public Vector2Int SlotSize;
        public Vector2Int PixelOffset;
        public bool IsEquipped;
    }

    class Inventory
    {
        public List<InventoryItem> ItemList = new List<InventoryItem>();
    }
}
