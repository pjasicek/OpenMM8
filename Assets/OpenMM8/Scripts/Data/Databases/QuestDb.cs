using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    public class QuestDb : DataDb<QuestData>
    {
        override public QuestData ProcessCsvDataRow(int row, string[] columns)
        {
            // ID ; Text ; Note ; Owner ; Unknown
            int id;
            if (int.TryParse(columns[0], out id))
            {
                QuestData questData = new QuestData();
                questData.Id = id;
                questData.QuestNote = columns[1];

                return questData;
            }

            return null;
        }
    }
}
