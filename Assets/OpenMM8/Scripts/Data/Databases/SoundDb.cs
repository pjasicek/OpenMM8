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
    public enum SoundLoadType
    {
        System, // Always ?
        Swap, // Always ?
        Lock, // ?
        OnDemand // == "0" ?
    }

    // SOUNDS
    public class SoundData : DbData
    {
        // int Id = internal sound ID, referenced from other databases
        public string SoundName; // Sounds/${SoundName}.wav
        public SoundLoadType SoundLoadType;
        public bool Is3D;

        // Unity
        // AudioClip Sound = null;
    }

    public class SoundDb : DataDb<SoundData>
    {
        override public SoundData ProcessCsvDataRow(int row, string[] columns)
        {
            if (row == 0)
            {
                return null;
            }

            int id = int.Parse(columns[1]);
            if (Data.ContainsKey(id))
            {
                Debug.LogWarning("Duplicate ID: " + id + " (" + columns[0] + ")");
                return null;
            }

            SoundData data = new SoundData();
            data.Id = id;

            data.SoundName = columns[0];
            switch (columns[2])
            {
                case "0": data.SoundLoadType = SoundLoadType.OnDemand; break;
                case "system": data.SoundLoadType = SoundLoadType.System; break;
                case "swap": data.SoundLoadType = SoundLoadType.Swap; break;
                case "lock": data.SoundLoadType = SoundLoadType.Lock; break;
                default: data.SoundLoadType = SoundLoadType.Swap; break;
            }

            data.Is3D = columns[3] == "x";

            return data;
        }
    }
}
