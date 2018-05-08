using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data.Databases;
using Assets.OpenMM8.Scripts.Data;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    public class NpcTopicDb : DataDb
    {
        Dictionary<int, NpcTopicData> NpcTopicMap = new Dictionary<int, NpcTopicData>();

        override public bool ProcessCsvDataRow(int row, string[] columns)
        {
            // ID ; Text ; Note ; Owner ; Unknown
            int id;
            if (int.TryParse(columns[0], out id))
            {
                NpcTopicData npcText = new NpcTopicData();
                npcText.Id = id;
                npcText.Topic = columns[1];
                // Skip Requires . not used anyway
                npcText.Note = columns[3];
                int.TryParse(columns[4], out npcText.TextId);
                npcText.Owner = columns[5];

                NpcTopicMap.Add(id, npcText);
            }

            return NpcTopicMap.Count > 0;
        }

        public NpcTopicData GetNpcTopic(int id)
        {
            NpcTopicData npcText = null;
            if (NpcTopicMap.ContainsKey(id))
            {
                npcText = NpcTopicMap[id];
            }

            return npcText;
        }
    }
}
