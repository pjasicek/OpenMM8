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
    public class NpcTextDb : DataDb<NpcTextData>
    {
        override public NpcTextData ProcessCsvDataRow(int row, string[] columns)
        {
            // ID ; Text ; Note ; Owner ; Unknown
            int id;
            if (int.TryParse(columns[0], out id))
            {
                NpcTextData npcText = new NpcTextData();
                npcText.Id = id;
                npcText.Text = columns[1];
                return npcText;
            }

            return null;
        }
    }
}
