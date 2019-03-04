using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.OpenMM8.Scripts.Gameplay;
using UnityEngine.EventSystems;

public delegate void InventoryCellClicked(int x, int y);

public class InventoryClickHandler : MonoBehaviour, IPointerDownHandler
{
    static public event InventoryCellClicked OnInventoryCellClicked;

    private int m_NumCellsX = 0;
    private int m_NumCellsY = 0;
    private RectTransform m_Rect;


	// Use this for initialization
	void Start ()
    {
        m_Rect = GetComponent<RectTransform>();
        m_NumCellsX = (int)m_Rect.rect.width / InventoryUI.INVENTORY_CELL_SIZE;
        m_NumCellsY = (int)m_Rect.rect.height / InventoryUI.INVENTORY_CELL_SIZE;
        

        Debug.Log("Num cells: " + m_NumCellsX + " " + m_NumCellsY);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Vector2 localCursor;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(m_Rect, 
                eventData.position, eventData.pressEventCamera, out localCursor))
            {
                return;
            }

            Debug.Log("LocalCursor:" + localCursor);

            Vector2 posClick = new Vector2(m_Rect.rect.position.x - localCursor.x,
                m_Rect.rect.position.y + localCursor.y) * -1.0f;

            Debug.Log(posClick);

            int x = (int)posClick.x / InventoryUI.INVENTORY_CELL_SIZE;
            int y = (int)posClick.y / InventoryUI.INVENTORY_CELL_SIZE;

            if (OnInventoryCellClicked != null)
            {
                OnInventoryCellClicked(x, y);
            }
        }
    }
}
