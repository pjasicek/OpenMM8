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
    public class StartingStatsDb : DataDb<StartingStatsData, CharacterRace>
    {
        private Dictionary<int, CharacterRace> ColumnToRaceMap = new Dictionary<int, CharacterRace>();

        override public StartingStatsData ProcessCsvDataRow(int row, string[] columns)
        {
            if (row == 0)
            {
                foreach (CharacterRace race in Enum.GetValues(typeof(CharacterRace)))
                {
                    if (race == CharacterRace.None)
                    {
                        continue;
                    }

                    Data.Add(race, new StartingStatsData());
                }

                ColumnToRaceMap[1] = CharacterRace.Human;
                ColumnToRaceMap[2] = CharacterRace.Vampire;
                ColumnToRaceMap[3] = CharacterRace.DarkElf;
                ColumnToRaceMap[4] = CharacterRace.Minotaur;
                ColumnToRaceMap[5] = CharacterRace.Troll;
                ColumnToRaceMap[6] = CharacterRace.Dragon;
                ColumnToRaceMap[7] = CharacterRace.Undead;
                ColumnToRaceMap[8] = CharacterRace.Elf;
                ColumnToRaceMap[9] = CharacterRace.Goblin;

                return null;
            }

            CharAttribute attr = CharAttribute.None;
            if (Enum.TryParse(columns[0], out attr))
            {
                // TODO
            }

            return null;
        }
    }
}
