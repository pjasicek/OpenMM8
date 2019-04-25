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

        // TODO: For the pseudo code, fix later
        private float m_ExpressionTimeLength = 0;
        private float m_ExpressionTimePassed = 0;

        public CharFaceUpdater(Character owner)
        {
            chr = owner;
        }

        public void Reset()
        {
            chr.UI.PlayerCharacter.sprite = chr.UI.Sprites.ConditionToSpriteMap[chr.Condition];
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
            // This is pseudo code for now
            Condition worstCondition = chr.GetWorstCondition();
            if (worstCondition == Condition.Good || worstCondition == Condition.Zombie)
            {
                if (m_ExpressionTimePassed < m_ExpressionTimeLength)
                {
                    return;
                }
            }
            else if (chr.CurrentExpression != CharacterExpression.DamageReceiveMinor &&
                     chr.CurrentExpression != CharacterExpression.DamageReceiveModerate &&
                     chr.CurrentExpression != CharacterExpression.DamageReceiveMajor ||
                     m_ExpressionTimePassed >= m_ExpressionTimeLength)
            {
                m_ExpressionTimePassed = 0.0f;
                m_ExpressionTimeLength = 0.0f;

                switch (worstCondition)
                {
                    case Condition.Cursed:

                        break;
                    case Condition.Weak:
                        break;
                    case Condition.Sleep:
                        break;
                    case Condition.Fear:
                        break;
                    case Condition.Drunk:
                        break;
                    case Condition.Insane:
                        break;
                    case Condition.PoisonWeak:
                        break;
                    case Condition.DiseaseWeak:
                        break;
                    case Condition.PoisonMedium:
                        break;
                    case Condition.DiseaseMedium:
                        break;
                    case Condition.PoisonSevere:
                        break;
                    case Condition.DiseaseSevere:
                        break;
                    case Condition.Paralyzed:
                        break;
                    case Condition.Unconcious:
                        break;
                    case Condition.Dead:
                        break;
                    case Condition.Petrified:
                        break;
                    case Condition.Eradicated:
                        break;
                    case Condition.Zombie:
                        break;
                    case Condition.Good:
                        break;
                    default:
                        break;
                }
            }

            if (chr.UI.PlayerCharacter.sprite != chr.UI.Sprites.ConditionToSpriteMap[chr.Condition])
            {
                m_TimeInOtherAvatar += secDiff;
                if (m_TimeInOtherAvatar > m_AvatarDuration)
                {
                    chr.UI.PlayerCharacter.sprite = chr.UI.Sprites.ConditionToSpriteMap[chr.Condition];
                    m_TimeUntilIdleAvatar = UnityEngine.Random.Range(m_MinIdleAvatar, m_MaxIdleAvatar);
                    m_TimeInOtherAvatar = 0.0f;
                    m_AvatarDuration = m_IdleAvatarDuration;
                }
            }
            else if (chr.Condition == Condition.Good)
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
