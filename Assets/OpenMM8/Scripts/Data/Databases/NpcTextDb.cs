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
    public class NpcTextDb : DataDb
    {
        Dictionary<int, NpcText> NpcTextMap = new Dictionary<int, NpcText>();

        override public bool ProcessCsvDataRow(int row, string[] columns)
        {
            // ID ; Text ; Note ; Owner ; Unknown
            int id;
            if (int.TryParse(columns[0], out id))
            {
                NpcText npcText = new NpcText();
                npcText.Id = id;
                npcText.Text = columns[1];
                npcText.Note = columns[2];
                npcText.Owner = columns[3];
                npcText.Owner = columns[4];

                NpcTextMap.Add(id, npcText);
            }

            return NpcTextMap.Count > 0;
        }

        public NpcText GetNpcText(int id)
        {
            NpcText npcText = null;
            if (NpcTextMap.ContainsKey(id))
            {
                npcText = NpcTextMap[id];
            }

            return npcText;
        }
    }
}
