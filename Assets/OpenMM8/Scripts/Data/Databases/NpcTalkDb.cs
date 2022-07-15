using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    public class NpcTalkDb : DataDb<NpcTalkData>
    {
        override public NpcTalkData ProcessCsvDataRow(int row, string[] columns)
        {
            // ID ; Text ; Notes
            int id;
            if (int.TryParse(columns[0], out id))
            {
                if (columns[1].Equals("Boob"))
                {
                    return null;
                }

                NpcTalkData data = new NpcTalkData();
                data.Id = id;
                data.Name = columns[1];
                data.PictureId = int.Parse(columns[2]);
                data.GreetId = int.Parse(columns[8]);
                for (int evtIdx = 10; evtIdx <= 15; evtIdx++)
                {
                    int topicId = int.Parse(columns[evtIdx]);
                    if (topicId > 0)
                    {
                        data.TopicList.Add(topicId);
                    }
                }

                return data;
            }

            return null;
        }
    }
}
