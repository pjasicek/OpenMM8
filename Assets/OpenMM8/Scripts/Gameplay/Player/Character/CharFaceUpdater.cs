using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class CharFaceUpdater
    {
        private Character chr;

        private float TimeInOtherAvatar = 0.0f;
        private float MinIdleAvatar = 2.0f;
        private float MaxIdleAvatar = 7.0f;
        private float TimeUntilIdleAvatar = 7.0f;

        public CharFaceUpdater(Character owner)
        {
            chr = owner;
        }

        public void OnFixedUpdate(float secDiff)
        {
            if (chr.UI.PlayerCharacter.sprite != chr.Sprites.ConditionToSpriteMap[chr.Data.Condition])
            {
                TimeInOtherAvatar += secDiff;
                if (TimeInOtherAvatar > 1.0f)
                {
                    chr.UI.PlayerCharacter.sprite = chr.Sprites.ConditionToSpriteMap[chr.Data.Condition];
                    TimeUntilIdleAvatar = UnityEngine.Random.Range(MinIdleAvatar, MaxIdleAvatar);
                    TimeInOtherAvatar = 0.0f;
                }
            }
            else if (chr.Data.Condition == Condition.Good)
            {
                TimeUntilIdleAvatar -= secDiff;
                if (TimeUntilIdleAvatar < 0.0f)
                {
                    chr.UI.PlayerCharacter.sprite = chr.Sprites.Idle[UnityEngine.Random.Range(0, chr.Sprites.Idle.Count)];
                }
            }
        }
    }
}
