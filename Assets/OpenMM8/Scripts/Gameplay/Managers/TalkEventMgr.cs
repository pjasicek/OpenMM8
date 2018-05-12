using Assets.OpenMM8.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public delegate void RefreshNpcTalk(TalkProperties talkProp);
    public delegate void NpcTalkTextChanged(string text);
    public delegate void TalkWithConcreteNpc(TalkProperties talkProp);
    public delegate void NpcLeavingLocation(TalkProperties talkProp);
    public delegate void CharacterFinishedEvent(Character character);

    public class TalkEventMgr : Singleton<TalkEventMgr>
    {
        //=================================== Member Variables ===================================

        public static event NpcTalkTextChanged OnNpcTalkTextChanged;
        public static event RefreshNpcTalk OnRefreshNpcTalk;
        public static event TalkWithConcreteNpc OnTalkWithConcreteNpc;
        public static event NpcLeavingLocation OnNpcLeavingLocation;
        public static event CharacterFinishedEvent OnCharacterFinishedEvent;

        private PlayerParty m_PlayerParty;

        private Dictionary<int, TalkProperties> m_TalkPropertiesMap = 
            new Dictionary<int, TalkProperties>();

        // Event processing
        internal class RosterInvite
        {
            public int CharRosterId;
            public int PartyFullResponseId;
            public List<int> YesNoTopics;
        }

        private RosterInvite m_RosterInvite = null;

        //=================================== Unity Lifecycle ===================================

        void Awake()
        {

        }

        public bool Init()
        {
            m_PlayerParty = GameMgr.Instance.PlayerParty;

            foreach (var talkDataPair in DbMgr.Instance.NpcTalkDb.Data)
            {
                if (talkDataPair.Key < 1)
                {
                    Logger.LogError("Db TalkData has invalid entry: " + talkDataPair.Value.Name);
                    continue;
                }

                NpcTalkData talkData = talkDataPair.Value;

                TalkProperties talkProp = new TalkProperties();
                talkProp.Name = talkData.Name;
                talkProp.GreetId = talkData.GreetId;
                talkProp.Avatar = UiMgr.Instance.GetNpcAvatarSprite(talkData.PictureId);
                foreach (int topicId in talkData.TopicList)
                {
                    talkProp.TopicIds.Add(topicId);
                }

                // Up to 5 topics - Capacity is not working for some reason
                for (int fillIdx = talkProp.TopicIds.Count; fillIdx < 5; fillIdx++)
                {
                    talkProp.TopicIds.Add(0);
                }

                m_TalkPropertiesMap.Add(talkDataPair.Key, talkProp);
            }

            // Add Yes/No topics to the DB
            NpcTopicData yes = new NpcTopicData()
            {
                Id = 10000,
                TextId = 200,
                Topic = "Yes"
            };

            NpcTopicData no = new NpcTopicData()
            {
                Id = 10001,
                TextId = 200,
                Topic = "No"
            };

            DbMgr.Instance.NpcTopicDb.Data.Add(yes.Id, yes);
            DbMgr.Instance.NpcTopicDb.Data.Add(no.Id, no);

            return true;
        }


        //=================================== Methods ===================================


        public TalkProperties GetNpcTalkProperties(int npcId)
        {
            if (m_TalkPropertiesMap.ContainsKey(npcId))
            {
                return m_TalkPropertiesMap[npcId];
            }
            else
            {
                Logger.LogError("TalkEventMgr does not contain NPC with ID: " + npcId);
            }

            return null;
        }

        // Regular greet, mostly in houses and such
        static public string GetCurrNpcGreet(TalkProperties tlk)
        {
            string greetText = "Oops !";

            int currId = tlk.GreetId;

            NpcGreetData greet = DbMgr.Instance.NpcGreetDb.Get(currId);
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

            NpcNewsData news = DbMgr.Instance.NpcNewsDb.Get(currId);
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

            ProcessTopicClickEvent(topicBtnCtx.TopicId, topicBtnCtx.TalkProperties);
        }

        public void OnAvatarClicked(AvatarBtnContext avatarBtnContext)
        {
            if (OnTalkWithConcreteNpc != null)
            {
                OnTalkWithConcreteNpc(avatarBtnContext.TalkProperties);
            }
        }


        //========================== Talk Event Processing ==========================

        private void SetMessage(int id)
        {
            string message = "Oops !";

            NpcTextData textData = DbMgr.Instance.NpcTextDb.Get(id);
            if (textData != null)
            {
                message = textData.Text;
            }
            
            if (OnNpcTalkTextChanged != null)
            {
                OnNpcTalkTextChanged(message);
            }
        }

        private void SetNpcTopic(int npcId, int topicIdx, int setTopicId)
        {
            TalkProperties talkProp = m_TalkPropertiesMap[npcId];
            if (talkProp != null)
            {
                Debug.Log("Setting topic idx: " + topicIdx);
                talkProp.TopicIds[topicIdx] = setTopicId;
            }
        }

        private void SetNpcGreeting(int npcId, int greetingId)
        {
            TalkProperties talkProp = m_TalkPropertiesMap[npcId];
            if (talkProp != null)
            {
                talkProp.GreetId = greetingId;
            }
        }

        private void SetGreetMessage(TalkProperties talkProp)
        {
            string greet = GetCurrNpcGreet(talkProp);
            if (OnNpcTalkTextChanged != null)
            {
                OnNpcTalkTextChanged(greet);
            }
        }

        private void HandleRosterJoinEvent(int rosterId, int partyFullMsgId, TalkProperties talkProp)
        {
            m_RosterInvite = new RosterInvite()
            {
                CharRosterId = rosterId,
                PartyFullResponseId = partyFullMsgId,
                YesNoTopics = new List<int>() { 10000, 10001 }
            };

            talkProp.NestedTopicIds.Push(m_RosterInvite.YesNoTopics);
        }

        private void EvictNpc(TalkProperties talkProp)
        {
            talkProp.IsPresent = false;
            talkProp.NestedTopicIds.Clear();

            if (OnNpcLeavingLocation != null)
            {
                OnNpcLeavingLocation(talkProp);
            }
        }

        private void AddAward(Character character, int awardId)
        {
            if (OnCharacterFinishedEvent != null)
            {
                OnCharacterFinishedEvent(character);
            }
        }

        private void AddAutonote(int autonoteId)
        {

        }

        private void RemoveAutonote(int autonoteId)
        {

        }

        private List<Character> PartyCharacters()
        {
            return m_PlayerParty.Characters;
        }

        private void AddQuestBit(int questId)
        {
            QuestMgr.Instance.SetQuestBit(questId, 1);
        }

        private void RemoveQuestBit(int questId)
        {
            QuestMgr.Instance.SetQuestBit(questId, 0);
        }

        private bool IsQuestBitSet(int questId)
        {
            return QuestMgr.Instance.IsQuestBitSet(questId);
        }

        private bool HaveItem(int itemId)
        {
            return true;
        }

        private void RemoveItem(int itemId)
        {

        }

        private void AddItem(int itemId)
        {

        }

        private void AddGold(int numGold)
        {

        }

        private void AddExperience(Character chr, int numExperience)
        {
            // TODO: Add the experience
        }

        private void AddRosterNpcToParty(int rosterId)
        {
            GameMgr.Instance.AddRosterNpcToParty(rosterId);
        }

        /*
         * Some topics can be displayed only at some circumstances. Unfortunately,
         * this cannot be a part of ProcessTopicClickEvent. And wouldn't it be nice.
         */
        public bool CanShowTopic(int topicId)
        {
            // This is invalid topic
            if (topicId < 1)
            {
                return false;
            }

            switch (topicId)
            {
                case 12: // Frederick Talimere - party has to have Power Stone
                    if (!HaveItem(617)) { return false; }
                    break;

                case 21: // Brekish Onefang - Power Stone quest cannot be finished
                    if (IsQuestBitSet(8)) { return false; }
                    break;
            }

            return true;
        }

        /*
         * This method implements handling in the same manner as MM8's "GLOBAL.EVT" script.
         * In original game the script was handed Event ID (= Talk Topic ID) and it did
         * all necessary changes to the game state, e.g.:
         *    - Setting/Removing quest bits
         *    - Adding/Removing Topic IDs from NPCs
         *    - Adding awards
         *    - Handling character promotions
         *    - Handling quest rewards
         *    
         * In MM8 this script had ~8000 lines
         */
        private void ProcessTopicClickEvent(int topicId, TalkProperties talkProp)
        {
            Debug.Log("[" + GetType().Name + "] " + talkProp.Name + ": Processing TalkEvent: #" + topicId);

            switch (topicId)
            {
                case 1: // "Cataclysm"
                    SetMessage(1);
                    AddQuestBit(232);
                    break;

                case 2: // "Caravan Master"
                    SetMessage(2);
                    AddQuestBit(232);
                    break;

                case 3: // "Pirates of Regna"
                    SetMessage(3);
                    AddQuestBit(232);
                    break;

                case 4: // "Cataclysm"
                    SetMessage(4);
                    SetNpcTopic(2, 0, 20);
                    break;

                case 5: // "Pirates"
                    if (IsQuestBitSet(6))
                    {
                        SetMessage(616);
                    }
                    else
                    {
                        SetMessage(5);
                    }
                    break;

                case 6: // "Caravan"
                    if (IsQuestBitSet(6))
                    {
                        SetMessage(617);
                    }
                    else
                    {
                        SetMessage(6);
                    }
                    break;

                case 7: // "Cataclysm" -- Brekish Onefang
                    if (IsQuestBitSet(6))
                    {
                        SetMessage(615);
                    }
                    else
                    {
                        SetMessage(7);
                    }
                    break;

                case 8: // "Portals of Stone" -- Brekish Onefang
                    SetMessage(8);
                    SetNpcTopic(1, 1, 21);
                    break;

                case 9: // "Frederick Talimere" -- Brekish Onefang
                    SetMessage(9);
                    break;

                case 10: // "Portals of Stone" -- Frederick Talimere
                    SetMessage(10);
                    break;

                case 11: // "Cataclysm" -- Frederick Talimere
                    SetMessage(11);
                    break;

                case 12: // "Power Stone" -- Frederick Talimere
                    SetMessage(12);
                    PartyCharacters().ForEach(chr => { AddAward(chr, 2); AddExperience(chr, 1500); });
                    RemoveQuestBit(7);
                    AddQuestBit(8);
                    SetNpcTopic(32, 2, 602);
                    SetNpcTopic(1, 2, 0);
                    break;

                case 13: // "Abandoned Temple" -- Frederick Talimere
                    SetMessage(13);
                    break;

                case 14:
                    break;

                case 15:
                    break;

                case 16:
                    break;

                case 17:
                    break;

                case 18:
                    break;

                case 19:
                    break;

                case 20:
                    break;

                case 21: // "Quest" (Portals of Stone) - Brekish Onefang
                    if (IsQuestBitSet(7))
                    {
                        SetMessage(23);
                    }
                    else
                    {
                        SetMessage(22);
                        AddItem(617);
                        AddQuestBit(212);
                        AddQuestBit(7);
                        SetNpcTopic(32, 2, 12);
                        SetNpcTopic(32, 3, 13);
                    }
                    break;

                case 602: // "Roster Join Event" - Frederick Talimere
                    SetMessage(202);
                    HandleRosterJoinEvent(2, 203, talkProp);
                    break;

                case 10000: // "Yes" - Roster join event, custom
                    if (m_PlayerParty.IsFull())
                    {
                        AddRosterNpcToParty(m_RosterInvite.CharRosterId);

                        // This is ugly and unfortunately "coupled" with:
                        // 1) UiMgr::OnRefreshNpcTalk
                        // 2) Talkable::OnNpcLeavingLocation
                        talkProp.HasGoodbyeMessage = true;
                        EvictNpc(talkProp);
                        if (OnRefreshNpcTalk != null)
                        {
                            OnRefreshNpcTalk(talkProp);
                        }
                        SetMessage(m_RosterInvite.PartyFullResponseId);
                        return;
                    }
                    else
                    {
                        AddRosterNpcToParty(m_RosterInvite.CharRosterId);
                        EvictNpc(talkProp);
                    }
                    break;

                case 10001: // "No" - Roster join event, custom
                    SetGreetMessage(talkProp);
                    talkProp.NestedTopicIds.Pop();
                    break;

                default:
                    Logger.LogError("Unimplemented TalkEventId: " + topicId + " for Talker: " + talkProp.Name);
                    break;
            }

            // TODO: Better name. It did not need to change, but we want to refresh it.
            if (OnRefreshNpcTalk != null)
            {
                OnRefreshNpcTalk(talkProp);
            }
        }
    }
}
