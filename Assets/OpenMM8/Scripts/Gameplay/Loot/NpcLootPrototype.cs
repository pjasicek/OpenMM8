﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class NpcLootPrototype
    {
        public int MinGold = 0;
        public int MaxGold = 0;
        public int ItemChance = 0;
        public EquipType ItemType = EquipType.NotAvailable;
        public LootItemLevel ItemLevel = LootItemLevel.None;

        public NpcLootPrototype(string treasureDef)
        {
            treasureDef = treasureDef.ToLower();

            // Check if there is some treasure associated with the loot
            if (treasureDef.Contains('+'))
            {
                // Loot item level
                if (treasureDef.Contains("l1"))
                {
                    ItemLevel = LootItemLevel.L1;
                }
                else if (treasureDef.Contains("l2"))
                {
                    ItemLevel = LootItemLevel.L2;
                }
                else if (treasureDef.Contains("l3"))
                {
                    ItemLevel = LootItemLevel.L3;
                }
                else if (treasureDef.Contains("l4"))
                { 
                    ItemLevel = LootItemLevel.L4;
                }
                else if (treasureDef.Contains("l5"))
                {
                    ItemLevel = LootItemLevel.L5;
                }
                else if (treasureDef.Contains("l6"))
                {
                    ItemLevel = LootItemLevel.L6;
                }
                else
                {
                    System.Diagnostics.Debug.Assert(false, "Unknown item level: " + treasureDef);
                }

                // Loot item type if supplied
                // TODO: Needs a separate enum
                /*if (treasureDef.Contains("gem"))
                {
                    ItemType = EquipType.Gem;
                }
                else if (treasureDef.Contains("sword"))
                {
                    ItemType = EquipType.WeaponDualWield;
                }
                else if (treasureDef.Contains("bow"))
                {
                    ItemType = EquipType.Missile;
                }
                else if (treasureDef.Contains("wand"))
                {
                    ItemType = EquipType.Wand;
                }
                else if (treasureDef.Contains("club"))
                {
                    ItemType = EquipType.WeaponOneHanded;
                }
                else if (treasureDef.Contains("ring"))
                {
                    ItemType = EquipType.Ring;
                }
                else if (treasureDef.Contains("plate"))
                {
                    ItemType = EquipType.Armor;
                }
                else if (treasureDef.Contains("staff"))
                {
                    ItemType = EquipType.staf;
                }
                else if (treasureDef.Contains("boots"))
                {
                    ItemType = EquipType.Gem;
                }
                else if (treasureDef.Contains("gloves"))
                {
                    ItemType = EquipType.Gem;
                }
                else if (treasureDef.Contains("cloak"))
                {
                    ItemType = EquipType.Gem;
                }
                else if (treasureDef.Contains("ore"))
                {
                    ItemType = EquipType.Gem;
                }
                else if (treasureDef.Contains("bow"))
                {
                    ItemType = EquipType.Gem;
                }
                else if (treasureDef.Contains("scroll"))
                {
                    ItemType = EquipType.Gem;
                }
                else if (treasureDef.Contains("amulet"))
                {
                    ItemType = EquipType.Gem;
                }
                else if (treasureDef.Contains("dagger"))
                {
                    ItemType = EquipType.Gem;
                }
                else if (treasureDef.Contains("spear"))
                {
                    ItemType = EquipType.Gem;
                }
                else if (treasureDef.Contains("chain"))
                {
                    ItemType = EquipType.Gem;
                }*/

                // Chance if supplied
                int percentSignIdx = treasureDef.IndexOf('%');
                if (percentSignIdx > 0)
                {
                    ItemChance = int.Parse(treasureDef.Substring(0, percentSignIdx));
                }
            }

            Match diceRoll = Regex.Match(treasureDef, "[0-9]*d[0-9]*");
            if (diceRoll.Success)
            {
                string diceRollStr = diceRoll.Value;
                int numDiceSides = int.Parse(diceRollStr.Substring(0, diceRollStr.IndexOf('d')));
                int numDiceRolls = int.Parse(diceRoll.Value.Substring(diceRoll.Value.LastIndexOf('d') + 1));

                MinGold = numDiceSides;
                MaxGold = numDiceSides * numDiceRolls;
            }
        }
    }
}