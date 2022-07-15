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
    // MONSTER_LIST
    public class MonsterObjectData : DbData
    {
        // int Id
        public string Name;
        public int Height;
        public int Radius;
        public int Speed;
        public int Radius2; // UNUSED ?

        public string StandFramesName;
        public string WalkFramesName;
        public string AttackFramesName;
        public string ShootFramesName;
        public string StunFramesName;
        public string DieFramesName;
        public string DeadFramesName;
        public string FidgetFramesName;

        public int AttackSoundId;
        public int DieSoundId;
        public int GetHitSoundId;
        public int FidgetSoundId;
    }

    public class MonsterObjectDb : DataDb<MonsterObjectData>
    {
        override public MonsterObjectData ProcessCsvDataRow(int row, string[] columns)
        {
            if (row == 0)
            {
                return null;
            }

            int id;
            if (!int.TryParse(columns[0], out id))
            {
                return null;
            }

            MonsterObjectData data = new MonsterObjectData();
            data.Id = id + 1;

            data.Height = int.Parse(columns[1]);
            data.Radius = int.Parse(columns[2]);
            data.Speed = int.Parse(columns[3]);
            data.Radius2 = int.Parse(columns[4]);

            data.AttackSoundId = int.Parse(columns[9]);
            data.DieSoundId = int.Parse(columns[10]);
            data.GetHitSoundId = int.Parse(columns[11]);
            data.FidgetSoundId = int.Parse(columns[12]);

            data.Name = columns[13];

            data.StandFramesName = columns[14];
            data.WalkFramesName = columns[15];
            data.AttackFramesName = columns[16];
            data.ShootFramesName = columns[17];
            data.StunFramesName = columns[18];
            data.DieFramesName = columns[19];
            data.DeadFramesName = columns[20];
            data.FidgetFramesName = columns[21];

            return data;
        }
    }
}
