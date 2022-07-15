using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    class InteractableGameEvent : Interactable
    {
        [Header("Event")]
        public int EventNumber;

        protected override bool Interact(GameObject interacter, RaycastHit interactRay)
        {
            // Should never happen
            if (!interacter.GetComponent<PlayerParty>())
            {
                return false;
            }

            GameEventMgr.Instance.ProcessGameEvent(EventNumber);

            return true;
        }
    }
}
