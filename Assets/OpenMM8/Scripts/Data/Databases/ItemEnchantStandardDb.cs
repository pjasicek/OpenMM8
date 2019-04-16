using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    /*
        Bonus range for Standard by Level					Value Mod= (+100 / Plus of Item)									
	    lvl	min	max
	    1	0	0
	    2	1	5
	    3	3	8
	    4	6	12
	    5	10	17
	    6	15	25

	    (note weapons can only have Special Bonuses)
    */

    // ITEM_ENCHANT_STANDARD
    public class ItemEnchantStandardData : DbData
    {
        public StatBonusType BonusType;
        public string OfName;
        public Dictionary<ItemType, int> ChanceToApplyMap = new Dictionary<ItemType, int>();
    }

    public class ItemEnchantStandardDb : DataDb<ItemEnchantStandardData>
    {
        // Just a helper
        private int m_Id = 0;

        override public ItemEnchantStandardData ProcessCsvDataRow(int row, string[] columns)
        {
            if (row < 4)
            {
                return null;
            }

            if (string.IsNullOrEmpty(columns[0]))
            {
                return null;
            }


            ItemEnchantStandardData data = new ItemEnchantStandardData();
            data.Id = m_Id++;

            switch (columns[0])
            {
                case "Might": data.BonusType = StatBonusType.Might; break;
                case "Intellect": data.BonusType = StatBonusType.Intellect; break;
                case "Personality": data.BonusType = StatBonusType.Personality; break;
                case "Endurance": data.BonusType = StatBonusType.Endurance; break;
                case "Accuracy": data.BonusType = StatBonusType.Accuracy; break;
                case "Speed": data.BonusType = StatBonusType.Speed; break;
                case "Luck": data.BonusType = StatBonusType.Luck; break;
                case "Hit Points": data.BonusType = StatBonusType.HitPoints; break;
                case "Spell Points": data.BonusType = StatBonusType.SpellPoints; break;
                case "Armor Class": data.BonusType = StatBonusType.ArmorClass; break;
                case "Fire Resistance": data.BonusType = StatBonusType.FireResistance; break;
                case "Air Resistance": data.BonusType = StatBonusType.AirResistance; break;
                case "Water Resistance": data.BonusType = StatBonusType.WaterResistance; break;
                case "Earth Resistance": data.BonusType = StatBonusType.EarthResistance; break;
                case "Mind Resistance": data.BonusType = StatBonusType.MindResistance; break;
                case "Body Resistance": data.BonusType = StatBonusType.BodyResistance; break;
                case "Alchemy skill": data.BonusType = StatBonusType.Alchemy; break;
                case "Stealing skill": data.BonusType = StatBonusType.Stealing; break;
                case "Disarm skill": data.BonusType = StatBonusType.DisarmTraps; break;
                case "ID Item skill": data.BonusType = StatBonusType.IdentifyItem; break;
                case "ID Monster skill": data.BonusType = StatBonusType.IdentifyMonster; break;
                case "Armsmaster skill": data.BonusType = StatBonusType.Armsmaster; break;
                case "Dodge skill": data.BonusType = StatBonusType.Dodging; break;
                case "Unarmed skill": data.BonusType = StatBonusType.Unarmed; break;

                default:
                    Debug.LogError("Unknown bonus stat: " + columns[0]);
                    return null;
            }

            data.OfName = columns[1];
            data.ChanceToApplyMap.Add(ItemType.Armor, int.Parse(columns[2]));
            data.ChanceToApplyMap.Add(ItemType.Shield, int.Parse(columns[3]));
            data.ChanceToApplyMap.Add(ItemType.Helmet, int.Parse(columns[4]));
            data.ChanceToApplyMap.Add(ItemType.Belt, int.Parse(columns[5]));
            data.ChanceToApplyMap.Add(ItemType.Cloak, int.Parse(columns[6]));
            data.ChanceToApplyMap.Add(ItemType.Gauntlets, int.Parse(columns[7]));
            data.ChanceToApplyMap.Add(ItemType.Boots, int.Parse(columns[8]));
            data.ChanceToApplyMap.Add(ItemType.Ring, int.Parse(columns[9]));
            data.ChanceToApplyMap.Add(ItemType.Amulet, int.Parse(columns[10]));

            return data;
        }
    }
}
