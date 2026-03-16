using System;
using System.Collections.Generic;
using System.Linq;
using Assets.OpenMM8.Scripts.Data;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class HouseSkillOffer
    {
        public string OptionId;
        public string Text;
        public SkillType SkillType;
    }

    public static class HouseSkillTeaching
    {
        public static List<HouseSkillOffer> GetConfiguredSkillOffers(HouseData houseData)
        {
            if (houseData == null || houseData.OfferedSkills == null || houseData.OfferedSkills.Count == 0)
            {
                return new List<HouseSkillOffer>();
            }

            return CreateSkillOffers(houseData.OfferedSkills);
        }

        public static List<HouseDialogueOption> GetLearnableOptions(
            HouseServiceContext context,
            IReadOnlyList<HouseSkillOffer> offers)
        {
            Character character = GetSelectedCharacter(context);
            if (character == null)
            {
                return new List<HouseDialogueOption>();
            }

            return offers
                .Where(offer => CanLearnSkill(character, offer.SkillType))
                .Select(
                    offer => new HouseDialogueOption()
                    {
                        Id = offer.OptionId,
                        Text = offer.Text
                    })
                .ToList();
        }

        public static string GetLearnSkillsGreeting(
            HouseServiceContext context,
            IReadOnlyList<HouseSkillOffer> offers)
        {
            Character character = GetSelectedCharacter(context);
            if (character == null)
            {
                return "Choose a skill to learn.";
            }

            if (!offers.Any(offer => CanLearnSkill(character, offer.SkillType)))
            {
                return "Seek knowledge elsewhere " + character.Name + " the " + character.Class +
                    "\n\nI can offer you nothing further.";
            }

            return "Skill Cost: " + GetSkillLearningCost(character, context?.HouseData);
        }

        public static HouseServiceResult HandleLearnSkillOption(
            string optionId,
            HouseServiceContext context,
            IReadOnlyList<HouseSkillOffer> offers)
        {
            HouseSkillOffer offer = offers.FirstOrDefault(currOffer => currOffer.OptionId == optionId);
            if (offer == null)
            {
                return null;
            }

            Character character = GetSelectedCharacter(context);
            PlayerParty party = GameCore.GetParty();
            if (character == null || party == null)
            {
                return new HouseServiceResult()
                {
                    ResponseText = "No character selected.",
                    RefreshOptions = false
                };
            }

            if (!CanLearnSkill(character, offer.SkillType))
            {
                return new HouseServiceResult()
                {
                    ResponseText = GetLearnSkillsGreeting(context, offers),
                    RefreshOptions = true
                };
            }

            int price = GetSkillLearningCost(character, context?.HouseData);
            if (party.Gold < price)
            {
                character.PlayEventReaction(CharacterReaction.NotEnoughGold);
                return new HouseServiceResult()
                {
                    ResponseText = "You don't have enough gold.",
                    RefreshOptions = false
                };
            }

            party.AddGold(-price);
            character.LearnSkill(offer.SkillType);
            character.PlayEventReaction(CharacterReaction.ShopLearnedSkill);

            return new HouseServiceResult()
            {
                ResponseText = GetLearnSkillsGreeting(context, offers),
                RefreshOptions = true
            };
        }

        public static int GetSkillLearningCost(Character character, HouseData houseData)
        {
            if (character == null || houseData == null)
            {
                return 0;
            }

            bool isGuild =
                houseData.TypeName == "Fire Guild" ||
                houseData.TypeName == "Air Guild" ||
                houseData.TypeName == "Water Guild" ||
                houseData.TypeName == "Earth Guild" ||
                houseData.TypeName == "Spirit Guild" ||
                houseData.TypeName == "Mind Guild" ||
                houseData.TypeName == "Body Guild" ||
                houseData.TypeName == "Light Guild" ||
                houseData.TypeName == "Dark Guild" ||
                houseData.TypeName == "Elemental Guild" ||
                houseData.TypeName == "Self Guild";

            int baseTeachPrice = (int)((isGuild ? houseData.PriceMultiplier : houseData.SkillPriceMultiplier) * 500.0f);
            int effectivePrice = ApplyMerchantDiscount(character, baseTeachPrice);
            int minimumPrice = baseTeachPrice / 3;

            return Math.Max(effectivePrice, minimumPrice);
        }

        private static bool CanLearnSkill(Character character, SkillType skillType)
        {
            if (character == null || character.HasSkill(skillType))
            {
                return false;
            }

            ClassSkillsData classSkills = DbMgr.Instance.ClassSkillsDb.Get(character.Class);
            if (classSkills == null || !classSkills.SkillTypeToSkillMasteryMap.ContainsKey(skillType))
            {
                return false;
            }

            return classSkills.SkillTypeToSkillMasteryMap[skillType] > SkillMastery.None;
        }

        private static Character GetSelectedCharacter(HouseServiceContext context)
        {
            PlayerParty party = GameCore.GetParty();
            if (party == null)
            {
                return null;
            }

            return party.GetActiveCharacter() ?? party.GetFirstCharacter();
        }

        private static int ApplyMerchantDiscount(Character character, int goldAmount)
        {
            return goldAmount * (100 - GetMerchantModifier(character)) / 100;
        }

        private static int GetMerchantModifier(Character character)
        {
            if (character == null || !character.HasSkill(SkillType.Merchant))
            {
                return 0;
            }

            int level = character.GetActualSkillLevel(SkillType.Merchant);
            SkillMastery mastery = character.GetSkillMastery(SkillType.Merchant);

            if (mastery == SkillMastery.Grandmaster)
            {
                return 100;
            }

            int multiplier = 0;
            switch (mastery)
            {
                case SkillMastery.Normal:
                    multiplier = 1;
                    break;
                case SkillMastery.Expert:
                    multiplier = 2;
                    break;
                case SkillMastery.Master:
                    multiplier = 3;
                    break;
                case SkillMastery.Grandmaster:
                    multiplier = 5;
                    break;
            }

            int bonus = multiplier * level;
            if (bonus == 0)
            {
                return 0;
            }

            return Math.Min(bonus + 7, 100);
        }

        private static List<HouseSkillOffer> CreateSkillOffers(IEnumerable<SkillType> skillTypes)
        {
            List<HouseSkillOffer> offers = new List<HouseSkillOffer>();
            foreach (SkillType skillType in skillTypes.Distinct())
            {
                offers.Add(new HouseSkillOffer()
                {
                    OptionId = "learn_" + skillType,
                    Text = SkillTypeToText(skillType),
                    SkillType = skillType
                });
            }

            return offers;
        }

        private static string SkillTypeToText(SkillType skillType)
        {
            switch (skillType)
            {
                case SkillType.AirMagic: return "Air Magic";
                case SkillType.Alchemy: return "Alchemy";
                case SkillType.Armsmaster: return "Armsmaster";
                case SkillType.Axe: return "Axe";
                case SkillType.BodyMagic: return "Body Magic";
                case SkillType.Bodybuilding: return "Bodybuilding";
                case SkillType.Bow: return "Bow";
                case SkillType.ChainArmor: return "Chain";
                case SkillType.DarkMagic: return "Dark Magic";
                case SkillType.Dagger: return "Dagger";
                case SkillType.DisarmTraps: return "Disarm Traps";
                case SkillType.Dodging: return "Dodge";
                case SkillType.EarthMagic: return "Earth Magic";
                case SkillType.FireMagic: return "Fire Magic";
                case SkillType.IdentifyItem: return "Identify Item";
                case SkillType.IdentifyMonster: return "Identify Monster";
                case SkillType.LeatherArmor: return "Leather";
                case SkillType.Learning: return "Learning";
                case SkillType.LightMagic: return "Light Magic";
                case SkillType.Mace: return "Mace";
                case SkillType.Meditation: return "Meditation";
                case SkillType.Merchant: return "Merchant";
                case SkillType.MindMagic: return "Mind Magic";
                case SkillType.Perception: return "Perception";
                case SkillType.PlateArmor: return "Plate";
                case SkillType.RepairItem: return "Repair";
                case SkillType.Shield: return "Shield";
                case SkillType.Spear: return "Spear";
                case SkillType.SpiritMagic: return "Spirit Magic";
                case SkillType.Staff: return "Staff";
                case SkillType.Stealing: return "Stealing";
                case SkillType.Sword: return "Sword";
                case SkillType.Unarmed: return "Unarmed";
                case SkillType.WaterMagic: return "Water Magic";
                default: return skillType.ToString();
            }
        }
    }
}
