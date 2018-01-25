using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnityEngine;

using Assets.OpenMM8.Scripts.Gameplay.Items;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    class ItemDb
    {
        Dictionary<int, BaseItem> Items = new Dictionary<int, BaseItem>();

        public ItemDb()
        {
            long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            using (StreamReader reader = new StreamReader(@"Assets/OpenMM8/Data/ItemData.csv"))
            {
                int rowNum = 1;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    // First 3 rows are headers / garbage
                    if (rowNum++ < 4)
                    {
                        continue;
                    }

                    string[] columns = line.Split(';');

                    ItemData itemData = new ItemData();
                    itemData.Id = int.Parse(columns[0]);
                    itemData.ImageName = columns[1];
                    itemData.Name = columns[2];
                    itemData.GoldValue = int.Parse(columns[3]);
                    switch (columns[4])
                    {
                        case "Weapon": itemData.EquipType = EquipType.WeaponOneHanded; break;
                        case "Weapon2": itemData.EquipType = EquipType.WeaponTwoHanded; break;
                        case "Weapon1or2": itemData.EquipType = EquipType.WeaponDualWield; break;
                        case "WeaponW": itemData.EquipType = EquipType.Wand; break;
                        case "Missile": itemData.EquipType = EquipType.Missile; break;
                        case "Armor": itemData.EquipType = EquipType.Armor; break;
                        case "Shield": itemData.EquipType = EquipType.Shield; break;
                        case "Helm": itemData.EquipType = EquipType.Helmet; break;
                        case "Belt": itemData.EquipType = EquipType.Belt; break;
                        case "Cloak": itemData.EquipType = EquipType.Cloak; break;
                        case "Gauntlets": itemData.EquipType = EquipType.Gauntlets; break;
                        case "Boots": itemData.EquipType = EquipType.Boots; break;
                        case "Ring": itemData.EquipType = EquipType.Ring; break;
                        case "Amulet": itemData.EquipType = EquipType.Amulet; break;
                        case "Gem": itemData.EquipType = EquipType.Gem; break;
                        case "Gold": itemData.EquipType = EquipType.Gold; break;
                        case "Reagent": itemData.EquipType = EquipType.Reagent; break;
                        case "Bottle": itemData.EquipType = EquipType.Bottle; break;
                        case "Sscroll": itemData.EquipType = EquipType.SpellScroll; break;
                        case "Book": itemData.EquipType = EquipType.SpellBook; break;
                        case "Misc": itemData.EquipType = EquipType.Misc; break;
                        case "Ore": itemData.EquipType = EquipType.Ore; break;
                        case "Mscroll": itemData.EquipType = EquipType.MessageScroll; break;
                        case "N / A": itemData.EquipType = EquipType.NotAvailable; break;
                        default: itemData.EquipType = EquipType.NotAvailable; break;
                    }
                    switch (columns[5])
                    {
                        case "Sword": itemData.SkillGroup = SkillGroup.Sword; break;
                        case "Dagger": itemData.SkillGroup = SkillGroup.Dagger; break;
                        case "Axe": itemData.SkillGroup = SkillGroup.Axe; break;
                        case "Spear": itemData.SkillGroup = SkillGroup.Spear; break;
                        case "Bow": itemData.SkillGroup = SkillGroup.Bow; break;
                        case "Mace": itemData.SkillGroup = SkillGroup.Mace; break;
                        case "Club": itemData.SkillGroup = SkillGroup.Club; break;
                        case "Staff": itemData.SkillGroup = SkillGroup.Staff; break;
                        case "Leather": itemData.SkillGroup = SkillGroup.Leather; break;
                        case "Chain": itemData.SkillGroup = SkillGroup.Chain; break;
                        case "Plate": itemData.SkillGroup = SkillGroup.Plate; break;
                        case "Shield": itemData.SkillGroup = SkillGroup.Shield; break;
                        case "Misc": itemData.SkillGroup = SkillGroup.Misc; break;
                        default: itemData.SkillGroup = SkillGroup.Misc; break;
                    }
                    itemData.Mod1 = columns[6];
                    itemData.Mod2 = columns[7];
                    itemData.Material = columns[8];
                    itemData.QualityLevel = int.Parse(columns[9]);
                    itemData.NotIdentifiedName = columns[10];
                    itemData.SpriteIndex = int.Parse(columns[11]);
                    itemData.VarA = columns[12];
                    itemData.VarB = columns[13];
                    itemData.EquipX = int.Parse(columns[14]);
                    itemData.EquipY = int.Parse(columns[15]);
                    itemData.Notes = columns[16];

                    BaseItem item = ItemFactory.CreateItem(ref itemData);

                    Items.Add(itemData.Id, item);
                }
            }
        }

        public BaseItem GetItem(int itemId)
        {
            BaseItem item = null;
            if (Items.ContainsKey(itemId))
            {
                item = Items[itemId];
            }

            return item;
        }
    }
}
