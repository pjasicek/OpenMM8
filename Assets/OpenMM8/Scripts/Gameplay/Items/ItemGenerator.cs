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
        private Dictionary<TreasureLevel, int> m_ItemLootSummChanceMap = new Dictionary<TreasureLevel, int>();
        private Dictionary<ItemType, int> m_StandardEnchantSummChanceMap = new Dictionary<ItemType, int>();
        private Dictionary<ItemType, int> m_SpecialEnchantSummChanceMap = new Dictionary<ItemType, int>();

        // ===============================================================

        // TotallyRandom
        /*static public Item GenerateItem()
        {

        }

        static public Item GenerateItem(int treasureLevel)
        {

        }

        static public Item GenerateItem(int treasureLevel, ItemSkillGroup skillGroup)
        {

        }

        static public Item GenerateItem(int treasureLevel, EquipSlot equipSlot)
        {

        }*/

        public int GenStandardItemEnchantAmount(TreasureLevel treasureLevel)
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

        public bool IsItemLevelEligibleForSpecialEnchant(TreasureLevel treasureLevel, SpecialEnchantType specialEnchantType)
        {
            ItemEnchantSpecialData enchantData = DbMgr.Instance.ItemEnchantSpecialDb.Get(specialEnchantType);
            if (enchantData == null)
            {
                return false;
            }

            switch (treasureLevel)
            {
                case TreasureLevel.L1:
                case TreasureLevel.L2:
                    return false;

                case TreasureLevel.L3:
                    return enchantData.RarityLevel == "A" || enchantData.RarityLevel == "B";

                case TreasureLevel.L4:
                    return enchantData.RarityLevel == "A" || enchantData.RarityLevel == "B" || 
                        enchantData.RarityLevel == "C";

                case TreasureLevel.L5:
                    return enchantData.RarityLevel == "B" || enchantData.RarityLevel == "C" ||
                        enchantData.RarityLevel == "D";

                case TreasureLevel.L6:
                    return enchantData.RarityLevel == "D";
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
                    itemEnchant.StatBonusMap.Add(StatType.OfImmunity, 1);
                    break;

                case SpecialEnchantType.OfSanity:
                    itemEnchant.StatBonusMap.Add(StatType.OfSanity, 1);
                    break;

                case SpecialEnchantType.OfFreedom:
                    itemEnchant.StatBonusMap.Add(StatType.OfFreedom, 1);
                    break;

                case SpecialEnchantType.OfAntidotes:
                    itemEnchant.StatBonusMap.Add(StatType.OfAntidotes, 1);
                    break;

                case SpecialEnchantType.OfAlarms:
                    itemEnchant.StatBonusMap.Add(StatType.OfAlarms, 1);
                    break;

                case SpecialEnchantType.OfMedusa:
                    itemEnchant.StatBonusMap.Add(StatType.OfMedusa, 1);
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
                    itemEnchant.StatBonusMap.Add(StatType.OfRegeneration, 1);
                    break;

                case SpecialEnchantType.OfMana:
                    itemEnchant.StatBonusMap.Add(StatType.OfMana, 1);
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
                    itemEnchant.StatBonusMap.Add(StatType.OfLife, 1); // TODO: Does item hp regen stack ?
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
                    itemEnchant.StatBonusMap.Add(StatType.OfTheEclipse, 1); // TODO: Does item hp/sp regen stack ?
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
                    itemEnchant.StatBonusMap.Add(StatType.OfThePhoenix, 1); // TODO: Does item hp regen stack ?
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
                    itemEnchant.StatBonusMap.Add(StatType.OfTheTroll, 1); // TODO: Does item hp regen stack ?
                    break;

                case SpecialEnchantType.OfTheUnicorn:
                    itemEnchant.StatBonusMap.Add(StatType.Luck, 15);
                    itemEnchant.StatBonusMap.Add(StatType.OfTheUnicorn, 1); // TODO: Does item sp regen stack ?
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
                    itemEnchant.StatBonusMap.Add(StatType.OfPlenty, 1);
                    break;

                case SpecialEnchantType.Assasins:
                    itemEnchant.StatBonusMap.Add(StatType.DisarmTraps, 2);
                    itemEnchant.StatBonusMap.Add(StatType.Assasins, 1);
                    break;

                case SpecialEnchantType.Barbarians:
                    itemEnchant.StatBonusMap.Add(StatType.ArmorClass, 5);
                    itemEnchant.StatBonusMap.Add(StatType.Barbarians, 1);
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

        }
    }
}
