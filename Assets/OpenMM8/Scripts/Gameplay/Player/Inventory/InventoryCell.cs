using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Assets.OpenMM8.Scripts.Gameplay.Items;

public class InventoryCell
{
    public BaseItem Item;

    public void Clear()
    {
        Item = null;
    }
}
