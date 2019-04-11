using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    // Thanks to: https://grayface.github.io/mm/mechanics/
    static public class GameMechanics
    {
        static public AttackResult DamageFromPlayerToNpc(AttackInfo hitInfo, 
            Dictionary<SpellElement, int> npcResistances, 
            int npcArmorClass)
        {
            AttackResult result = new AttackResult();

            float chanceBeingHit = (float)(15 + hitInfo.AttackMod * 2) / (float)(30 + hitInfo.AttackMod * 2 + npcArmorClass);
            if (UnityEngine.Random.Range(0.0f, 1.0f) > chanceBeingHit)
            {
                result.Type = AttackResultType.Miss;
                result.DamageDealt = 0;
                return result;
            }
            else
            {
                result.Type = AttackResultType.Hit;
            }

            //float damageDealt = (Gaussian.Random() * (hitInfo.MaxDamage - hitInfo.MinDamage)) + hitInfo.MinDamage;
            float damageDealt = Gaussian.RandomRange(hitInfo.MinDamage, hitInfo.MaxDamage);

            // Apply resistances
            int attackResistance = npcResistances[hitInfo.DamageType];
            damageDealt *= GetResistanceReductionCoeff(attackResistance);

            result.DamageDealt = Mathf.RoundToInt(damageDealt);

            return result;
        }

        static public AttackResult DamageFromNpcToPlayer(AttackInfo hitInfo, 
            Dictionary<SpellElement, int> playerResistances, 
            int playerArmorClass, 
            int playerLuck)
        {
            AttackResult result = new AttackResult();

            float chanceBeingHit = (float)(5 + hitInfo.SourceLevel * 2) / (float)(10 + hitInfo.SourceLevel * 2 + playerArmorClass);
            if (UnityEngine.Random.Range(0.0f, 1.0f) > chanceBeingHit)
            {
                result.Type = AttackResultType.Miss;
                result.DamageDealt = 0;
                return result;
            }
            else
            {
                result.Type = AttackResultType.Hit;
            }

            float damageDealt = Gaussian.RandomRange(hitInfo.MinDamage, hitInfo.MaxDamage);// (Gaussian.Random() * (hitInfo.MaxDamage - hitInfo.MinDamage)) + hitInfo.MinDamage;

            // Apply resistances
            int attackResistance = playerResistances[hitInfo.DamageType];
            damageDealt *= GetResistanceReductionCoeff(attackResistance + playerLuck);

            result.DamageDealt = Mathf.RoundToInt(damageDealt);

            return result;
        }

        static public float GetResistanceReductionCoeff(int resistanceAmount)
        {
            if (resistanceAmount >= 300)
            {
                return 0.2f;
            }
            else if (resistanceAmount >= 200)
            {
                return 0.26f;
            }
            else if (resistanceAmount >= 150)
            {
                return 0.31f;
            }
            else if (resistanceAmount >= 100)
            {
                return 0.39f;
            }
            else if (resistanceAmount >= 60)
            {
                return 0.5f;
            }
            else if (resistanceAmount >= 40)
            {
                return 0.6f;
            }
            else if (resistanceAmount >= 20)
            {
                return 0.75f;
            }
            else
            {
                return 1.0f;
            }
        }

        static public int GetAttributeEffect(int attributeAmount)
        {
            if (attributeAmount >= 500)
            {
                return 30;
            }
            else if (attributeAmount >= 400)
            {
                return 25;
            }
            else if (attributeAmount >= 350)
            {
                return 20;
            }
            else if (attributeAmount >= 300)
            {
                return 19;
            }
            else if (attributeAmount >= 275)
            {
                return 18;
            }
            else if (attributeAmount >= 250)
            {
                return 17;
            }
            else if (attributeAmount >= 225)
            {
                return 16;
            }
            else if (attributeAmount >= 200)
            {
                return 15;
            }
            else if (attributeAmount >= 175)
            {
                return 14;
            }
            else if (attributeAmount >= 150)
            {
                return 13;
            }
            else if (attributeAmount >= 125)
            {
                return 12;
            }
            else if (attributeAmount >= 100)
            {
                return 11;
            }
            else if (attributeAmount >= 75)
            {
                return 10;
            }
            else if (attributeAmount >= 50)
            {
                return 9;
            }
            else if (attributeAmount >= 40)
            {
                return 8;
            }
            else if (attributeAmount >= 35)
            {
                return 7;
            }
            else if (attributeAmount >= 30)
            {
                return 6;
            }
            else if (attributeAmount >= 25)
            {
                return 5;
            }
            else if (attributeAmount >= 21)
            {
                return 4;
            }
            else if (attributeAmount >= 19)
            {
                return 3;
            }
            else if (attributeAmount >= 17)
            {
                return 2;
            }
            else if (attributeAmount >= 15)
            {
                return 1;
            }
            else if (attributeAmount >= 13)
            {
                return 0;
            }
            else if (attributeAmount >= 11)
            {
                return -1;
            }
            else if (attributeAmount >= 9)
            {
                return -2;
            }
            else if (attributeAmount >= 7)
            {
                return -3;
            }
            else if (attributeAmount >= 5)
            {
                return -4;
            }
            else if (attributeAmount >= 3)
            {
                return -5;
            }
            else
            {
                return -6;
            }
        }

        static public int GetTotalExperienceRequired(int level)
        {
            return level * (level + 1) * 500;
        }

        // e.g. how many xp is required from 10 to 11
        static public int GetExperienceRequired(int toLevel)
        {
            return toLevel * 1000;
        }
    }
}
