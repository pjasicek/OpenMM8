using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class CharacterSprites
    {
        public List<Sprite> TakeDamage = new List<Sprite>();
        public List<Sprite> Idle = new List<Sprite>();
        public List<Sprite> Smile = new List<Sprite>();
        //public Sprite Talk; // Should be animation but way too much effort now
        public Sprite Yell;
        public Dictionary<Condition, Sprite> ConditionToSpriteMap = new Dictionary<Condition, Sprite>();

        public static CharacterSprites Load(CharacterType type)
        {
            CharacterSprites characterSprites = new CharacterSprites();

            string path = "Player/PlayerCharacters/Sprites/PC_" + ((int)type).ToString();
            Sprite[] sprites = Resources.LoadAll<Sprite>(path);

            path = "Player/PlayerCharacters/Sprites/PC_Common";
            Sprite[] commonSprites = Resources.LoadAll<Sprite>(path);

            // 01 - Good
            // 03 - Weak
            // 04 - Sleep
            // 05 - Fear
            // 06 - Drunk
            // 07 - Insane
            // 08 - Poisoned
            // 09 - Diseased
            // 10 - Paralyzed
            // 11 - Unconscious
            // 12 - Stoned

            // 13-20 - Idle
            // 23 - Fail (attack miss ?)

            // 37, 38, 39 - Take damage
            // 40 - Smile
            // 50 - Big Smile
            // 53 - Yell

            characterSprites.TakeDamage.Add(sprites[36]);
            characterSprites.TakeDamage.Add(sprites[37]);
            characterSprites.TakeDamage.Add(sprites[38]);

            characterSprites.Idle.Add(sprites[12]);
            characterSprites.Idle.Add(sprites[13]);
            characterSprites.Idle.Add(sprites[14]);
            characterSprites.Idle.Add(sprites[15]);
            characterSprites.Idle.Add(sprites[16]);
            characterSprites.Idle.Add(sprites[17]);
            characterSprites.Idle.Add(sprites[18]);
            characterSprites.Idle.Add(sprites[19]);

            characterSprites.Smile.Add(sprites[39]);
            characterSprites.Smile.Add(sprites[49]);

            characterSprites.Yell = sprites[52];

            characterSprites.ConditionToSpriteMap[Condition.Good] = sprites[0];
            characterSprites.ConditionToSpriteMap[Condition.Weak] = sprites[1];
            characterSprites.ConditionToSpriteMap[Condition.Insane] = sprites[6];
            characterSprites.ConditionToSpriteMap[Condition.Poisoned] = sprites[7];
            characterSprites.ConditionToSpriteMap[Condition.Diseased] = sprites[8];
            characterSprites.ConditionToSpriteMap[Condition.Unconsious] = sprites[10];
            characterSprites.ConditionToSpriteMap[Condition.Dead] = commonSprites[0];
            characterSprites.ConditionToSpriteMap[Condition.Eradicated] = commonSprites[1];
            characterSprites.ConditionToSpriteMap[Condition.Stoned] = sprites[11];
            characterSprites.ConditionToSpriteMap[Condition.Paralyzed] = sprites[9];
            characterSprites.ConditionToSpriteMap[Condition.Sleeping] = sprites[3];
            characterSprites.ConditionToSpriteMap[Condition.Drunk] = sprites[5];

            return characterSprites;
        }
    }
}
