using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    // General data container.. shared among all buffs
    public class SpellEffect
    {
        // public SpellEffectType SpellEffectType;
        public SkillMastery SkillMastery;
        public int Power;
        public GameTime ExpiryTime;
        public Character Caster;
        public int Flags;

        bool Apply(SkillMastery skillMastery, int power, GameTime expiryTime, Character caster = null, int flags = 0)
        {
            if (expiryTime.TotalMinutes() <= TimeMgr.Instance.GetCurrentTime().TotalMinutes())
            {
                Debug.LogError("Attempted to cast already expired spell");
                return false;
            }

            SkillMastery = skillMastery;
            Power = power;
            ExpiryTime = expiryTime;
            Caster = caster;
            Flags = flags;

            return true;
        }

        void Reset()
        {
            SkillMastery = SkillMastery.None;
            Power = 0;
            ExpiryTime = null;
            Caster = null;
            Flags = 0;
        }

        bool IsActive()
        {
            if (ExpiryTime == null)
            {
                return false;
            }

            return ExpiryTime.TotalMinutes() > TimeMgr.Instance.GetCurrentTime().TotalMinutes();
        }

        bool IsExpired()
        {
            if (ExpiryTime == null)
            {
                return true;
            }

            return ExpiryTime.TotalMinutes() <= TimeMgr.Instance.GetCurrentTime().TotalMinutes();
        }
    }
}
