using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Assets.OpenMM8.Scripts.Gameplay.Items;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class NpcLootPrototype
    {
        public int GoldDiceRolls = 0;
        public int GoldDiceSides = 0;
        public int ItemChance = 0;
        public TreasureLevel ItemLevel = TreasureLevel.None;
        public int CertainItemId = 0;

        public ItemType ItemType = ItemType.None;
        public ItemSkillGroup ItemSkillGroup = ItemSkillGroup.None;

        public NpcLootPrototype(string treasureDef)
        {
            treasureDef = treasureDef.ToLower();

            // Check if there is some treasure associated with the loot
            if (treasureDef.Contains('+'))
            {
                // Loot item level
                if (treasureDef.Contains("l1"))
                {
                    ItemLevel = TreasureLevel.L1;
                }
                else if (treasureDef.Contains("l2"))
                {
                    ItemLevel = TreasureLevel.L2;
                }
                else if (treasureDef.Contains("l3"))
                {
                    ItemLevel = TreasureLevel.L3;
                }
                else if (treasureDef.Contains("l4"))
                { 
                    ItemLevel = TreasureLevel.L4;
                }
                else if (treasureDef.Contains("l5"))
                {
                    ItemLevel = TreasureLevel.L5;
                }
                else if (treasureDef.Contains("l6"))
                {
                    ItemLevel = TreasureLevel.L6;
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false, "Unknown item level: " + treasureDef);
                }

                // Loot item type if supplied
                if (treasureDef.Contains("gem"))
                {
                    ItemType = ItemType.Gem;
                }
                else if (treasureDef.Contains("sword"))
                {
                    ItemSkillGroup = ItemSkillGroup.Sword;
                }
                else if (treasureDef.Contains("bow"))
                {
                    ItemSkillGroup = ItemSkillGroup.Bow;
                }
                else if (treasureDef.Contains("wand"))
                {
                    ItemType = ItemType.Wand;
                }
                else if (treasureDef.Contains("club"))
                {
                    ItemSkillGroup = ItemSkillGroup.Club;
                }
                else if (treasureDef.Contains("ring"))
                {
                    ItemType = ItemType.Ring;
                }
                else if (treasureDef.Contains("plate"))
                {
                    ItemSkillGroup = ItemSkillGroup.Plate;
                }
                else if (treasureDef.Contains("staff"))
                {
                    ItemSkillGroup = ItemSkillGroup.Staff;
                }
                else if (treasureDef.Contains("boots"))
                {
                    ItemType = ItemType.Boots;
                }
                else if (treasureDef.Contains("gloves"))
                {
                    ItemType = ItemType.Gauntlets;
                }
                else if (treasureDef.Contains("cloak"))
                {
                    ItemType = ItemType.Cloak;
                }
                else if (treasureDef.Contains("ore"))
                {
                    ItemType = ItemType.Ore;
                }
                else if (treasureDef.Contains("bow"))
                {
                    ItemSkillGroup = ItemSkillGroup.Bow;
                }
                else if (treasureDef.Contains("scroll"))
                {
                    ItemType = ItemType.SpellScroll;
                }
                else if (treasureDef.Contains("amulet"))
                {
                    ItemType = ItemType.Amulet;
                }
                else if (treasureDef.Contains("dagger"))
                {
                    ItemSkillGroup = ItemSkillGroup.Dagger;
                }
                else if (treasureDef.Contains("spear"))
                {
                    ItemSkillGroup = ItemSkillGroup.Spear;
                }
                else if (treasureDef.Contains("chain"))
                {
                    ItemSkillGroup = ItemSkillGroup.Chain;
                }

                // Chance if supplied
                int percentSignIdx = treasureDef.IndexOf('%');
                if (percentSignIdx > 0)
                {
                    ItemChance = int.Parse(treasureDef.Substring(0, percentSignIdx));
                }
                else
                {
                    ItemChance = 100;
                }
            }

            Match diceRoll = Regex.Match(treasureDef, "[0-9]*d[0-9]*");
            if (diceRoll.Success)
            {
                string diceRollStr = diceRoll.Value;
                GoldDiceRolls = int.Parse(diceRollStr.Substring(0, diceRollStr.IndexOf('d')));
                GoldDiceSides = int.Parse(diceRoll.Value.Substring(diceRoll.Value.LastIndexOf('d') + 1));
            }
        }
    }
}
