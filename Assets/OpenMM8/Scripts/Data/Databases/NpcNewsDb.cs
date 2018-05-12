using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    public class NpcNewsDb : DataDb<NpcNewsData>
    {
        override public NpcNewsData ProcessCsvDataRow(int row, string[] columns)
        {
            // ID ; Text ; Notes
            int id;
            if (int.TryParse(columns[0], out id))
            {
                NpcNewsData npcNews = new NpcNewsData();
                npcNews.Id = id;
                npcNews.Text = columns[1];

                return npcNews;
            }

            return null;
        }
    }
}
