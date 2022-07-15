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
    public class NpcTopicDb : DataDb<NpcTopicData>
    {
        override public NpcTopicData ProcessCsvDataRow(int row, string[] columns)
        {
            // ID ; Text ; Note ; Owner ; Unknown
            int id;
            if (int.TryParse(columns[0], out id))
            {
                NpcTopicData npcTopic = new NpcTopicData();
                npcTopic.Id = id;
                npcTopic.Topic = columns[1];
                int.TryParse(columns[4], out npcTopic.TextId);

                if (npcTopic.Topic == "Roster Join Event")
                {
                    npcTopic.Topic = "Join";
                }

                return npcTopic;
            }

            return null;
        }
    }
}
