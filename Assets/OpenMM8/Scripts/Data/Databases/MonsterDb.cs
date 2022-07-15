using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    public class MonsterDb : DataDb<MonsterData>
    {
        Dictionary<MonsterType, MonsterData> Npcs = new Dictionary<MonsterType, MonsterData>();

        override public MonsterData ProcessCsvDataRow(int row, string[] columns)
        {
            // Header
            if (row == 0)
            {
                return null;
            }

            MonsterData data = new MonsterData();

            data.Id = int.Parse(columns[0]);
            data.MonsterType = (MonsterType)data.Id;
            data.Name = columns[1];
            data.Picture = columns[2];
            data.Level = int.Parse(columns[3]);
            data.HitPoints = int.Parse(columns[4]);
            data.ArmorClass = int.Parse(columns[5]);
            data.ExperienceWorth = int.Parse(columns[6].Replace(",", ""));
            data.Treasure = new NpcLootPrototype(columns[7]);
            data.Quest = int.Parse(columns[8]);
            data.Fly = columns[9].ToLower() == "y";
            switch (columns[10].ToLower())
            {
                case "short": data.MoveType = MonsterMoveType.Short; break;
                case "med": data.MoveType = MonsterMoveType.Medium; break;
                case "long": data.MoveType = MonsterMoveType.Long; break;
                case "free": data.MoveType = MonsterMoveType.Free; break;
                case "stationary": data.MoveType = MonsterMoveType.Stationary; break;
                case "global": data.MoveType = MonsterMoveType.Global; break;
                default: data.MoveType = MonsterMoveType.Free; break;
            }

            switch (columns[11].ToLower())
            {
                case "wimp": data.Agressivity = MonsterAggresivityType.Wimp; break;
                case "normal": data.Agressivity = MonsterAggresivityType.Normal; break;
                case "aggress": data.Agressivity = MonsterAggresivityType.Agressive; break;
                case "suicidal": data.Agressivity = MonsterAggresivityType.Suicidal; break;
                default: break;
            }
            data.Hostility = int.Parse(columns[12]);
            data.Speed = int.Parse(columns[13]);
            // Not sure why but this is how it should be
            data.RecoveryTime = (int.Parse(columns[14]) / 128.0f) * 2.13333f; 

            switch (columns[15].ToLower())
            {
                case "c": data.AttackPreferenceMask |= AttackPreferenceMask.ClassCleric; break;
                case "k": data.AttackPreferenceMask |= AttackPreferenceMask.ClassKnight; break;
                case "n": data.AttackPreferenceMask |= AttackPreferenceMask.ClassNecromancer; break;

                case "x": data.AttackPreferenceMask |= AttackPreferenceMask.GenderMale; break;
                case "o": data.AttackPreferenceMask |= AttackPreferenceMask.GenderFemale; break;

                case "v": data.AttackPreferenceMask |= AttackPreferenceMask.RaceVampire; break;
                case "de": data.AttackPreferenceMask |= AttackPreferenceMask.RaceDarkElf; break;
                case "m": data.AttackPreferenceMask |= AttackPreferenceMask.RaceMinotaur; break;
                case "t": data.AttackPreferenceMask |= AttackPreferenceMask.RaceTroll; break;
                case "d": data.AttackPreferenceMask |= AttackPreferenceMask.RaceDragon; break;
                case "u": data.AttackPreferenceMask |= AttackPreferenceMask.RaceUndead; break;
                case "e": data.AttackPreferenceMask |= AttackPreferenceMask.RaceElf; break;
                case "g": data.AttackPreferenceMask |= AttackPreferenceMask.RaceGoblin; break;
            }

            data.NumCharactersAffectedByBonusAbility = 1;
            int.TryParse(columns[15], out data.NumCharactersAffectedByBonusAbility);
            data.BonusAbility = columns[16];

            data.AttackAmountText = columns[18];

            // Attack 1
            data.Attack1_Element = CsvSpellElementToEnum(columns[17]);
            data.Attack1_Missile = columns[19];
            CsvDamageRangeToInt(columns[18], 
                out data.Attack1_DamageDiceRolls, 
                out data.Attack1_DamageDiceSides, 
                out data.Attack1_DamageBonus);

            // Attack 2
            data.Attack2_UseChance = int.Parse(columns[20]);
            data.Attack2_Element = CsvSpellElementToEnum(columns[21]);
            data.Attack2_Missile = columns[23];
            CsvDamageRangeToInt(columns[22],
                out data.Attack2_DamageDiceRolls,
                out data.Attack2_DamageDiceSides,
                out data.Attack2_DamageBonus);

            SpellDataDb spellDb = DbMgr.Instance.SpellDataDb;

            // Spell Attack 1
            if (columns[25] != "0")
            {
                data.Spell1_UseChance = int.Parse(columns[24]);
                SpellInfo spell1 = new SpellInfo(columns[25]);
                foreach (SpellData spellData in spellDb.Data.Values)
                {
                    if (spell1.SpellName == spellData.Name.ToLower())
                    {
                        data.Spell1_SpellType = spellData.Id;
                        data.Spell1_SkillLevel = spell1.SpellLevel;
                        data.Spell1_SkillMastery = spell1.SpellMastery;
                        break;
                    }
                }
                if (data.Spell1_SpellType == SpellType.None)
                {
                    Debug.LogError("Failed to retrieve spell info for: " + columns[25]);
                }
            }
            // Spell Attack 2
            if (columns[27] != "0")
            {
                data.Spell2_UseChance = int.Parse(columns[26]);
                SpellInfo spell2 = new SpellInfo(columns[27]);
                foreach (SpellData spellData in spellDb.Data.Values)
                {
                    if (spell2.SpellName == spellData.Name.ToLower())
                    {
                        data.Spell2_SpellType = spellData.Id;
                        data.Spell2_SkillLevel = spell2.SpellLevel;
                        data.Spell2_SkillMastery = spell2.SpellMastery;
                        break;
                    }
                }
                if (data.Spell2_SpellType == SpellType.None)
                {
                    Debug.LogError("Failed to retrieve spell info for: " + columns[27]);
                }
            }
            // Resistances
            data.Resistances[SpellElement.Fire] = CsvResistanceAmountToInt(columns[28]);
            data.Resistances[SpellElement.Air] = CsvResistanceAmountToInt(columns[29]);
            data.Resistances[SpellElement.Water] = CsvResistanceAmountToInt(columns[30]);
            data.Resistances[SpellElement.Earth] = CsvResistanceAmountToInt(columns[31]);
            data.Resistances[SpellElement.Mind] = CsvResistanceAmountToInt(columns[32]);
            data.Resistances[SpellElement.Spirit] = CsvResistanceAmountToInt(columns[33]);
            data.Resistances[SpellElement.Body] = CsvResistanceAmountToInt(columns[34]);
            data.Resistances[SpellElement.Light] = CsvResistanceAmountToInt(columns[35]);
            data.Resistances[SpellElement.Dark] = CsvResistanceAmountToInt(columns[36]);
            data.Resistances[SpellElement.Physical] = CsvResistanceAmountToInt(columns[37]);
            // Special
            data.SpecialAbility = columns[38];

            return data;
        }

        private static SpellElement CsvSpellElementToEnum(string csv)
        {
            switch (csv.ToLower())
            {
                case "phys": return SpellElement.Physical;
                case "dark": return SpellElement.Dark;
                case "light": return SpellElement.Light;
                case "fire": return SpellElement.Fire;
                case "water": return SpellElement.Water;
                case "earth": return SpellElement.Earth;
                case "air": return SpellElement.Air;
                default: return SpellElement.Physical;
            }
        }

        private static int CsvResistanceAmountToInt(string csv)
        {
            int val = 0;
            if (csv == "Imm")
            {
                val = 1000000;
            }
            else
            {
                val = int.Parse(csv);
            }

            return val;
        }

        private static bool CsvDamageRangeToInt(string csv, out int diceRolls, out int diceSides, out int bonus)
        {
            diceRolls = 0;
            diceSides = 0;
            bonus = 0;

            csv = csv.ToLower();

            if (csv == "0")
            {
                return true;
            }

            if (csv.Contains('+'))
            {
                bonus = int.Parse(csv.Substring(csv.LastIndexOf('+') + 1));
            }

            Match diceRoll = Regex.Match(csv, "[0-9]*d[0-9]*");
            if (diceRoll.Success)
            {
                string diceRollStr = diceRoll.Value;
                diceRolls = int.Parse(diceRollStr.Substring(0, diceRollStr.IndexOf('d')));
                diceSides = int.Parse(diceRoll.Value.Substring(diceRoll.Value.LastIndexOf('d') + 1));

                return true;
            }

            return false;
        }
    }
}
