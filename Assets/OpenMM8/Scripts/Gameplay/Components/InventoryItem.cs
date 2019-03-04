using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public delegate void InventoryItemHoverStart(InventoryItem inventoryItem);
    public delegate void InventoryItemHoverEnd(InventoryItem inventoryItem);
    public delegate void InventoryItemClicked(InventoryItem inventoryItem);

    [RequireComponent(typeof(Image))]
    public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        public BaseItem Item = null;
        public Image Image;
        public bool IsHeld = false;
        public bool isEquipped = false;

        // Events
        static public event InventoryItemHoverStart OnInventoryItemHoverStart;
        static public event InventoryItemHoverEnd OnInventoryItemHoverEnd;
        static public event InventoryItemClicked OnInventoryItemClicked;

        public void Awake()
        {
            Image = GetComponent<Image>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!IsHeld && OnInventoryItemHoverStart != null)
            {
                OnInventoryItemHoverStart(this);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!IsHeld && OnInventoryItemHoverEnd != null)
            {
                OnInventoryItemHoverEnd(this);
            }
        }

        public void SetOwnImage(CharacterType chrType)
        {
            Sprite sprite = Item.Data.EquipSprites[0];
            // TODO: Add the logic here ... e.g. Armor + female = itemXXXv2a etc

            GetComponent<Image>().sprite = sprite;
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
        }*/

        public void OnPointerDown(PointerEventData eventData)
        {
            if (!IsHeld && eventData.button == PointerEventData.InputButton.Left)
            {
                Debug.Log("Pointer Down: " + Item.Data.Name);
                if (OnInventoryItemClicked != null)
                {
                    OnInventoryItemClicked(this);
                }
            }
        }

        /*public void OnPointerUp(PointerEventData eventData)
        {
            Debug.Log("Pointer Up: " + Item.Data.Name);
        }*/
    }
}
