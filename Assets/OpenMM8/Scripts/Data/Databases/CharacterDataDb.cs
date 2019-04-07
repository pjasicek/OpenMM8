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
    // CHARACTER_DATA
    public class CharacterData : DbData
    {
        // int Id
        public CharacterRace Race;
        public int DollId;
        public int DefaultClass;
        public int DefaultVoice;
        public int DefaultSex;
        public bool IsAvailable;
        public Vector2Int DollBodyPos;
        public string Background;
        public string Body;
        public string Head; // Unused
        public string LHd;
        public string LHu;
        public string LHo;
        public string RHb;
        public string RHd;
        public string RHu;
        public string FacePicturesPrefix;
    }

    public class CharacterDataDb : DataDb<CharacterData>
    {
        override public CharacterData ProcessCsvDataRow(int row, string[] columns)
        {
            if (row == 0)
            {
                return null;
            }

            CharacterData data = new CharacterData();
            data.Id = int.Parse(columns[0]);

            data.DollId = int.Parse(columns[1]);
            data.DefaultClass = int.Parse(columns[2]);
            data.DefaultVoice = int.Parse(columns[3]);
            data.DefaultSex = int.Parse(columns[4]);
            data.IsAvailable = columns[5] == "x";
            data.DollBodyPos = new Vector2Int(int.Parse(columns[6]), -1 * int.Parse(columns[7]));
            data.Background = columns[8].ToLower();
            data.Body = columns[9].ToLower();
            data.Head = columns[10].ToLower();
            data.LHd = columns[11].ToLower();
            data.LHu = columns[12].ToLower();
            data.LHo = columns[13].ToLower();
            data.RHb = columns[14].ToLower();
            // Unk
            // unk
            data.RHd = columns[17].ToLower();
            data.RHu = columns[18].ToLower();
            data.FacePicturesPrefix = columns[19];
            data.Race = (CharacterRace)int.Parse(columns[23]);

            return data;
        }
    }
}
