using UnityEngine;
using System.Collections;

using Assets.OpenMM8.Scripts.Gameplay;
using Assets.OpenMM8.Scripts.Gameplay.Items;



// InspectableItem = "3D" item laying in the world (e.g. on the ground)
// InventoryItem = "2D" item in inventory or on character's doll
public class InspectableItem : Inspectable
{
    public Item Item;

    // Use this for initialization
    void Start()
    {
        //ItemData = GetComponent<BaseItem>().Data;
        // Item = ItemFactory.CreateItem(DbMgr.Instance.ItemDb.Get(100));
    }

    public override void StartInspect(Character inspector)
    {
        GameEvents.InvokeEvent_OnOutdoorItemInspectStart(Item);
    }

    public override void EndInspect(Character inspector)
    {
        GameEvents.InvokeEvent_OnOutdoorItemInspectEnd(Item);
    }
}
