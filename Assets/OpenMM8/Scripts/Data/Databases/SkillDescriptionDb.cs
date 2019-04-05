using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    public class SkillDescriptionDb : DataDb<SkillDescriptionData, SkillType>
    {
        override public SkillDescriptionData ProcessCsvDataRow(int row, string[] columns)
        {
            string skillTypeStr = columns[0];
            SkillType skillType = SkillType.None;
            SkillGroupType skillGroup = SkillGroupType.None;
            switch (skillTypeStr)
            {
                case "Staff": skillType = SkillType.Staff; skillGroup = SkillGroupType.Weapon; break;
                case "Sword": skillType = SkillType.Sword; skillGroup = SkillGroupType.Weapon; break;
                case "Dagger": skillType = SkillType.Dagger; skillGroup = SkillGroupType.Weapon; break;
                case "Axe": skillType = SkillType.Axe; skillGroup = SkillGroupType.Weapon; break;
                case "Spear": skillType = SkillType.Spear; skillGroup = SkillGroupType.Weapon; break;
                case "Bow": skillType = SkillType.Bow; skillGroup = SkillGroupType.Weapon; break;
                case "Mace": skillType = SkillType.Mace; skillGroup = SkillGroupType.Weapon; break;
                case "Blaster": skillType = SkillType.Blaster; skillGroup = SkillGroupType.Weapon; break;
                case "Shield": skillType = SkillType.Shield; skillGroup = SkillGroupType.Armor; break;
                case "Leather": skillType = SkillType.LeatherArmor; skillGroup = SkillGroupType.Armor; break;
                case "Chain": skillType = SkillType.ChainArmor; skillGroup = SkillGroupType.Armor; break;
                case "Plate": skillType = SkillType.PlateArmor; skillGroup = SkillGroupType.Armor; break;
                case "Fire Magic": skillType = SkillType.FireMagic; skillGroup = SkillGroupType.Magic; break;
                case "Air Magic": skillType = SkillType.AirMagic; skillGroup = SkillGroupType.Magic; break;
                case "Water Magic": skillType = SkillType.WaterMagic; skillGroup = SkillGroupType.Magic; break;
                case "Earth Magic": skillType = SkillType.EarthMagic; skillGroup = SkillGroupType.Magic; break;
                case "Spirit Magic": skillType = SkillType.SpiritMagic; skillGroup = SkillGroupType.Magic; break;
                case "Mind Magic": skillType = SkillType.MindMagic; skillGroup = SkillGroupType.Magic; break;
                case "Body Magic": skillType = SkillType.BodyMagic; skillGroup = SkillGroupType.Magic; break;
                case "Light Magic": skillType = SkillType.LightMagic; skillGroup = SkillGroupType.Magic; break;
                case "Dark Magic": skillType = SkillType.DarkMagic; skillGroup = SkillGroupType.Magic; break;
                case "Dark Elf Ability": skillType = SkillType.DarkElfAbility; skillGroup = SkillGroupType.Magic; break;
                case "Vampire Ability": skillType = SkillType.VampireAbility; skillGroup = SkillGroupType.Magic; break;
                case "Dragon Ability": skillType = SkillType.DragonAbility; skillGroup = SkillGroupType.Magic; break;
                case "Identify Item": skillType = SkillType.IdentifyItem; skillGroup = SkillGroupType.Misc; break;
                case "Merchant": skillType = SkillType.Merchant; skillGroup = SkillGroupType.Misc; break;
                case "Repair": skillType = SkillType.RepairItem; skillGroup = SkillGroupType.Misc; break;
                case "Bodybuilding": skillType = SkillType.Bodybuilding; skillGroup = SkillGroupType.Misc; break;
                case "Meditation": skillType = SkillType.Meditation; skillGroup = SkillGroupType.Misc; break;
                case "Perception": skillType = SkillType.Perception; skillGroup = SkillGroupType.Misc; break;
                case "Regeneration": skillType = SkillType.Regeneration; skillGroup = SkillGroupType.Misc; break;
                case "Disarm Traps": skillType = SkillType.DisarmTraps; skillGroup = SkillGroupType.Misc; break;
                case "Dodging": skillType = SkillType.Dodging; skillGroup = SkillGroupType.Misc; break;
                case "Unarmed": skillType = SkillType.Unarmed; skillGroup = SkillGroupType.Misc; break;
                case "Identify Monster": skillType = SkillType.IdentifyMonster; skillGroup = SkillGroupType.Misc; break;
                case "Armsmaster": skillType = SkillType.Armsmaster; skillGroup = SkillGroupType.Misc; break;
                case "Stealing": skillType = SkillType.Stealing; skillGroup = SkillGroupType.Misc; break;
                case "Alchemy": skillType = SkillType.Alchemy; skillGroup = SkillGroupType.Misc; break;
                case "Learning": skillType = SkillType.Learning; skillGroup = SkillGroupType.Misc; break;
            }

            if (skillType == SkillType.None || skillGroup == SkillGroupType.None)
            {
                Debug.LogError("Unknown skill: " + skillTypeStr);
                return null;
            }

            SkillDescriptionData data = new SkillDescriptionData();
            data.Id = skillType;
            data.SkillType = skillType;
            data.SkillGroup = skillGroup;
            data.Name = columns[0];
            data.Description = columns[1];
            data.Normal = columns[2];
            data.Expert = columns[3];
            data.Master = columns[4];
            data.GrandMaster = columns[5];

            return data;
        }
    }
}
