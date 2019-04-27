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
            string spellAnimPrefix = "";
            switch (spellType)
            {
                 
            }
        }

        public static void SetPlayerBuffAnim(string buffAnimPrefix, Character character)
        {
            Sprite[] allSprites = UiMgr.Instance.SpriteMap.Values.Where(
                sprite => sprite.name.StartsWith(buffAnimPrefix)).ToArray();
            if (allSprites.Length == 0)
            {
                return;
            }

            SpriteAnimation overlayAnim = character.UI.FaceOverlayAnimation;
            overlayAnim.Loop = false;
            overlayAnim.AnimationTime = 1.0f;
            overlayAnim.AnimationSprites = allSprites;
            overlayAnim.Play();
        }
    }
}
