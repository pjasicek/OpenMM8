using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using IngameDebugConsole;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public partial class UiMgr
    {
        public class ConsoleUIState : UIState
        {
            private DebugLogManager m_ConsoleManager;
            private bool m_WasGamePaused;

            public override bool OnActionPressed(string action)
            {
                if (action == "Escape")
                {
                    //UiMgr.Instance.ReturnToGame();

                    // Has to be done manually here
                    LeaveState();
                    UiMgr.Instance.m_ConsoleUIState = null;
                    if (!m_WasGamePaused)
                    {
                        GameMgr.Instance.UnpauseGame();
                    }

                    return true;
                }

                return false;
            }

            public override bool EnterState(object stateArgs)
            {
                Debug.Log("Entered console");

                m_ConsoleManager = OpenMM8Util.GetComponentAtScenePath<DebugLogManager>("/IngameDebugConsole");
                if (m_ConsoleManager == null)
                {
                    return false;
                }

                m_ConsoleManager.Show();
                m_WasGamePaused = GameMgr.Instance.IsGamePaused();

                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                GameMgr.Instance.PauseGame();

                return true;
            }

            public override void LeaveState()
            {
                Debug.Log("Left console");

                m_ConsoleManager.Hide();
            }
        }
    } // public partial class UiMgr
} // namespace
