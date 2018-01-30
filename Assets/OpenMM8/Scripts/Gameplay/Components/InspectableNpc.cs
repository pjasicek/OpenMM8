using UnityEngine;
using System.Collections;

using Assets.OpenMM8.Scripts.Gameplay;

[RequireComponent(typeof(BaseNpc))]
public class InspectableNpc : Inspectable
{
    private NpcData NpcData;

    void Start()
    {
        NpcData = GetComponent<BaseNpc>().NpcData;
    }

    public override Canvas SetupInspectCanvas()
    {
        return null;
    }
}
