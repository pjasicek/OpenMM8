using System.Collections.Generic;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class TavernHouseService : HouseServiceBase
    {
        private const string SkillsMenu = "skills";
        private const string ArcomageMenu = "arcomage";
        private const int ArcomageDeckItemId = 651;
        private const string NotEnoughGoldText = "You don't have enough gold";
        private const string PacksFullText = "Your packs are already full!";

        public override HouseServiceType ServiceType => HouseServiceType.Tavern;

        public override List<HouseDialogueOption> GetOptions(HouseServiceContext context)
        {
            if (context.CurrentMenuId == SkillsMenu)
            {
                return HouseSkillTeaching.GetLearnableOptions(context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData));
            }

            if (context.CurrentMenuId == ArcomageMenu)
            {
                List<HouseDialogueOption> options = new List<HouseDialogueOption>()
                {
                    new HouseDialogueOption() { Id = "arcomage_rules", Text = "Rules" },
                    new HouseDialogueOption() { Id = "arcomage_victory_conditions", Text = "Victory Conditions" }
                };

                if (EventAPI.HaveItem(ArcomageDeckItemId))
                {
                    options.Add(new HouseDialogueOption() { Id = "arcomage_play", Text = "Play" });
                }

                return options;
            }

            Character activeCharacter = GetSelectedCharacter();
            int roomPrice = GetRoomPrice(activeCharacter, context?.HouseData);
            int foodPrice = GetFoodPrice(activeCharacter, context?.HouseData);
            int foodAmount = GetFoodAmount(context?.HouseData);

            return new List<HouseDialogueOption>()
            {
                new HouseDialogueOption() { Id = "rent_room", Text = "Rent Room for " + roomPrice + " gold" },
                new HouseDialogueOption() { Id = "buy_food", Text = "Fill Packs to " + foodAmount + " days for " + foodPrice + " gold" },
                new HouseDialogueOption() { Id = "learn_skills", Text = "Learn Skills" },
                new HouseDialogueOption() { Id = "play_arcomage", Text = "Play Arcomage" }
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
                case "rent_room":
                    return HandleRentRoom(context);
                case "buy_food":
                    return HandleBuyFood(context);
                case "learn_skills":
                    return OpenMenu(SkillsMenu, HouseSkillTeaching.GetLearnSkillsGreeting(context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData)));
                case "play_arcomage":
                    return OpenMenu(ArcomageMenu, "Choose an Arcomage option.");
                case "arcomage_rules":
                    return Todo("TODO: Arcomage rules dialogue");
                case "arcomage_victory_conditions":
                    return Todo("TODO: Arcomage victory conditions dialogue");
                case "arcomage_play":
                    return Todo("TODO: Play Arcomage");
                default:
                    return HouseSkillTeaching.HandleLearnSkillOption(optionId, context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData))
                        ?? base.HandleOption(optionId, context);
            }
        }

        private static HouseServiceResult HandleRentRoom(HouseServiceContext context)
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

            int roomPrice = GetRoomPrice(character, context?.HouseData);
            if (party.Gold < roomPrice)
            {
                character.PlayEventReaction(CharacterReaction.NotEnoughGold);
                return new HouseServiceResult()
                {
                    ResponseText = NotEnoughGoldText,
                    RefreshOptions = true
                };
            }

            party.AddGold(-roomPrice);
            SoundMgr.PlaySoundById(SoundType.FoundLoot, party.PlayerAudioSource);

            int restMinutes = GetRoomRestMinutes();
            TimeMgr.Instance.AddMinutes(restMinutes);
            RestAndHealParty(party);

            // TODO: Match OE tavern-rest flow more closely. Original game enters the rest screen,
            // applies town-specific overnight timing, and marks party members as sleeping.

            return new HouseServiceResult()
            {
                CloseDialogue = true,
                RefreshOptions = false
            };
        }

        private static HouseServiceResult HandleBuyFood(HouseServiceContext context)
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

            int foodAmount = GetFoodAmount(context?.HouseData);
            if (party.Food >= foodAmount)
            {
                character.PlayEventReaction(CharacterReaction.HaveMoreFoodThanYou);
                return new HouseServiceResult()
                {
                    ResponseText = PacksFullText,
                    RefreshOptions = true
                };
            }

            int foodPrice = GetFoodPrice(character, context?.HouseData);
            if (party.Gold < foodPrice)
            {
                character.PlayEventReaction(CharacterReaction.NotEnoughGold);
                return new HouseServiceResult()
                {
                    ResponseText = NotEnoughGoldText,
                    RefreshOptions = true
                };
            }

            party.AddGold(-foodPrice);
            SoundMgr.PlaySoundById(SoundType.FoundLoot, party.PlayerAudioSource);
            party.AddFood(foodAmount - party.Food);

            return new HouseServiceResult()
            {
                RefreshOptions = true
            };
        }

        private static void RestAndHealParty(PlayerParty party)
        {
            if (party == null)
            {
                return;
            }

            foreach (SpellEffect partyBuff in party.PartyBuffMap.Values)
            {
                partyBuff.Reset();
            }

            party.DaysPlayedWithoutRest = 0;

            foreach (Character member in party.Characters)
            {
                if (member == null)
                {
                    continue;
                }

                foreach (SpellEffect playerBuff in member.PlayerBuffMap.Values)
                {
                    playerBuff.Reset();
                }

                if (member.IsDead() || member.IsPetrified() || member.IsEradicated())
                {
                    continue;
                }

                member.RemoveCondition(Condition.Unconcious);
                member.RemoveCondition(Condition.Drunk);
                member.RemoveCondition(Condition.Fear);
                member.RemoveCondition(Condition.Sleep);
                member.RemoveCondition(Condition.Weak);
                member.TimeUntilRecovery = 0.0f;

                member.AddCurrHitPoints(member.GetMaxHitPoints() - member.CurrHitPoints);
                member.AddCurrSpellPoints(member.GetMaxSpellPoints() - member.CurrSpellPoints);
                member.UI?.Refresh();
                member.UI?.StatsUI?.Refresh();
                member.UI?.SkillsUI?.Refresh();
            }

            party.PartyUI?.Refresh();
        }

        private static int GetRoomRestMinutes()
        {
            GameTime currentTime = TimeMgr.GetCurrentTime();
            long currentMinutesOfDay = currentTime.GetHoursOfDay() * 60 + currentTime.GetMinutesFraction();
            long nextFiveAmMinutes = 5 * 60;
            long minutesUntilFiveAm = currentMinutesOfDay < nextFiveAmMinutes
                ? nextFiveAmMinutes - currentMinutesOfDay
                : (24 * 60 - currentMinutesOfDay) + nextFiveAmMinutes;

            return (int)minutesUntilFiveAm + 60;
        }

        private static int GetRoomPrice(Character character, Assets.OpenMM8.Scripts.Data.HouseData houseData)
        {
            if (houseData == null)
            {
                return 1;
            }

            float houseMultiplier = houseData.PriceMultiplier;
            int basePrice = (int)(houseMultiplier * houseMultiplier / 10.0f);
            int effectivePrice = ApplyMerchantDiscount(character, basePrice);
            int minimumPrice = basePrice / 3;

            if (effectivePrice < minimumPrice)
            {
                effectivePrice = minimumPrice;
            }

            return effectivePrice <= 0 ? 1 : effectivePrice;
        }

        private static int GetFoodPrice(Character character, Assets.OpenMM8.Scripts.Data.HouseData houseData)
        {
            if (houseData == null)
            {
                return 1;
            }

            float houseMultiplier = houseData.PriceMultiplier;
            int basePrice = (int)(houseMultiplier * houseMultiplier * houseMultiplier / 100.0f);
            int effectivePrice = ApplyMerchantDiscount(character, basePrice);
            int minimumPrice = basePrice / 3;

            if (effectivePrice < minimumPrice)
            {
                effectivePrice = minimumPrice;
            }

            return effectivePrice <= 0 ? 1 : effectivePrice;
        }

        private static int GetFoodAmount(Assets.OpenMM8.Scripts.Data.HouseData houseData)
        {
            if (houseData == null)
            {
                return 0;
            }

            return (int)houseData.PriceMultiplier;
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

            if (multiplier == 0)
            {
                return 0;
            }

            return System.Math.Min(multiplier * level + 7, 100);
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
    }
}
