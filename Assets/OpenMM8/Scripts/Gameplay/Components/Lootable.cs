using UnityEngine;
using System.Collections;

using Assets.OpenMM8.Scripts.Gameplay;

public class Lootable : Interactable
{
    public Loot Loot;

    protected override bool Interact(GameObject interacter, RaycastHit interactRay)
    {
        if (Loot == null)
        {
            return true;
        }

        if (interacter.CompareTag("Player"))
        {
            interacter.GetComponent<PlayerParty>().OnAcquiredLoot(Loot);
        }

        Destroy(this.gameObject);

        return true;
    }
}
