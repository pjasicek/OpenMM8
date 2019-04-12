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
                return null;
            }

            CharacterRace race = CharacterRace.None;
            if (columns[0].StartsWith("Human")) race = CharacterRace.Human;
            else if (columns[0].StartsWith("Vampire")) race = CharacterRace.Vampire;
            else if (columns[0].StartsWith("Dark elf")) race = CharacterRace.DarkElf;
            else if (columns[0].StartsWith("Minotaur")) race = CharacterRace.Minotaur;
            else if (columns[0].StartsWith("Troll")) race = CharacterRace.Troll;
            else if (columns[0].StartsWith("Dragon")) race = CharacterRace.Dragon;
            else if (columns[0].StartsWith("Undead")) race = CharacterRace.Undead;
            else if (columns[0].StartsWith("Elf")) race = CharacterRace.Elf;
            else if (columns[0].StartsWith("Goblin")) race = CharacterRace.Goblin;

            if (race == CharacterRace.None)
            {
                return null;
            }

            StartingStatsData data = new StartingStatsData();
            data.Id = race;

            AddStartingStat(CharAttribute.Might, data, columns[1], columns[2]);
            AddStartingStat(CharAttribute.Intellect, data, columns[3], columns[4]);
            AddStartingStat(CharAttribute.Personality, data, columns[5], columns[6]);
            AddStartingStat(CharAttribute.Endurance, data, columns[7], columns[8]);
            AddStartingStat(CharAttribute.Accuracy, data, columns[9], columns[10]);
            AddStartingStat(CharAttribute.Speed, data, columns[11], columns[12]);
            AddStartingStat(CharAttribute.Luck, data, columns[13], columns[14]);

            Debug.LogError("Added: " + data.Id);

            return data;
        }

        private void AddStartingStat(CharAttribute attr, StartingStatsData data, string attrStr, string gainStr)
        {
            string[] defMaxPair = attrStr.Split('/');
            int def = int.Parse(defMaxPair[0]);
            int max = int.Parse(defMaxPair[1]);

            float gain = 1.0f;
            if (gainStr == "1/2")
            {
                gain = 0.5f;
            }
            else if (gainStr == "2")
            {
                gain = 2.0f;
            }

            data.Gain[attr] = gain;
            data.DefaultStats[attr] = def;
            data.MaxStats[attr] = max;
        }
    }
}
