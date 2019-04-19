using Assets.OpenMM8.Scripts.Gameplay;
using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class ItemMgr
{
    static public Item GetRandomItem()
    {
        return null;
    }

    static public void ThrowItem(Transform transform, Vector3 direction, Item item)
    {
        GameObject outdoorItem = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Objects/OutdoorItem"),
            transform.position + (transform.forward * 2.5f), transform.rotation);

        outdoorItem.GetComponent<SpriteRenderer>().sprite = item.Data.OutdoorSprite;
        outdoorItem.GetComponent<Lootable>().Loot.Item = item;
        outdoorItem.GetComponent<InspectableItem>().Item = item;

        Debug.Log("[ThrowItem] Id: " + item.Data.Id);

        Vector3 speed = UiMgr.GetCrosshairRay().direction * 5.0f;
        outdoorItem.GetComponent<Rigidbody>().velocity = speed;
    }
}
