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
        public class SpellbookUIState : UIState
        {
            private Character m_SpellbookOwner = null;

            public override bool OnActionPressed(string action)
            {
                if (action == "Escape" || action == "Spellbook")
                {
                    UiMgr.Instance.ReturnToGame();

                    return true;
                }

                return false;
            }

            public override bool EnterState(object stateArgs)
            {
                m_SpellbookOwner = (Character)stateArgs;
                if (m_SpellbookOwner == null)
                {
                    return false;
                }

                UiMgr.Instance.SetupForFullscreenUiState(this);
                UiMgr.Instance.SpellbookUI.DisplayForCharacter(m_SpellbookOwner);

                UiMgr.Instance.SpellbookUI.CloseButton.onClick.AddListener(delegate
                {
                    UiMgr.Instance.ReturnToGame();
                });

                return true;
            }

            public override void LeaveState()
            {
                UiMgr.Instance.SpellbookUI.CloseButton.onClick.RemoveAllListeners();

                UiMgr.Instance.SpellbookUI.Hide(m_SpellbookOwner);
            }
        }
    } // public partial class UiMgr
} // namespace
