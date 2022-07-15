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
    // DOLL_TYPES
    public class DollTypeData : DbData
    {
        public bool CanEquipBow;
        public bool CanEquipArmor;
        public bool CanEquipHelm;
        public bool CanEquipBelt;
        public bool CanEquipBoots;
        public bool CanEquipCloak;
        public bool CanEquipWeapon;

        public Vector2Int RH_OpenPos;
        public Vector2Int RH_ClosedPos;
        public Vector2Int RH_FingersPos;
        public Vector2Int LH_OpenPos;
        public Vector2Int LH_ClosedPos;
        public Vector2Int LH_FingersPos;
        public Vector2Int OH_Offset;
        public Vector2Int MH_Offset;
        public Vector2Int BowOffset;
        public Vector2Int ShieldPos;
    }

    public class DollTypeDb : DataDb<DollTypeData>
    {
        override public DollTypeData ProcessCsvDataRow(int row, string[] columns)
        {
            if (row == 0)
            {
                return null;
            }

            DollTypeData data = new DollTypeData();
            data.Id = int.Parse(columns[0]);

            data.CanEquipBow = columns[1] == "x";
            data.CanEquipArmor = columns[2] == "x";
            data.CanEquipHelm = columns[3] == "x";
            data.CanEquipBelt = columns[4] == "x";
            data.CanEquipBoots = columns[5] == "x";
            data.CanEquipCloak = columns[6] == "x";
            data.CanEquipWeapon = columns[7] == "x";

            data.RH_OpenPos = new Vector2Int(int.Parse(columns[8]), -1 * int.Parse(columns[9]));
            data.RH_ClosedPos = new Vector2Int(int.Parse(columns[10]), -1 * int.Parse(columns[11]));
            data.RH_FingersPos = new Vector2Int(int.Parse(columns[12]), -1 * int.Parse(columns[13]));
            data.LH_OpenPos = new Vector2Int(int.Parse(columns[14]), -1 * int.Parse(columns[15]));
            data.LH_ClosedPos = new Vector2Int(int.Parse(columns[16]), -1 * int.Parse(columns[17]));
            data.LH_FingersPos = new Vector2Int(int.Parse(columns[18]), -1 * int.Parse(columns[19]));
            data.OH_Offset = new Vector2Int(int.Parse(columns[20]), -1 * int.Parse(columns[21]));
            data.MH_Offset = new Vector2Int(int.Parse(columns[22]), -1 * int.Parse(columns[23]));
            data.BowOffset = new Vector2Int(int.Parse(columns[24]), -1 * int.Parse(columns[25]));
            data.ShieldPos = new Vector2Int(int.Parse(columns[26]), -1 * int.Parse(columns[27]));

            return data;
        }
    }
}
