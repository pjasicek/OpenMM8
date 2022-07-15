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
    // SPRITE_FRAME_TABLE
    public class SpriteObjectData : DbData<string>
    {
        // string Id
        public string Name;                // same as ID
        public List<string> AnimFrameNames = new List<string>(); // Does NOT include 0/1/2/3/4 suffix for rotated sprite
        public float Scale;
        public int LightIntensity;
        public bool IsAlwaysLookingFront;  // Most decals / vegetation only look front - they have ONLY these sprites, they do not have rotated ones
        public float TotalAnimationLengthSeconds; // Sum of @AnimFrameLengths is this
        public List<float> AnimFrameLengths = new List<float>();
        public int X; // Radius ??
        public int Y; // Height ??
        public float Alpha; // Always 0.0f
        public int Glow;    // Unused ??
        public bool IsCentered; // What is the purpose ??
        //public int Flags; // Always 0

        // NOTE: I am not sure if this is "Animation" since not all of these entries are animated - 
        //       they consist only of one sprite
    }

    public class SpriteObjectDb : DataDb<SpriteObjectData, string>
    {
        private SpriteObjectData m_Previous = null;

        override public SpriteObjectData ProcessCsvDataRow(int row, string[] columns)
        {
            if (!string.IsNullOrEmpty(columns[0]) && columns[0].StartsWith("//"))
            {
                return null;
            }

            //Debug.Log("[" + (row + 1).ToString() + "] Processing: " + columns[0]);

            SpriteObjectData data = null;
            bool isNewAnim = !string.IsNullOrEmpty(columns[2]) && columns[2].ToLower() == "new";
            if (isNewAnim)
            {
                string animName = columns[0].ToLower();
                if (Data.ContainsKey(animName))
                {
                    Debug.LogError("[" + (row + 1).ToString() + "]" + "SpriteObjectDb already contains key: " + animName);
                    return null;
                }

                data = new SpriteObjectData();
                data.Id = animName;
                data.Scale = float.Parse(columns[4].Replace('.', ','));
                data.LightIntensity = int.Parse(columns[5]);
                data.IsAlwaysLookingFront = (int.Parse(columns[7]) & 4) == 0; // Flag 0x4
                data.X = int.Parse(columns[8]);
                data.Y = int.Parse(columns[9]);
                data.Alpha = float.Parse(columns[10]);
                data.Glow = int.Parse(columns[11]);
                data.IsCentered = int.Parse(columns[12]) == 1;
            }
            else
            {
                data = m_Previous;
            }

            data.AnimFrameNames.Add(columns[1].ToLower());

            // 1 time unit in this = 62.5 miliseconds (8 * 16 = 128 = 1000ms)
            float frameLength = (int.Parse(columns[6]) * 62.5f) / 1000.0f;
            data.TotalAnimationLengthSeconds += frameLength;
            data.AnimFrameLengths.Add(frameLength);

            if (data.Equals(m_Previous))
            {
                return null;
            }
            else
            {
                m_Previous = data;
                return data;
            }
        }
    }
}
