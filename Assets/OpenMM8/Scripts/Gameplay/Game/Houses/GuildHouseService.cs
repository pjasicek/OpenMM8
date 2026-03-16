using System.Collections.Generic;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class GuildHouseService : HouseServiceBase
    {
        private const string SkillsMenu = "skills";

        public override HouseServiceType ServiceType => HouseServiceType.Guild;

        public override bool UsesDedicatedUiState => true;

        public override List<HouseDialogueOption> GetOptions(HouseServiceContext context)
        {
            if (context.CurrentMenuId == SkillsMenu)
            {
                return HouseSkillTeaching.GetLearnableOptions(context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData));
            }

            return new List<HouseDialogueOption>()
            {
                new HouseDialogueOption() { Id = "buy_spellbooks", Text = "Buy Spellbooks" },
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
                case "learn_skills":
                    return OpenMenu(SkillsMenu, HouseSkillTeaching.GetLearnSkillsGreeting(context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData)));
                default:
                    return HouseSkillTeaching.HandleLearnSkillOption(optionId, context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData))
                        ?? base.HandleOption(optionId, context);
            }
        }
    }
}
