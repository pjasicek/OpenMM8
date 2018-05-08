using Assets.OpenMM8.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public delegate void NpcTalkTopicListChanged(TalkProperties talkProp);
    public delegate void NpcTalkTextChanged(string text, TalkProperties talkProp);
    public delegate void TalkWithConcreteNpc(TalkProperties talkProp);

    public class TalkMgr : Singleton<TalkMgr>
    {
        //=================================== Member Variables ===================================

        public static event NpcTalkTextChanged OnNpcTalkTextChanged;
        public static event NpcTalkTopicListChanged OnNpcTalkTopicListChanged;
        public static event TalkWithConcreteNpc OnTalkWithConcreteNpc;

        private Dictionary<int, bool> m_VisitedTopicIdsMap = new Dictionary<int, bool>();


        //=================================== Unity Lifecycle ===================================

        void Awake()
        {

        }

        public bool Init()
        {
            return true;
        }


        //=================================== Methods ===================================

        // Regular greet, mostly in houses and such
        static public string GetCurrNpcGreet(TalkProperties tlk)
        {
            string greetText = "Oops !";

            // TODO: Add logic
            int currId = tlk.GreetId;

            NpcGreetData greet = DbMgr.Instance.NpcGreetDb.GetNpcGreet(currId);
            if (greet != null)
            {
                if (tlk.IsVisited == false)
                {
                    tlk.IsVisited = true;
                    greetText = greet.Greeting1;
                }
                else
                {
                    greetText = greet.Greeting2;
                }
            }
            else
            {
                Logger.LogError("No NpcGreet for ID: " + currId);
            }

            return greetText;
        }

        // Base NPC News context -> Game NPC news context
        // e.g. at the start of the games NPC lizards are worried about pirates,
        //      but once pirates are gone they are happy
        static public string GetCurrentNpcNews(TalkProperties tlk)
        {
            string newsText = "Oops !";

            // TODO: Add logic
            int currId = tlk.GreetId;

            NpcNewsData news = DbMgr.Instance.NpcNewsDb.GetNpcNews(currId);
            if (news != null)
            {
                newsText = news.Text;
            }
            else
            {
                Logger.LogError("No NpcNews for ID: " + currId);
            }

            return newsText;
        }

        static public int TryUpdateClickedTopic(int clickedTopicId, TalkProperties talkProp, out bool shouldTextChange)
        {
            // Logic - This Talkmgr can access QuestMgr for possible update

            // Types of topics handled here: 
            // [1] Sell/Buy stuff
            // [2] Quests
            // [3] Party join requests
            // ???

            shouldTextChange = true;
            int newTopicId = clickedTopicId;
            switch (clickedTopicId)
            {
                case 8: newTopicId = 21; shouldTextChange = false; break;
                default: break;
            }

            return newTopicId;
        }

        private bool IsTopicVisited(int topicId)
        {
            if (m_VisitedTopicIdsMap.ContainsKey(topicId) && m_VisitedTopicIdsMap[topicId])
            {
                return true;
            }

            return false;
        }

        public string GetCurrentTopicText(int topicId)
        {
            switch (topicId)
            {
                // Brekish Onefang - Portals of stone
                case 21:
                    if (IsTopicVisited(topicId))
                    {
                        return DbMgr.Instance.NpcTextDb.GetNpcText(23).Text;
                    }
                    else
                    {
                        return DbMgr.Instance.NpcTextDb.GetNpcText(22).Text;
                    }

                default: break;
            }

            // Is this really necessary ? Should be always same, no logic here I think
            NpcTopicData npcTopic = DbMgr.Instance.NpcTopicDb.GetNpcTopic(topicId);
            return DbMgr.Instance.NpcTextDb.GetNpcText(npcTopic.TextId).Text;
        }

        public bool HasGreetText(TalkProperties talkProp)
        {
            return talkProp.GreetId > 0;
        }

        //=================================== Events ===================================

        public void OnTopicClicked(TopicBtnContext topicBtnCtx)
        {
            if (topicBtnCtx == null || topicBtnCtx.TalkProperties == null)
            {
                Logger.LogError("null context");
                return;
            }

            TalkProperties talkProp = topicBtnCtx.TalkProperties;
            int topicId = topicBtnCtx.TopicId;

            bool shouldTextChange;
            int origTopicId = topicId;
            int updatedTopicId = TryUpdateClickedTopic(topicId, talkProp, out shouldTextChange);
            if (topicId != updatedTopicId)
            {
                // Replace old one with new one
                talkProp.TopicIds[talkProp.TopicIds.FindIndex(id => id == topicId)] = updatedTopicId;

                // Topic ID changed
                if (OnNpcTalkTopicListChanged != null)
                {
                    OnNpcTalkTopicListChanged(talkProp);
                }
            }

            string talkText = "";
            if (shouldTextChange)
            {
                talkText = GetCurrentTopicText(updatedTopicId);
            }
            else
            {
                talkText = GetCurrentTopicText(origTopicId);
            }

            //Logger.LogDebug("TalkText: " + talkText);
            if (OnNpcTalkTextChanged != null)
            {
                OnNpcTalkTextChanged(talkText, talkProp);
            }

            m_VisitedTopicIdsMap[origTopicId] = true;
        }

        public void OnAvatarClicked(AvatarBtnContext avatarBtnContext)
        {
            if (OnTalkWithConcreteNpc != null)
            {
                OnTalkWithConcreteNpc(avatarBtnContext.TalkProperties);
            }
        }
    }
}
