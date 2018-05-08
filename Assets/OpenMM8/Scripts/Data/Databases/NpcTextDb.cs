using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data.Databases;
using Assets.OpenMM8.Scripts.Data;
using System.Text.RegularExpressions;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    public class NpcTextDb : DataDb
    {
        private Dictionary<int, NpcTextData> m_NpcTextMap = new Dictionary<int, NpcTextData>();
        private int m_LastId = -1;

        override public bool ProcessCsvDataRow(int row, string[] columns)
        {
            // Multi-lines in CSV => Remove the string letter
            //columns[0] = columns[0].Replace("’", "'");
            columns[0] = Regex.Replace(columns[0], "[^a-zA-Z0-9_.?! ]+", "");

            //columns[0] = Encoding.ASCII.GetString(Encoding.Convert(Encoding.UTF8, Encoding.ASCII, Encoding.UTF8.GetBytes(columns[0])));

            // ID ; Text ; Note ; Owner ; Unknown
            int id;
            if (int.TryParse(columns[0], out id))
            {
                NpcTextData npcText = new NpcTextData();
                npcText.Id = id;
                npcText.Text = columns[1];
                npcText.Note = columns[2];
                npcText.Owner = columns[3];
                npcText.Unknown = columns[4];
                m_NpcTextMap.Add(id, npcText);

                m_LastId = id;
            }
            else
            {
                // This is next part of Multi-line Text
                if (m_LastId != -1)
                {
                    m_NpcTextMap[m_LastId].Text += Environment.NewLine + columns[0];
                    // If this is the last Text line then it contains the other data
                    m_NpcTextMap[m_LastId].Note = columns[1];
                    m_NpcTextMap[m_LastId].Owner = columns[2];
                    m_NpcTextMap[m_LastId].Unknown = columns[3];
                }
            }

            /*if (m_LastId != -1)
            {
                Logger.LogDebug("NpcText: " + m_NpcTextMap[m_LastId].Text);
            }*/

            return true;
        }

        public NpcTextData GetNpcText(int id)
        {
            NpcTextData npcText = null;
            if (m_NpcTextMap.ContainsKey(id))
            {
                npcText = m_NpcTextMap[id];
            }

            return npcText;
        }
    }
}
