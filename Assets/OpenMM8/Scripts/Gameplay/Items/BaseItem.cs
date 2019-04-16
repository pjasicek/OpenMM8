using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay.Items
{
    public class Item
    {
        public ItemData Data;
        public ItemEnchant Enchant = null;

        public bool IsIdentified = true;
        public bool IsBroken = false;


        // Unity UI
        public Vector2Int InvCellPosition;

        public Item(ItemData itemData)
        {
            Data = itemData;
        }

        virtual public int GetValue()
        {
            if (IsBroken)
            {
                return 1;
            }

            int baseValue = Data.GoldValue;
            if (Enchant != null)
            {
                if (Enchant.EnchantPriceMultType == EnchantPriceMultType.Add)
                {
                    baseValue += Enchant.PriceModAmount;
                }
                else if (Enchant.EnchantPriceMultType == EnchantPriceMultType.Multiply)
                {
                    baseValue *= Enchant.PriceModAmount;
                }
                else
                {
                    Debug.LogError("Unknown EnchantPriceMultType: " + Enchant.EnchantPriceMultType);
                }
            }

            return baseValue;
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

        public bool IsEnchantable()
        {
            return false;
        }

        public void SetIdentified(bool identified)
        {
            if (!identified && IsAlwaysIdentified())
            {
                return;
            }

            IsIdentified = identified;
        }

        public void SetBroken(bool broken)
        {
            if (broken && !CanBeBroken())
            {
                return;
            }

            IsBroken = broken;
        }

        public int GetStatBonusAmount(StatBonusType statBonusType)
        {
            if (!HasStatBonus(statBonusType))
            {
                return 0;
            }

            return Enchant.StatBonusMap[statBonusType];
        }

        public bool HasStatBonus(StatBonusType statBonusType)
        {
            if (Enchant == null || Enchant.EnchantType == EnchantType.None)
            {
                return false;
            }

            return Enchant.StatBonusMap.ContainsKey(statBonusType);
        }

        private bool IsAlwaysIdentified()
        {
            return true;
        }

        private bool CanBeBroken()
        {
            return true;
        }
    }
}
