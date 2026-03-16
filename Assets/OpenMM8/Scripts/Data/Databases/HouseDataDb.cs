using System;
using System.Globalization;
using System.Linq;
using Assets.OpenMM8.Scripts.Data;
using Assets.OpenMM8.Scripts.Gameplay;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    public class HouseDataDb : DataDb<HouseData>
    {
        public override HouseData ProcessCsvDataRow(int row, string[] columns)
        {
            if (!TryParseInt(GetColumn(columns, 0), out int id))
            {
                return null;
            }

            HouseData data = new HouseData();
            data.Id = id;
            data.LocalId = ParseInt(GetColumn(columns, 1));
            data.TypeName = GetColumn(columns, 2);
            data.MapId = ParseInt(GetColumn(columns, 3));
            data.AnimationId = ParseInt(GetColumn(columns, 4));
            data.Name = GetColumn(columns, 5);
            data.ProprietorName = GetColumn(columns, 6);
            data.ProprietorTitle = GetColumn(columns, 7);
            data.PictureId = ParseInt(GetColumn(columns, 8));
            data.State = ParseInt(GetColumn(columns, 9));
            data.Reputation = ParseInt(GetColumn(columns, 10));
            data.Personality = ParseInt(GetColumn(columns, 11));
            data.PriceMultiplier = ParseFloat(GetColumn(columns, 12));
            data.SkillPriceMultiplier = ParseFloat(GetColumn(columns, 13));
            data.ValueC = GetColumn(columns, 14);
            data.GenerationIntervalDays = ParseInt(GetColumn(columns, 15));
            data.TrainingMaxLevel = ParseTrainingMaxLevel(GetColumn(columns, 17));
            data.OpenFrom = ParseInt(GetColumn(columns, 18));
            data.OpenTo = ParseInt(GetColumn(columns, 19));
            data.ExitPictureId = ParseInt(GetColumn(columns, 20));
            data.ExitMapId = ParseInt(GetColumn(columns, 21));
            data.RestrictionQuestBit = ParseInt(GetColumn(columns, 22));
            data.EnterText = GetColumn(columns, 23);
            data.OfferedSkills = ParseSkills(GetColumn(columns, 24));

            return data;
        }

        private static string GetColumn(string[] columns, int idx)
        {
            if (idx >= columns.Length)
            {
                return string.Empty;
            }

            return columns[idx].Trim();
        }

        private static bool TryParseInt(string value, out int parsed)
        {
            return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out parsed);
        }

        private static int ParseInt(string value)
        {
            TryParseInt(value, out int parsed);
            return parsed;
        }

        private static float ParseFloat(string value)
        {
            if (float.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out float parsed))
            {
                return parsed;
            }

            return 0.0f;
        }

        private static System.Collections.Generic.List<SkillType> ParseSkills(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return new System.Collections.Generic.List<SkillType>();
            }

            return value
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(part => part.Trim())
                .Where(part => Enum.TryParse(part, out SkillType parsedSkill) && parsedSkill != SkillType.None)
                .Select(part => (SkillType)Enum.Parse(typeof(SkillType), part))
                .ToList();
        }

        private static int ParseTrainingMaxLevel(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return 0;
            }

            if (string.Equals(value, "No Max", StringComparison.OrdinalIgnoreCase))
            {
                return int.MaxValue;
            }

            return ParseInt(value);
        }
    }
}
