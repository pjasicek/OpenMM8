using UnityEngine;
using System.Collections;

using Assets.OpenMM8.Scripts.Gameplay;
using Assets.OpenMM8.Scripts.Gameplay.Items;

[RequireComponent(typeof(BaseItem))]
public class InspectableItem : Inspectable
{
    private ItemData ItemData;

    // Use this for initialization
    void Start()
    {
        ItemData = GetComponent<BaseItem>().ItemData;
    }

    public override void StartInspect(Character inspector)
    {
        throw new System.NotImplementedException();
    }

    public override void EndInspect(Character inspector)
    {
        throw new System.NotImplementedException();
    }
}
