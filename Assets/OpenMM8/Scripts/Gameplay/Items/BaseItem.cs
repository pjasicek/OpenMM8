using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay.Items
{
    public class BaseItem : DbData
    {
        public ItemData Data;
        public Vector2Int InvCellPosition;

        public BaseItem(ItemData itemData)
        {
            Data = itemData;
        }

        virtual public ItemInteractResult InteractWithDoll(Character player)
        {
            return ItemInteractResult.Invalid;
        }

        public bool IsEquippable()
        {
            return Data.EquipType == EquipType.WeaponOneHanded ||
                Data.EquipType == EquipType.WeaponTwoHanded ||
                Data.EquipType == EquipType.WeaponDualWield ||
                Data.EquipType == EquipType.Wand ||
                Data.EquipType == EquipType.Missile ||
                Data.EquipType == EquipType.Armor ||
                Data.EquipType == EquipType.Shield ||
                Data.EquipType == EquipType.Helmet ||
                Data.EquipType == EquipType.Belt ||
                Data.EquipType == EquipType.Cloak ||
                Data.EquipType == EquipType.Gauntlets ||
                Data.EquipType == EquipType.Boots ||
                Data.EquipType == EquipType.Ring ||
                Data.EquipType == EquipType.Amulet;
        }

        public bool IsConsumable()
        {
            return Data.EquipType == EquipType.Reagent ||
                Data.EquipType == EquipType.Bottle; /* && Data.Id != EMPTY_BOTTLE */
        }

        public bool IsLearnable()
        {
            return Data.EquipType == EquipType.SpellBook;
        }

        public bool IsReadable()
        {
            return Data.EquipType == EquipType.MessageScroll;
        }

        public bool IsCastable()
        {
            return Data.EquipType == EquipType.SpellScroll;
        }

        public bool IsInteractibleWithDoll()
        {
            return IsEquippable() || IsConsumable() || IsCastable() || IsReadable() || IsLearnable();
        }
    }
}
