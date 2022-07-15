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
    public class CharacterReactionData : DbData<CharacterReaction>
    {
        // CharacterExpression Id
        public CharacterSpeech[] SpeechVariants = new CharacterSpeech[0];
        public CharacterExpression[] ExpressionVariants = new CharacterExpression[0];
    }

    public class CharacterReactionDb : DataDb<CharacterReactionData, CharacterReaction>
    {
        override public CharacterReactionData ProcessCsvDataRow(int row, string[] columns)
        {
            if (row == 0)
            {
                return null;
            }

            CharacterReactionData data = new CharacterReactionData();
            int id = int.Parse(columns[0]);
            if (!Enum.IsDefined(typeof(CharacterReaction), id))
            {
                Debug.LogError("ID: " + id + " is not defined as CharacterReaction");
                return null;
            }

            data.Id = (CharacterReaction)id;

            if (!string.IsNullOrEmpty(columns[1]))
            {
                string[] speechVariantsStr = columns[1].Trim().Split(',');
                data.SpeechVariants = new CharacterSpeech[speechVariantsStr.Length];
                for (int i = 0; i < speechVariantsStr.Length; i++)
                {
                    int speechVariantId = int.Parse(speechVariantsStr[i]);
                    if (!Enum.IsDefined(typeof(CharacterSpeech), speechVariantId))
                    {
                        Debug.LogError("Speech variant ID: " + speechVariantId + " is not defined !");
                        continue;
                    }

                    data.SpeechVariants[i] = (CharacterSpeech)speechVariantId;
                }
            }
            
            if (!string.IsNullOrEmpty(columns[2]))
            {
                string[] expressionVariantsStr = columns[2].Trim().Split(',');
                data.ExpressionVariants = new CharacterExpression[expressionVariantsStr.Length];
                for (int i = 0; i < expressionVariantsStr.Length; i++)
                {
                    int expressionVariantId = int.Parse(expressionVariantsStr[i]);
                    if (!Enum.IsDefined(typeof(CharacterExpression), expressionVariantId))
                    {
                        Debug.LogError("Expression variant ID: " + expressionVariantId + " is not defined !");
                        continue;
                    }

                    data.ExpressionVariants[i] = (CharacterExpression)expressionVariantId;
                }
            }

            return data;
        }
    }
}
