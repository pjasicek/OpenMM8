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
    public class ClassHpSpDb : DataDb<ClassHpSpData, CharacterClass>
    {
        override public ClassHpSpData ProcessCsvDataRow(int row, string[] columns)
        {
            CharacterClass classType = CharacterClass.None;
            if (Enum.TryParse(columns[0], out classType))
            {
                ClassHpSpData data = new ClassHpSpData();
                data.Id = classType;
                data.HitPointsBase = int.Parse(columns[1]);
                data.HitPointsFactor = int.Parse(columns[2]);
                data.SpellPointsBase = int.Parse(columns[3]);
                data.SpellPointsFactor = int.Parse(columns[4]);
                data.IsSpellPointsFromIntellect = columns[5].Contains("I");
                data.IsSpellPointsFromPersonality = columns[5].Contains("P");

                return data;
            }
            else
            {
                return null;
            }
        }
    }
}
