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
    public class NpcNewsDb : DataDb
    {
        Dictionary<int, NpcNews> m_NpcNewsMap = new Dictionary<int, NpcNews>();

        override public bool ProcessCsvDataRow(int row, string[] columns)
        {
            // ID ; Text ; Notes
            int id;
            if (int.TryParse(columns[0], out id))
            {
                NpcNews npcNews = new NpcNews();
                npcNews.Id = id;
                npcNews.Text = columns[1];
                npcNews.Notes = columns[2];

                m_NpcNewsMap.Add(id, npcNews);
            }

            return true;
        }

        public NpcNews GetNpcNews(int id)
        {
            NpcNews npcNews = null;
            if (m_NpcNewsMap.ContainsKey(id))
            {
                npcNews = m_NpcNewsMap[id];
            }

            return npcNews;
        }
    }
}
