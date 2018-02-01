using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    class NpcDb
    {
        Dictionary<NpcType, NpcData> Npcs = new Dictionary<NpcType, NpcData>();

        public NpcDb()
        {
            long now = DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
            using (StreamReader reader = new StreamReader(@"Assets/OpenMM8/Data/MonsterNpcData.csv"))
            {
                int rowNum = 1;
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();

                    // First 3 rows are headers / garbage
                    if (rowNum++ < 5)
                    {
                        continue;
                    }

                    string[] columns = line.Split(';');
                    if (string.IsNullOrEmpty(columns[1]))
                    {
                        continue;
                    }

                    NpcData npcData = new NpcData();

                    npcData.Id = int.Parse(columns[0]);
                    npcData.NpcType = (NpcType)npcData.Id;
                    npcData.Name = columns[1];
                    npcData.Picture = columns[2];
                    npcData.Level = int.Parse(columns[3]);
                    npcData.HitPoints = int.Parse(columns[4]);
                    npcData.ArmorClass = int.Parse(columns[5]);
                    npcData.ExperienceWorth = int.Parse(columns[6].Replace(",", ""));
                    npcData.Treasure = new NpcLootPrototype(columns[7]);
                    npcData.Quest = int.Parse(columns[8]);
                    npcData.Fly = columns[9].ToLower() == "y";
                    npcData.Move = columns[10];
                    switch (columns[11].ToLower())
                    {
                        case "wimp": npcData.Agressivity = NpcAgressivityType.Wimp; break;
                        case "normal": npcData.Agressivity = NpcAgressivityType.Normal; break;
                        case "aggress": npcData.Agressivity = NpcAgressivityType.Agressive; break;
                        case "suicidal": npcData.Agressivity = NpcAgressivityType.Suicidal; break;
                        default: break;
                    }
                    npcData.Hostility = int.Parse(columns[12]); 
                    npcData.Speed = int.Parse(columns[13]);
                    npcData.Rec = int.Parse(columns[14]);
                    switch (columns[15].ToLower())
                    {
                        case "d": npcData.PreferredClass = Class.Dragon; break;
                        case "n": npcData.PreferredClass = Class.Necromancer; break;
                        case "c": npcData.PreferredClass = Class.Cleric; break;
                        case "m": npcData.PreferredClass = Class.Minotaur; break;
                        case "t": npcData.PreferredClass = Class.Troll; break;
                        case "k": npcData.PreferredClass = Class.Knight; break;
                        //case "x": npcData.PreferredClass = Class.Male; break;
                        //case "o": npcData.PreferredClass = Class.Female; break;
                        default: npcData.PreferredClass = Class.None; break;
                    }
                    npcData.BonusAbility = columns[16];

                    // Attack 1
                    npcData.Attack1 = new AttackInfo();
                    npcData.Attack1.DamageType = CsvSpellElementToEnum(columns[17]);
                    npcData.AttackAmountText = columns[18];
                    CsvDamageRangeToInt(columns[18], out npcData.Attack1.MinDamage, out npcData.Attack1.MaxDamage);
                    npcData.Attack1.Missile = columns[19];
                    // Attack 2
                    npcData.ChanceAttack2 = int.Parse(columns[20]);
                    npcData.Attack2 = new AttackInfo();
                    npcData.Attack2.DamageType = CsvSpellElementToEnum(columns[21]);
                    CsvDamageRangeToInt(columns[22], out npcData.Attack2.MinDamage, out npcData.Attack2.MaxDamage);
                    npcData.Attack2.Missile = columns[23];
                    // Spell Attack 1
                    npcData.ChanceSpellAttack1 = int.Parse(columns[24]);
                    npcData.SpellAttack1 = new SpellInfo(columns[25]);
                    // Spell Attack 2
                    npcData.ChanceSpellAttack2 = int.Parse(columns[26]);
                    npcData.SpellAttack2 = new SpellInfo(columns[27]);
                    // Resistances
                    npcData.Resistances[SpellElement.Fire] = CsvResistanceAmountToInt(columns[28]);
                    npcData.Resistances[SpellElement.Air] = CsvResistanceAmountToInt(columns[29]);
                    npcData.Resistances[SpellElement.Water] = CsvResistanceAmountToInt(columns[30]);
                    npcData.Resistances[SpellElement.Earth] = CsvResistanceAmountToInt(columns[31]);
                    npcData.Resistances[SpellElement.Mind] = CsvResistanceAmountToInt(columns[32]);
                    npcData.Resistances[SpellElement.Spirit] = CsvResistanceAmountToInt(columns[33]);
                    npcData.Resistances[SpellElement.Body] = CsvResistanceAmountToInt(columns[34]);
                    npcData.Resistances[SpellElement.Light] = CsvResistanceAmountToInt(columns[35]);
                    npcData.Resistances[SpellElement.Dark] = CsvResistanceAmountToInt(columns[36]);
                    npcData.Resistances[SpellElement.Physical] = CsvResistanceAmountToInt(columns[37]);
                    // Special
                    npcData.SpecialAbility = columns[38];

                    Npcs.Add(npcData.NpcType, npcData);
                }
            }
        }

        public NpcData GetNpcData(NpcType npcType)
        {
            NpcData npc = null;
            if (Npcs.ContainsKey(npcType))
            {
                npc = Npcs[npcType];
            }

            return npc;
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

        private static bool CsvDamageRangeToInt(string csv, out int minDamage, out int maxDamage)
        {
            int baseDamage = 0;
            minDamage = 0;
            maxDamage = 0;

            csv = csv.ToLower();

            if (csv == "0")
            {
                return true;
            }

            if (csv.Contains('+'))
            {
                baseDamage = int.Parse(csv.Substring(csv.LastIndexOf('+') + 1));
            }

            Match diceRoll = Regex.Match(csv, "[0-9]*d[0-9]*");
            if (diceRoll.Success)
            {
                string diceRollStr = diceRoll.Value;
                int numDiceSides = int.Parse(diceRollStr.Substring(0, diceRollStr.IndexOf('d')));
                int numDiceRolls = int.Parse(diceRoll.Value.Substring(diceRoll.Value.LastIndexOf('d') + 1));

                minDamage = numDiceSides + baseDamage;
                maxDamage = numDiceSides * numDiceRolls + baseDamage;

                return true;
            }

            return false;
        }
    }
}
