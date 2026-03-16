using System.Globalization;
using Assets.OpenMM8.Scripts.Data;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    public class HouseAnimationDb : DataDb<HouseAnimationData>
    {
        public override HouseAnimationData ProcessCsvDataRow(int row, string[] columns)
        {
            if (!int.TryParse(GetColumn(columns, 0), NumberStyles.Integer, CultureInfo.InvariantCulture, out int id))
            {
                return null;
            }

            HouseAnimationData data = new HouseAnimationData();
            data.Id = id;
            data.AnimationId = ParseInt(GetColumn(columns, 1));
            data.BuildingName = GetColumn(columns, 2);

            string npcList = GetColumn(columns, 3);
            if (!string.IsNullOrEmpty(npcList))
            {
                foreach (string npc in npcList.Split(','))
                {
                    if (int.TryParse(npc.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int npcId))
                    {
                        data.NpcsInsideList.Add(npcId);
                    }
                }
            }

            data.VideoResourcePath = GetColumn(columns, 4);
            data.EnterSoundResourcePath = GetColumn(columns, 5);

            return data;
        }

        private static string GetColumn(string[] columns, int idx)
        {
            if (idx >= columns.Length)
            {
                return string.Empty;
            }

            string value = columns[idx].Trim();
            if (value == "TODO")
            {
                return string.Empty;
            }

            return value;
        }

        private static int ParseInt(string value)
        {
            int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out int parsed);
            return parsed;
        }
    }
}
