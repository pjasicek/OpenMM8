using System.Collections.Generic;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public interface IHouseService
    {
        HouseServiceType ServiceType { get; }
        bool UsesDedicatedUiState { get; }

        string GetGreeting(HouseServiceContext context);
        List<HouseDialogueOption> GetOptions(HouseServiceContext context);
        HouseServiceResult HandleOption(string optionId, HouseServiceContext context);
        HouseServiceResult HandleTextInput(string inputId, string inputText, HouseServiceContext context);
    }
}
