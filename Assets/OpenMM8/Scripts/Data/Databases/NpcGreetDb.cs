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
    public class NpcGreetDb : DataDb
    {
        Dictionary<int, NpcGreet> NpcGreetMap = new Dictionary<int, NpcGreet>();

        override public bool ProcessCsvDataRow(int row, string[] columns)
        {
            // ID ; Greeting 1 ; Greeting 2 ; Note ; Owner
            int id;
            if (int.TryParse(columns[0], out id))
            {
                NpcGreetMap.Add(id, new NpcGreet(id, columns[1], columns[2], columns[3], columns[4]));
            }

            return NpcGreetMap.Count > 0;
        }

        public NpcGreet GetNpcGreet(int id)
        {
            NpcGreet npcGreet = null;
            if (NpcGreetMap.ContainsKey(id))
            {
                npcGreet = NpcGreetMap[id];
            }

            return npcGreet;
        }
    }
}
