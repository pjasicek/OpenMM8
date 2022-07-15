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
    // INTERFACE_FRAME_TABLE
    public class InterfaceAnimData : DbData<string>
    {
        // string Id
        public string Name;                // same as ID
        public List<string> AnimFrameNames = new List<string>();
        public List<float> AnimFrameLengths = new List<float>();
        public float TotalAnimationLengthSeconds;
    }

    public class InterfaceAnimDb : DataDb<InterfaceAnimData, string>
    {
        private InterfaceAnimData m_Previous = null;

        override public InterfaceAnimData ProcessCsvDataRow(int row, string[] columns)
        {
            if (!string.IsNullOrEmpty(columns[0]) && columns[0].StartsWith("//"))
            {
                return null;
            }

            //Debug.Log("[" + (row + 1).ToString() + "] Processing: " + columns[0]);

            InterfaceAnimData data = null;
            bool isNewAnim = !string.IsNullOrEmpty(columns[2]) && columns[2].ToLower() == "new";
            if (isNewAnim)
            {
                string animName = columns[0].ToLower();
                if (Data.ContainsKey(animName))
                {
                    Debug.LogError("[" + (row + 1).ToString() + "]" + "InterfaceAnimDb already contains key: " + animName);
                    return null;
                }

                data = new InterfaceAnimData();
                data.Id = animName;
            }
            else
            {
                data = m_Previous;
            }

            data.AnimFrameNames.Add(columns[1].ToLower());

            // 1 time unit in this = 62.5 miliseconds (8 * 16 = 128 = 1000ms)
            float frameLength = (int.Parse(columns[3]) * 62.5f) / 1000.0f;
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
