using UnityEngine;
using System.Collections;

using Assets.OpenMM8.Scripts.Gameplay;

[RequireComponent(typeof(Monster))]
public class InspectableNpc : Inspectable
{
    private MonsterData MonsterData = null;
    private Monster Monster = null;

    void Start()
    {
        Monster = GetComponent<Monster>();
    }

    public override void StartInspect(Character inspector)
    {
        if (MonsterData == null)
        {
            MonsterData = Monster.Data;
        }

        GameEvents.InvokeEvent_OnNpcInspectStart(inspector, Monster, MonsterData);
    }

    public override void EndInspect(Character inspector)
    {
        if (MonsterData == null)
        {
            MonsterData = Monster.Data;
        }

        GameEvents.InvokeEvent_OnNpcInspectEnd(inspector, Monster, MonsterData);
    }
}
