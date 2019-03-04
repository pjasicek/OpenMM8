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

        public ItemInteractResult TryEquipItem(BaseItem heldItem, out BaseItem replacedItem)
        {
            replacedItem = null;

            // 1) Check if item is equippable

            /* bool ok = false; //tmp
             switch (heldItem.Data.EquipType)
             {
                 case EquipType.Armor:
                 case EquipType.Missile:
                 case EquipType.Helmet:
                 case EquipType.Belt:
                 case EquipType.Cloak:
                 case EquipType.Boots:
                     ok = true;
                     break;
                 default:
                     break;
             }

             if (!ok)
             {
                 return ItemInteractResult.Invalid;
             }*/

            // 2) Filter if character can use this item (Skill group)
            // ....

            // 
            
            // TODO: Add support for this
            if (heldItem.Data.EquipType == EquipType.Amulet ||
                heldItem.Data.EquipType == EquipType.Gauntlets ||
                heldItem.Data.EquipType == EquipType.Ring)
            {
                return ItemInteractResult.Invalid;
            }

            bool isWeapon = false;
            InventoryItem placedInvItem = null;
            switch (heldItem.Data.EquipType)
            {
                case EquipType.Armor:
                    placedInvItem = Owner.UI.DollUI.Armor;
                    break;
                case EquipType.Missile:
                    placedInvItem = Owner.UI.DollUI.Bow;
                    break;
                case EquipType.Helmet:
                    placedInvItem = Owner.UI.DollUI.Helmet;
                    break;
                case EquipType.Belt:
                    placedInvItem = Owner.UI.DollUI.Belt;
                    break;
                case EquipType.Cloak:
                    placedInvItem = Owner.UI.DollUI.Cloak;
                    break;
                case EquipType.Boots:
                    placedInvItem = Owner.UI.DollUI.Boots;
                    break;
                case EquipType.Wand:
                case EquipType.WeaponDualWield:
                case EquipType.WeaponOneHanded:
                case EquipType.WeaponTwoHanded:
                    placedInvItem = Owner.UI.DollUI.RH_Weapon;
                    isWeapon = true;
                    break;
                default: break;
            }

            if (placedInvItem == null)
            {
                Logger.LogError("null placedInvItem: " + heldItem.Data.Name);
                return ItemInteractResult.Invalid;
            }

            if (placedInvItem != null && placedInvItem.Item != null)
            {
                replacedItem = placedInvItem.Item;
            }

            Vector2Int equipPos;
            Sprite equipSprite = null;
            if (isWeapon)
            {
                equipPos = new Vector2Int(heldItem.Data.EquipX, heldItem.Data.EquipY);
                equipSprite = heldItem.Data.InvSprite;

                Owner.UI.DollUI.RH_OpenImage.gameObject.SetActive(false);
                Owner.UI.DollUI.RH_HoldImage.gameObject.SetActive(true);
                Owner.UI.DollUI.RH_WeaponAnchorHolder.gameObject.SetActive(true);
            }
            else if (heldItem.Data.EquipType == EquipType.Missile)
            {
                // TODO: Fix, wrong calculation
                equipPos = new Vector2Int(-1 * heldItem.Data.EquipX, heldItem.Data.EquipY);
                equipSprite = heldItem.Data.InvSprite;
            }
            else
            {
                equipPos = DbMgr.Instance.ItemEquipPosDb.Get(heldItem.Data.Id).FemaleItemPos;
                heldItem.Data.EquipSprites.Find(sprite => sprite.name.Contains("v2a"));
                if (equipSprite == null)
                {
                    equipSprite = heldItem.Data.EquipSprites.Find(sprite => sprite.name.Contains("v2"));
                }

                if (equipSprite == null)
                {
                    Logger.LogError("No equip for female for item: " + heldItem.Data.ImageName);
                    //return ItemInteractResult.Invalid;
                }

                if (equipSprite == null)
                {
                    // Fallback to inventory display sprite
                    equipSprite = heldItem.Data.InvSprite;
                }
            }

            //placedInvItem = inventoryItem;
            placedInvItem.Image.enabled = true;
            placedInvItem.Image.sprite = equipSprite;
            placedInvItem.Image.SetNativeSize();
            placedInvItem.Image.alphaHitTestMinimumThreshold = 0.1f;
            placedInvItem.Image.rectTransform.anchoredPosition = new Vector3(
                equipPos.x,
                equipPos.y,
                0.0f);
            placedInvItem.Item = heldItem;

            placedInvItem.isEquipped = true;
            placedInvItem.IsHeld = false;

            Logger.LogDebug("Sprite: " + equipSprite.name + ", pos: " + equipPos.ToString());


            // Invoke item interact event here
            return ItemInteractResult.Equipped;
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
            Debug.Log("ItemPos: " + itemPos.ToString());
            GetCells(itemPos.x, itemPos.y, itemData.InvSize.x, itemData.InvSize.y)
                .ForEach(cell => cell.Item = item);

            InventoryItems.Add(item);

            Owner.UI.InventoryUI.AddItem(item);

            Debug.Log("Item (" + itemId + ") added");

            return true;
        }

        public bool CanReplaceItem(BaseItem oldItem, BaseItem newItem, out Vector2Int newPos)
        {
            newPos = new Vector2Int();

            // Temporarily clear space
            GetCells(oldItem.InvCellPosition.x, oldItem.InvCellPosition.y, oldItem.Data.InvSize.x, oldItem.Data.InvSize.y)
                .ForEach(cell => cell.Clear());

            bool ret = false;
            if (IsCellsFree(oldItem.InvCellPosition.x, oldItem.InvCellPosition.y,
                newItem.Data.InvSize.x, newItem.Data.InvSize.y))
            {
                // Can replace directly
                newPos.Set(oldItem.InvCellPosition.x, oldItem.InvCellPosition.y);
                ret = true;
            }
            else
            {
                if (CanPlaceItem(newItem.Data, out newPos))
                {
                    // Can replace on some other slot
                    ret = true;
                }
            }

            GetCells(oldItem.InvCellPosition.x, oldItem.InvCellPosition.y, oldItem.Data.InvSize.x, oldItem.Data.InvSize.y)
                .ForEach(cell => cell.Item = oldItem);

            return ret;
        }

        public bool PlaceItem(BaseItem item, int x, int y, bool allowAnyPos = false)
        {
            ItemData itemData = item.Data;
            if (!IsCellsFree(x, y, itemData.InvSize.x, itemData.InvSize.y))
            {
                if (allowAnyPos)
                {
                    Vector2Int newPos;
                    if (CanPlaceItem(item.Data, out newPos))
                    {
                        x = newPos.x;
                        y = newPos.y;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    Debug.Log("Cannot place item - not rly free");
                    return false;
                }
            }

            item.InvCellPosition = new Vector2Int(x, y);
            //Debug.Log("ItemPos: " + itemPos.ToString());
            GetCells(item.InvCellPosition.x, item.InvCellPosition.y, itemData.InvSize.x, itemData.InvSize.y)
                .ForEach(cell => cell.Item = item);

            InventoryItems.Add(item);

            Owner.UI.InventoryUI.AddItem(item);

            Debug.Log("Item (" + itemData.Id + ") added");

            return true;
        }

        public bool RemoveItem(BaseItem item)
        {
            if (!HasItem(item))
            {
                return false;
            }

            ClearCells(item.InvCellPosition.x, item.InvCellPosition.y, item.Data.InvSize.x, item.Data.InvSize.y);
            /*GetCells(item.InvCellPosition.x, item.InvCellPosition.y, item.Data.InvSize.x, item.Data.InvSize.y)
                .ForEach(cell => cell.Clear());*/
            InventoryItems.Remove(item);
            Owner.UI.InventoryUI.RemoveItem(item);            

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
            if ((startX + width) > INVENTORY_COLUMNS ||
                (startY + height) > INVENTORY_ROWS)
            {
                return;
            }

            List<InventoryCell> cells = new List<InventoryCell>();
            for (int x = startX; x < startX + width; x++)
            {
                for (int y = startY; y < startY + height; y++)
                {
                    Debug.Log("Cleared cell: [" + x + "," + y + "]");
                    InventoryCell cell = GetCell(x, y);
                    cell.Clear();
                }
            }
        }
    }
}
