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
            OnAttackReceieved(hitInfo, source);
        }

        return AttackResult.None;
    }

    public SpellResult ReceiveSpell(SpellInfo hitInfo, GameObject source)
    {
        if (OnSpellReceived != null)
        {
            OnSpellReceived(hitInfo, source);
        }

        return SpellResult.None;
    }
}
