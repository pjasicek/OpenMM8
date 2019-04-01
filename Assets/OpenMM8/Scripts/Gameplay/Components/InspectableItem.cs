using UnityEngine;
using System.Collections;

using Assets.OpenMM8.Scripts.Gameplay;
using Assets.OpenMM8.Scripts.Gameplay.Items;


public delegate void OutdoorItemInspectStart(BaseItem item);
public delegate void OutdoorItemInspectEnd(BaseItem item);

public class InspectableItem : Inspectable
{
    static public event OutdoorItemInspectStart OnOutdoorItemInspectStart;
    static public event OutdoorItemInspectEnd OnOutdoorItemInspectEnd;

    public BaseItem Item;

    // Use this for initialization
    void Start()
    {
        //ItemData = GetComponent<BaseItem>().Data;
        // Item = ItemFactory.CreateItem(DbMgr.Instance.ItemDb.Get(100));
    }

    public override void StartInspect(Character inspector)
    {
        if (OnOutdoorItemInspectStart != null)
        {
            OnOutdoorItemInspectStart(Item);
        }
        Debug.Log("Inspect start");
    }

    public override void EndInspect(Character inspector)
    {
        if (OnOutdoorItemInspectEnd != null)
        {
            OnOutdoorItemInspectEnd(Item);
        }
        Debug.Log("Inspect end");
    }
}
