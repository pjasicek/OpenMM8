using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    public class NpcGreetDb : DataDb<NpcGreetData>
    {
        override public NpcGreetData ProcessCsvDataRow(int row, string[] columns)
        {
            // ID ; Greeting 1 ; Greeting 2 ; Note ; Owner
            int id;
            if (int.TryParse(columns[0], out id))
            {
                return new NpcGreetData(id, columns[1], columns[2]);
            }

            return null;
        }
    }
}
