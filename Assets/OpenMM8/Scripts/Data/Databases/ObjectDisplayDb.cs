using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Assets.OpenMM8.Scripts.Gameplay.Items;
using Assets.OpenMM8.Scripts.Data;

namespace Assets.OpenMM8.Scripts.Gameplay.Data
{
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
            data.SpriteName = columns[1];
            data.Radius = int.Parse(columns[3]);
            data.Height = int.Parse(columns[4]);

            data.Lifetime = int.Parse(columns[5]);
            data.Speed = int.Parse(columns[6]);

            /*data.IsInvisible = columns[4] == "x";
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

            data.SpriteName = columns[15];
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
