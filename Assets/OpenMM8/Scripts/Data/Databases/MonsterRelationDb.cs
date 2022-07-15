using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    // MONSTER_RELATION_DATA
    public class MonsterRelationData : DbData
    {
        // int Id - (MonsterData.Id - 1) / 3 + 1    - Monster have 3 types, indexes for monster start from id 1
        // If Id == 0 then it is relation table for player

        // OtherMonsterId -> Relation
        // 0 = Friendly
        // 4 = Hostile
        // Anything in between is probably group
        public Dictionary<int, int> RelationMap = new Dictionary<int, int>();
    }

    public class MonsterRelationDb : DataDb<MonsterRelationData>
    {
        override public MonsterRelationData ProcessCsvDataRow(int row, string[] columns)
        {
            if (row == 0)
            {
                return null;
            }

            MonsterRelationData data = new MonsterRelationData();
            data.Id = row - 1;

            for (int otherRelationId = 1; otherRelationId < columns.Length; otherRelationId++)
            {
                if (string.IsNullOrEmpty(columns[otherRelationId]))
                {
                    continue;
                }
                data.RelationMap.Add(otherRelationId - 1, int.Parse(columns[otherRelationId]));
            }

            return data;
        }

        public static int GetRelation(int id1, int id2)
        {
            MonsterRelationData srcMonster = DbMgr.Instance.MonsterRelationDb.Get(id1);
            return srcMonster.RelationMap[id2];
        }
    }
}
