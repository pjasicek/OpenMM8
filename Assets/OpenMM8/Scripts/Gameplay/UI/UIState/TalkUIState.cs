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
            private Character m_TalkCharInitiator;
            private TalkScene m_TalkScene;
            private NpcTalkUI m_NpcTalkUI;

            private NpcTalkProperties m_CurrTalkProp;
            private VideoScene m_CurrVideoScene;

            public override bool OnActionPressed(string action)
            {
                if (action == "Escape")
                {
                    bool returnToGame = true;

                    // Check if we are in the middle of conversation
                    if (m_CurrTalkProp != null && m_CurrTalkProp.NestedTopicIds.Count > 0)
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
                        UiMgr.Instance.ReturnToGame();
                    }

                    return true;
                }

                return false;
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

                ShowTalkScene(m_TalkCharInitiator, m_TalkScene);

                return true;
            }

            public override void LeaveState()
            {
                GameEvents.InvokeEvent_OnTalkSceneEnd(m_TalkCharInitiator, m_TalkScene);

                GameEvents.OnNpcTalkTextChanged -= OnNpcTalkTextChanged;
                GameEvents.OnRefreshNpcTalk -= OnRefreshNpcTalk;
                GameEvents.OnTalkWithConcreteNpc -= OnTalkWithConcreteNpc;

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

                    if (talkProp.IsNpcNews)
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

                List<int> currentTopics;
                if (talkProp.NestedTopicIds.Count == 0)
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
                foreach (int topicId in currentTopics)
                {
                    // Only topic IDs > 0 are valid
                    if (!TalkEventMgr.Instance.CanShowTopic(topicId))
                    {
                        continue;
                    }

                    string topic = DbMgr.Instance.NpcTopicDb.Get(topicId).Topic;

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

                    totalTextHeight += btnHeight;
                    buttIdx++;
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
        }
    }   
}
