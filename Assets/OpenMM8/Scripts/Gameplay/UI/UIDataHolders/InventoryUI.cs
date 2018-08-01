using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class InventoryUI
    {
        public const int INVENTORY_CELL_SIZE = 32;

        public GameObject Holder;

        private List<InventoryItem> Items = new List<InventoryItem>();

        private Vector3 m_TopLeftOffset = new Vector3(0, 0, 0);

        static public InventoryUI Create()
        {
            GameObject parent = OpenMM8Util.GetGameObjAtScenePath("/PartyCanvas/CharDetailCanvas/Inventory");
            if (parent == null)
            {
                throw new Exception("Could not find inventory's parent");
            }

            InventoryUI ui = new InventoryUI();
            ui.Holder = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/UI/Inventory/CharInventory"), 
                parent.transform);

            ui.m_TopLeftOffset = new Vector3(
                (ui.Holder.GetComponent<RectTransform>().rect.width / 2.0f) * -1.0f,
                (ui.Holder.GetComponent<RectTransform>().rect.height/ 2.0f),
                0.0f);

            return ui;
        }

        public void AddItem(BaseItem item)
        {
            Vector2 offset = new Vector2(
                item.InvCellPosition.x * INVENTORY_CELL_SIZE,
                (item.InvCellPosition.y * INVENTORY_CELL_SIZE) * -1.0f);

            //Holder.transform.localPosition

            GameObject inventoryItemObj = (GameObject)GameObject.Instantiate(
                Resources.Load("Prefabs/UI/Inventory/InventoryItem"), Holder.transform);

            Image itemImage = inventoryItemObj.GetComponent<Image>();
            InventoryItem inventoryItem = inventoryItemObj.GetComponent<InventoryItem>();

            itemImage.sprite = item.Data.InvSprite;
            itemImage.SetNativeSize();

            /*inventoryItemObj.transform.localPosition = new Vector3(
                offset.x + m_TopLeftOffset.x + itemImage.sprite.rect.width / 2.0f,
                offset.y + m_TopLeftOffset.y - itemImage.sprite.rect.height / 2.0f,
                0.0f);*/

            inventoryItemObj.transform.localPosition = new Vector3(
                offset.x + m_TopLeftOffset.x + (item.Data.InvSize.x * INVENTORY_CELL_SIZE) / 2.0f,
                offset.y + m_TopLeftOffset.y - (item.Data.InvSize.y * INVENTORY_CELL_SIZE) / 2.0f,
                0.0f);

            inventoryItem.Item = item;

            Items.Add(inventoryItem);
        }

        public void RemoveItem(BaseItem item)
        {

        }

    }
}
