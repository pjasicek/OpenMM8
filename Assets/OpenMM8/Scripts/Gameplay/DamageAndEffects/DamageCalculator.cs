using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    static public class DamageCalculator
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
    }
}
