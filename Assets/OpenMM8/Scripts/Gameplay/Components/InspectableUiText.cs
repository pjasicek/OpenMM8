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
    [RequireComponent(typeof(Text))]
    public class InspectableUiText : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
    {
        [SerializeField]
        public string Header;
        [SerializeField]
        public string InfoText;

        public void Awake()
        {
            
        }

        public string GetHeader()
        {
            return Header;
        }

        public string GetInfoText()
        {
            return InfoText;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            
        }
    }
}
