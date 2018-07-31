using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class Inventory
    {
        //=================================== Member Variables ===================================

        public Character Owner;

        // Inventory has 9x14 cells, items usually span over multiple cells
        public const int INVENTORY_ROWS = 9;
        public const int INVENTORY_COLUMNS = 14;
        public InventoryCell[] InventoryCells = new InventoryCell[INVENTORY_ROWS * INVENTORY_COLUMNS];

        // Items in inventory
        public List<BaseItem> InventoryItems = new List<BaseItem>();

        // Equipped items
        public BaseItem RightHandSlot;
        public BaseItem LeftHandSlot;
        public BaseItem BowSlot;
        public BaseItem ArmorSlot;
        public BaseItem HelmetSlot;
        public BaseItem BeltSlot;
        public BaseItem CloakSlot;
        public BaseItem BootsSlot;
        public BaseItem AmuletSlot;
        public BaseItem GauntletsSlot;
        public BaseItem[] RingSlots = new BaseItem[6];

        //=================================== Methods ===================================

        public Inventory()
        {
            for (int i = 0; i < InventoryCells.Length; i++)
            {
                InventoryCells[i] = new InventoryCell();
            }
        }

        public bool AddItem(BaseItem item)
        {
            

            return true;
        }

        public bool AddItem(int itemId)
        {
            ItemData itemData = DbMgr.Instance.ItemDb.Get(itemId);

            Vector2Int itemPos;
            if (!CanPlaceItem(itemData, out itemPos))
            {
                Debug.Log("Inventory full");
                return false;
            }

            BaseItem item = ItemFactory.CreateItem(itemData);
            item.InvCellPosition = itemPos;
            GetCells(itemPos.x, itemPos.y, itemData.InvSize.x, itemData.InvSize.y)
                .ForEach(cell => cell.Item = item);

            InventoryItems.Add(item);

            Owner.UI.InventoryUI.AddItem(item);

            Debug.Log("Item (" + itemId + ") added");

            return true;
        }

        public bool RemoveItem(BaseItem item)
        {
            if (!HasItem(item))
            {
                return false;
            }

            return true;
        }

        public bool RemoveItem(int itemId)
        {
            if (!HasItem(itemId))
            {
                return false;
            }

            return true;
        }

        public bool HasItem(BaseItem item)
        {
            return true;
        }

        public bool HasItem(int itemId)
        {
            return true;
        }

        bool CanPlaceItem(ItemData itemData, out Vector2Int itemPos)
        {
            itemPos = new Vector2Int();
            Vector2Int itemSize = itemData.InvSize;

            // | y
            // |
            // |             x
            // L______________
            for (int x = 0; x < INVENTORY_COLUMNS; x++)
            {
                for (int y = 0; y < INVENTORY_ROWS; y++)
                {
                    if (IsCellsFree(x, y, itemData.InvSize.x, itemData.InvSize.y))
                    {
                        itemPos.x = x;
                        itemPos.y = y;

                        return true;
                    }
                }
            }

            return false;
        }

        // =================== Inventory Cell handling =======================

        public InventoryCell GetCell(int x, int y)
        {
            if (x >= INVENTORY_COLUMNS || y >= INVENTORY_ROWS)
            {
                return null;
            }

            return InventoryCells[y * INVENTORY_COLUMNS + x];
        }

        public List<InventoryCell> GetCells(int startX, int startY, int width, int height)
        {
            if ((startX + width) > INVENTORY_COLUMNS ||
                (startY + height) > INVENTORY_ROWS)
            {
                return null;
            }

            List<InventoryCell> cells = new List<InventoryCell>();
            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    InventoryCell cell = GetCell(x, y);
                    cells.Add(cell);
                }
            }

            return cells;
        }

        public bool IsCellsFree(int startX, int startY, int width, int height)
        {
            if ((startX + width) > INVENTORY_COLUMNS ||
                (startY + height) > INVENTORY_ROWS)
            {
                return false;
            }

            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    InventoryCell cell = GetCell(x, y);
                    if (cell == null || cell.Item != null)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        public void ClearAllCells()
        {
            foreach (InventoryCell cell in InventoryCells)
            {
                cell.Clear();
            }
        }

        public void ClearCells(int startX, int startY, int width, int height)
        {
            if ((startX + height) > INVENTORY_COLUMNS ||
                (startY + width) > INVENTORY_ROWS)
            {
                return;
            }

            List<InventoryCell> cells = new List<InventoryCell>();
            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    InventoryCell cell = GetCell(x, y);
                    cell.Clear();
                }
            }
        }
    }
}
