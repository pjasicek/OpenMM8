using UnityEngine;
using System.Collections;

using Assets.OpenMM8.Scripts.Gameplay;

[RequireComponent(typeof(BaseNpc))]
public class InspectableNpc : Inspectable
{
    private MonsterData NpcData = null;
    private BaseNpc Npc = null;

    void Start()
    {
        Npc = GetComponent<BaseNpc>();
    }

    public override void StartInspect(Character inspector)
    {
        if (NpcData == null)
        {
            NpcData = Npc.NpcData;
        }

        GameEvents.InvokeEvent_OnNpcInspectStart(inspector, Npc, NpcData);
    }

    public override void EndInspect(Character inspector)
    {
        if (NpcData == null)
        {
            NpcData = Npc.NpcData;
        }

        GameEvents.InvokeEvent_OnNpcInspectEnd(inspector, Npc, NpcData);
    }
}
