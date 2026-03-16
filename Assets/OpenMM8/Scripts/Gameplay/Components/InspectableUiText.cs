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
    //[RequireComponent(typeof(Text))]
    abstract public class InspectableUiText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Character Owner;

        abstract public string GetHeader();
        abstract public string GetInfoText();

        public void OnPointerEnter(PointerEventData eventData)
        {
            UiMgr.Instance.HandleInspectableUiTextHoverStart(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UiMgr.Instance.HandleInspectableUiTextHoverEnd(this);
        }
    }
}
