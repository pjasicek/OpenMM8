using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IngameDebugConsole;

namespace Assets.OpenMM8.Scripts.Gameplay.Game
{
    public class ConsoleCommands
    {
        [ConsoleMethod("additem", "Adds item to active player's inventory")]
        public static void AddItem(int itemId)
        {
            GameMgr.Instance.PlayerParty.ActiveCharacter?.Inventory.AddItem(itemId);
        }
    }
}
