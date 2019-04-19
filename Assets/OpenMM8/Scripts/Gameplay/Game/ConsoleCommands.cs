using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.OpenMM8.Scripts.Gameplay.Items;
using IngameDebugConsole;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay.Game
{
    public class ConsoleCommands
    {
        [ConsoleMethod("additem", "Adds item to active player's inventory")]
        public static void AddItem(int itemId)
        {
            GameMgr.Instance.PlayerParty.ActiveCharacter?.Inventory.AddItem(itemId);
        }

        [ConsoleMethod("genitem", "Generates random item to active player's inventory based on Treasure Level")]
        public static void GenerateItem(int treasureLevel)
        {
            Item item = ItemGenerator.GenerateItem((TreasureLevel)treasureLevel);
            if (item == null)
            {
                Debug.LogError("Failed to generate item");
                return;
            }

            GameMgr.Instance.PlayerParty.GetActiveOrFirstCharacter().Inventory.AddItem(item);
        }

        [ConsoleMethod("genitemtype", "Generates random item to active player's inventory based on Treasure Level and ItemType")]
        public static void GenerateItemType(int treasureLevel, int itemType)
        {
            ItemType type = (ItemType)itemType;
            Item item = ItemGenerator.GenerateItem((TreasureLevel)treasureLevel, type);
            if (item == null)
            {
                Debug.LogError("Failed to generate item");
                return;
            }

            GameMgr.Instance.PlayerParty.GetActiveOrFirstCharacter().Inventory.AddItem(item);
        }

        [ConsoleMethod("genitemskill", "Generates random item to active player's inventory based on Treasure Level and ItemSkillGroup")]
        public static void GenerateItemSkill(int treasureLevel, int itemSkill)
        {
            ItemSkillGroup type = (ItemSkillGroup)itemSkill;
            Item item = ItemGenerator.GenerateItem((TreasureLevel)treasureLevel, type);
            if (item == null)
            {
                Debug.LogError("Failed to generate item");
                return;
            }

            GameMgr.Instance.PlayerParty.GetActiveOrFirstCharacter().Inventory.AddItem(item);
        }
    }
}
