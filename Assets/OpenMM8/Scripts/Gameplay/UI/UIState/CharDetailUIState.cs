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
    public partial class UiMgr
    {
        public enum CharDetailState
        {
            None,
            Stats,
            Inventory,
            Skills,
            Awards
        }

        public class CharDetailUIStateArgs
        {
            public CharDetailState EnterState = CharDetailState.None;

            public CharDetailUIStateArgs(CharDetailState enterState)
            {
                EnterState = enterState;
            }
        }

        public class CharDetailUIState : UIState
        {
            private CharDetailState m_State;
            private CharDetailUI m_UI;

            public override bool OnActionPressed(string action)
            {
                if (action == "Escape")
                {
                    UiMgr.Instance.ReturnToGame();

                    return true;
                }
                else if (action == "Stats")
                {
                    SwitchState(CharDetailState.Stats);
                }
                else if (action == "Inventory")
                {
                    SwitchState(CharDetailState.Inventory);
                }
                else if (action == "Skills")
                {
                    SwitchState(CharDetailState.Skills);
                }
                else if (action == "Awards")
                {
                    SwitchState(CharDetailState.Awards);
                }
                else if (action == "NextPlayer")
                {
                    UiMgr.Instance.m_PlayerParty.SelectNextCharacter();
                }

                return false;
            }

            public override bool EnterState(object stateArgs)
            {
                m_UI = UiMgr.Instance.m_CharDetailUI;
                if (UiMgr.Instance.m_PlayerParty.ActiveCharacter == null)
                {
                    UiMgr.Instance.m_PlayerParty.SelectCharacter(0);
                }

                if (m_UI.CurrDollUI != null && m_UI.CurrDollUI.Holder != null)
                {
                    m_UI.CurrDollUI.Holder.SetActive(false);
                }
                m_UI.CurrDollUI = UiMgr.Instance.m_PlayerParty.ActiveCharacter.UI.DollUI;
                m_UI.CurrDollUI.Holder.SetActive(true);
                m_UI.InventoryUI.InventoryUI = UiMgr.Instance.m_PlayerParty.ActiveCharacter.UI.InventoryUI;
                m_UI.StatsUI.StatsUI = UiMgr.Instance.m_PlayerParty.ActiveCharacter.UI.StatsUI;
                m_UI.SkillsUI.SkillsUI = UiMgr.Instance.m_PlayerParty.ActiveCharacter.UI.SkillsUI;

                CharDetailUIStateArgs args = (CharDetailUIStateArgs)stateArgs;

                UiMgr.Instance.SetupForPartialUiState(this);

                m_UI.CanvasHolder.enabled = true;
                SwitchState(args.EnterState);

                // Register events
                GameEvents.OnActiveCharacterChanged += OnActiveCharacterChanged;

                return true;
            }

            public override void LeaveState()
            {
                // Unregister from registered events
                GameEvents.OnActiveCharacterChanged -= OnActiveCharacterChanged;

                UiMgr.Instance.m_CharDetailUI.CanvasHolder.enabled = false;

                HideCurrCharUI();
            }

            private void OnActiveCharacterChanged(Character chr)
            {
                HideCurrCharUI();

                m_UI.CurrDollUI = UiMgr.Instance.m_PlayerParty.ActiveCharacter.UI.DollUI;
                m_UI.InventoryUI.InventoryUI = UiMgr.Instance.m_PlayerParty.ActiveCharacter.UI.InventoryUI;
                m_UI.StatsUI.StatsUI = UiMgr.Instance.m_PlayerParty.ActiveCharacter.UI.StatsUI;
                m_UI.SkillsUI.SkillsUI = UiMgr.Instance.m_PlayerParty.ActiveCharacter.UI.SkillsUI;
                m_UI.CurrDollUI.Holder.SetActive(true);

                DisplayDetailState(chr);
            }

            private void DisplayDetailState(Character chr)
            {
                if (chr == null)
                {
                    Logger.LogError("NULL character - cannot display char detail state info");
                    return;
                }

                switch (m_State)
                {
                    case CharDetailState.Stats:
                        chr.UI.StatsUI.Refresh();

                        if (m_UI.StatsUI.StatsUI != null)
                        {
                            m_UI.StatsUI.StatsUI.Holder.SetActive(false);
                        }
                        m_UI.StatsUI.StatsUI = chr.UI.StatsUI;
                        m_UI.StatsUI.StatsUI.Holder.SetActive(true);
                        break;

                    case CharDetailState.Inventory:
                        if (m_UI.InventoryUI.InventoryUI != null)
                        {
                            m_UI.InventoryUI.InventoryUI.Holder.SetActive(false);
                        }
                        m_UI.InventoryUI.InventoryUI = chr.UI.InventoryUI;
                        m_UI.InventoryUI.InventoryUI.Holder.SetActive(true);
                        break;

                    case CharDetailState.Skills:
                        if (m_UI.SkillsUI.SkillsUI != null)
                        {
                            m_UI.SkillsUI.SkillsUI.Holder.SetActive(false);
                        }
                        m_UI.SkillsUI.SkillsUI = chr.UI.SkillsUI;
                        m_UI.SkillsUI.SkillsUI.Holder.SetActive(true);
                        break;

                    case CharDetailState.Awards:
                        break;

                    default:
                        Debug.Log("Unknown state: " + m_State.ToString());
                        break;
                }
            }

            private void HideCurrCharUI()
            {
                if (m_UI.CurrDollUI != null && m_UI.CurrDollUI.Holder != null)
                {
                    m_UI.CurrDollUI.Holder.SetActive(false);
                }
                if (m_UI.InventoryUI.InventoryUI != null && m_UI.InventoryUI.InventoryUI.Holder != null)
                {
                    m_UI.InventoryUI.InventoryUI.Holder.SetActive(false);
                }
                if (m_UI.StatsUI.StatsUI != null && m_UI.StatsUI.StatsUI.Holder != null)
                {
                    m_UI.StatsUI.StatsUI.Holder.SetActive(false);
                }
                if (m_UI.SkillsUI.SkillsUI != null && m_UI.SkillsUI.SkillsUI.Holder != null)
                {
                    m_UI.SkillsUI.SkillsUI.Holder.SetActive(false);
                }
                /*if (m_UI.AwardsUI.AwardsUI != null && m_UI.AwardsUI.AwardsUI.Holder != null)
                {
                    m_UI.AwardsUI.AwardsUI.Holder.SetActive(false);
                }*/
            }

            private void HideAllSubstates()
            {
                UiMgr.Instance.m_CharDetailUI.StatsUI.Holder.SetActive(false);
                UiMgr.Instance.m_CharDetailUI.SkillsUI.Holder.SetActive(false);
                UiMgr.Instance.m_CharDetailUI.InventoryUI.Holder.SetActive(false);
                UiMgr.Instance.m_CharDetailUI.AwardsUI.Holder.SetActive(false);
            }

            private void SwitchState(CharDetailState newState)
            {
                if (m_State == newState)
                {
                    return;
                }

                m_State = newState;

                HideAllSubstates();

                switch (newState)
                {
                    case CharDetailState.Stats:
                        m_UI.StatsUI.Holder.SetActive(true);
                        break;

                    case CharDetailState.Skills:
                        m_UI.SkillsUI.Holder.SetActive(true);
                        break;

                    case CharDetailState.Inventory:
                        m_UI.InventoryUI.Holder.SetActive(true);
                        break;

                    case CharDetailState.Awards:
                        m_UI.AwardsUI.Holder.SetActive(true);
                        break;

                    case CharDetailState.None:
                        UiMgr.Instance.ReturnToGame();
                        break;

                    default:
                        break;
                }

                DisplayDetailState(UiMgr.Instance.m_PlayerParty.ActiveCharacter);
            }

            // ====================================================================================
            // Inventory methods
            // ====================================================================================

            /*
             * Equip char's right hand item offset calculation:
             * PosY = ItemOffsetY - ItemHeight
             * PosX = ItemOffsetX
             * Item is Middle+Center aligned in UI
             */

            /*
             * Equip FEMALE char's armor offset calculation:
             * 
             * 
             */

            Item GetItemAtSlot(int row, int col)
            {
                return null;
            }

            List<Item> GetItemsAtSlots(int row, int col, Vector2Int span)
            {
                return null;
            }

            bool CanPlaceItem(Item item, int row, int col)
            {
                return true;
            }

        }
    } // public partial class UiMgr
} // namespace
