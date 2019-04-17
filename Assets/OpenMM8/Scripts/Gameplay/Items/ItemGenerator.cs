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
        private Dictionary<ItemLevel, int> m_ItemLootSummChanceMap = new Dictionary<ItemLevel, int>();
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

        public int GenStandardItemEnchantAmount(ItemLevel itemLevel)
        {
            // Unity int range is max exclusive - that is why it looks like its max is 1 more than
            //    it should be
            switch (itemLevel)
            {
                case ItemLevel.L1:
                    return 0;

                case ItemLevel.L2:
                    return UnityEngine.Random.Range(1, 6);

                case ItemLevel.L3:
                    return UnityEngine.Random.Range(3, 9);

                case ItemLevel.L4:
                    return UnityEngine.Random.Range(6, 13);

                case ItemLevel.L5:
                    return UnityEngine.Random.Range(10, 18);

                case ItemLevel.L6:
                    return UnityEngine.Random.Range(15, 26);
            }

            Debug.LogError("Unhandled item level: " + itemLevel);
            return 0;
        }

        public bool IsItemLevelEligibleForSpecialEnchant(ItemLevel itemLevel, SpecialEnchantType specialEnchantType)
        {
            ItemEnchantSpecialData enchantData = DbMgr.Instance.ItemEnchantSpecialDb.Get(specialEnchantType);
            if (enchantData == null)
            {
                return false;
            }

            switch (itemLevel)
            {
                case ItemLevel.L1:
                case ItemLevel.L2:
                    return false;

                case ItemLevel.L3:
                    return enchantData.RarityLevel == "A" || enchantData.RarityLevel == "B";

                case ItemLevel.L4:
                    return enchantData.RarityLevel == "A" || enchantData.RarityLevel == "B" || 
                        enchantData.RarityLevel == "C";

                case ItemLevel.L5:
                    return enchantData.RarityLevel == "B" || enchantData.RarityLevel == "C" ||
                        enchantData.RarityLevel == "D";

                case ItemLevel.L6:
                    return enchantData.RarityLevel == "D";
            }

            return false;
        }

        public void ApplyStandardEnchant(StatType bonusStat, int amount, ItemLevel itemLevel, Item destItem)
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

            int statAmount = GenStandardItemEnchantAmount(itemLevel);
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
                case SpecialEnchantType.OfGods:
                case SpecialEnchantType.OfCarnage:
                case SpecialEnchantType.OfCold:
                case SpecialEnchantType.OfFrost:
                case SpecialEnchantType.OfIce:
                case SpecialEnchantType.OfSparks:
                case SpecialEnchantType.OfLightning:
                case SpecialEnchantType.OfThunderbolts:
                case SpecialEnchantType.OfFire:
                case SpecialEnchantType.OfFlame:
                case SpecialEnchantType.OfInfernos:
                case SpecialEnchantType.OfPoison:
                case SpecialEnchantType.OfVenom:
                case SpecialEnchantType.OfAcid:
                case SpecialEnchantType.Vampiric:
                case SpecialEnchantType.OfRecovery:
                case SpecialEnchantType.OfImmunity:
                case SpecialEnchantType.OfSanity:
                case SpecialEnchantType.OfFreedom:
                case SpecialEnchantType.OfAntidotes:
                case SpecialEnchantType.OfAlarms:
                case SpecialEnchantType.OfMedusa:
                case SpecialEnchantType.OfForce:
                case SpecialEnchantType.OfPower:
                case SpecialEnchantType.OfAirMagic:
                case SpecialEnchantType.OfBodyMagic:
                case SpecialEnchantType.OfDarkMagic:
                case SpecialEnchantType.OfEarthMagic:
                case SpecialEnchantType.OfFireMagic:
                case SpecialEnchantType.OfLightMagic:
                case SpecialEnchantType.OfMindMagic:
                case SpecialEnchantType.OfSpiritMagic:
                case SpecialEnchantType.OfWaterMagic:
                case SpecialEnchantType.OfThievery:
                case SpecialEnchantType.OfShielding:
                case SpecialEnchantType.OfRegeneration:
                case SpecialEnchantType.OfMana:
                case SpecialEnchantType.OfOgreSlaying:
                case SpecialEnchantType.OfDragonSlaying:
                case SpecialEnchantType.OfDarkness:
                case SpecialEnchantType.OfDoom:
                case SpecialEnchantType.OfEarth:
                case SpecialEnchantType.OfLife:
                case SpecialEnchantType.Rogues:
                case SpecialEnchantType.OfTheDragon:
                case SpecialEnchantType.OfTheEclipse:
                case SpecialEnchantType.OfTheGolem:
                case SpecialEnchantType.OfTheMoon:
                case SpecialEnchantType.OfThePhoenix:
                case SpecialEnchantType.OfTheSky:
                case SpecialEnchantType.OfTheStars:
                case SpecialEnchantType.OfTheSun:
                case SpecialEnchantType.OfTheTroll:
                case SpecialEnchantType.OfTheUnicorn:
                case SpecialEnchantType.Warriors:
                case SpecialEnchantType.Wizards:
                case SpecialEnchantType.Antique:
                case SpecialEnchantType.Swift:
                case SpecialEnchantType.Monks:
                case SpecialEnchantType.Thieves:
                case SpecialEnchantType.OfIdentifying:
                case SpecialEnchantType.OfElementalSlaying:
                case SpecialEnchantType.OfUndeadSlaying:
                case SpecialEnchantType.OfDavid:
                case SpecialEnchantType.OfPlenty:
                case SpecialEnchantType.Assasins:
                case SpecialEnchantType.Barbarians:
                case SpecialEnchantType.OfTheStorm:
                case SpecialEnchantType.OfTheOcean:
                case SpecialEnchantType.OfWaterWalking:
                case SpecialEnchantType.OfFeatherFalling:

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
