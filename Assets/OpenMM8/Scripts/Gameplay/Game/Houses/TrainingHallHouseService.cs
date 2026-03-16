using System.Collections.Generic;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class TrainingHallHouseService : HouseServiceBase
    {
        private const string SkillsMenu = "skills";
        private const string NotEnoughGoldText = "You don't have enough gold";
        private const string UnableToTrainText = "With your skills, you should be working here as a teacher.\n\nSorry, but we are unable to train you.";

        public override HouseServiceType ServiceType => HouseServiceType.TrainingHall;

        public override List<HouseDialogueOption> GetOptions(HouseServiceContext context)
        {
            if (context.CurrentMenuId == SkillsMenu)
            {
                return HouseSkillTeaching.GetLearnableOptions(context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData));
            }

            return new List<HouseDialogueOption>()
            {
                new HouseDialogueOption() { Id = "train", Text = GetTrainOptionText(context) },
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
                case "train":
                    return HandleTrain(context);
                case "learn_skills":
                    return OpenMenu(SkillsMenu, HouseSkillTeaching.GetLearnSkillsGreeting(context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData)));
                default:
                    return HouseSkillTeaching.HandleLearnSkillOption(optionId, context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData))
                        ?? base.HandleOption(optionId, context);
            }
        }

        private static HouseServiceResult HandleTrain(HouseServiceContext context)
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

            string failureText = GetTrainingFailureText(character, context?.HouseData);
            if (!string.IsNullOrEmpty(failureText))
            {
                return new HouseServiceResult()
                {
                    ResponseText = failureText,
                    RefreshOptions = true
                };
            }

            int trainingCost = GetTrainingCost(character, context?.HouseData);
            if (party.Gold < trainingCost)
            {
                character.PlayEventReaction(CharacterReaction.NotEnoughGold);
                return new HouseServiceResult()
                {
                    ResponseText = NotEnoughGoldText,
                    RefreshOptions = true
                };
            }

            party.AddGold(-trainingCost);
            SoundMgr.PlaySoundById(SoundType.FoundLoot, party.PlayerAudioSource);

            character.Level++;
            int skillPointsEarned = character.Level / 10 + 5;
            character.SkillPoints += skillPointsEarned;
            character.AddCurrHitPoints(character.GetMaxHitPoints() - character.CurrHitPoints);
            character.AddCurrSpellPoints(character.GetMaxSpellPoints() - character.CurrSpellPoints);

            // TODO: Match OE training-time progression. Successful training should advance game time,
            // but only when this visit reaches a new party-wide max number of trained levels.

            character.UI?.Refresh();
            character.UI?.StatsUI?.Refresh();
            character.UI?.SkillsUI?.Refresh();
            party.PartyUI?.Refresh();

            character.PlayEventReaction(CharacterReaction.TrainedToNextLevel);

            string successText = character.Name + " is now Level " + character.Level +
                " and has earned " + skillPointsEarned + " Skill Points!";
            GameCore.SetStatusBarText(successText);

            return new HouseServiceResult()
            {
                ResponseText = successText,
                RefreshOptions = true
            };
        }

        private static string GetTrainOptionText(HouseServiceContext context)
        {
            Character character = GetSelectedCharacter();
            if (character == null)
            {
                return "Train";
            }

            string failureText = GetTrainingFailureText(character, context?.HouseData);
            if (!string.IsNullOrEmpty(failureText))
            {
                return failureText;
            }

            int nextLevel = character.Level + 1;
            int trainingCost = GetTrainingCost(character, context?.HouseData);
            return "Train to level " + nextLevel + " for " + trainingCost + " gold";
        }

        private static string GetTrainingFailureText(Character character, Assets.OpenMM8.Scripts.Data.HouseData houseData)
        {
            if (character == null || houseData == null)
            {
                return "Train";
            }

            int trainingMaxLevel = houseData.TrainingMaxLevel;
            if (trainingMaxLevel > 0 && character.Level >= trainingMaxLevel)
            {
                return UnableToTrainText;
            }

            int requiredExperience = GameMechanics.GetTotalExperienceRequired(character.Level + 1);
            if (character.Experience < requiredExperience)
            {
                int missingExperience = requiredExperience - character.Experience;
                return "You need " + missingExperience + " more experience to train to level " + (character.Level + 1);
            }

            return string.Empty;
        }

        private static int GetTrainingCost(Character character, Assets.OpenMM8.Scripts.Data.HouseData houseData)
        {
            if (character == null || houseData == null)
            {
                return 0;
            }

            int baseTrainPrice = (int)(character.Level * houseData.PriceMultiplier * GameMechanics.GetClassTier(character.Class));
            int effectivePrice = GameMechanics.ApplyMerchantDiscount(character, baseTrainPrice);
            int minimumPrice = baseTrainPrice / 3;

            return effectivePrice < minimumPrice ? minimumPrice : effectivePrice;
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
