using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class ObjectDisplayData : DbData
    {
        public string ObjectName;
        public int Radius;
        public int Height;
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
        public string SpriteName;
        public int Lifetime;
        public int Speed;
        public int ParticleRed;
        public int ParticleGreen;
        public int ParticleBlue;
        public int ParticleAlpha;

        // Loaded in UiMgr init
        public Sprite DisplaySprite;
    }
}
