using System.Collections.Generic;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public abstract class HouseServiceBase : IHouseService
    {
        public abstract HouseServiceType ServiceType { get; }

        public virtual bool UsesDedicatedUiState => false;

        public virtual string GetGreeting(HouseServiceContext context)
        {
            if (context?.HouseData != null)
            {
                if (!string.IsNullOrEmpty(context.HouseData.EnterText) &&
                    context.HouseData.EnterText != "0")
                {
                    return context.HouseData.EnterText;
                }

                if (!string.IsNullOrEmpty(context.HouseData.ProprietorName) &&
                    context.HouseData.ProprietorName != "Placeholder")
                {
                    return "Welcome to " + context.HouseData.Name + ".";
                }

                return context.HouseData.Name;
            }

            return string.Empty;
        }

        public abstract List<HouseDialogueOption> GetOptions(HouseServiceContext context);

        public virtual HouseServiceResult HandleOption(string optionId, HouseServiceContext context)
        {
            if (optionId == "exit")
            {
                return new HouseServiceResult()
                {
                    CloseDialogue = true,
                    RefreshOptions = false
                };
            }

            string houseName = context?.HouseData != null ? context.HouseData.Name : "House";
            string responseText = "TODO: " + houseName + " - " + optionId;
            if (UsesDedicatedUiState)
            {
                responseText = "TODO: " + houseName + " service UI (" + optionId + ")";
            }

            return new HouseServiceResult()
            {
                ResponseText = responseText,
                RefreshOptions = false
            };
        }

        public virtual HouseServiceResult HandleTextInput(string inputId, string inputText, HouseServiceContext context)
        {
            return Todo("TODO: " + inputId);
        }

        protected static HouseServiceResult OpenMenu(string menuId, string responseText = null)
        {
            return new HouseServiceResult()
            {
                PushMenuId = menuId,
                ResponseText = responseText,
                RefreshOptions = true
            };
        }

        protected static HouseServiceResult Todo(string responseText)
        {
            return new HouseServiceResult()
            {
                ResponseText = responseText,
                RefreshOptions = false
            };
        }

        protected static HouseServiceResult RequestTextInput(string inputId, string prompt, string responseText = null)
        {
            return new HouseServiceResult()
            {
                StartTextInput = true,
                TextInputId = inputId,
                TextInputPrompt = prompt,
                ResponseText = responseText,
                RefreshOptions = false
            };
        }
    }
}
