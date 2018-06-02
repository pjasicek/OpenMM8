using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class CharFaceUpdater
    {
        private Character chr;

        private float m_TimeInOtherAvatar = 0.0f;
        private float m_TimeUntilIdleAvatar = 7.0f;
        private const float m_IdleAvatarDuration = 1.0f;
        private const float m_MinIdleAvatar = 2.0f;
        private const float m_MaxIdleAvatar = 7.0f;

        private float m_AvatarDuration = 0.0f;

        public CharFaceUpdater(Character owner)
        {
            chr = owner;
        }

        public void Reset()
        {
            chr.UI.PlayerCharacter.sprite = chr.UI.Sprites.ConditionToSpriteMap[chr.Data.Condition];
            ResetTimer();
        }

        public void ResetTimer()
        {
            m_TimeInOtherAvatar = 0.0f;
            m_TimeUntilIdleAvatar = 7.0f;
        }

        public void SetAvatar(Sprite sprite, float duration)
        {
            ResetTimer();
            chr.UI.PlayerCharacter.sprite = sprite;
            m_AvatarDuration = duration;
        }

        public void OnFixedUpdate(float secDiff)
        {
            if (chr.UI.PlayerCharacter.sprite != chr.UI.Sprites.ConditionToSpriteMap[chr.Data.Condition])
            {
                m_TimeInOtherAvatar += secDiff;
                if (m_TimeInOtherAvatar > m_AvatarDuration)
                {
                    chr.UI.PlayerCharacter.sprite = chr.UI.Sprites.ConditionToSpriteMap[chr.Data.Condition];
                    m_TimeUntilIdleAvatar = UnityEngine.Random.Range(m_MinIdleAvatar, m_MaxIdleAvatar);
                    m_TimeInOtherAvatar = 0.0f;
                    m_AvatarDuration = m_IdleAvatarDuration;
                }
            }
            else if (chr.Data.Condition == Condition.Good)
            {
                m_TimeUntilIdleAvatar -= secDiff;
                if (m_TimeUntilIdleAvatar < 0.0f)
                {
                    chr.UI.PlayerCharacter.sprite = chr.UI.Sprites.Idle[UnityEngine.Random.Range(0, chr.UI.Sprites.Idle.Count)];
                }
            }
        }
    }
}
