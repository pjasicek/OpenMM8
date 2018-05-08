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
    public class QuestDb : DataDb
    {
        public Dictionary<int, QuestData> QuestDataMap = new Dictionary<int, QuestData>();

        override public bool ProcessCsvDataRow(int row, string[] columns)
        {
            // ID ; Text ; Note ; Owner ; Unknown
            int id;
            if (int.TryParse(columns[0], out id))
            {
                QuestData questData = new QuestData();
                questData.Id = id;
                questData.QuestNote = columns[1];

                QuestDataMap.Add(id, questData);
            }

            return true;
        }

        public QuestData GetQuestData(int id)
        {
            QuestData npcText = null;
            if (QuestDataMap.ContainsKey(id))
            {
                npcText = QuestDataMap[id];
            }

            return npcText;
        }
    }
}
