using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.OpenMM8.Scripts.Gameplay;
using UnityEngine.EventSystems;



public class DollClickHandler : MonoBehaviour, IPointerDownHandler
{
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            GameEvents.InvokeEvent_OnDollClicked(this);
        }
    }
}
