using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    // ITEM_ENCHANT_SPECIAL
    public class ItemEnchantSpecialData : DbData<SpecialEnchantType>
    {
        public string OfNameText;
        public string BonusText;
        public Dictionary<ItemType, int> ChanceToApplyMap = new Dictionary<ItemType, int>();
        public EnchantPriceMultType EnchantPriceMultType;
        public int ValueMod;
        public string RarityLevel; // A = L3-L4, B = L3-L5, C = L4-L5, D = L5-L6
    }

    public class ItemEnchantSpecialDb : DataDb<ItemEnchantSpecialData, SpecialEnchantType>
    {
        // Just a helper
        private SpecialEnchantType m_LastType = SpecialEnchantType.None; // = 0

        override public ItemEnchantSpecialData ProcessCsvDataRow(int row, string[] columns)
        {
            if (row < 1)
            {
                return null;
            }

            if (string.IsNullOrEmpty(columns[0]))
            {
                return null;
            }


            ItemEnchantSpecialData data = new ItemEnchantSpecialData();

            // !!!!!!!!!!!!!
            // !!!!!!!!!!!!! This starts with "of Protection" enchant and has to conform to
            // !!!!!!!!!!!!! the SpecialEnchantType enum order
            // !!!!!!!!!!!!!

            SpecialEnchantType currId = m_LastType + 1;
            if (currId >= SpecialEnchantType.Max)
            { 
                Debug.LogError("Overflown last defined special enchant type: " + columns[1]);
                return null;
            }

            data.Id = currId;
            m_LastType++;

            data.BonusText = columns[0];
            data.OfNameText = columns[1];
            data.ChanceToApplyMap.Add(ItemType.WeaponOneHanded, int.Parse(columns[2]));
            data.ChanceToApplyMap.Add(ItemType.WeaponTwoHanded, int.Parse(columns[3]));
            data.ChanceToApplyMap.Add(ItemType.Missile, int.Parse(columns[4]));
            data.ChanceToApplyMap.Add(ItemType.Armor, int.Parse(columns[5]));
            data.ChanceToApplyMap.Add(ItemType.Shield, int.Parse(columns[6]));
            data.ChanceToApplyMap.Add(ItemType.Helmet, int.Parse(columns[7]));
            data.ChanceToApplyMap.Add(ItemType.Belt, int.Parse(columns[8]));
            data.ChanceToApplyMap.Add(ItemType.Cloak, int.Parse(columns[9]));
            data.ChanceToApplyMap.Add(ItemType.Gauntlets, int.Parse(columns[10]));
            data.ChanceToApplyMap.Add(ItemType.Boots, int.Parse(columns[11]));
            data.ChanceToApplyMap.Add(ItemType.Ring, int.Parse(columns[12]));
            data.ChanceToApplyMap.Add(ItemType.Amulet, int.Parse(columns[13]));

            if (columns[14].ToLower().StartsWith("x"))
            {
                columns[14] = columns[14].Remove(0, 1).Trim();
                data.EnchantPriceMultType = EnchantPriceMultType.Multiply;
                data.ValueMod = int.Parse(columns[14]);
            }
            else
            {
                data.EnchantPriceMultType = EnchantPriceMultType.Add;
                data.ValueMod = int.Parse(columns[14]);
            }
            data.ValueMod = int.Parse(columns[14]);
            data.RarityLevel = columns[15];

            return data;
        }
    }
}
