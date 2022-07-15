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
    // SPELLS.TXT
    public class SpellData : DbData<SpellType>
    {
        // SpellType Id = spell type, all spells in the table need to be defined in the enum
        public int SpellLevel;
        public string Name;
        public string ShortName;
        public SpellElement SpellElement;
        public string Description;
        public string NormalDescription;
        public string ExpertDescription;
        public string MasterDescription;
        public string GrandmasterDescription;
        public string Stats; // Flags ?
        public int ManaCostNormal;
        public int ManaCostExpert;
        public int ManaCostMaster;
        public int ManaCostGrandmaster;
        public int RecoveryRateNormal;
        public int RecoveryRateExpert;
        public int RecoveryRateMaster;
        public int RecoveryRateGrandmaster;
        public int EffectSoundId;
        public int DisplayObjectId;
        public int ImpactDisplayObjectId;
        public int BaseDamage;
        public int DamageDiceSides;
    }

    public class SpellDataDb : DataDb<SpellData, SpellType>
    {
        override public SpellData ProcessCsvDataRow(int row, string[] columns)
        {
            int id;
            if (!int.TryParse(columns[0], out id))
            {
                return null;
            }

            SpellData data = new SpellData();
            data.Id = (SpellType)id;
            data.SpellLevel = int.Parse(columns[1]);
            data.Name = columns[2];
            data.ShortName = columns[4];

            //Debug.Log(data.Id + "/" + data.Name);

            switch (columns[3])
            {
                case "Fire": data.SpellElement = SpellElement.Fire; break;
                case "Air": data.SpellElement = SpellElement.Air; break;
                case "Water": data.SpellElement = SpellElement.Water; break;
                case "Earth": data.SpellElement = SpellElement.Earth; break;
                case "Dark": data.SpellElement = SpellElement.Dark; break;
                case "Light": data.SpellElement = SpellElement.Light; break;
                case "Body": data.SpellElement = SpellElement.Body; break;
                case "Spirit": data.SpellElement = SpellElement.Spirit; break;
                case "Mind": data.SpellElement = SpellElement.Mind; break;
                case "DarkFire": data.SpellElement = SpellElement.Dark; break;
                default: data.SpellElement = SpellElement.None; break;
            }

            data.Description = columns[5];
            data.NormalDescription = columns[6];
            data.ExpertDescription = columns[7];
            data.MasterDescription = columns[8];
            data.GrandmasterDescription = columns[9];
            data.Stats = columns[10];

            data.ManaCostNormal = int.Parse(columns[11]);
            data.ManaCostExpert = int.Parse(columns[12]);
            data.ManaCostMaster = int.Parse(columns[13]);
            data.ManaCostGrandmaster = int.Parse(columns[14]);

            data.RecoveryRateNormal = int.Parse(columns[15]);
            data.RecoveryRateExpert = int.Parse(columns[16]);
            data.RecoveryRateMaster = int.Parse(columns[17]);
            data.RecoveryRateGrandmaster = int.Parse(columns[18]);

            data.EffectSoundId = int.Parse(columns[19]);

            data.DisplayObjectId = int.Parse(columns[20]);
            data.ImpactDisplayObjectId = int.Parse(columns[21]);

            data.BaseDamage = int.Parse(columns[22]);
            data.DamageDiceSides = int.Parse(columns[23]);

            return data;
        }
    }
}
