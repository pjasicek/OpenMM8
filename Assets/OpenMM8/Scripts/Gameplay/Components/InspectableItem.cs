using UnityEngine;
using System.Collections;

using Assets.OpenMM8.Scripts.Gameplay;
using Assets.OpenMM8.Scripts.Gameplay.Items;




public class InspectableItem : Inspectable
{
    public BaseItem Item;

    // Use this for initialization
    void Start()
    {
        //ItemData = GetComponent<BaseItem>().Data;
        // Item = ItemFactory.CreateItem(DbMgr.Instance.ItemDb.Get(100));
    }

    public override void StartInspect(Character inspector)
    {
        GameEvents.InvokeEvent_OnOutdoorItemInspectStart(Item);
        Debug.Log("Inspect start");
    }

    public override void EndInspect(Character inspector)
    {
        GameEvents.InvokeEvent_OnOutdoorItemInspectEnd(Item);
        Debug.Log("Inspect end");
    }
}
