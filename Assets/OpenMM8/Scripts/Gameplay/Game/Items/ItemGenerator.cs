using Assets.OpenMM8.Scripts.Gameplay.Data;
using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class ItemGenerator : Singleton<ItemGenerator>
    {
        private Dictionary<int, ItemData> m_ItemDb = null;

        // Pre-calculation

        private Dictionary<TreasureLevel, int> m_TreasureLevelToSumOfItemChanceMap = new Dictionary<TreasureLevel, int>();

        // Standard enchant type chances do not differ via TreasureLevel, only by Amount
        private Dictionary<ItemType, int> m_StandardEnchantChanceSummMap = new Dictionary<ItemType, int>();

        private Dictionary<TreasureLevel, Dictionary<ItemType, int>> m_SpecialEnchantChanceSummMap =
            new Dictionary<TreasureLevel, Dictionary<ItemType, int>>();


        /* ========================================================================================================= //

        This item/enchant generation algorithm was taken from the original M&M games, this is how it works:

        Based on a treasure level:
        1) Get sum of all chances for given treasure level from ALL items in the game (ITEMS_RANDOM_GENERATION.txt)
        2) Get a random number between 0 and total chance calculated in 1)
        3) Loop through all items in the game, and add for each looped item its chance to a variable
        4) If a variable is > random seed number from 2), pick this item

        If item is generated with ItemSkillGroup criteria or ItemType criteria, do the same, but only for the items
        which fulfil the requirements (have give ItemSkillGroup or ItemType)

        Random enchants are chosen in the same way (same algorithm)
        * Standard enchants can be applied to all TreasureLevels, TreasureLevels determine the stat amount
        * Special enchants also have "RarityLevel" criteria - special enchants can be applied only on
          given TreasureLevels, e.g. RarityLevel A can be applied only on L3 and L4, RarityLevel D can
          be only applied on L5 and L6
            
        /* ========================================================================================================= */

        // TotallyRandom - should ONLY be used for debugging
        static public Item GenerateItem()
        {
            ItemData genItemData = Instance.m_ItemDb[UnityEngine.Random.Range(0, Instance.m_ItemDb.Count)];
            Item rndItem = new Item(genItemData);
            
            return rndItem;
        }

        // Generate any item within treasure level
        static public Item GenerateItem(TreasureLevel treasureLevel)
        {
            int sumOfChances = Instance.m_TreasureLevelToSumOfItemChanceMap[treasureLevel];
            int rndItemMarker = UnityEngine.Random.Range(0, sumOfChances);

            Item newItem = null;
            int currSumOfChances = 0;
            foreach (ItemData itemData in Instance.m_ItemDb.Values)
            {
                currSumOfChances += itemData.TreasureLevelDropChanceMap[treasureLevel];
                if (currSumOfChances > rndItemMarker)
                {
                    newItem = new Item(itemData);
                    break;
                }
            }

            if (newItem == null)
            {
                Debug.LogError("Failed to generate item from marker: " + rndItemMarker);
                return new Item(Instance.m_ItemDb[0]);
            }

            /*Debug.Log("Total Sum: " + sumOfChances);
            Debug.Log("Marker: " + rndItemMarker);
            Debug.Log("Current Sum: " + currSumOfChances);*/
            Debug.Log("Item: " + newItem.Data.Name);

            bool enchanted = TryApplyEnchant(treasureLevel, newItem);

            return newItem;
        }

        // Generate item which requires skill within SkillGroup - useful for e.g. mobs only dropping plate armor
        static public Item GenerateItem(TreasureLevel treasureLevel, ItemSkillGroup skillGroup)
        {
            int sumOfChances = 0;
            foreach (ItemData itemData in Instance.m_ItemDb.Values)
            {
                if (itemData.SkillGroup == skillGroup)
                {
                    sumOfChances += itemData.TreasureLevelDropChanceMap[treasureLevel];
                }
            }

            int rndItemMarker = UnityEngine.Random.Range(0, sumOfChances);

            Item newItem = null;
            int currSumOfChances = 0;
            foreach (ItemData itemData in Instance.m_ItemDb.Values)
            {
                if (itemData.SkillGroup == skillGroup)
                {
                    currSumOfChances += itemData.TreasureLevelDropChanceMap[treasureLevel];
                    if (currSumOfChances > rndItemMarker)
                    {
                        newItem = new Item(itemData);
                        break;
                    }
                }
            }

            if (newItem == null)
            {
                Debug.LogError("Failed to generate item from marker: " + rndItemMarker);
                return new Item(Instance.m_ItemDb[0]);
            }

            bool enchanted = TryApplyEnchant(treasureLevel, newItem);

            return newItem;
        }

        // Generate item which can be equipped at specified slot - e.g. mobs only dropping armor
        static public Item GenerateItem(TreasureLevel treasureLevel, ItemType itemType)
        {
            int sumOfChances = 0;
            foreach (ItemData itemData in Instance.m_ItemDb.Values)
            {
                if (itemData.ItemType == itemType)
                {
                    sumOfChances += itemData.TreasureLevelDropChanceMap[treasureLevel];
                }
            }

            int rndItemMarker = UnityEngine.Random.Range(0, sumOfChances);

            Item newItem = null;
            int currSumOfChances = 0;
            foreach (ItemData itemData in Instance.m_ItemDb.Values)
            {
                if (itemData.ItemType == itemType)
                {
                    currSumOfChances += itemData.TreasureLevelDropChanceMap[treasureLevel];
                    if (currSumOfChances > rndItemMarker)
                    {
                        newItem = new Item(itemData);
                        break;
                    }
                }
            }

            if (newItem == null)
            {
                Debug.LogError("Failed to generate item from marker: " + rndItemMarker);
                return new Item(Instance.m_ItemDb[0]);
            }

            bool enchanted = TryApplyEnchant(treasureLevel, newItem);

            return newItem;
        }

        static public bool TryApplyEnchant(TreasureLevel treasureLevel, Item item)
        {
            if (item.Enchant != null)
            {
                // Already enchanted
                return false;
            }

            if (item.IsEnchantable())
            {
                Debug.Log("Is enchantable: true");
                bool isItemEnchanted = item.Enchant != null;

                // First try standard
                int standardEnchantChance = GetStandardEnchantChance(treasureLevel, item);
                int rnd = UnityEngine.Random.Range(0, 100);
                Debug.Log("StandardEnchantChance: " + standardEnchantChance + ", Random: " + rnd);
                if (!isItemEnchanted && standardEnchantChance > rnd)
                {
                    Debug.Log("Will apply standard: " + item.Data.ItemType);
                    int stdEnchChanceSum = Instance.m_StandardEnchantChanceSummMap[item.Data.ItemType];
                    int rndStdEnchantMarker = UnityEngine.Random.Range(0, stdEnchChanceSum);
                    int currSumOfChances = 0;
                    foreach (ItemEnchantStandardData stdEnchData in DbMgr.Instance.ItemEnchantStandardDb.Data.Values)
                    {
                        Debug.Log("Stat: " + stdEnchData.BonusType);

                        if (!stdEnchData.ChanceToApplyMap.ContainsKey(item.Data.ItemType))
                        {
                            continue;
                        }

                        currSumOfChances += stdEnchData.ChanceToApplyMap[item.Data.ItemType];
                        Debug.Log(currSumOfChances + "/" + rndStdEnchantMarker + "(" + stdEnchChanceSum + ")");
                        if (currSumOfChances > rndStdEnchantMarker)
                        {
                            StatType bonusStat = stdEnchData.BonusType;
                            int enchantAmount = GenStandardItemEnchantAmount(treasureLevel);

                            Instance.ApplyStandardEnchant(bonusStat, enchantAmount, treasureLevel, item);
                            return true;
                        }
                    }
                }

                // Then try special enchant
                int specialEnchantChance = GetSpecialEnchantChance(treasureLevel, item);
                rnd = UnityEngine.Random.Range(0, 100);
                if (!isItemEnchanted && specialEnchantChance > rnd)
                {
                    int specialEnchChanceSumm = Instance.m_SpecialEnchantChanceSummMap[treasureLevel][item.Data.ItemType];
                    int rndSpecialEnchantMarker = UnityEngine.Random.Range(0, specialEnchChanceSumm);
                    int currSumOfChances = 0;
                    foreach (ItemEnchantSpecialData specialEnchData in DbMgr.Instance.ItemEnchantSpecialDb.Data.Values)
                    {
                        if (!IsTreasureLevelEligibleForRarityLevel(treasureLevel, specialEnchData.RarityLevel))
                        {
                            continue;
                        }

                        if (!specialEnchData.ChanceToApplyMap.ContainsKey(item.Data.ItemType))
                        {
                            continue;
                        }

                        currSumOfChances += specialEnchData.ChanceToApplyMap[item.Data.ItemType];
                        if (currSumOfChances > rndSpecialEnchantMarker)
                        {
                            Instance.ApplySpecialEnchant(specialEnchData.Id, item);
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        static public int GenStandardItemEnchantAmount(TreasureLevel treasureLevel)
        {
            // Unity int range is max exclusive - that is why it looks like its max is 1 more than
            //    it should be
            switch (treasureLevel)
            {
                case TreasureLevel.L1:
                    return 0;

                case TreasureLevel.L2:
                    return UnityEngine.Random.Range(1, 6);

                case TreasureLevel.L3:
                    return UnityEngine.Random.Range(3, 9);

                case TreasureLevel.L4:
                    return UnityEngine.Random.Range(6, 13);

                case TreasureLevel.L5:
                    return UnityEngine.Random.Range(10, 18);

                case TreasureLevel.L6:
                    return UnityEngine.Random.Range(15, 26);
            }

            Debug.LogError("Unhandled item level: " + treasureLevel);
            return 0;
        }

        // Cannot be applied to weapons
        static public int GetStandardEnchantChance(TreasureLevel treasureLevel, Item item)
        {
            switch (treasureLevel)
            {
                case TreasureLevel.L1: return 0;
                case TreasureLevel.L2: return 40;
                case TreasureLevel.L3: return 40;
                case TreasureLevel.L4: return 40;
                case TreasureLevel.L5: return 40;
                case TreasureLevel.L6: return 75;
            }

            return 0;
        }

        // Cannot be applie
        static public int GetSpecialEnchantChance(TreasureLevel treasureLevel, Item item)
        {
            if (item.IsWeapon())
            {
                switch (treasureLevel)
                {
                    case TreasureLevel.L1: return 0;
                    case TreasureLevel.L2: return 0;
                    case TreasureLevel.L3: return 10;
                    case TreasureLevel.L4: return 20;
                    case TreasureLevel.L5: return 30;
                    case TreasureLevel.L6: return 50;
                }
            }
            else
            {
                switch (treasureLevel)
                {
                    case TreasureLevel.L1: return 0;
                    case TreasureLevel.L2: return 0;
                    case TreasureLevel.L3: return 10;
                    case TreasureLevel.L4: return 15;
                    case TreasureLevel.L5: return 20;
                    case TreasureLevel.L6: return 25;
                }
            }

            return 0;
        }

        static public bool IsTreasureLevelEligibleForRarityLevel(TreasureLevel treasureLevel, string rarityLevel)
        {
            switch (treasureLevel)
            {
                case TreasureLevel.L1:
                case TreasureLevel.L2:
                    return false;

                case TreasureLevel.L3:
                    return rarityLevel == "A" || rarityLevel == "B";

                case TreasureLevel.L4:
                    return rarityLevel == "A" || rarityLevel == "B" ||
                        rarityLevel == "C";

                case TreasureLevel.L5:
                    return rarityLevel == "B" || rarityLevel == "C" ||
                        rarityLevel == "D";

                case TreasureLevel.L6:
                    return rarityLevel == "D";
            }

            return false;
        }

        public void ApplyStandardEnchant(StatType bonusStat, int amount, TreasureLevel treasureLevel, Item destItem)
        {
            if (!destItem.IsEnchantable())
            {
                Debug.LogError("Attempted to enchant item which is not enchantable, ID: " + destItem.Data.Id);
                return;
            }

            ItemEnchantStandardData enchantData = DbMgr.Instance.ItemEnchantStandardDb.Get(bonusStat);
            if (enchantData == null)
            {
                return;
            }

            ItemEnchant itemEnchant = new ItemEnchant();
            itemEnchant.EnchantType = EnchantType.Standard;
            itemEnchant.OfTypeText = enchantData.OfName;

            int statAmount = GenStandardItemEnchantAmount(treasureLevel);
            itemEnchant.BonusDescText = "+" + statAmount + " " + enchantData.StatDisplayNameText; // e.g. Hit Points
            itemEnchant.EnchantPriceMultType = EnchantPriceMultType.Add;
            itemEnchant.PriceModAmount = statAmount * 100;
            itemEnchant.StatBonusMap.Add(bonusStat, statAmount);

            destItem.Enchant = itemEnchant;
        }

        public void ApplySpecialEnchant(SpecialEnchantType type, Item destItem)
        {
            if (!destItem.IsEnchantable())
            {
                Debug.LogError("Attempted to enchant item which is not enchantable, ID: " + destItem.Data.Id);
                return;
            }

            ItemEnchantSpecialData enchantData = DbMgr.Instance.ItemEnchantSpecialDb.Get(type);
            if (enchantData == null)
            {
                return;
            }

            ItemEnchant itemEnchant = new ItemEnchant();
            itemEnchant.EnchantType = EnchantType.Special;
            itemEnchant.OfTypeText = enchantData.OfNameText;
            itemEnchant.BonusDescText = enchantData.BonusText;
            itemEnchant.EnchantPriceMultType = enchantData.EnchantPriceMultType;
            itemEnchant.PriceModAmount = enchantData.ValueMod;

            switch (type)
            {
                case SpecialEnchantType.OfProtection:
                    itemEnchant.StatBonusMap.Add(StatType.AirResistance, 10);
                    itemEnchant.StatBonusMap.Add(StatType.EarthResistance, 10);
                    itemEnchant.StatBonusMap.Add(StatType.FireResistance, 10);
                    itemEnchant.StatBonusMap.Add(StatType.WaterResistance, 10);
                    break;

                case SpecialEnchantType.OfGods:
                    itemEnchant.StatBonusMap.Add(StatType.Might, 10);
                    itemEnchant.StatBonusMap.Add(StatType.Endurance, 10);
                    itemEnchant.StatBonusMap.Add(StatType.Intellect, 10);
                    itemEnchant.StatBonusMap.Add(StatType.Personality, 10);
                    itemEnchant.StatBonusMap.Add(StatType.Speed, 10);
                    itemEnchant.StatBonusMap.Add(StatType.Accuracy, 10);
                    itemEnchant.StatBonusMap.Add(StatType.Luck, 10);
                    break;

                case SpecialEnchantType.OfCarnage:
                    itemEnchant.StatBonusMap.Add(StatType.OfCarnage, 1);
                    break;

                case SpecialEnchantType.OfCold:
                    itemEnchant.StatBonusMap.Add(StatType.OfCold, 1);
                    break;

                case SpecialEnchantType.OfFrost:
                    itemEnchant.StatBonusMap.Add(StatType.OfFrost, 1);
                    break;

                case SpecialEnchantType.OfIce:
                    itemEnchant.StatBonusMap.Add(StatType.OfIce, 1);
                    break;

                case SpecialEnchantType.OfSparks:
                    itemEnchant.StatBonusMap.Add(StatType.OfSparks, 1);
                    break;

                case SpecialEnchantType.OfLightning:
                    itemEnchant.StatBonusMap.Add(StatType.OfLightning, 1);
                    break;

                case SpecialEnchantType.OfThunderbolts:
                    itemEnchant.StatBonusMap.Add(StatType.OfThunderbolts, 1);
                    break;

                case SpecialEnchantType.OfFire:
                    itemEnchant.StatBonusMap.Add(StatType.OfFire, 1);
                    break;

                case SpecialEnchantType.OfFlame:
                    itemEnchant.StatBonusMap.Add(StatType.OfFlame, 1);
                    break;

                case SpecialEnchantType.OfInfernos:
                    itemEnchant.StatBonusMap.Add(StatType.OfInfernos, 1);
                    break;

                case SpecialEnchantType.OfPoison:
                    itemEnchant.StatBonusMap.Add(StatType.OfPoison, 1);
                    break;

                case SpecialEnchantType.OfVenom:
                    itemEnchant.StatBonusMap.Add(StatType.OfVenom, 1);
                    break;

                case SpecialEnchantType.OfAcid:
                    itemEnchant.StatBonusMap.Add(StatType.OfAcid, 1);
                    break;

                case SpecialEnchantType.Vampiric:
                    itemEnchant.StatBonusMap.Add(StatType.Vampiric, 1);
                    break;

                case SpecialEnchantType.OfRecovery:
                    itemEnchant.StatBonusMap.Add(StatType.OfRecovery, 1);
                    break;

                case SpecialEnchantType.OfImmunity:
                    itemEnchant.StatBonusMap.Add(StatType.DiseaseImmunity, 1);
                    break;

                case SpecialEnchantType.OfSanity:
                    itemEnchant.StatBonusMap.Add(StatType.InsanityImmunity, 1);
                    break;

                case SpecialEnchantType.OfFreedom:
                    itemEnchant.StatBonusMap.Add(StatType.ParalyzeImmunity, 1);
                    break;

                case SpecialEnchantType.OfAntidotes:
                    itemEnchant.StatBonusMap.Add(StatType.PoisonImmunity, 1);
                    break;

                case SpecialEnchantType.OfAlarms:
                    itemEnchant.StatBonusMap.Add(StatType.SleepImmunity, 1);
                    break;

                case SpecialEnchantType.OfMedusa:
                    itemEnchant.StatBonusMap.Add(StatType.PetrifyImmunity, 1);
                    break;

                case SpecialEnchantType.OfForce:
                    itemEnchant.StatBonusMap.Add(StatType.OfForce, 1);
                    break;

                case SpecialEnchantType.OfPower:
                    itemEnchant.StatBonusMap.Add(StatType.Level, 5);
                    break;

                case SpecialEnchantType.OfAirMagic:
                    itemEnchant.StatBonusMap.Add(StatType.OfAirMagic, 1);
                    break;

                case SpecialEnchantType.OfBodyMagic:
                    itemEnchant.StatBonusMap.Add(StatType.OfBodyMagic, 1);
                    break;

                case SpecialEnchantType.OfDarkMagic:
                    itemEnchant.StatBonusMap.Add(StatType.OfDarkMagic, 1);
                    break;

                case SpecialEnchantType.OfEarthMagic:
                    itemEnchant.StatBonusMap.Add(StatType.OfEarthMagic, 1);
                    break;

                case SpecialEnchantType.OfFireMagic:
                    itemEnchant.StatBonusMap.Add(StatType.OfFireMagic, 1);
                    break;

                case SpecialEnchantType.OfLightMagic:
                    itemEnchant.StatBonusMap.Add(StatType.OfLightMagic, 1);
                    break;

                case SpecialEnchantType.OfMindMagic:
                    itemEnchant.StatBonusMap.Add(StatType.OfMindMagic, 1);
                    break;

                case SpecialEnchantType.OfSpiritMagic:
                    itemEnchant.StatBonusMap.Add(StatType.OfSpiritMagic, 1);
                    break;

                case SpecialEnchantType.OfWaterMagic:
                    itemEnchant.StatBonusMap.Add(StatType.OfWaterMagic, 1);
                    break;

                case SpecialEnchantType.OfThievery:
                    itemEnchant.StatBonusMap.Add(StatType.OfThievery, 1);
                    break;

                case SpecialEnchantType.OfShielding:
                    itemEnchant.StatBonusMap.Add(StatType.OfShielding, 1);
                    break;

                case SpecialEnchantType.OfRegeneration:
                    itemEnchant.StatBonusMap.Add(StatType.HpRegeneration, 1);
                    break;

                case SpecialEnchantType.OfMana:
                    itemEnchant.StatBonusMap.Add(StatType.SpRegeneration, 1);
                    break;

                case SpecialEnchantType.OfOgreSlaying:
                    itemEnchant.StatBonusMap.Add(StatType.OfOgreSlaying, 1);
                    break;

                case SpecialEnchantType.OfDragonSlaying:
                    itemEnchant.StatBonusMap.Add(StatType.OfDragonSlaying, 1);
                    break;

                case SpecialEnchantType.OfDarkness:
                    itemEnchant.StatBonusMap.Add(StatType.OfDarkness, 1);
                    break;

                case SpecialEnchantType.OfDoom:
                    itemEnchant.StatBonusMap.Add(StatType.Might, 1);
                    itemEnchant.StatBonusMap.Add(StatType.Endurance, 1);
                    itemEnchant.StatBonusMap.Add(StatType.Intellect, 1);
                    itemEnchant.StatBonusMap.Add(StatType.Personality, 1);
                    itemEnchant.StatBonusMap.Add(StatType.Speed, 1);
                    itemEnchant.StatBonusMap.Add(StatType.Accuracy, 1);
                    itemEnchant.StatBonusMap.Add(StatType.Luck, 1);

                    itemEnchant.StatBonusMap.Add(StatType.HitPoints, 1);
                    itemEnchant.StatBonusMap.Add(StatType.SpellPoints, 1);
                    itemEnchant.StatBonusMap.Add(StatType.ArmorClass, 1);

                    itemEnchant.StatBonusMap.Add(StatType.AirResistance, 1);
                    itemEnchant.StatBonusMap.Add(StatType.EarthResistance, 1);
                    itemEnchant.StatBonusMap.Add(StatType.WaterResistance, 1);
                    itemEnchant.StatBonusMap.Add(StatType.FireResistance, 1);
                    break;

                case SpecialEnchantType.OfEarth:
                    itemEnchant.StatBonusMap.Add(StatType.Endurance, 10);
                    itemEnchant.StatBonusMap.Add(StatType.ArmorClass, 10);
                    itemEnchant.StatBonusMap.Add(StatType.HitPoints, 10);
                    break;

                case SpecialEnchantType.OfLife:
                    itemEnchant.StatBonusMap.Add(StatType.HitPoints, 10);
                    itemEnchant.StatBonusMap.Add(StatType.HpRegeneration, 1);
                    break;

                case SpecialEnchantType.Rogues:
                    itemEnchant.StatBonusMap.Add(StatType.Speed, 5);
                    itemEnchant.StatBonusMap.Add(StatType.Accuracy, 5);
                    break;

                case SpecialEnchantType.OfTheDragon:
                    itemEnchant.StatBonusMap.Add(StatType.Might, 25);
                    itemEnchant.StatBonusMap.Add(StatType.OfTheDragon, 1);
                    break;

                case SpecialEnchantType.OfTheEclipse:
                    itemEnchant.StatBonusMap.Add(StatType.SpellPoints, 10);
                    itemEnchant.StatBonusMap.Add(StatType.SpRegeneration, 1);
                    break;

                case SpecialEnchantType.OfTheGolem:
                    itemEnchant.StatBonusMap.Add(StatType.Endurance, 15);
                    itemEnchant.StatBonusMap.Add(StatType.ArmorClass, 5);
                    break;

                case SpecialEnchantType.OfTheMoon:
                    itemEnchant.StatBonusMap.Add(StatType.Luck, 10);
                    itemEnchant.StatBonusMap.Add(StatType.Intellect, 10);
                    break;

                case SpecialEnchantType.OfThePhoenix:
                    itemEnchant.StatBonusMap.Add(StatType.FireResistance, 30);
                    itemEnchant.StatBonusMap.Add(StatType.HpRegeneration, 1);
                    break;

                case SpecialEnchantType.OfTheSky:
                    itemEnchant.StatBonusMap.Add(StatType.Intellect, 10);
                    itemEnchant.StatBonusMap.Add(StatType.Speed, 10);
                    itemEnchant.StatBonusMap.Add(StatType.SpellPoints, 10);
                    break;

                case SpecialEnchantType.OfTheStars:
                    itemEnchant.StatBonusMap.Add(StatType.Endurance, 10);
                    itemEnchant.StatBonusMap.Add(StatType.Accuracy, 10);
                    break;

                case SpecialEnchantType.OfTheSun:
                    itemEnchant.StatBonusMap.Add(StatType.Might, 10);
                    itemEnchant.StatBonusMap.Add(StatType.Personality, 10);
                    break;

                case SpecialEnchantType.OfTheTroll:
                    itemEnchant.StatBonusMap.Add(StatType.Endurance, 15);
                    itemEnchant.StatBonusMap.Add(StatType.HpRegeneration, 1);
                    break;

                case SpecialEnchantType.OfTheUnicorn:
                    itemEnchant.StatBonusMap.Add(StatType.Luck, 15);
                    itemEnchant.StatBonusMap.Add(StatType.SpRegeneration, 1);
                    break;

                case SpecialEnchantType.Warriors:
                    itemEnchant.StatBonusMap.Add(StatType.Might, 5);
                    itemEnchant.StatBonusMap.Add(StatType.Endurance, 5);
                    break;

                case SpecialEnchantType.Wizards:
                    itemEnchant.StatBonusMap.Add(StatType.Intellect, 5);
                    itemEnchant.StatBonusMap.Add(StatType.Personality, 5);
                    break;

                case SpecialEnchantType.Antique:
                    break;

                case SpecialEnchantType.Swift:
                    itemEnchant.StatBonusMap.Add(StatType.Swift, 1);
                    break;

                case SpecialEnchantType.Monks:
                    itemEnchant.StatBonusMap.Add(StatType.Dodging, 3);
                    itemEnchant.StatBonusMap.Add(StatType.Unarmed, 3);
                    break;

                case SpecialEnchantType.Thieves:
                    itemEnchant.StatBonusMap.Add(StatType.Stealing, 3);
                    itemEnchant.StatBonusMap.Add(StatType.DisarmTraps, 3);
                    break;

                case SpecialEnchantType.OfIdentifying:
                    itemEnchant.StatBonusMap.Add(StatType.IdentifyMonster, 3);
                    itemEnchant.StatBonusMap.Add(StatType.IdentifyItem, 3);
                    break;

                case SpecialEnchantType.OfElementalSlaying:
                    itemEnchant.StatBonusMap.Add(StatType.OfElementalSlaying, 1);
                    break;

                case SpecialEnchantType.OfUndeadSlaying:
                    itemEnchant.StatBonusMap.Add(StatType.OfUndeadSlaying, 1);
                    break;

                case SpecialEnchantType.OfDavid:
                    itemEnchant.StatBonusMap.Add(StatType.OfDavid, 1);
                    break;

                case SpecialEnchantType.OfPlenty:
                    itemEnchant.StatBonusMap.Add(StatType.HpRegeneration, 1);
                    itemEnchant.StatBonusMap.Add(StatType.SpRegeneration, 1);
                    break;

                case SpecialEnchantType.Assasins:
                    itemEnchant.StatBonusMap.Add(StatType.DisarmTraps, 2);
                    itemEnchant.StatBonusMap.Add(StatType.OfPoison, 1);
                    break;

                case SpecialEnchantType.Barbarians:
                    itemEnchant.StatBonusMap.Add(StatType.ArmorClass, 5);
                    itemEnchant.StatBonusMap.Add(StatType.OfFrost, 1);
                    break;

                case SpecialEnchantType.OfTheStorm:
                    itemEnchant.StatBonusMap.Add(StatType.AirResistance, 20);
                    itemEnchant.StatBonusMap.Add(StatType.OfShielding, 1);
                    break;

                case SpecialEnchantType.OfTheOcean:
                    itemEnchant.StatBonusMap.Add(StatType.WaterResistance, 10);
                    itemEnchant.StatBonusMap.Add(StatType.Alchemy, 2);
                    break;

                case SpecialEnchantType.OfWaterWalking:
                    itemEnchant.StatBonusMap.Add(StatType.OfWaterWalking, 1);
                    break;

                case SpecialEnchantType.OfFeatherFalling:
                    itemEnchant.StatBonusMap.Add(StatType.OfFeatherFalling, 1);
                    break;


                default:
                    Debug.LogError("Unhandled special enchant type: " + type);
                    return;
            }

            destItem.Enchant = itemEnchant;
        }

        // This has to be called AFTER databases are initialized
        public void Init()
        {
            m_ItemDb = DbMgr.Instance.ItemDb.Data;

            ItemEnchantSpecialDb specialEnchantDb = DbMgr.Instance.ItemEnchantSpecialDb;

            foreach (TreasureLevel treasureLevel in Enum.GetValues(typeof(TreasureLevel)))
            {
                m_TreasureLevelToSumOfItemChanceMap.Add(treasureLevel, 0);

                m_SpecialEnchantChanceSummMap[treasureLevel] = new Dictionary<ItemType, int>();

                foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
                {
                    m_SpecialEnchantChanceSummMap[treasureLevel].Add(itemType, 0);
                }
            }

            foreach (ItemType itemType in Enum.GetValues(typeof(ItemType)))
            {
                m_StandardEnchantChanceSummMap.Add(itemType, 0);
            }

            foreach (ItemData itemData in m_ItemDb.Values)
            {
                foreach (var treasureDropChancePair in itemData.TreasureLevelDropChanceMap)
                {
                    m_TreasureLevelToSumOfItemChanceMap[treasureDropChancePair.Key] += treasureDropChancePair.Value;
                }
            }

            foreach (ItemEnchantStandardData stdEnchantData in DbMgr.Instance.ItemEnchantStandardDb.Data.Values)
            {
                foreach (var enchChancePair in stdEnchantData.ChanceToApplyMap)
                {
                    m_StandardEnchantChanceSummMap[enchChancePair.Key] += enchChancePair.Value;
                }
            }

            foreach (ItemEnchantSpecialData specialEnchantData in DbMgr.Instance.ItemEnchantSpecialDb.Data.Values)
            {
                foreach (TreasureLevel treasureLevel in Enum.GetValues(typeof(TreasureLevel)))
                {
                    if (IsTreasureLevelEligibleForRarityLevel(treasureLevel, specialEnchantData.RarityLevel))
                    {
                        foreach (var enchChancePair in specialEnchantData.ChanceToApplyMap)
                        {
                            m_SpecialEnchantChanceSummMap[treasureLevel][enchChancePair.Key] += enchChancePair.Value;
                        }
                    }
                }
            }
        }
    }
}
