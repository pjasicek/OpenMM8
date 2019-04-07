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
    public class ClassStartingSkillsDb : DataDb<ClassStartingSkillsData, CharacterClass>
    {
        private Dictionary<int, SkillType> ColumnToSkillMap = new Dictionary<int, SkillType>();

        override public ClassStartingSkillsData ProcessCsvDataRow(int row, string[] columns)
        {
            if (row == 0)
            {
                int columnIndex = 0;
                foreach (string column in columns)
                {
                    SkillType skillType = SkillType.None;
                    if (Enum.TryParse(column, out skillType))
                    {
                        ColumnToSkillMap.Add(columnIndex, skillType);
                    }
                    else
                    {
                        ColumnToSkillMap.Add(columnIndex, SkillType.None);
                    }

                    columnIndex++;
                }

                return null;
            }


            CharacterClass classType = CharacterClass.None;
            if (Enum.TryParse(columns[0], out classType))
            {
                ClassStartingSkillsData data = new ClassStartingSkillsData();
                data.Id = classType;

                int columnIndex = 0;
                foreach (string column in columns)
                {
                    StartingSkillAvailability skillAvailability = StartingSkillAvailability.None;
                    if (column == "C")
                    {
                        skillAvailability = StartingSkillAvailability.CanLearn;
                    }
                    else if (column == "F")
                    {
                        skillAvailability = StartingSkillAvailability.HasByDefault;
                    }

                    SkillType skillType = ColumnToSkillMap[columnIndex];
                    data.SkillAvailabilityMap[skillType] = skillAvailability;

                    columnIndex++;
                }

                return data;
            }
            else
            {
                return null;
            }
        }
    }
}
