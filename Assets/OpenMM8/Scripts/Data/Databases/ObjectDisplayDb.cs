using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
    // Outdoor items + spells
    public class ObjectDisplayData : DbData
    {
        public string ObjectName;
        public float Radius;
        public float Height;
        public bool IsInvisible;
        public bool IsUntouchable;
        public bool IsTemporary;
        public bool IsLifetimeInSFT;
        public bool IsNoPickup;
        public bool IsNoGravity;
        public bool IsInterceptAction;
        public bool IsBouncing;
        public bool IsTrailParticle;
        public bool IsTrailFire;
        public bool IsTrailLine;
        public string SFTLabel;
        public float Lifetime;
        public float Speed;
        public int ParticleRed;
        public int ParticleGreen;
        public int ParticleBlue;
        public int ParticleAlpha;
    }

    public class ObjectDisplayDb : DataDb<ObjectDisplayData>
    {
        override public ObjectDisplayData ProcessCsvDataRow(int row, string[] columns)
        {
            if (row < 1)
            {
                return null;
            }

            if (columns[0].Trim().Equals(""))
            {
                return null;
            }

            if (columns[0].StartsWith("//"))
            {
                return null;
            }

            ObjectDisplayData data = new ObjectDisplayData();
            //Logger.LogDebug("Id: " + columns[1]);
            data.Id = int.Parse(columns[2]);

            data.ObjectName = columns[0];
            data.SFTLabel = columns[1];
            data.Radius = GameMechanics.ConvertToUnitySize(int.Parse(columns[3]));
            data.Height = GameMechanics.ConvertToUnitySize(int.Parse(columns[4]));

            data.Lifetime = int.Parse(columns[5]) / 128.0f;
            data.Speed = GameMechanics.ConvertToUnitySpeed(int.Parse(columns[6]));

            /*data.Id = int.Parse(columns[1]);
            data.ObjectName = columns[0];
            data.Radius = int.Parse(columns[2]);
            data.Height = int.Parse(columns[3]);

            data.IsInvisible = columns[4] == "x";
            data.IsUntouchable = columns[5] == "x";
            data.IsTemporary = columns[6] == "x";
            data.IsLifetimeInSFT = columns[7] == "x";
            data.IsNoPickup = columns[8] == "x";
            data.IsNoGravity = columns[9] == "x";
            data.IsInterceptAction = columns[10] == "x";
            data.IsBouncing = columns[11] == "x";
            data.IsTrailParticle = columns[12] == "x";
            data.IsTrailFire = columns[13] == "x";
            data.IsTrailLine = columns[14] == "x";

            data.SFTLabel = columns[15];
            data.Lifetime = int.Parse(columns[16]);
            data.Speed = int.Parse(columns[17]);
            data.ParticleRed = int.Parse(columns[18]);
            data.ParticleGreen = int.Parse(columns[19]);
            data.ParticleBlue = int.Parse(columns[20]);
            data.ParticleAlpha = int.Parse(columns[21]);*/

            return data;
        }
    }
}
