using System.Collections.Generic;
using Assets.OpenMM8.Scripts.Data;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class TempleHouseService : HouseServiceBase
    {
        private const string SkillsMenu = "skills";
        private const string ThankYouText = "Thank you";
        private const string NotEnoughGoldText = "You don't have enough gold";

        public override HouseServiceType ServiceType => HouseServiceType.Temple;

        public override List<HouseDialogueOption> GetOptions(HouseServiceContext context)
        {
            if (context.CurrentMenuId == SkillsMenu)
            {
                return HouseSkillTeaching.GetLearnableOptions(context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData));
            }

            Character activeCharacter = GetSelectedCharacter();
            bool canHeal = CanHealCharacter(activeCharacter);
            int healPrice = GetTempleHealingCost(activeCharacter, context?.HouseData);

            return new List<HouseDialogueOption>()
            {
                new HouseDialogueOption()
                {
                    Id = "heal",
                    Text = "Heal " + healPrice + " gold",
                    IsEnabled = canHeal
                },
                new HouseDialogueOption() { Id = "donate", Text = "Donate" },
                new HouseDialogueOption() { Id = "learn_skills", Text = "Learn Skills" }
            };
        }

        public override string GetGreeting(HouseServiceContext context)
        {
            if (context.CurrentMenuId == SkillsMenu)
            {
                return HouseSkillTeaching.GetLearnSkillsGreeting(context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData));
            }

            return base.GetGreeting(context);
        }

        public override HouseServiceResult HandleOption(string optionId, HouseServiceContext context)
        {
            switch (optionId)
            {
                case "heal":
                    return HandleHeal(context);
                case "donate":
                    return HandleDonate(context);
                case "learn_skills":
                    return OpenMenu(SkillsMenu, HouseSkillTeaching.GetLearnSkillsGreeting(context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData)));
                default:
                    return HouseSkillTeaching.HandleLearnSkillOption(optionId, context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData))
                        ?? base.HandleOption(optionId, context);
            }
        }

        private static HouseServiceResult HandleHeal(HouseServiceContext context)
        {
            PlayerParty party = GameCore.GetParty();
            Character character = GetSelectedCharacter();
            if (party == null || character == null)
            {
                return new HouseServiceResult()
                {
                    RefreshOptions = false
                };
            }

            if (!CanHealCharacter(character))
            {
                return new HouseServiceResult()
                {
                    ResponseText = ThankYouText,
                    RefreshOptions = true
                };
            }

            int price = GetTempleHealingCost(character, context?.HouseData);
            if (party.Gold < price)
            {
                character.PlayEventReaction(CharacterReaction.NotEnoughGold);
                return new HouseServiceResult()
                {
                    ResponseText = NotEnoughGoldText,
                    RefreshOptions = true
                };
            }

            party.AddGold(-price);

            foreach (Condition condition in character.Conditions.Keys)
            {
                character.RemoveCondition(condition);
            }

            character.Condition = Condition.Good;
            character.AddCurrHitPoints(character.GetMaxHitPoints() - character.CurrHitPoints);
            character.AddCurrSpellPoints(character.GetMaxSpellPoints() - character.CurrSpellPoints);
            character.UI?.Refresh();
            party.PartyUI?.Refresh();

            SoundMgr.PlaySoundById(SoundType.Heal, party.PlayerAudioSource);
            character.PlayEventReaction(CharacterReaction.ShopHealedInTemple);

            return new HouseServiceResult()
            {
                ResponseText = ThankYouText,
                RefreshOptions = true
            };
        }

        private static HouseServiceResult HandleDonate(HouseServiceContext context)
        {
            PlayerParty party = GameCore.GetParty();
            Character character = GetSelectedCharacter();
            if (party == null || character == null)
            {
                return new HouseServiceResult()
                {
                    RefreshOptions = false
                };
            }

            int price = GetTempleDonationCost(context?.HouseData);
            if (party.Gold < price)
            {
                character.PlayEventReaction(CharacterReaction.NotEnoughGold);
                return new HouseServiceResult()
                {
                    ResponseText = NotEnoughGoldText,
                    RefreshOptions = true
                };
            }

            party.AddGold(-price);
            character.PlayEventReaction(CharacterReaction.ShopDonatedInTemple);
            SoundMgr.PlaySoundById(SoundType.FoundLoot, party.PlayerAudioSource);

            return new HouseServiceResult()
            {
                ResponseText = ThankYouText,
                RefreshOptions = true
            };
        }

        private static Character GetSelectedCharacter()
        {
            PlayerParty party = GameCore.GetParty();
            if (party == null)
            {
                return null;
            }

            return party.GetActiveCharacter() ?? party.GetFirstCharacter();
        }

        private static bool CanHealCharacter(Character character)
        {
            if (character == null)
            {
                return false;
            }

            return character.CurrHitPoints < character.GetMaxHitPoints() ||
                character.CurrSpellPoints < character.GetMaxSpellPoints() ||
                character.GetWorstCondition() != Condition.Good;
        }

        private static int GetTempleDonationCost(HouseData houseData)
        {
            if (houseData == null)
            {
                return 1;
            }

            int price = (int)houseData.PriceMultiplier;
            return price <= 0 ? 1 : price;
        }

        private static int GetTempleHealingCost(Character character, HouseData houseData)
        {
            if (character == null || houseData == null)
            {
                return 0;
            }

            Condition worstCondition = character.GetWorstCondition();
            int conditionTimeMultiplier = 1;
            int baseConditionMultiplier = 1;

            if (worstCondition == Condition.Dead ||
                worstCondition == Condition.Petrified ||
                worstCondition == Condition.Eradicated)
            {
                baseConditionMultiplier = worstCondition == Condition.Eradicated ? 10 : 5;
                conditionTimeMultiplier = GetConditionDaysPassed(character, worstCondition);
            }
            else if (worstCondition != Condition.Good)
            {
                Condition[] templeConditions =
                {
                    Condition.Cursed,
                    Condition.Weak,
                    Condition.Sleep,
                    Condition.Fear,
                    Condition.Drunk,
                    Condition.Insane,
                    Condition.PoisonWeak,
                    Condition.DiseaseWeak,
                    Condition.PoisonMedium,
                    Condition.DiseaseMedium,
                    Condition.PoisonSevere,
                    Condition.DiseaseSevere,
                    Condition.Paralyzed,
                    Condition.Unconcious
                };

                foreach (Condition condition in templeConditions)
                {
                    int daysPassed = GetConditionDaysPassed(character, condition);
                    if (daysPassed > conditionTimeMultiplier)
                    {
                        conditionTimeMultiplier = daysPassed;
                    }
                }
            }

            int result = (int)(conditionTimeMultiplier * baseConditionMultiplier * houseData.PriceMultiplier);
            if (result < 1)
            {
                return 1;
            }

            return result > 10000 ? 10000 : result;
        }

        private static int GetConditionDaysPassed(Character character, Condition condition)
        {
            if (character == null ||
                !character.Conditions.ContainsKey(condition) ||
                !character.Conditions[condition].IsValid())
            {
                return 0;
            }

            GameTime currentTime = TimeMgr.GetCurrentTime();
            GameTime conditionTime = character.Conditions[condition];
            long diffSeconds = currentTime.GetSeconds() - conditionTime.GetSeconds();
            if (diffSeconds < 0)
            {
                diffSeconds = 0;
            }

            return (int)(GameTime.FromSeconds(diffSeconds).GetDays() + 1);
        }
    }
}
