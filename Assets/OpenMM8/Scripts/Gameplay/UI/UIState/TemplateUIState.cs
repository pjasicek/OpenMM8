﻿using System;
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
        public class TemplateUIState : UIState
        {
            public override bool OnActionPressed(string action)
            {
                if (action == "Escape")
                {
                    UiMgr.Instance.ReturnToGame();

                    return true;
                }

                return false;
            }

            public override bool EnterState(object stateArgs)
            {
                UiMgr.Instance.SetupForFullscreenUiState(this);
                UiMgr.Instance.m_MapQuestNotesUI.Canvas.enabled = true;

                return true;
            }

            public override void LeaveState()
            {
                UiMgr.Instance.m_MapQuestNotesUI.Canvas.enabled = true;
            }
        }
    } // public partial class UiMgr
} // namespace
