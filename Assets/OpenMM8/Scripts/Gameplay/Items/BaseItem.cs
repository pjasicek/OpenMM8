using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay.Items
{
    public class BaseItem
    {
        public ItemData Data;
        public Vector2Int InvCellPosition;

        public bool CanBeEnchanted = false;
        public ItemEnchant Enchant = null;
        public bool IsIdentified = true;
        public bool IsBroken = false;


        public BaseItem(ItemData itemData)
        {
            Data = itemData;
        }

        virtual public ItemInteractResult InteractWithDoll(Character player)
        {
            return ItemInteractResult.Invalid;
        }

        virtual public string GetItemDescription()
        {
            return "Placeholder";
        }

        virtual public int GetValue()
        {
            return -1;
        }

        public bool IsEquippable()
        {
            return Data.ItemType == ItemType.WeaponOneHanded ||
                Data.ItemType == ItemType.WeaponTwoHanded ||
                Data.ItemType == ItemType.WeaponDualWield ||
                Data.ItemType == ItemType.Wand ||
                Data.ItemType == ItemType.Missile ||
                Data.ItemType == ItemType.Armor ||
                Data.ItemType == ItemType.Shield ||
                Data.ItemType == ItemType.Helmet ||
                Data.ItemType == ItemType.Belt ||
                Data.ItemType == ItemType.Cloak ||
                Data.ItemType == ItemType.Gauntlets ||
                Data.ItemType == ItemType.Boots ||
                Data.ItemType == ItemType.Ring ||
                Data.ItemType == ItemType.Amulet;
        }

        public bool IsConsumable()
        {
            return Data.ItemType == ItemType.Reagent ||
                Data.ItemType == ItemType.Bottle; /* && Data.Id != EMPTY_BOTTLE */
        }

        public bool IsLearnable()
        {
            return Data.ItemType == ItemType.SpellBook;
        }

        public bool IsReadable()
        {
            return Data.ItemType == ItemType.MessageScroll;
        }

        public bool IsCastable()
        {
            return Data.ItemType == ItemType.SpellScroll;
        }

        public bool IsInteractibleWithDoll()
        {
            return IsEquippable() || IsConsumable() || IsCastable() || IsReadable() || IsLearnable();
        }
    }
}
