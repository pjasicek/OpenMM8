using System.Collections.Generic;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class ShopHouseService : HouseServiceBase
    {
        private const string EquipmentMenu = "equipment";
        private const string SkillsMenu = "skills";

        public override HouseServiceType ServiceType => HouseServiceType.Shop;

        public override bool UsesDedicatedUiState => true;

        public override List<HouseDialogueOption> GetOptions(HouseServiceContext context)
        {
            if (context.CurrentMenuId == EquipmentMenu)
            {
                if (context.HouseData != null && context.HouseData.TypeName == "Alchemist")
                {
                    return new List<HouseDialogueOption>()
                    {
                        new HouseDialogueOption() { Id = "sell", Text = "Sell" },
                        new HouseDialogueOption() { Id = "identify", Text = "Identify" }
                    };
                }

                return new List<HouseDialogueOption>()
                {
                    new HouseDialogueOption() { Id = "sell", Text = "Sell" },
                    new HouseDialogueOption() { Id = "identify", Text = "Identify" },
                    new HouseDialogueOption() { Id = "repair", Text = "Repair" }
                };
            }

            if (context.CurrentMenuId == SkillsMenu)
            {
                return HouseSkillTeaching.GetLearnableOptions(context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData));
            }

            return new List<HouseDialogueOption>()
            {
                new HouseDialogueOption() { Id = "buy_standard", Text = "Buy Standard" },
                new HouseDialogueOption() { Id = "buy_special", Text = "Buy Special" },
                new HouseDialogueOption() { Id = "display_equipment", Text = "Display Equipment" },
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
                case "display_equipment":
                    return OpenMenu(EquipmentMenu, "Choose a service.");
                case "learn_skills":
                    return OpenMenu(SkillsMenu, HouseSkillTeaching.GetLearnSkillsGreeting(context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData)));
                default:
                    return HouseSkillTeaching.HandleLearnSkillOption(optionId, context, HouseSkillTeaching.GetConfiguredSkillOffers(context.HouseData))
                        ?? base.HandleOption(optionId, context);
            }
        }
    }
}
