using UnityEngine;
using System.Collections;

using Assets.OpenMM8.Scripts.Gameplay;

public delegate void NpcInspectStartDlg(Character inspector, BaseNpc npc, MonsterData npcData);
public delegate void NpcInspecEndDlg(Character inspector, BaseNpc npc, MonsterData npcData);

[RequireComponent(typeof(BaseNpc))]
public class InspectableNpc : Inspectable
{
    static public event NpcInspectStartDlg OnNpcInspectStart;
    static public event NpcInspecEndDlg OnNpcInspectEnd;

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

        if (OnNpcInspectStart != null)
        {
            OnNpcInspectStart(inspector, Npc, NpcData);
        }
    }

    public override void EndInspect(Character inspector)
    {
        if (NpcData == null)
        {
            NpcData = Npc.NpcData;
        }

        if (OnNpcInspectEnd != null)
        {
            OnNpcInspectEnd(inspector, Npc, NpcData);
        }
    }
}
