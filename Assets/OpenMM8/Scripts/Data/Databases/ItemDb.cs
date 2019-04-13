using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    public class ItemDb : DataDb<ItemData>
    {
        override public ItemData ProcessCsvDataRow(int row, string[] columns)
        {
            // Header
            if (row == 0)
            {
                return null;
            }

            if (columns[1].ToLower().Equals("null"))
            {
                return null;
            }

            ItemData itemData = new ItemData();
            itemData.Id = int.Parse(columns[0]);
            itemData.ImageName = columns[1].ToLower();
            itemData.Name = columns[2];
            itemData.GoldValue = int.Parse(columns[3]);
            switch (columns[4])
            {
                case "Weapon": itemData.ItemType = ItemType.WeaponOneHanded; break;
                case "Weapon2": itemData.ItemType = ItemType.WeaponTwoHanded; break;
                case "Weapon1or2": itemData.ItemType = ItemType.WeaponDualWield; break;
                case "WeaponW": itemData.ItemType = ItemType.Wand; break;
                case "Missile": itemData.ItemType = ItemType.Missile; break;
                case "Armor": itemData.ItemType = ItemType.Armor; break;
                case "Shield": itemData.ItemType = ItemType.Shield; break;
                case "Helm": itemData.ItemType = ItemType.Helmet; break;
                case "Belt": itemData.ItemType = ItemType.Belt; break;
                case "Cloak": itemData.ItemType = ItemType.Cloak; break;
                case "Gauntlets": itemData.ItemType = ItemType.Gauntlets; break;
                case "Boots": itemData.ItemType = ItemType.Boots; break;
                case "Ring": itemData.ItemType = ItemType.Ring; break;
                case "Amulet": itemData.ItemType = ItemType.Amulet; break;
                case "Gem": itemData.ItemType = ItemType.Gem; break;
                case "Gold": itemData.ItemType = ItemType.Gold; break;
                case "Reagent": itemData.ItemType = ItemType.Reagent; break;
                case "Bottle": itemData.ItemType = ItemType.Bottle; break;
                case "Sscroll": itemData.ItemType = ItemType.SpellScroll; break;
                case "Book": itemData.ItemType = ItemType.SpellBook; break;
                case "Misc": itemData.ItemType = ItemType.Misc; break;
                case "Ore": itemData.ItemType = ItemType.Ore; break;
                case "Mscroll": itemData.ItemType = ItemType.MessageScroll; break;
                case "N / A": itemData.ItemType = ItemType.NotAvailable; break;
                default: itemData.ItemType = ItemType.NotAvailable; break;
            }
            switch (columns[5])
            {
                case "Sword": itemData.SkillGroup = ItemSkillGroup.Sword; break;
                case "Dagger": itemData.SkillGroup = ItemSkillGroup.Dagger; break;
                case "Axe": itemData.SkillGroup = ItemSkillGroup.Axe; break;
                case "Spear": itemData.SkillGroup = ItemSkillGroup.Spear; break;
                case "Bow": itemData.SkillGroup = ItemSkillGroup.Bow; break;
                case "Mace": itemData.SkillGroup = ItemSkillGroup.Mace; break;
                case "Club": itemData.SkillGroup = ItemSkillGroup.Club; break;
                case "Staff": itemData.SkillGroup = ItemSkillGroup.Staff; break;
                case "Leather": itemData.SkillGroup = ItemSkillGroup.Leather; break;
                case "Chain": itemData.SkillGroup = ItemSkillGroup.Chain; break;
                case "Plate": itemData.SkillGroup = ItemSkillGroup.Plate; break;
                case "Shield": itemData.SkillGroup = ItemSkillGroup.Shield; break;
                case "Misc": itemData.SkillGroup = ItemSkillGroup.Misc; break;
                default: itemData.SkillGroup = ItemSkillGroup.Misc; break;
            }
            itemData.Mod1 = columns[6];
            itemData.Mod2 = columns[7];
            itemData.Material = columns[8];
            itemData.QualityLevel = int.Parse(columns[9]);
            itemData.NotIdentifiedName = columns[10];
            itemData.SpriteIndex = int.Parse(columns[11]);
            itemData.VarA = columns[12];
            itemData.VarB = columns[13];
            itemData.EquipX = -1 * int.Parse(columns[14]);
            itemData.EquipY = int.Parse(columns[15]);
            itemData.Notes = columns[16];

            return itemData;
        }
    }
}
