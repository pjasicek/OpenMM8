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
    public class ClassSkillsDb : DataDb<ClassSkillsData, CharacterClass>
    {
        private Dictionary<int, CharacterClass> ColumnToClassMap = new Dictionary<int, CharacterClass>();

        override public ClassSkillsData ProcessCsvDataRow(int row, string[] columns)
        {
            if (row == 0)
            {
                int colIdx = 0;
                foreach (string column in columns)
                {
                    CharacterClass classType = CharacterClass.None;
                    if (Enum.TryParse(column, out classType))
                    {
                        ClassSkillsData data = new ClassSkillsData();
                        data.Id = classType;
                        Data.Add(classType, data);

                        ColumnToClassMap.Add(colIdx, classType);
                    }
                    else
                    {
                        ColumnToClassMap.Add(colIdx, CharacterClass.None);
                    }

                    colIdx++;
                }
            }
            else
            {
                int colIdx = 0;
                SkillType skillType = SkillType.None;
                foreach (string column in columns)
                {
                    if (colIdx == 0)
                    {
                        if (!Enum.TryParse(column, out skillType))
                        {
                            Debug.LogError("Failed to parse skill: " + column);
                            break;
                        }
                        colIdx++;
                        continue;
                    }

                    CharacterClass classType = ColumnToClassMap[colIdx];
                    if (classType != CharacterClass.None)
                    {
                        ClassSkillsData data = Data[classType];
                        switch (column)
                        {
                            case "B": data.SkillTypeToSkillMasteryMap[skillType] = SkillMastery.Normal; break;
                            case "E": data.SkillTypeToSkillMasteryMap[skillType] = SkillMastery.Expert; break;
                            case "M": data.SkillTypeToSkillMasteryMap[skillType] = SkillMastery.Master; break;
                            case "G": data.SkillTypeToSkillMasteryMap[skillType] = SkillMastery.Grandmaster; break;

                            case "-":
                            default:
                                data.SkillTypeToSkillMasteryMap[skillType] = SkillMastery.None;
                                break;
                        }
                    }

                    colIdx++;
                }
            }

            return null;
        }

        protected override void PostLoad()
        {
            base.PostLoad();
        }
    }
}
