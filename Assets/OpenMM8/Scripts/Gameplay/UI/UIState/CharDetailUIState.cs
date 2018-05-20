using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public partial class UiMgr
    {
        public class CharDetailUIState : UIState
        {
            private enum CharDetailState
            {
                None,
                Stats,
                Inventory,
                Skills,
                Awards
            }

            private CharDetailState m_State;

            public override bool OnActionPressed(string action)
            {
                if (action == "Escape")
                {
                    UiMgr.Instance.ReturnToGame();

                    return true;
                }
                else if (action == "Stats")
                {

                }
                else if (action == "Inventory")
                {

                }
                else if (action == "Skills")
                {

                }
                else if (action == "Awards")
                {

                }

                return false;
            }

            public override bool EnterState(object stateArgs)
            {
                UiMgr.Instance.SetupForPartialUiState(this);

                // Register for party member change event

                return true;
            }

            public override void LeaveState()
            {
                // Unregister from registered events

                // Hide shown UI elements
                UiMgr.Instance.m_MapQuestNotesUI.Canvas.enabled = true;
            }
        }
    } // public partial class UiMgr
} // namespace
