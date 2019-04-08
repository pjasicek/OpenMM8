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

            if (!heldItem.IsEquippable())
            {
                return ItemInteractResult.Invalid;
            }

            ItemInteractResult eqResult = Owner.CanEquipItem(heldItem);
            if (eqResult != ItemInteractResult.Equipped)
            {
                return eqResult;
            }

            bool isWeapon = false;
            InventoryItem placedInvItem = null;
            switch (heldItem.Data.ItemType)
            {
                case ItemType.Armor:
                    placedInvItem = Owner.UI.DollUI.Armor;
                    break;
                case ItemType.Missile:
                    placedInvItem = Owner.UI.DollUI.Bow;
                    break;
                case ItemType.Helmet:
                    placedInvItem = Owner.UI.DollUI.Helmet;
                    break;
                case ItemType.Belt:
                    placedInvItem = Owner.UI.DollUI.Belt;
                    break;
                case ItemType.Cloak:
                    placedInvItem = Owner.UI.DollUI.Cloak;
                    break;
                case ItemType.Boots:
                    placedInvItem = Owner.UI.DollUI.Boots;
                    break;
                case ItemType.Amulet:
                    placedInvItem = Owner.UI.DollUI.Necklace;
                    break;
                case ItemType.Gauntlets:
                    placedInvItem = Owner.UI.DollUI.Gauntlets;
                    break;
                case ItemType.Ring:
                    if (Owner.UI.DollUI.Ring_1.Item == null)
                    {
                        placedInvItem = Owner.UI.DollUI.Ring_1;
                    }
                    else if (Owner.UI.DollUI.Ring_2.Item == null)
                    {
                        placedInvItem = Owner.UI.DollUI.Ring_2;
                    }
                    else if (Owner.UI.DollUI.Ring_3.Item == null)
                    {
                        placedInvItem = Owner.UI.DollUI.Ring_3;
                    }
                    else if (Owner.UI.DollUI.Ring_4.Item == null)
                    {
                        placedInvItem = Owner.UI.DollUI.Ring_4;
                    }
                    else if (Owner.UI.DollUI.Ring_5.Item == null)
                    {
                        placedInvItem = Owner.UI.DollUI.Ring_5;
                    }
                    else if (Owner.UI.DollUI.Ring_6.Item == null)
                    {
                        placedInvItem = Owner.UI.DollUI.Ring_6;
                    }
                    else
                    {
                        placedInvItem = Owner.UI.DollUI.Ring_1;
                    }
                    break;
                case ItemType.Wand:
                case ItemType.WeaponDualWield:
                case ItemType.WeaponOneHanded:
                case ItemType.WeaponTwoHanded:
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

            Vector2 equipPos = new Vector2(0, 0);
            Sprite equipSprite = null;
            if (isWeapon)
            {
                equipPos = new Vector2Int(heldItem.Data.EquipX + Owner.DollTypeData.MH_Offset.x / 2,
                    heldItem.Data.EquipY + Owner.DollTypeData.MH_Offset.y / 2);
                equipSprite = heldItem.Data.InvSprite;

                Owner.UI.DollUI.RH_OpenImage.gameObject.SetActive(false);
                Owner.UI.DollUI.RH_HoldImage.gameObject.SetActive(true);
                Owner.UI.DollUI.RH_WeaponAnchorHolder.gameObject.SetActive(true);
            }
            else if (heldItem.Data.ItemType == ItemType.Missile)
            {
                // TODO: Fix, wrong calculation
                equipPos = Owner.DollTypeData.BowOffset;
                equipSprite = heldItem.Data.InvSprite;
            }
            else if (heldItem.Data.ItemType == ItemType.Amulet ||
                     heldItem.Data.ItemType == ItemType.Gauntlets ||
                     heldItem.Data.ItemType == ItemType.Ring)
            {
                equipPos = new Vector2(placedInvItem.Image.rectTransform.anchoredPosition.x,
                    placedInvItem.Image.rectTransform.anchoredPosition.y);
                equipSprite = heldItem.Data.InvSprite;
            }
            else
            {
                List<string> preferredEqSpriteExts = new List<string>();
                if (Owner.DollTypeData.Id == 0)
                {
                    equipPos = DbMgr.Instance.ItemEquipPosDb.Get(heldItem.Data.Id).MaleItemPos;

                    if (Owner.UI.DollUI.RH_Weapon.Item == null)
                    {
                        preferredEqSpriteExts.Add("v1a");
                        preferredEqSpriteExts.Add("v1");
                    }
                    else
                    {
                        preferredEqSpriteExts.Add("v1");
                        preferredEqSpriteExts.Add("v1a");
                    }
                }
                else if (Owner.DollTypeData.Id == 1)
                {
                    equipPos = DbMgr.Instance.ItemEquipPosDb.Get(heldItem.Data.Id).FemaleItemPos;

                    if (Owner.UI.DollUI.RH_Weapon.Item == null)
                    {
                        preferredEqSpriteExts.Add("v2a");
                        preferredEqSpriteExts.Add("v2");
                    }
                    else
                    {
                        preferredEqSpriteExts.Add("v2");
                        preferredEqSpriteExts.Add("v2a");
                    }
                }
                else if (Owner.DollTypeData.Id == 2)
                {
                    equipPos = DbMgr.Instance.ItemEquipPosDb.Get(heldItem.Data.Id).MinotaurItemPos;

                    if (heldItem.Data.ItemType == ItemType.Cloak ||
                        heldItem.Data.ItemType == ItemType.Belt)
                    {
                        preferredEqSpriteExts.Add("v1");
                    }
                    else
                    {
                        preferredEqSpriteExts.Add("v3");
                    }
                }
                else if (Owner.DollTypeData.Id == 3)
                {
                    equipPos = DbMgr.Instance.ItemEquipPosDb.Get(heldItem.Data.Id).TrollItemPos;

                    if (heldItem.Data.ItemType == ItemType.Cloak ||
                        heldItem.Data.ItemType == ItemType.Belt)
                    {
                        preferredEqSpriteExts.Add("v1");
                    }
                    else
                    {
                        preferredEqSpriteExts.Add("v4");
                    }
                }
                else
                {
                    Logger.LogError("Unhandled doll type: " + Owner.DollTypeData.Id);
                }

                foreach (string prefferedExt in preferredEqSpriteExts)
                {
                    equipSprite = heldItem.Data.EquipSprites.Find(sprite => sprite.name.Contains(prefferedExt));
                    if (equipSprite != null)
                    {
                        break;
                    }
                }
                

                if (equipSprite == null)
                {
                    // Fallback to inventory display sprite
                    Logger.LogDebug("No equip for character: " + Owner.Name + " for item: " + heldItem.Data.ImageName);
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
            ItemData itemData = DbMgr.Instance.ItemDb.Get(item.Data.Id);

            Vector2Int itemPos;
            if (!CanPlaceItem(itemData, out itemPos))
            {
                Debug.Log("Inventory full");
                return false;
            }

            item.InvCellPosition = itemPos;
            GetCells(itemPos.x, itemPos.y, itemData.InvSize.x, itemData.InvSize.y)
                .ForEach(cell => cell.Item = item);

            InventoryItems.Add(item);
             
            Owner.UI.InventoryUI.AddItem(item);

            Debug.Log("Item (" + item.Data.Id + ") added");

            return true;
        }

        public void RemoveItemFromDoll(InventoryItem inventoryItem)
        {
            if (inventoryItem.Equals(Owner.UI.DollUI.RH_Weapon))
            {
                Owner.UI.DollUI.RH_OpenImage.gameObject.SetActive(true);
                Owner.UI.DollUI.RH_HoldImage.gameObject.SetActive(false);
                Owner.UI.DollUI.RH_WeaponAnchorHolder.gameObject.SetActive(false);
            }

            inventoryItem.Image.enabled = false;
            inventoryItem.Item = null;
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
