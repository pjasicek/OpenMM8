using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public delegate void InventoryItemHoverStart(InventoryItem inventoryItem);
    public delegate void InventoryItemHoverEnd(InventoryItem inventoryItem);

    public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public BaseItem Item;

        // Events
        static public event InventoryItemHoverStart OnInventoryItemHoverStart;
        static public event InventoryItemHoverEnd OnInventoryItemHoverEnd;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (OnInventoryItemHoverStart != null)
            {
                OnInventoryItemHoverStart(this);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (OnInventoryItemHoverEnd != null)
            {
                OnInventoryItemHoverEnd(this);
            }
        }

        /*public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                Debug.Log("Right Click on: " + Item.Data.Name);
            }
            else if (eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log("Left Click on: " + Item.Data.Name);
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            Debug.Log("Pointer Down: " + Item.Data.Name);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("Pointer Up: " + Item.Data.Name);
        }*/
    }
}
