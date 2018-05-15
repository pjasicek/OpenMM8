using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data;
using System.Text.RegularExpressions;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    public class BuildingDb : DataDb<BuildingData>
    {
        override public BuildingData ProcessCsvDataRow(int row, string[] columns)
        {
            // ID ; Text ; Note ; Owner ; Unknown
            int id;
            if (int.TryParse(columns[0], out id))
            {
                BuildingData data = new BuildingData();
                data.Id = id;
                data.MapId = int.Parse(columns[1]);
                data.BuildingName = columns[2];

                if (!string.IsNullOrEmpty(columns[3]))
                {
                    string[] npcs = columns[3].Split(',');
                    foreach (string npc in npcs)
                    {
                        data.NpcsInsideList.Add(int.Parse(npc));
                    }
                }
                
                data.VideoResourcePath = columns[4];
                data.EnterSoundResourcePath = columns[5];
                int.TryParse(columns[6], out data.OpenFrom);
                int.TryParse(columns[7], out data.OpenTo);

                return data;
            }

            return null;
        }
    }
}
