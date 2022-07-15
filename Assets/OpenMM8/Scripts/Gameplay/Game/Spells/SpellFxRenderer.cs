using Assets.OpenMM8.Scripts.Gameplay.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class SpellFxRenderer
    {
        public static void SetPlayerBuffAnim(SpellType spellType, Character character)
        {
            // This is key to the InterfaceAnimDb.Data
            string animName = "placeholder";
            switch (spellType)
            {
                case SpellType.Misc_Disease:
                    animName = "zapp";
                    break;

                case SpellType.Air_FeatherFall:
                case SpellType.Spirit_DetectLife:
                case SpellType.Spirit_Fate:
                    animName = "spboost1";
                    break;

                case SpellType.Misc_QuestCompleted:
                case SpellType.Air_Invisibility:
                case SpellType.Water_WaterWalk:
                case SpellType.Spirit_Preservation:
                    animName = "spboost2";
                    break;

                case SpellType.Light_HourOfPower:
                case SpellType.Light_DayOfTheGods:
                case SpellType.Light_DayOfProtection:
                case SpellType.Light_DivineIntervention:
                    animName = "spboost3";
                    break;

                case SpellType.Spirit_RemoveCurse:
                case SpellType.Mind_RemoveFear:
                case SpellType.Body_CureWeakness:
                    animName = "spheal1";
                    break;

                case SpellType.Spirit_SharedLife:
                case SpellType.Mind_CureParalysis:
                case SpellType.Mind_CureInsanity:
                case SpellType.Body_FirstAid:
                case SpellType.Body_CurePoison:
                case SpellType.Body_CureDisease:
                    animName = "spheal2";
                    break;

                case SpellType.Body_PowerCure:
                case SpellType.Dark_Souldrinker:
                    animName = "spheal3";
                    break;

                case SpellType.Fire_ProtectionFromFire:
                case SpellType.Fire_Immolation:
                    animName = "spell03";
                    break;

                case SpellType.Fire_Haste:
                    animName = "spell105";
                    break;

                case SpellType.Air_ProtectionFromAir:
                    animName = "spell14";
                    break;

                case SpellType.Air_Shield:
                    animName = "spell17";
                    break;

                case SpellType.Water_ProtectionFromWater:
                    animName = "spell25";
                    break;

                case SpellType.Earth_ProtectionFromEarth:
                    animName = "spell36";
                    break;

                case SpellType.Earth_Stoneskin:
                    animName = "spell38";
                    break;

                case SpellType.Spirit_Bless:
                    animName = "spell46";
                    break;

                case SpellType.Spirit_Heroism:
                    animName = "spell51";
                    break;

                case SpellType.Spirit_Ressurection:
                    animName = "spell55";
                    break;

                case SpellType.Mind_ProtectionFromMind:
                    animName = "spell58";
                    break;

                case SpellType.Body_ProtectionFromBody:
                    animName = "spell69";
                    break;

                case SpellType.Body_Regeneration:
                    animName = "spell71";
                    break;

                case SpellType.Body_Hammerhands:
                    animName = "spell73";
                    break;

                case SpellType.Body_ProtectionFromMagic:
                    animName = "spell75";
                    break;

                default:
                    Debug.LogError("No spell fx animation for: " + spellType);
                    break;
            }

            InterfaceAnimData animData = DbMgr.Instance.InterfaceAnimDb.Get(animName);
            if (animData == null)
            {
                Debug.LogError("No anim data for: " + animName);
                return;
            }
            if (animData.AnimFrameNames.Count == 0)
            {
                Debug.LogError("Empty anim frames for: " + animData.Id);
                return;
            }

            List<Sprite> animSprites = new List<Sprite>();
            foreach (string spriteName in animData.AnimFrameNames)
            {
                Sprite sprite;
                if (!UiMgr.Instance.SpriteMap.TryGetValue(spriteName, out sprite))
                {
                    Debug.LogError("Failed to get sprite: " + spriteName);
                    return;
                }

                animSprites.Add(sprite);
            }

            SpriteAnimation overlayAnim = character.UI.FaceOverlayAnimation;
            overlayAnim.Loop = false;
            overlayAnim.AnimationTime = animData.TotalAnimationLengthSeconds;
            overlayAnim.AnimationSprites = animSprites.ToArray();
            overlayAnim.Play();
        }
    }
}
