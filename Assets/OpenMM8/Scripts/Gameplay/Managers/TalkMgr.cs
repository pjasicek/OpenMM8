using Assets.OpenMM8.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public delegate void NpcTalkTextChanged(string text, TalkProperties talkProp);

    public class TalkMgr : Singleton<TalkMgr>
    {
        public static event NpcTalkTextChanged OnNpcTalkTextChanged;

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

            NpcGreet greet = DbMgr.Instance.NpcGreetDb.GetNpcGreet(currId);
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

            NpcNews news = DbMgr.Instance.NpcNewsDb.GetNpcNews(currId);
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

        static public int UpdateTopicId(int clickedTopicId, TalkProperties talkProp)
        {
            return clickedTopicId;
        }

        static public string GetCurrentTopicText(int topicId)
        {
            NpcTopic npcTopic = DbMgr.Instance.NpcTopicDb.GetNpcTopic(topicId);
            return DbMgr.Instance.NpcTextDb.GetNpcText(npcTopic.TextId).Text;
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

            int updatedTopicId = UpdateTopicId(topicId, talkProp);
            if (topicId != updatedTopicId)
            {
                // Topic ID changed
            }

            string talkText = GetCurrentTopicText(updatedTopicId);
            Logger.LogDebug("TalkText: " + talkText);
            if (OnNpcTalkTextChanged != null)
            {
                OnNpcTalkTextChanged(talkText, talkProp);
            }
        }
    }
}
