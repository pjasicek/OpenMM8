using Assets.OpenMM8.Scripts.Gameplay.Data;
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
        private CharacterFaceExpressionDb m_ExpressionDb;

        public CharFaceUpdater(Character owner)
        {
            chr = owner;
            m_ExpressionDb = DbMgr.Instance.CharacterFaceExpressionDb;
        }

        // Party.cpp:628
        public void OnFixedUpdate(float secDiff)
        {
            CharacterExpressionData expressionData = null;

            chr.CurrExpressionTimePassed += secDiff;
        
            Condition worstCondition = chr.GetWorstCondition();
            if (worstCondition == Condition.Good || worstCondition == Condition.Zombie)
            {
                bool isExpressionAnimFinished = chr.CurrExpressionTimePassed >= chr.CurrExpressionTimeLength;
                if (!isExpressionAnimFinished)
                {
                    RenderAvatar();
                    return;
                }

                chr.CurrExpressionTimePassed = 0.0f;

                if (chr.CurrExpression != CharacterExpression.Good || UnityEngine.Random.Range(0, 5) > 0)
                {
                    chr.CurrExpression = CharacterExpression.Good;
                    // 0.25 - 2.25 seconds of still face
                    chr.CurrExpressionTimeLength = UnityEngine.Random.Range(0.0f, 2.0f) + 0.25f;
                }
                else
                {
                    int rnd = UnityEngine.Random.Range(0, 100);
                    if (rnd < 25)
                        chr.CurrExpression = CharacterExpression.Idle_1;
                    else if (rnd < 31)
                        chr.CurrExpression = CharacterExpression.Idle_2;
                    else if (rnd < 37)
                        chr.CurrExpression = CharacterExpression.Idle_3;
                    else if (rnd < 43)
                        chr.CurrExpression = CharacterExpression.Idle_4;
                    else if (rnd < 46)
                        chr.CurrExpression = CharacterExpression.Idle_5;
                    else if (rnd < 52)
                        chr.CurrExpression = CharacterExpression.Idle_6;
                    else if (rnd < 58)
                        chr.CurrExpression = CharacterExpression.Idle_7;
                    else if (rnd < 64)
                        chr.CurrExpression = CharacterExpression.Idle_8;
                    else if (rnd < 70)
                        chr.CurrExpression = CharacterExpression.Idle_11;
                    else if (rnd < 76)
                        chr.CurrExpression = CharacterExpression.Idle_12;
                    else if (rnd < 82)
                        chr.CurrExpression = CharacterExpression.Idle_13;
                    else if (rnd < 88)
                        chr.CurrExpression = CharacterExpression.Idle_14;
                    else if(rnd < 94)
                        chr.CurrExpression = CharacterExpression.Idle_9;
                    else
                        chr.CurrExpression = CharacterExpression.Idle_10;

                    expressionData = DbMgr.Instance.CharacterFaceExpressionDb.Get(chr.CurrExpression);
                    chr.CurrExpressionTimeLength = expressionData.AnimDurationSeconds;
                }
            }
            else if (chr.CurrExpression != CharacterExpression.DamageReceiveMinor &&
                     chr.CurrExpression != CharacterExpression.DamageReceiveModerate &&
                     chr.CurrExpression != CharacterExpression.DamageReceiveMajor ||
                     chr.CurrExpressionTimePassed >= chr.CurrExpressionTimeLength)
            {
                chr.CurrExpressionTimePassed = 0.0f;
                chr.CurrExpressionTimeLength = 0.0f;

                switch (worstCondition)
                {
                    case Condition.Dead:
                        chr.CurrExpression = CharacterExpression.Dead;
                        break;
                    case Condition.Petrified:
                        chr.CurrExpression = CharacterExpression.Petrified;
                        break;
                    case Condition.Eradicated:
                        chr.CurrExpression = CharacterExpression.Eradicated;
                        break;
                    case Condition.Cursed:
                        chr.CurrExpression = CharacterExpression.Cursed;
                        break;
                    case Condition.Weak:
                        chr.CurrExpression = CharacterExpression.Weak;
                        break;
                    case Condition.Sleep:
                        chr.CurrExpression = CharacterExpression.Sleep;
                        break;
                    case Condition.Fear:
                        chr.CurrExpression = CharacterExpression.Fear;
                        break;
                    case Condition.Drunk:
                        chr.CurrExpression = CharacterExpression.Drunk;
                        break;
                    case Condition.Insane:
                        chr.CurrExpression = CharacterExpression.Insane;
                        break;
                    case Condition.PoisonWeak:
                    case Condition.PoisonMedium:
                    case Condition.PoisonSevere:
                        chr.CurrExpression = CharacterExpression.Poisoned;
                        break;
                    case Condition.DiseaseWeak:
                    case Condition.DiseaseMedium:
                    case Condition.DiseaseSevere:
                        chr.CurrExpression = CharacterExpression.Diseased;
                        break;
                    case Condition.Paralyzed:
                        chr.CurrExpression = CharacterExpression.Paralyzed;
                        break;
                    case Condition.Unconcious:
                        chr.CurrExpression = CharacterExpression.Unconcious;
                        break;
                    default:
                        Debug.LogError("Unhandled condition: " + worstCondition);
                        break;
                }
            }

            RenderAvatar();
        }

        private void RenderAvatar()
        {
            string spriteName = "";
            if (chr.CurrExpression == CharacterExpression.Dead || chr.CurrExpression == CharacterExpression.Eradicated)
            {
                // These sprites are common for all characters and they dont have animations / entries in db
                if (chr.CurrExpression == CharacterExpression.Dead)
                {
                    spriteName = "dead";
                }
                else if (chr.CurrExpression == CharacterExpression.Eradicated)
                {
                    spriteName = "eradcate";
                }
            }
            else
            {
                // Update the actual sprites here
                CharacterExpressionData expressionData = DbMgr.Instance.CharacterFaceExpressionDb.Get(chr.CurrExpression);
                int totalFrames = expressionData.AnimSpriteIndexes.Length;
                int currentFrame = 0;
                if (totalFrames > 1 && chr.CurrExpressionTimeLength > 0.0f)
                {
                    currentFrame = (int)((chr.CurrExpressionTimePassed / chr.CurrExpressionTimeLength) * totalFrames);
                }

                int spriteIdx = expressionData.AnimSpriteIndexes[currentFrame];
                spriteName = chr.CharacterData.FacePicturesPrefix + spriteIdx.ToString("00");
            }

            chr.UI.CharacterAvatarImage.sprite = chr.UI.AvatarSpriteMap[spriteName];
        }
    }
}
