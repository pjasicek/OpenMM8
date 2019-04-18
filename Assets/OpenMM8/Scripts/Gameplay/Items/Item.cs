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

        private CharacterRace m_RaceRestriction = CharacterRace.None;
        private CharacterClass m_ClassRestriction = CharacterClass.None;

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

        public bool IsEquippableByRace(CharacterRace race)
        {
            if (m_RaceRestriction == CharacterRace.None)
            {
                return true;
            }

            return race == m_RaceRestriction;
        }

        public bool IsEquippableByClass(CharacterClass characterClass)
        {
            if (m_ClassRestriction == CharacterClass.None)
            {
                return true;
            }

            if (m_ClassRestriction == characterClass)
            {
                return true;
            }

            // Check for class supertype
            CharacterClass nextClass = GameMechanics.GetNextClassPromotion(m_ClassRestriction);
            if (nextClass == characterClass)
            {
                return true;
            }

            return false;
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

                case 503:
                    Enchant.StatBonusMap.Add(StatType.Endurance, 40);
                    Enchant.StatBonusMap.Add(StatType.Luck, 40);
                    Enchant.StatBonusMap.Add(StatType.OfOgreSlaying, 1);
                    break;

                case 504: // Elderaxe
                    Enchant.StatBonusMap.Add(StatType.Might, 20);
                    Enchant.StatBonusMap.Add(StatType.Swift, 1);
                    Enchant.StatBonusMap.Add(StatType.Elderaxe, 1);
                    m_RaceRestriction = CharacterRace.Minotaur;
                    break;

                case 505:
                    Enchant.StatBonusMap.Add(StatType.FireResistance, 40);
                    Enchant.StatBonusMap.Add(StatType.Volcano, 1);
                    break;

                case 506:
                    Enchant.StatBonusMap.Add(StatType.Endurance, 20);
                    Enchant.StatBonusMap.Add(StatType.Swift, 1);
                    Enchant.StatBonusMap.Add(StatType.OfDragonSlaying, 1);
                    break;

                case 507:
                    Enchant.StatBonusMap.Add(StatType.Might, 10);
                    Enchant.StatBonusMap.Add(StatType.Endurance, 10);
                    Enchant.StatBonusMap.Add(StatType.Intellect, 10);
                    Enchant.StatBonusMap.Add(StatType.Personality, 10);
                    Enchant.StatBonusMap.Add(StatType.Speed, 10);
                    Enchant.StatBonusMap.Add(StatType.Luck, 10);
                    Enchant.StatBonusMap.Add(StatType.Accuracy, 10);
                    Enchant.StatBonusMap.Add(StatType.Guardian, 1);
                    break;

                case 508:
                    Enchant.StatBonusMap.Add(StatType.Vampiric, 1);
                    Enchant.StatBonusMap.Add(StatType.Foulfang, 1);
                    m_RaceRestriction = CharacterRace.Vampire;
                    break;

                case 509:
                    Enchant.StatBonusMap.Add(StatType.Personality, 40);
                    Enchant.StatBonusMap.Add(StatType.HpRegeneration, 1);
                    break;

                case 510:
                    Enchant.StatBonusMap.Add(StatType.Might, 20);
                    Enchant.StatBonusMap.Add(StatType.Endurance, 20);
                    Enchant.StatBonusMap.Add(StatType.Breaker, 1);
                    break;

                case 511:
                    Enchant.StatBonusMap.Add(StatType.OfShielding, 1);
                    Enchant.StatBonusMap.Add(StatType.DiseaseImmunity, 1);
                    Enchant.StatBonusMap.Add(StatType.ParalyzeImmunity, 1);
                    Enchant.StatBonusMap.Add(StatType.PoisonImmunity, 1);
                    break;

                case 512:
                    Enchant.StatBonusMap.Add(StatType.Accuracy, 50);
                    Enchant.StatBonusMap.Add(StatType.Bow, 4);
                    Enchant.StatBonusMap.Add(StatType.Swift, 1);
                    break;

                case 513:
                    Enchant.StatBonusMap.Add(StatType.Endurance, 30);
                    Enchant.StatBonusMap.Add(StatType.SpRegeneration, 1);
                    break;

                case 514:
                    Enchant.StatBonusMap.Add(StatType.Might, 10);
                    Enchant.StatBonusMap.Add(StatType.Endurance, 10);
                    Enchant.StatBonusMap.Add(StatType.Intellect, 10);
                    Enchant.StatBonusMap.Add(StatType.Personality, 10);
                    Enchant.StatBonusMap.Add(StatType.Speed, 10);
                    Enchant.StatBonusMap.Add(StatType.Luck, 10);
                    Enchant.StatBonusMap.Add(StatType.Accuracy, 10);
                    Enchant.StatBonusMap.Add(StatType.FireResistance, 10);
                    Enchant.StatBonusMap.Add(StatType.WaterResistance, 10);
                    Enchant.StatBonusMap.Add(StatType.EarthResistance, 10);
                    Enchant.StatBonusMap.Add(StatType.AirResistance, 10);
                    Enchant.StatBonusMap.Add(StatType.BodyResistance, 10);
                    Enchant.StatBonusMap.Add(StatType.MindResistance, 10);
                    m_RaceRestriction = CharacterRace.DarkElf;
                    break;

                case 515:
                    Enchant.StatBonusMap.Add(StatType.Swift, 1);
                    Enchant.StatBonusMap.Add(StatType.Speed, 15);
                    Enchant.StatBonusMap.Add(StatType.Accuracy, 15);
                    m_ClassRestriction = CharacterClass.Knight;
                    break;

                case 516:
                    Enchant.StatBonusMap.Add(StatType.OfSpiritMagic, 1);
                    Enchant.StatBonusMap.Add(StatType.OfMindMagic, 1);
                    Enchant.StatBonusMap.Add(StatType.OfBodyMagic, 1);
                    m_ClassRestriction = CharacterClass.Cleric;
                    break;

                case 517:
                    Enchant.StatBonusMap.Add(StatType.DisarmTraps, 8);
                    Enchant.StatBonusMap.Add(StatType.Bow, 8);
                    Enchant.StatBonusMap.Add(StatType.Armsmaster, 8);
                    break;

                case 518:
                    Enchant.StatBonusMap.Add(StatType.Speed, 30);
                    Enchant.StatBonusMap.Add(StatType.Swift, 1);
                    Enchant.StatBonusMap.Add(StatType.SleepImmunity, 1);
                    break;

                case 519:
                    Enchant.StatBonusMap.Add(StatType.FireResistance, 40);
                    Enchant.StatBonusMap.Add(StatType.WaterResistance, 40);
                    Enchant.StatBonusMap.Add(StatType.EarthResistance, 40);
                    Enchant.StatBonusMap.Add(StatType.AirResistance, 40);
                    break;

                case 520:
                    Enchant.StatBonusMap.Add(StatType.Personality, 15);
                    Enchant.StatBonusMap.Add(StatType.Intellect, 15);
                    Enchant.StatBonusMap.Add(StatType.HpRegeneration, 1);
                    break;

                case 521:
                    Enchant.StatBonusMap.Add(StatType.Intellect, 50);
                    Enchant.StatBonusMap.Add(StatType.OfDarkMagic, 1);
                    m_ClassRestriction = CharacterClass.Lich;
                    break;

                case 522:
                    Enchant.StatBonusMap.Add(StatType.Intellect, 30);
                    Enchant.StatBonusMap.Add(StatType.OfFeatherFalling, 1);
                    Enchant.StatBonusMap.Add(StatType.FireResistance, 10);
                    Enchant.StatBonusMap.Add(StatType.WaterResistance, 10);
                    Enchant.StatBonusMap.Add(StatType.EarthResistance, 10);
                    Enchant.StatBonusMap.Add(StatType.AirResistance, 10);
                    Enchant.StatBonusMap.Add(StatType.BodyResistance, 10);
                    Enchant.StatBonusMap.Add(StatType.MindResistance, 10);
                    break;

                // Relics

                case 523:
                    Enchant.StatBonusMap.Add(StatType.WaterResistance, -50);
                    Enchant.StatBonusMap.Add(StatType.Personality, -15);
                    Enchant.StatBonusMap.Add(StatType.OfSlowTarget, 1);
                    break;

                case 524:
                    Enchant.StatBonusMap.Add(StatType.Speed, 70);
                    Enchant.StatBonusMap.Add(StatType.Accuracy, 70);
                    Enchant.StatBonusMap.Add(StatType.ArmorClass, -20);
                    break;

                case 525:
                    Enchant.StatBonusMap.Add(StatType.Speed, -20);
                    Enchant.StatBonusMap.Add(StatType.Finality, 1);
                    Enchant.StatBonusMap.Add(StatType.OfSlowTarget, 1);
                    break;

                case 526:
                    Enchant.StatBonusMap.Add(StatType.Might, 70);
                    Enchant.StatBonusMap.Add(StatType.Accuracy, 70);
                    Enchant.StatBonusMap.Add(StatType.Intellect, -50);
                    Enchant.StatBonusMap.Add(StatType.Personality, -50);
                    break;

                case 527:
                    Enchant.StatBonusMap.Add(StatType.Vampiric, 1);
                    Enchant.StatBonusMap.Add(StatType.Might, 50);
                    Enchant.StatBonusMap.Add(StatType.Luck, -40);
                    break;

                case 528:
                    Enchant.StatBonusMap.Add(StatType.OfWaterWalking, 1);
                    Enchant.StatBonusMap.Add(StatType.WaterResistance, 70);
                    Enchant.StatBonusMap.Add(StatType.FireResistance, -70);
                    break;

                case 529:
                    Enchant.StatBonusMap.Add(StatType.Might, 40);
                    Enchant.StatBonusMap.Add(StatType.OfLightning, 1);
                    Enchant.StatBonusMap.Add(StatType.Accuracy, -40);
                    m_ClassRestriction = CharacterClass.Necromancer;
                    break;

                case 530:
                    Enchant.StatBonusMap.Add(StatType.OfFireMagic, 1);
                    Enchant.StatBonusMap.Add(StatType.OfAirMagic, 1);
                    Enchant.StatBonusMap.Add(StatType.OfWaterMagic, 1);
                    Enchant.StatBonusMap.Add(StatType.OfEarthMagic, 1);
                    Enchant.StatBonusMap.Add(StatType.ArmorClass, -40);
                    break;

                case 531:
                    Enchant.StatBonusMap.Add(StatType.Accuracy, 100);
                    Enchant.StatBonusMap.Add(StatType.Bow, 5);
                    Enchant.StatBonusMap.Add(StatType.ArmorClass, -20);
                    break;

                case 532:
                    Enchant.StatBonusMap.Add(StatType.Swift, 1);
                    Enchant.StatBonusMap.Add(StatType.Accuracy, -50);
                    m_RaceRestriction = CharacterRace.DarkElf;
                    break;

                case 533:
                    Enchant.StatBonusMap.Add(StatType.Personality, 80);
                    Enchant.StatBonusMap.Add(StatType.Intellect, 70);
                    Enchant.StatBonusMap.Add(StatType.MindResistance, -30);
                    //Enchant.StatBonusMap.Add(StatType.SpiritResistance, -30); // There is not spirit resistance ?!
                    break;

                case 534:
                    Enchant.StatBonusMap.Add(StatType.FearImmunity, 1);
                    Enchant.StatBonusMap.Add(StatType.StoneImmunity, 1);
                    Enchant.StatBonusMap.Add(StatType.ParalyzeImmunity, 1);
                    Enchant.StatBonusMap.Add(StatType.SleepImmunity, 1);
                    Enchant.StatBonusMap.Add(StatType.Personality, -15);
                    Enchant.StatBonusMap.Add(StatType.Luck, -15);
                    break;

                case 535:
                    Enchant.StatBonusMap.Add(StatType.OfWaterMagic, 1);
                    Enchant.StatBonusMap.Add(StatType.Alchemy, 5);
                    Enchant.StatBonusMap.Add(StatType.Intellect, 40);
                    Enchant.StatBonusMap.Add(StatType.Endurance, -20);
                    break;

                case 536:
                    Enchant.StatBonusMap.Add(StatType.Luck, 90);
                    Enchant.StatBonusMap.Add(StatType.Personality, -50);
                    break;

                case 537:
                    Enchant.StatBonusMap.Add(StatType.Might, 100);
                    Enchant.StatBonusMap.Add(StatType.FearImmunity, 1);
                    Enchant.StatBonusMap.Add(StatType.Accuracy, -30);
                    Enchant.StatBonusMap.Add(StatType.ArmorClass, -15);
                    break;

                // Special items

                case 538:
                    Enchant.StatBonusMap.Add(StatType.OfElementalSlaying, 1);
                    break;

                case 539:
                    Enchant.StatBonusMap.Add(StatType.OfDragonSlaying, 1);
                    break;

                case 540:
                    Enchant.StatBonusMap.Add(StatType.OfDragonSlaying, 1);
                    break;

                case 541:
                    Enchant.StatBonusMap.Add(StatType.OfIce, 1);
                    break;

                case 542:
                    Enchant.StatBonusMap.Add(StatType.OfCarnage, 1);
                    break;

                default:
                    Debug.LogError("Item " + Data.Id + " has no special ability set");
                    break;
            }
        }
    }
}
