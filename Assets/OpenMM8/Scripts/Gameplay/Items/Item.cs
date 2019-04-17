using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay.Items
{
    public class Item
    {
        //=================================================

        public ItemData Data;
        public ItemEnchant Enchant = null;

        public bool IsIdentified = true;
        public bool IsBroken = false;


        // Unity UI
        public Vector2Int InvCellPosition;


        //=================================================

        private int m_DiceRolls = 0;
        private int m_DiceSides = 0;
        private int m_Mod = 0;


        //=================================================

        public Item(ItemData itemData)
        {
            Data = itemData;

            string[] tokens = Data.Mod1.Split('d');
            if (tokens.Length == 2)
            {
                m_DiceRolls = int.Parse(tokens[0]);
                m_DiceSides = int.Parse(tokens[1]);
            }
            else if (tokens.Length == 0 && char.ToLower(tokens[0][0]) == 's')
            {
                tokens[0].Remove(0, 1);
                m_DiceRolls = int.Parse(tokens[0]);
                m_DiceSides = 1;
            }
            else
            {
                m_DiceRolls = 0;
                m_DiceSides = 0;
            }

            m_Mod = int.Parse(Data.Mod2);

            if (IsSpecial() || IsArtifact() || IsRelic())
            {
                AddSpecialEnchant();
            }
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

        public int GetDiceRolls()
        {
            return m_DiceRolls;
        }

        public int GetDiceSides()
        {
            return m_DiceSides;
        }

        public int GetMod()
        {
            return m_Mod;
        }

        public bool IsEquippable()
        {
            return Data.ItemType == ItemType.WeaponOneHanded ||
                Data.ItemType == ItemType.WeaponTwoHanded ||
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

        public int GetStatBonusAmount(StatType statBonusType)
        {
            if (!HasStatBonus(statBonusType))
            {
                return 0;
            }

            return Enchant.StatBonusMap[statBonusType];
        }

        public bool HasStatBonus(StatType statBonusType)
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

        public bool IsArtifact()
        {
            return Data.Material == "Artifact";
        }

        public bool IsRelic()
        {
            return Data.Material == "Relic";
        }

        public bool IsSpecial()
        {
            return Data.Material == "Special";
        }

        private void AddSpecialEnchant()
        {
            if (!IsArtifact() && !IsRelic() && !IsSpecial())
            {
                Debug.LogError("This item (" + Data.Name + ") is not artifact/relic/Special !");
                return;
            }

            /*if (!Enum.IsDefined(typeof(ItemID), Data.Id))
            {
                Debug.LogError("Artifact/")
            }*/

            Enchant = new ItemEnchant()
            {
                EnchantType = EnchantType.Other
            };

            switch (Data.Id)
            {
                case 500:
                    Enchant.StatBonusMap.Add(StatType.Accuracy, 40);
                    // TODO: +10-20 light damage
                    break;

                case 501:
                    Enchant.StatBonusMap.Add(StatType.Might, 40);
                    // TODO: +10-20 dark damage
                    break;

                case 502:
                    Enchant.StatBonusMap.Add(StatType.Armsmaster, 7);
                    Enchant.StatBonusMap.Add(StatType.AirResistance, 30);
                    break;

                // TODO: Add the rest

            }
        }
    }
}
