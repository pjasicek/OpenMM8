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
        public class TalkUIStateArgs
        {
            public TalkUIStateArgs(Character talkCharInitiator, TalkScene talkScene, NpcTalkUI npcTalkUI)
            {
                TalkCharInitiator = talkCharInitiator;
                TalkScene = talkScene;
                NpcTalkUI = npcTalkUI;
            }

            public Character TalkCharInitiator;
            public TalkScene TalkScene;
            public NpcTalkUI NpcTalkUI;
        }

        public class TalkUIState : UIState
        {
            private const float RuntimeTextInputStatusDuration = 3600.0f;

            private Character m_TalkCharInitiator;
            private TalkScene m_TalkScene;
            private NpcTalkUI m_NpcTalkUI;

            private NpcTalkProperties m_CurrTalkProp;
            private VideoScene m_CurrVideoScene;
            private bool m_IsRuntimeTextInputActive;
            private string m_RuntimeTextInputId;
            private string m_RuntimeTextInputPrompt;
            private string m_RuntimeTextInputValue;
            private int m_RuntimeTextInputOpenedFrame = -1;

            public override bool OnActionPressed(string action)
            {
                if (action == "Escape")
                {
                    if (m_IsRuntimeTextInputActive)
                    {
                        CancelRuntimeTextInput();
                        return true;
                    }

                    bool returnToGame = true;

                    // Check if we are in the middle of conversation
                    if (m_CurrTalkProp != null &&
                        m_CurrTalkProp.HouseService != null &&
                        m_CurrTalkProp.RuntimeMenuIds.Count > 0)
                    {
                        m_CurrTalkProp.RuntimeMenuIds.Pop();
                        RefreshNpcTalkTopics(m_CurrTalkProp);
                        TryShowNpcGreet(m_CurrTalkProp);
                        returnToGame = false;
                    }
                    else if (m_CurrTalkProp != null && m_CurrTalkProp.CurrentOffer != null)
                    {
                        m_CurrTalkProp.CurrentOffer = null;
                        RefreshNpcTalkTopics(m_CurrTalkProp);
                        TryShowNpcGreet(m_CurrTalkProp);
                        returnToGame = false;
                    }
                    else if (m_CurrTalkProp != null && m_CurrTalkProp.NestedTopicIds.Count > 0)
                    {
                        // We are in the middle of conversation
                        m_CurrTalkProp.NestedTopicIds.Pop();
                        RefreshNpcTalkTopics(m_CurrTalkProp);

                        // When returning - show greet text. TODO: Clarify if this is really the case
                        TryShowNpcGreet(m_CurrTalkProp);
                        returnToGame = false;
                    }

                    if (returnToGame)
                    {
                        // No action was taken yet
                        if (m_TalkScene.TalkProperties.Count > 1)
                        {

                            // Multiple NPCs in the talkable 

                            if (m_CurrTalkProp != null)
                            {
                                // Speaking to a concrete NPC when there are multiple people in the talkable
                                // => Go back to the Avatar buttons "lobby"
                                ShowTalkLobby(m_TalkScene);
                                returnToGame = false;
                            }
                        }
                    }

                    if (returnToGame)
                    {
                        if (m_TalkScene != null && m_TalkScene.IsBuilding)
                        {
                            TalkEventMgr.Instance.OnLeaveBuilding(m_TalkScene);
                        }

                        UiMgr.Instance.ReturnToGame();
                    }

                    return true;
                }

                if (action == "NextPlayer" &&
                    m_TalkScene != null &&
                    m_TalkScene.IsBuilding &&
                    m_CurrTalkProp != null &&
                    m_CurrTalkProp.HouseService != null)
                {
                    GameCore.GetParty().SelectNextCharacter();
                    return true;
                }

                return false;
            }

            public override bool OnTextInput(string input)
            {
                if (!m_IsRuntimeTextInputActive || string.IsNullOrEmpty(input))
                {
                    return false;
                }

                bool consumed = false;
                foreach (char currChar in input)
                {
                    if (currChar == '\b')
                    {
                        if (!string.IsNullOrEmpty(m_RuntimeTextInputValue))
                        {
                            m_RuntimeTextInputValue = m_RuntimeTextInputValue.Substring(0, m_RuntimeTextInputValue.Length - 1);
                        }
                        consumed = true;
                    }
                    else if (currChar == '\n' || currChar == '\r')
                    {
                        if (Time.frameCount == m_RuntimeTextInputOpenedFrame)
                        {
                            continue;
                        }

                        SubmitRuntimeTextInput();
                        consumed = true;
                    }
                    else if (char.IsDigit(currChar))
                    {
                        m_RuntimeTextInputValue += currChar;
                        consumed = true;
                    }
                }

                if (consumed && m_IsRuntimeTextInputActive)
                {
                    RefreshRuntimeTextInputStatus();
                }

                return consumed;
            }

            public override bool EnterState(object stateArgs)
            {
                TalkUIStateArgs args = (TalkUIStateArgs)stateArgs;

                m_TalkCharInitiator = args.TalkCharInitiator;
                m_TalkScene = args.TalkScene;
                m_NpcTalkUI = args.NpcTalkUI;

                // Register for events
                GameEvents.OnNpcTalkTextChanged += OnNpcTalkTextChanged;
                GameEvents.OnRefreshNpcTalk += OnRefreshNpcTalk;
                GameEvents.OnTalkWithConcreteNpc += OnTalkWithConcreteNpc;
                GameEvents.OnActiveCharacterChanged += OnActiveCharacterChanged;

                ShowTalkScene(m_TalkCharInitiator, m_TalkScene);

                return true;
            }

            public override void LeaveState()
            {
                CancelRuntimeTextInput(false);

                GameEvents.OnNpcTalkTextChanged -= OnNpcTalkTextChanged;
                GameEvents.OnRefreshNpcTalk -= OnRefreshNpcTalk;
                GameEvents.OnTalkWithConcreteNpc -= OnTalkWithConcreteNpc;
                GameEvents.OnActiveCharacterChanged -= OnActiveCharacterChanged;

                if (m_CurrVideoScene != null)
                {
                    m_CurrVideoScene.Stop();
                    m_CurrVideoScene.gameObject.SetActive(false);
                    m_CurrVideoScene = null;
                }
            }


            // =================================== Methods ===================================


            public void ShowTalkScene(Character talkerChr, TalkScene talkScene)
            {
                m_TalkCharInitiator = talkerChr;
                CancelRuntimeTextInput();

                SetupInitialTalkCanvas(talkScene);

                if (talkScene.TalkProperties.Count == 0)
                {
                    // If noone in the house/etc then only location is displayed,
                    // everything else is hidden
                    m_NpcTalkUI.TalkAvatar.Holder.SetActive(false);
                    m_NpcTalkUI.NpcTalkObj.SetActive(false);
                }
                else if (talkScene.TalkProperties.Count == 1)
                {
                    // If only one talkable, then just display the one
                    OnTalkWithConcreteNpc(talkScene.TalkProperties[0]);

                    //talkerChr.CharFaceUpdater.SetAvatar(UiMgr.RandomSprite(talkerChr.UI.Sprites.Greet), 1.0f);
                }
                else
                {
                    ShowTalkLobby(talkScene);
                }

                if (talkScene.VideoScene != null)
                {
                    m_CurrVideoScene = talkScene.VideoScene;
                    m_CurrVideoScene.gameObject.SetActive(true);
                    m_CurrVideoScene.Play();
                }
            }

            private void OnNpcTalkTextChanged(string text)
            {
                UpdateNpcTalkText(text);
            }

            private void OnRefreshNpcTalk(NpcTalkProperties talkProp)
            {
                if (!talkProp.IsPresent)
                {
                    ShowTalkScene(m_TalkCharInitiator, m_TalkScene);
                    if (talkProp.HasGoodbyeMessage)
                    {
                        m_NpcTalkUI.NpcTalkObj.SetActive(true);
                    }
                }
                else
                {
                    RefreshNpcTalkTopics(talkProp);
                }
            }

            private void OnTalkWithConcreteNpc(NpcTalkProperties talkProp)
            {
                if (m_TalkScene != null)
                {
                    SetupInitialTalkCanvas(m_TalkScene);
                }

                CancelRuntimeTextInput();
                m_CurrTalkProp = talkProp;

                m_NpcTalkUI.TalkAvatar.Holder.SetActive(true);

                if (TryShowNpcGreet(talkProp))
                {
                    m_NpcTalkUI.NpcTalkObj.SetActive(true);
                }
                else
                {
                    m_NpcTalkUI.NpcTalkObj.SetActive(false);
                }

                RefreshNpcTalkTopics(talkProp);

                m_NpcTalkUI.TalkAvatar.NpcNameText.text = talkProp.Name;
                m_NpcTalkUI.TalkAvatar.Avatar.sprite = talkProp.Avatar;
            }

            private void OnActiveCharacterChanged(Character chr)
            {
                if (m_CurrTalkProp == null)
                {
                    return;
                }

                if (m_CurrTalkProp.HouseService != null)
                {
                    CancelRuntimeTextInput();

                    if (TryShowNpcGreet(m_CurrTalkProp))
                    {
                        m_NpcTalkUI.NpcTalkObj.SetActive(true);
                    }
                    else
                    {
                        m_NpcTalkUI.NpcTalkObj.SetActive(false);
                    }

                    RefreshNpcTalkTopics(m_CurrTalkProp);
                }
                else if (m_CurrTalkProp.CurrentOffer != null)
                {
                    TryShowNpcGreet(m_CurrTalkProp);
                    RefreshNpcTalkTopics(m_CurrTalkProp);
                }
            }

            private void UpdateNpcTalkText(string talkText)
            {
                m_NpcTalkUI.NpcResponseText.text = talkText;

                float height = UiMgr.GetTextHeight(m_NpcTalkUI.NpcResponseText);

                float textSizeY = (height /*/ 2.0f*/) / 10.0f;
                Vector2 v = new Vector2(
                    m_NpcTalkUI.NpcTalkBackgroundImg.rectTransform.anchoredPosition.x,
                    NpcTalkUI.DefaultResponseY + textSizeY + 10.0f);
                m_NpcTalkUI.NpcTalkBackgroundImg.rectTransform.anchoredPosition = v;
            }

            private void SetupInitialTalkCanvas(TalkScene talkScene)
            {
                m_NpcTalkUI.LocationNameText.text = talkScene.Location;
                m_NpcTalkUI.NpcTalkCanvas.enabled = true;

                // avatar btns tmp
                foreach (AvatarBtnContext avBtn in m_NpcTalkUI.AvatarBtnList)
                {
                    avBtn.Holder.SetActive(false);
                }

                foreach (GameObject topicButton in m_NpcTalkUI.TopicButtonList)
                {
                    topicButton.SetActive(false);
                }
            }

            private void ShowTalkLobby(TalkScene talkScene)
            {
                SetupInitialTalkCanvas(talkScene);

                // If more than 1 talkables, then display location name and talkable buttons
                if (talkScene.TalkProperties.Count > 3)
                {
                    Debug.LogError("Too many NPCs in talkable script: " + talkScene.TalkProperties.Count
                        + ", Displaying only 3 avatar buttons, ignoring rest !");
                }

                m_NpcTalkUI.TalkAvatar.Holder.SetActive(false);
                m_NpcTalkUI.NpcTalkObj.SetActive(false);

                int talkPropIdx = 0;
                foreach (NpcTalkProperties talkProp in talkScene.TalkProperties)
                {
                    // This is the limitation of max 3 avatar buttons
                    if (talkPropIdx >= 3)
                    {
                        break;
                    }

                    AvatarBtnContext avBtn = m_NpcTalkUI.AvatarBtnList[talkPropIdx];

                    avBtn.Holder.SetActive(true);
                    avBtn.TalkProperties = talkProp;
                    avBtn.Avatar.sprite = talkProp.Avatar;
                    avBtn.AvatarText.text = talkProp.Name;

                    talkPropIdx++;
                }

                m_CurrTalkProp = null;
            }

            private bool TryShowNpcGreet(NpcTalkProperties talkProp)
            {
                if (TalkEventMgr.Instance.HasGreetText(talkProp))
                {
                    String talkText = "Oops !";

                    if (talkProp.HouseService != null)
                    {
                        talkText = TalkEventMgr.Instance.GetHouseServiceGreeting(talkProp);
                    }
                    else if (talkProp.CurrentOffer != null)
                    {
                        talkText = TalkEventMgr.Instance.GetOfferMessageText(talkProp);
                    }
                    else if (talkProp.IsNpcNews)
                    {
                        talkText = TalkEventMgr.GetCurrentNpcNews(talkProp);
                    }
                    else
                    {
                        talkText = TalkEventMgr.GetCurrNpcGreet(talkProp);
                    }

                    UpdateNpcTalkText(talkText);

                    return true;
                }

                return false;
            }

            public void RefreshNpcTalkTopics(NpcTalkProperties talkProp)
            {
                if (!talkProp.IsPresent)
                {

                }

                List<HouseDialogueOption> runtimeOptions = null;
                List<int> currentTopics;
                if (talkProp.HouseService != null)
                {
                    runtimeOptions = TalkEventMgr.Instance.GetHouseServiceOptions(talkProp);
                    currentTopics = null;
                }
                else if (talkProp.CurrentOffer != null)
                {
                    currentTopics = talkProp.CurrentOffer.TopicIds;
                }
                else if (talkProp.NestedTopicIds.Count == 0)
                {
                    currentTopics = talkProp.TopicIds;
                }
                else
                {
                    currentTopics = talkProp.NestedTopicIds.First();
                }

                // TODO: Verify that "ToList()" does not screw everything up
                foreach (GameObject topicButton in m_NpcTalkUI.TopicButtonList.ToList())
                {
                    topicButton.SetActive(false);

                    // ..... 
                    if (topicButton == EventSystem.current.currentSelectedGameObject)
                    {
                        int idx = m_NpcTalkUI.TopicButtonList.IndexOf(topicButton);
                        m_NpcTalkUI.TopicButtonList[idx] = m_NpcTalkUI.TopicButtonList.Last();
                        m_NpcTalkUI.TopicButtonList[m_NpcTalkUI.TopicButtonList.Count - 1] = topicButton;
                    }
                }

                float totalTextHeight = 0.0f;
                int buttIdx = 0;
                if (runtimeOptions != null)
                {
                    foreach (HouseDialogueOption option in runtimeOptions)
                    {
                        if (!option.IsEnabled)
                        {
                            continue;
                        }

                        GameObject topicButton = m_NpcTalkUI.TopicButtonList[buttIdx];

                        topicButton.GetComponent<Text>().text = option.Text;
                        topicButton.SetActive(true);

                        float btnHeight = UiMgr.GetTextHeight(topicButton.GetComponent<Text>());
                        topicButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(
                            RectTransform.Axis.Vertical, btnHeight);

                        TopicBtnContext btnCtx = topicButton.GetComponent<TopicBtnContext>();
                        btnCtx.TalkProperties = talkProp;
                        btnCtx.TopicId = 0;
                        btnCtx.IsRuntimeOption = true;
                        btnCtx.RuntimeOptionId = option.Id;

                        totalTextHeight += btnHeight;
                        buttIdx++;
                    }
                }
                else
                {
                    foreach (int topicId in currentTopics)
                    {
                        // Only topic IDs > 0 are valid
                        if (!TalkEventMgr.Instance.CanShowTopic(topicId))
                        {
                            continue;
                        }

                        string topic = TalkEventMgr.Instance.GetTopicButtonText(topicId, talkProp);

                        GameObject topicButton = m_NpcTalkUI.TopicButtonList[buttIdx];

                        topicButton.GetComponent<Text>().text = topic;
                        topicButton.SetActive(true);

                        float btnHeight = UiMgr.GetTextHeight(topicButton.GetComponent<Text>());
                        topicButton.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(
                            RectTransform.Axis.Vertical, btnHeight);

                        // Set up data for click delegate
                        TopicBtnContext btnCtx = topicButton.GetComponent<TopicBtnContext>();
                        btnCtx.TalkProperties = talkProp;
                        btnCtx.TopicId = topicId;
                        btnCtx.IsRuntimeOption = false;
                        btnCtx.RuntimeOptionId = null;

                        totalTextHeight += btnHeight;
                        buttIdx++;
                    }
                }

                // 7.5px spaces between buttons
                const float spacerHeight = 240.0f;
                totalTextHeight += (buttIdx - 1) * spacerHeight;

                float topicCenterY = m_NpcTalkUI.TopicButtonHolder.anchoredPosition.y;
                float topPoint = topicCenterY + (totalTextHeight / 10.0f) / 2.0f;

                foreach (GameObject topicButton in m_NpcTalkUI.TopicButtonList)
                {
                    if (!topicButton.active)
                    {
                        continue;
                    }

                    float btnHeight = topicButton.GetComponent<RectTransform>().rect.height;
                    Vector2 currPos = topicButton.GetComponent<RectTransform>().anchoredPosition;
                    Vector2 newPos = new Vector2(currPos.x, topPoint - ((btnHeight / 10.0f) / 2.0f));
                    topicButton.GetComponent<RectTransform>().anchoredPosition = newPos;

                    topPoint -= btnHeight / 10.0f;
                    topPoint -= spacerHeight / 10.0f;
                }
            }

            public void BeginRuntimeTextInput(string prompt, string inputId, string initialValue = "")
            {
                m_IsRuntimeTextInputActive = true;
                m_RuntimeTextInputPrompt = prompt ?? string.Empty;
                m_RuntimeTextInputId = inputId;
                m_RuntimeTextInputValue = initialValue ?? string.Empty;
                m_RuntimeTextInputOpenedFrame = Time.frameCount;

                if (EventSystem.current != null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }

                RefreshRuntimeTextInputStatus();
            }

            private void SubmitRuntimeTextInput()
            {
                if (!m_IsRuntimeTextInputActive)
                {
                    return;
                }

                string inputId = m_RuntimeTextInputId;
                string inputValue = m_RuntimeTextInputValue;

                CancelRuntimeTextInput();
                TalkEventMgr.Instance.ProcessRuntimeTextInput(inputId, inputValue, m_CurrTalkProp);
            }

            private void CancelRuntimeTextInput(bool clearStatusBar = true)
            {
                m_IsRuntimeTextInputActive = false;
                m_RuntimeTextInputId = null;
                m_RuntimeTextInputPrompt = string.Empty;
                m_RuntimeTextInputValue = string.Empty;
                m_RuntimeTextInputOpenedFrame = -1;

                if (clearStatusBar)
                {
                    GameCore.SetStatusBarText(string.Empty);
                }
            }

            private void RefreshRuntimeTextInputStatus()
            {
                GameCore.SetStatusBarText(
                    m_RuntimeTextInputPrompt + m_RuntimeTextInputValue,
                    true,
                    RuntimeTextInputStatusDuration);
            }
        }
    }   
}
