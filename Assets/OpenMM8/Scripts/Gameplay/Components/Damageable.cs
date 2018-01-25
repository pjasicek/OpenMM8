using UnityEngine;
using System.Collections;

using Assets.OpenMM8.Scripts.Gameplay;

public class Damageable : MonoBehaviour
{
    public event DamageReceived OnDamageReceived;
    public event SpellReceived OnSpellReceived;

    public void ReceiveDamage(int amount, GameObject source)
    {
        if (OnDamageReceived != null)
        {
            OnDamageReceived(amount, source);
        }
    }

    public void ReceiveSpell(Spell spell, GameObject source)
    {
        if (OnSpellReceived != null)
        {
            OnSpellReceived(spell, source);
        }
    }
}
