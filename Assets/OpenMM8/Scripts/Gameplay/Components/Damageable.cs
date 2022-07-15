using UnityEngine;
using System.Collections;

using Assets.OpenMM8.Scripts.Gameplay;

public class Damageable : MonoBehaviour
{
    public event AttackReceived OnAttackReceieved;
    public event SpellReceived OnSpellReceived;

    public AttackResult ReceiveAttack(AttackInfo hitInfo, GameObject source)
    {
        if (OnAttackReceieved != null)
        {
            return OnAttackReceieved(hitInfo, source);
        }

        return null;
    }

    public SpellResult ReceiveSpell(SpellInfo hitInfo, GameObject source)
    {
        if (OnSpellReceived != null)
        {
            return OnSpellReceived(hitInfo, source);
        }

        return null;
    }
}
