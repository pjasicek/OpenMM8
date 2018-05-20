using Assets.OpenMM8.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public delegate void RefreshNpcTalk(NpcTalkProperties talkProp);
    public delegate void NpcTalkTextChanged(string text);
    public delegate void TalkWithConcreteNpc(NpcTalkProperties talkProp);
    public delegate void NpcLeavingLocation(NpcTalkProperties talkProp);
    public delegate void TalkSceneStart(Character talkerChr, TalkScene talkScene);


    public class TalkEventMgr : Singleton<TalkEventMgr>
    {
        //=================================== Member Variables ===================================

        public static event NpcTalkTextChanged OnNpcTalkTextChanged;
        public static event RefreshNpcTalk OnRefreshNpcTalk;
        public static event TalkWithConcreteNpc OnTalkWithConcreteNpc;
        public static event NpcLeavingLocation OnNpcLeavingLocation;
        static public event TalkSceneStart OnTalkSceneStart;

        private PlayerParty m_PlayerParty;

        private Dictionary<int, NpcTalkProperties> m_TalkPropertiesMap = 
            new Dictionary<int, NpcTalkProperties>();

        private Dictionary<int, TalkScene> m_BuildingTalkSceneMap =
            new Dictionary<int, TalkScene>();

        private Dictionary<string, VideoScene> m_VideoSceneMap = 
            new Dictionary<string, VideoScene>();

        private TalkScene m_NpcTalkScene = new TalkScene();

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

                NpcTalkProperties talkProp = new NpcTalkProperties();
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

            foreach (var buildingDataPair in DbMgr.Instance.BuildingDb.Data)
            {
                BuildingData bd = buildingDataPair.Value;
                TalkScene talkScene = new TalkScene();

                talkScene.Location = bd.BuildingName;

                // This will need to be checked upon level load to prevent
                // loading all videos unnecessarily
                if (bd.MapId == 1)
                {
                    if (!string.IsNullOrEmpty(bd.VideoResourcePath))
                    {
                        
                        if (m_VideoSceneMap.ContainsKey(bd.VideoResourcePath))
                        {
                            // Take it from Cache
                            talkScene.VideoScene = m_VideoSceneMap[bd.VideoResourcePath];
                        }
                        else
                        {
                            VideoClip video = Resources.Load<VideoClip>(bd.VideoResourcePath);
                            AudioClip audio = Resources.Load<AudioClip>(bd.VideoResourcePath);
                            if (video && audio)
                            {
                                GameObject videoSceneObj = (GameObject)Instantiate(Resources.Load("Prefabs/Videos/BuildingVideo"));
                                talkScene.VideoScene = videoSceneObj.GetComponent<VideoScene>();
                                talkScene.VideoScene.VideoToPlay = video;
                                talkScene.VideoScene.AudioToPlay = audio;
                                talkScene.VideoScene.enabled = true;

                                // Cache it
                                m_VideoSceneMap[bd.VideoResourcePath] = talkScene.VideoScene;
                            }
                            else
                            {
                                Logger.LogError("Failed to load: " + bd.VideoResourcePath);
                            }
                        }
                    }
                }

                foreach (int npcId in bd.NpcsInsideList)
                {
                    if (m_TalkPropertiesMap.ContainsKey(npcId))
                    {
                        talkScene.TalkProperties.Add(m_TalkPropertiesMap[npcId]);
                    }
                    else
                    {
                        Debug.LogError(bd.BuildingName + ": TalkProprties map does not contain NPC ID: " + npcId);
                    }
                }

                m_BuildingTalkSceneMap.Add(bd.Id, talkScene);
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

        public void EnterBuilding(int buildingId)
        {
            if (!m_BuildingTalkSceneMap.ContainsKey(buildingId))
            {
                Debug.LogError("Trying to enter building " + buildingId + " which does not exist");
                return;
            }

            if (OnTalkSceneStart != null)
            {
                Character currChar = m_PlayerParty.GetMostRecoveredCharacter();
                OnTalkSceneStart(currChar, m_BuildingTalkSceneMap[buildingId]);
            }
        }

        public void TalkWithNPC(int npcId)
        {
            if (!m_TalkPropertiesMap.ContainsKey(npcId))
            {
                Debug.LogError("Trying to talk with NPC " + npcId + " which does not exist");
                return;
            }

            m_NpcTalkScene.VideoScene = null;
            m_NpcTalkScene.TalkProperties.Clear();
            m_NpcTalkScene.TalkProperties.Add(m_TalkPropertiesMap[npcId]);

            if (OnTalkSceneStart != null)
            {
                Character currChar = m_PlayerParty.GetMostRecoveredCharacter();
                OnTalkSceneStart(currChar, m_NpcTalkScene);
            }
        }

        public void TalkNPCNews(int npcId, int npcNews)
        {
            if (!m_TalkPropertiesMap.ContainsKey(npcId))
            {
                Debug.LogError("Trying to talk with NPC " + npcId + " which does not exist");
                return;
            }

            m_NpcTalkScene.VideoScene = null;
            m_NpcTalkScene.TalkProperties.Clear();
            m_NpcTalkScene.TalkProperties.Add(m_TalkPropertiesMap[npcId]);
            m_NpcTalkScene.TalkProperties[0].IsNpcNews = true;
            m_NpcTalkScene.TalkProperties[0].GreetId = npcNews;

            if (OnTalkSceneStart != null)
            {
                Character currChar = m_PlayerParty.GetMostRecoveredCharacter();
                OnTalkSceneStart(currChar, m_NpcTalkScene);
            }
        }

        public NpcTalkProperties GetNpcTalkProperties(int npcId)
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
        static public string GetCurrNpcGreet(NpcTalkProperties tlk)
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
        static public string GetCurrentNpcNews(NpcTalkProperties tlk)
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

        public bool HasGreetText(NpcTalkProperties talkProp)
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
            NpcTalkProperties talkProp = m_TalkPropertiesMap[npcId];
            if (talkProp != null)
            {
                Debug.Log("Setting topic idx: " + topicIdx);
                talkProp.TopicIds[topicIdx] = setTopicId;
            }
        }

        private void SetNpcGreeting(int npcId, int greetingId)
        {
            NpcTalkProperties talkProp = m_TalkPropertiesMap[npcId];
            if (talkProp != null)
            {
                talkProp.GreetId = greetingId;
            }
        }

        private void SetGreetMessage(NpcTalkProperties talkProp)
        {
            string greet = GetCurrNpcGreet(talkProp);
            if (OnNpcTalkTextChanged != null)
            {
                OnNpcTalkTextChanged(greet);
            }
        }

        private void HandleRosterJoinEvent(int rosterId, int partyFullMsgId, NpcTalkProperties talkProp)
        {
            m_RosterInvite = new RosterInvite()
            {
                CharRosterId = rosterId,
                PartyFullResponseId = partyFullMsgId,
                YesNoTopics = new List<int>() { 10000, 10001 }
            };

            talkProp.NestedTopicIds.Push(m_RosterInvite.YesNoTopics);
        }

        private void EvictNpc(NpcTalkProperties talkProp)
        {
            talkProp.IsPresent = false;
            talkProp.NestedTopicIds.Clear();

            foreach (var housePair in m_BuildingTalkSceneMap)
            {
                TalkScene talkScene = housePair.Value;
                if (talkScene.TalkProperties.Contains(talkProp))
                {
                    talkScene.TalkProperties.Remove(talkProp);
                }
            }
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
                    if (!EventAPI.HaveItem(617)) { return false; }
                    break;

                case 21: // Brekish Onefang - Power Stone quest cannot be finished
                    if (EventAPI.IsQuestBitSet(8)) { return false; }
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
        private void ProcessTopicClickEvent(int topicId, NpcTalkProperties talkProp)
        {
            Debug.Log("[" + GetType().Name + "] " + talkProp.Name + ": Processing TalkEvent: #" + topicId);

            switch (topicId)
            {
                case 1: // "Cataclysm"
                    SetMessage(1);
                    EventAPI.AddQuestBit(232);
                    break;

                case 2: // "Caravan Master"
                    SetMessage(2);
                    EventAPI.AddQuestBit(232);
                    break;

                case 3: // "Pirates of Regna"
                    SetMessage(3);
                    EventAPI.AddQuestBit(232);
                    break;

                case 4: // "Cataclysm"
                    SetMessage(4);
                    SetNpcTopic(2, 0, 20);
                    break;

                case 5: // "Pirates"
                    if (EventAPI.IsQuestBitSet(6))
                    {
                        SetMessage(616);
                    }
                    else
                    {
                        SetMessage(5);
                    }
                    break;

                case 6: // "Caravan"
                    if (EventAPI.IsQuestBitSet(6))
                    {
                        SetMessage(617);
                    }
                    else
                    {
                        SetMessage(6);
                    }
                    break;

                case 7: // "Cataclysm" -- Brekish Onefang
                    if (EventAPI.IsQuestBitSet(6))
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
                    EventAPI.PartyCharacters().ForEach(chr => { EventAPI.AddAward(chr, 2); EventAPI.AddExperience(chr, 1500); });
                    EventAPI.RemoveQuestBit(7);
                    EventAPI.AddQuestBit(8);
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
                    if (EventAPI.IsQuestBitSet(7))
                    {
                        SetMessage(23);
                    }
                    else
                    {
                        SetMessage(22);
                        EventAPI.AddItem(617);
                        EventAPI.AddQuestBit(212);
                        EventAPI.AddQuestBit(7);
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
