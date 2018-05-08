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
        Dictionary<int, NpcGreetData> m_NpcGreetMap = new Dictionary<int, NpcGreetData>();

        override public bool ProcessCsvDataRow(int row, string[] columns)
        {
            // ID ; Greeting 1 ; Greeting 2 ; Note ; Owner
            int id;
            if (int.TryParse(columns[0], out id))
            {
                m_NpcGreetMap.Add(id, new NpcGreetData(id, columns[1], columns[2], columns[3], columns[4]));
            }

            return m_NpcGreetMap.Count > 0;
        }

        public NpcGreetData GetNpcGreet(int id)
        {
            NpcGreetData npcGreet = null;
            if (m_NpcGreetMap.ContainsKey(id))
            {
                npcGreet = m_NpcGreetMap[id];
            }

            return npcGreet;
        }
    }
}
