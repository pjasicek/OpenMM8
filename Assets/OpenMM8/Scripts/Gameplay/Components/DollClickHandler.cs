using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.OpenMM8.Scripts.Gameplay;
using UnityEngine.EventSystems;

public delegate void DollClicked(DollClickHandler sender);

public class DollClickHandler : MonoBehaviour, IPointerDownHandler
{
    static public event DollClicked OnDollClicked;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            //Debug.Log("DollClicked");
            if (OnDollClicked != null)
            {
                OnDollClicked(this);
            }
        }
    }
}
