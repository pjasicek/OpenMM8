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

        /*public bool IsEquippable()
        {
            return ItemData.EquipType == EquipType.WeaponOneHanded ||
                ItemData.EquipType == EquipType.WeaponTwoHanded ||
                ItemData.EquipType == EquipType.WeaponDualWield ||
                ItemData.EquipType == EquipType.Wand ||
                ItemData.EquipType == EquipType.Missile ||
                ItemData.EquipType == EquipType.Armor ||
                ItemData.EquipType == EquipType.Shield ||
                ItemData.EquipType == EquipType.Helmet ||
                ItemData.EquipType == EquipType.Belt ||
                ItemData.EquipType == EquipType.Cloak ||
                ItemData.EquipType == EquipType.Gauntlets ||
                ItemData.EquipType == EquipType.Boots ||
                ItemData.EquipType == EquipType.Ring ||
                ItemData.EquipType == EquipType.Amulet;
        }

        public bool IsConsumable()
        {

        }

        public bool IsLearnable()
        {

        }

        public bool IsReadable()
        {

        }

        public bool IsCastable()
        {

        }

        public bool IsInteractibleWithDoll()
        {

        }

        public Vector2Int GetInventorySize()
        {

        }*/
    }
}
