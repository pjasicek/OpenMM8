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
    // CHARACTER_DATA
    public class CharacterExpressionData : DbData<CharacterExpression>
    {
        // CharacterExpression Id
        public int[] AnimSpriteIndexes;
        public float AnimDurationSeconds;
    }

    public class CharacterFaceExpressionDb : DataDb<CharacterExpressionData, CharacterExpression>
    {
        override public CharacterExpressionData ProcessCsvDataRow(int row, string[] columns)
        {
            if (row == 0)
            {
                return null;
            }

            CharacterExpressionData data = new CharacterExpressionData();
            int id = int.Parse(columns[0]);
            if (!Enum.IsDefined(typeof(CharacterExpression), id))
            {
                Debug.LogError("ID: " + id + " is not defined as CharacterExpression");
                return null;
            }

            data.Id = (CharacterExpression)id;

            string[] animSpriteIndexes = columns[1].Trim().Split(',');
            data.AnimSpriteIndexes = new int[animSpriteIndexes.Length];
            for (int spriteIdx = 0; spriteIdx < animSpriteIndexes.Length; spriteIdx++)
            {
                data.AnimSpriteIndexes[spriteIdx] = int.Parse(animSpriteIndexes[spriteIdx]);
            }

            // In the table the duration is in miliseconds
            data.AnimDurationSeconds = int.Parse(columns[2]) / 1000.0f;

            return data;
        }
    }
}
