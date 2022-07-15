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
    public class TalkEventMgr : Singleton<TalkEventMgr>
    {
        //=================================== Member Variables ===================================

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
            m_PlayerParty = GameCore.Instance.PlayerParty;

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
                talkScene.IsBuilding = true;

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
                                //videoSceneObj.SetActive(false);

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

            Character currChar = m_PlayerParty.GetMostRecoveredCharacter();
            GameEvents.InvokeEvent_OnTalkSceneStart(currChar, m_BuildingTalkSceneMap[buildingId]);
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

            Character currChar = m_PlayerParty.GetMostRecoveredCharacter();
            GameEvents.InvokeEvent_OnTalkSceneStart(currChar, m_NpcTalkScene);
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

            Character currChar = m_PlayerParty.GetMostRecoveredCharacter();
            GameEvents.InvokeEvent_OnTalkSceneStart(currChar, m_NpcTalkScene);
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
            GameEvents.InvokeEvent_OnTalkWithConcreteNpc(avatarBtnContext.TalkProperties);
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
            
            GameEvents.InvokeEvent_OnNpcTalkTextChanged(message);
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
            GameEvents.InvokeEvent_OnNpcTalkTextChanged(greet);
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
            GameCore.Instance.AddRosterNpcToParty(rosterId);
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

                case 20: // Deliver Dadeross' letter to elgar fellmoon
                    if (EventAPI.IsQuestBitSet(4))
                    {
                        return false;
                    }
                    break;

                case 21: // Brekish Onefang - Power Stone quest cannot be finished
                    if (EventAPI.IsQuestBitSet(8)) { return false; }
                    break;

                case 31:
                    if (EventAPI.IsQuestBitSet(40))
                    {
                        return false;
                    }
                    break;

                case 32:
                    if (!EventAPI.HaveItem(742))
                    {
                        return false;
                    }
                    break;

                case 40:
                    if (!EventAPI.IsQuestBitSet(68))
                    {
                        return false;
                    }
                    break;

                case 41:
                    if (!EventAPI.IsQuestBitSet(11))
                    {
                        return false;
                    }
                    break;

                case 48:
                    if (!EventAPI.IsQuestBitSet(17))
                    {
                        return false;
                    }
                    break;

                case 49:
                    return EventAPI.IsQuestBitSet(72);

                case 61: return !EventAPI.IsQuestBitSet(75);
                case 63: return EventAPI.IsQuestBitSet(16);
                case 71: return EventAPI.HaveItem(732);
                case 73: return EventAPI.HaveItem(541);
                case 76: return EventAPI.HasAward(30) || EventAPI.HasAward(31);
                case 77: return !(EventAPI.HasAward(30) || EventAPI.HasAward(31));
                case 80: return !(EventAPI.HasAward(30) || EventAPI.HasAward(31));
                case 81: return EventAPI.HaveItem(626);
                case 87: return EventAPI.HasAward(34) || EventAPI.HasAward(35);
                case 88: return EventAPI.HasAward(34) || EventAPI.HasAward(35);
                case 94: return EventAPI.IsQuestBitSet(14);
                case 96: return !EventAPI.IsCharacterInParty(34);
                case 102: return EventAPI.IsQuestBitSet(15);
                case 138: return !EventAPI.IsQuestBitSet(57);
                case 147: return !EventAPI.HaveItem(519);
                case 175: return !EventAPI.IsQuestBitSet(108);
                case 176: return EventAPI.IsQuestBitSet(108);
                case 177: return !EventAPI.IsQuestBitSet(110);
                case 179: return (!EventAPI.IsQuestBitSet(113) && !EventAPI.IsQuestBitSet(114));
                case 182: return !EventAPI.IsQuestBitSet(112);
                case 186: return EventAPI.IsQuestBitSet(140);
                case 188: return !EventAPI.IsQuestBitSet(118);
                case 189: return EventAPI.IsQuestBitSet(118);
                case 190: return EventAPI.IsQuestBitSet(109);
                case 193: return !EventAPI.IsQuestBitSet(128);
                case 201: return (!EventAPI.IsQuestBitSet(115) && !EventAPI.IsQuestBitSet(116));
                case 203: return EventAPI.IsQuestBitSet(115);
                case 207: return !EventAPI.IsQuestBitSet(149);
                case 210: return (!EventAPI.IsQuestBitSet(121) && !EventAPI.IsQuestBitSet(122));
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

                case 14: // -- Quest
                    if (!EventAPI.IsQuestBitSet(138))
                    {
                        SetMessage(14);
                        EventAPI.AddQuestBit(137);
                        break;
                    }
                    else
                    {
                        SetMessage(749);
                        EventAPI.PartyCharacters().ForEach(
                            chr =>
                            {
                                EventAPI.AddExperience(chr, 750);
                                EventAPI.AddAward(chr, 54);
                            });
                        EventAPI.AddItem(222);
                        EventAPI.AddItem(222);
                        EventAPI.RemoveQuestBit(137);
                        SetNpcTopic(33, 0, 0);
                    }
                    break;

                case 15: // -- "Portals of Stone"
                    if (!EventAPI.IsQuestBitSet(1))
                    {
                        SetMessage(15);
                    }
                    else
                    {
                        SetMessage(748);
                    }
                    break;

                case 16:
                    SetMessage(16);
                    break;

                case 17:
                    SetMessage(17);
                    break;

                case 18:
                    SetMessage(18);
                    break;

                case 19:
                    if (!EventAPI.HaveItem(652))
                    {
                        SetMessage(19);
                        EventAPI.AddQuestBit(135);
                    }
                    else
                    {
                        SetMessage(750);
                        EventAPI.RemoveItem(652);
                        EventAPI.RemoveQuestBit(135);
                        EventAPI.AddGold(500);
                        EventAPI.PartyCharacters().ForEach(
                            chr =>
                            {
                                EventAPI.AddExperience(chr, 750);
                            });
                        SetNpcTopic(34, 1, 0);
                    }
                    break;

                case 20:
                    if (EventAPI.IsQuestBitSet(3))
                    {
                        SetMessage(21);
                    }
                    else
                    {
                        SetNpcGreeting(2, 8);
                        EventAPI.AddItem(741);
                        EventAPI.AddQuestBit(221);
                        EventAPI.AddQuestBit(3);
                        EventAPI.RemoveQuestBit(85);
                    }
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

                case 22:
                    SetMessage(24);
                    EventAPI.AddQuestBit(39);
                    SetNpcTopic(52, 0, 31);
                    break;

                case 23:
                    if (EventAPI.HasAward(19) ||
                        EventAPI.HasAward(20))
                    {
                        SetMessage(30);
                        EventAPI.AddGold(15000);
                        SetNpcTopic(52, 1, 0);
                    }
                    else
                    {
                        SetMessage(39);
                    }
                    break;

                case 24:
                    SetMessage(26);
                    break;

                case 25:
                    SetMessage(27);
                    EventAPI.PartyCharacters().ForEach(
                        chr =>
                        {
                            if (EventAPI.GetClass(chr) == CharacterClass.DarkElf)
                            {
                                EventAPI.AddExperience(chr, 35000);
                                EventAPI.AddAward(chr, 19);
                                chr.Class = CharacterClass.Patriarch;
                            }
                            else
                            {
                                EventAPI.AddExperience(chr, 25000);
                                EventAPI.AddAward(chr, 20);
                            }
                        });
                    EventAPI.RemoveQuestBit(39);
                    EventAPI.AddQuestBit(40);
                    EventAPI.AddQuestBit(430);
                    SetNpcTopic(55, 1, 38);
                    break;

                case 26:
                    SetMessage(28);
                    break;

                case 27:
                    if (EventAPI.IsQuestBitSet(39))
                    {
                        SetMessage(31);
                        EventAPI.PartyCharacters().ForEach(
                            chr =>
                            {
                                EventAPI.AddItem(chr, 339);
                            });
                    }
                    else if (EventAPI.IsQuestBitSet(40))
                    {
                        EventAPI.RemoveQuestBit(39);
                    }

                    SetNpcTopic(54, 0, 26);
                    break;

                case 28:
                    if (EventAPI.HaveItem(741))
                    {
                        SetMessage(33);
                        SetNpcTopic(3, 0, 29);
                        SetNpcGreeting(2, 58);
                        EventAPI.RemoveQuestBit(3);
                        EventAPI.AddQuestBit(4);
                        EventAPI.RemoveItem(741);
                        EventAPI.RemoveQuestBit(221);
                        EventAPI.AddGold(2500);
                        EventAPI.PartyCharacters().ForEach(
                            chr =>
                            {
                                EventAPI.AddExperience(chr, 7500);
                                EventAPI.AddAward(chr, 4);
                            });
                    }
                    else
                    {
                        SetMessage(32);
                    }

                    break;

                case 29:
                    if (EventAPI.IsQuestBitSet(12))
                    {
                        SetMessage(38);
                    }
                    else if (EventAPI.IsQuestBitSet(24))
                    {
                        SetMessage(37);
                        EventAPI.AddQuestBit(11);
                    }
                    else if (EventAPI.IsQuestBitSet(10))
                    {
                        SetMessage(36);
                        EventAPI.PartyCharacters().ForEach(
                            chr =>
                            {
                                EventAPI.AddAward(chr, 3);
                                EventAPI.AddExperience(chr, 12000);
                            });
                        EventAPI.AddQuestBit(24);
                        EventAPI.RemoveQuestBit(284);
                    }
                    else if (EventAPI.IsQuestBitSet(9))
                    {
                        SetMessage(35);
                    }
                    else
                    {
                        SetMessage(34);
                        EventAPI.AddQuestBit(9);
                        EventAPI.AddItem(742);
                        EventAPI.AddQuestBit(222);
                        SetNpcGreeting(3, 11);
                    }
                    break;

                case 30:
                    if (EventAPI.IsQuestBitSet(40))
                    {
                        SetMessage(30);
                        break;
                    }
                    else
                    {
                        SetMessage(39);
                    }
                    break;

                case 31:
                    SetMessage(25);
                    break;

                case 32:
                    SetMessage(40);
                    EventAPI.AddQuestBit(10);
                    EventAPI.AddQuestBit(284);
                    EventAPI.RemoveQuestBit(9);
                    EventAPI.RemoveQuestBit(222);
                    EventAPI.RemoveItem(742);
                    EventAPI.AddHistory("History3");
                    SetNpcTopic(4, 0, 33);
                    break;

                case 33:
                    SetMessage(41);
                    break;

                case 34:
                    SetMessage(42);
                    break;

                case 35:
                    SetMessage(43);
                    EventAPI.AddQuestBit(68);
                    SetNpcTopic(56, 0, 40);
                    break;

                case 36:
                    // TODO: troll promo
                    break;

                case 37:
                    SetMessage(46);
                    break;

                case 38:
                    SetMessage(47);
                    break;

                case 39:
                    SetMessage(48);
                    break;

                case 40:
                    SetMessage(49);
                    break;

                case 41:
                    SetMessage(50);
                    EventAPI.RemoveQuestBit(11);
                    EventAPI.AddQuestBit(12);
                    EventAPI.PartyCharacters().ForEach(
                        chr =>
                        {
                            EventAPI.AddExperience(chr, 15000);
                        });
                    EventAPI.AddGold(2000);
                    SetNpcTopic(5, 0, 42);
                    break;

                case 42:
                    if (EventAPI.IsQuestBitSet(25))
                    {
                        if (EventAPI.IsCharacterInParty(4))
                        {
                            // Troll is in party
                            SetMessage(892);
                        }
                        else
                        {
                            // Troll is not in party
                            if (EventAPI.IsQuestBitSet(60))
                            {
                                SetMessage(53);
                            }
                            else
                            {
                                SetMessage(52);
                            }
                        }
                    }
                    else
                    {
                        SetMessage(51);
                        EventAPI.AddQuestBit(25);
                        EventAPI.AddHistory("History4");
                        // TODO:
                        // // MoveNPC(7, 177);
                        // // MoveNPC(60, 177);
                    }
                    break;

                case 43:
                    // Alliance - necro/clerics + dragon/dragon hunters + minotaurs
                    if ((EventAPI.IsQuestBitSet(19) ||
                         EventAPI.IsQuestBitSet(20))
                         &&
                        (EventAPI.IsQuestBitSet(21) ||
                         EventAPI.IsQuestBitSet(22))
                         &&
                         EventAPI.IsQuestBitSet(23))
                    {
                        SetMessage(886);
                    }
                    else
                    {
                        SetMessage(54);
                    }
                    break;

                case 44:
                    SetMessage(55);
                    break;

                case 45:
                    SetMessage(56);
                    SetNpcTopic(7, 0, 46);
                    break;

                case 46:
                    if (EventAPI.IsQuestBitSet(62))
                    {
                        SetMessage(59);
                        EventAPI.AddQuestBit(63);
                        EventAPI.RemoveQuestBit(61);
                        EventAPI.PartyCharacters().ForEach(
                            chr =>
                            {
                                EventAPI.AddAward(chr, 6);
                                EventAPI.AddExperience(chr, 20000);
                            });
                        SetNpcTopic(7, 0, 604);
                        SetNpcGreeting(60, 18);
                        SetNpcTopic(60, 0, 47);
                    }
                    else if (EventAPI.IsQuestBitSet(61))
                    {
                        SetMessage(58);
                    }
                    else
                    {
                        SetMessage(57);
                        EventAPI.AddQuestBit(61);
                        EventAPI.AddItem(603);
                        EventAPI.AddQuestBit(202);
                    }
                    break;

                case 47:
                    SetMessage(60);
                    break;

                case 48:
                    SetMessage(72);
                    SetNpcTopic(21, 0, 49);
                    break;

                case 49:
                    if (EventAPI.IsQuestBitSet(21))
                    {
                        break;
                    }
                    else if (EventAPI.IsQuestBitSet(33))
                    {
                        if (EventAPI.HaveItem(605))
                        {
                            // TODO: EventAPI.ShowMovie("dragonsrevenge");
                            SetMessage(860);
                            EventAPI.AddQuestBit(22);
                            EventAPI.AddQuestBit(35);
                            EventAPI.RemoveQuestBit(16);
                            EventAPI.RemoveQuestBit(17);
                            EventAPI.RemoveQuestBit(31);
                            EventAPI.RemoveQuestBit(33);
                            SetNpcTopic(19, 0, 0);
                            SetNpcGreeting(21, 20);
                            SetNpcTopic(21, 0, 0);
                            EventAPI.PartyCharacters().ForEach(
                                chr =>
                                {
                                    EventAPI.AddExperience(chr, 20000);
                                    EventAPI.AddAward(chr, 7);
                                });
                            EventAPI.RemoveItem(605);
                            EventAPI.RemoveQuestBit(204);
                            EventAPI.AddGold(10000);
                            EventAPI.AddHistory("History7");
                            break;
                        }
                        else
                        {
                            SetMessage(74);
                            break;
                        }
                    }
                    else
                    {
                        SetMessage(73);
                        EventAPI.AddQuestBit(33);
                    }
                    break;

                case 50:
                    SetMessage(68);
                    SetNpcTopic(63, 0, 743);
                    EventAPI.AddItem(623);
                    EventAPI.AddQuestBit(73);
                    EventAPI.AddQuestBit(217);
                    EventAPI.RemoveQuestBit(72);
                    break;

                case 51:
                    SetMessage(61);
                    break;

                case 52:
                    SetMessage(62);
                    EventAPI.AddQuestBit(70);
                    SetNpcTopic(62, 0, 59);
                    break;

                case 53:
                    SetMessage(63);
                    break;

                case 54:
                    if (EventAPI.HaveItem(623))
                    {
                        SetMessage(531);
                        EventAPI.RemoveItem(623);
                        EventAPI.RemoveQuestBit(217);
                        EventAPI.AddQuestBit(134);
                        EventAPI.AddQuestBit(435);
                        EventAPI.PartyCharacters().ForEach(
                            chr =>
                            {
                                EventAPI.AddExperience(chr, 15000);
                                EventAPI.AddAward(chr, 25);
                            });
                        SetNpcTopic(295, 1, 0);
                        break;
                    }
                    else
                    {
                        SetMessage(64);
                        EventAPI.AddQuestBit(72);
                        break;
                    }

                case 55:
                    SetMessage(65);
                    break;

                case 56:
                    SetMessage(66);
                    break;

                case 57:
                    SetMessage(67);
                    break;

                case 58:
                    // TODO: Promotion to champion
                    break;

                case 59:
                    if (EventAPI.IsQuestBitSet(435) &&
                        (EventAPI.HaveItem(539) ||
                         EventAPI.HasAward(24)))
                    {
                        // Found father + have ebonest / returned ebonest to quixote
                        SetMessage(894);
                        EventAPI.PartyCharacters().ForEach(
                            chr =>
                            {
                                EventAPI.AddExperience(chr, 5000);
                            });
                        SetNpcTopic(62, 0, 611);
                        break;
                    }
                    else if (EventAPI.HaveItem(539) ||
                             EventAPI.HasAward(24))
                    {
                        // have ebonest / return ebonest to quixote but did not find father
                        SetMessage(893);
                        break;
                    }
                    else
                    {
                        SetMessage(69);
                        break;
                    }

                case 60:
                    if (EventAPI.IsQuestBitSet(21))
                    {
                        SetMessage(76);
                        EventAPI.AddQuestBit(74);
                        SetNpcTopic(21, 1, 61);
                        break;
                    }
                    else
                    {
                        SetMessage(75);
                        EventAPI.AddQuestBit(74);
                        SetNpcTopic(21, 1, 61);
                        SetNpcTopic(66, 1, 61);
                        break;
                    }

                case 61:
                    if (EventAPI.IsQuestBitSet(21))
                    {
                        break;
                    }
                    else if (EventAPI.IsQuestBitSet(22))
                    {
                        SetMessage(77);
                        break;
                    }
                    else
                    {
                        SetMessage(86);
                        break;
                    }

                case 62:
                    // TODO: Great wyrm promo
                    break;

                case 63:
                    SetMessage(82);
                    SetNpcTopic(19, 0, 64);
                    break;

                case 64:
                    if (EventAPI.IsQuestBitSet(22))
                    {
                        break;
                    }
                    else if (EventAPI.IsQuestBitSet(31))
                    {
                        if (EventAPI.HaveItem(605))
                        {
                            // TODO: EventAPI.ShowMovie("dragonhunters");
                            SetMessage(853);
                            EventAPI.AddQuestBit(21);
                            EventAPI.AddQuestBit(35);
                            EventAPI.RemoveQuestBit(16);
                            EventAPI.RemoveQuestBit(17);
                            EventAPI.RemoveQuestBit(31);
                            EventAPI.RemoveQuestBit(33);
                            SetNpcTopic(19, 0, 0);
                            SetNpcGreeting(19, 22);
                            SetNpcTopic(21, 0, 0);
                            // TODO: EventAPI.MoveNpc(443, 0;
                            EventAPI.PartyCharacters().ForEach(
                                chr =>
                                {
                                    EventAPI.AddExperience(chr, 20000);
                                    EventAPI.AddAward(chr, 8);
                                });
                            EventAPI.RemoveItem(605);
                            EventAPI.RemoveQuestBit(204);
                            EventAPI.AddGold(10000);
                            EventAPI.AddHistory("History8");
                            break;
                        }
                        else
                        {
                            SetMessage(84);
                            break;
                        }
                    }
                    else
                    {
                        SetMessage(83);
                        EventAPI.AddQuestBit(31);
                        break;
                    }

                case 65:
                    SetMessage(87);
                    EventAPI.PartyCharacters().ForEach(
                        chr =>
                        {
                            EventAPI.AddExperience(chr, 20000);
                            EventAPI.AddAward(chr, 9);
                        });
                    EventAPI.AddQuestBit(23);
                    EventAPI.RemoveQuestBit(18);
                    EventAPI.RemoveQuestBit(30);
                    EventAPI.AddHistory("History11");
                    SetNpcTopic(13, 0, 0);
                    SetNpcTopic(70, 0, 613);
                    SetNpcTopic(70, 1, 0);
                    SetNpcTopic(70, 2, 0);
                    break;

                case 66:
                    SetMessage(88);
                    SetNpcTopic(70, 0, 67);
                    break;

                case 67:
                    if (EventAPI.IsQuestBitSet(4))
                    {
                        SetMessage(90);
                        break;
                    }
                    else
                    {
                        SetMessage(89);
                        EventAPI.AddQuestBit(30);
                        break;
                    }

                case 68:
                    SetMessage(91);
                    break;

                case 69:
                    SetMessage(92);
                    EventAPI.AddQuestBit(76);
                    SetNpcTopic(71, 0, 71);
                    break;

                case 70:
                    SetMessage(93);
                    break;

                case 71:
                    // TODO: Minotaur promotion
                    break;

                case 72:
                    if (EventAPI.IsQuestBitSet(76))
                    {
                        SetNpcTopic(2, 3, 73);
                        break;
                    }
                    else
                    {
                        SetMessage(96);
                        break;
                    }

                case 73:
                    if (EventAPI.HaveItem(541))
                    {
                        SetMessage(97);
                        EventAPI.AddItem(732);
                        SetNpcTopic(2, 3, 75);
                        break;
                    }
                    else
                    {
                        SetMessage(100);
                        break;
                    }

                case 74:
                    SetMessage(99);
                    break;

                case 75:
                    if (EventAPI.IsQuestBitSet(87))
                    {
                        SetMessage(102);
                        break;
                    }
                    else
                    {
                        SetMessage(101);
                        break;
                    }

                case 76:
                    SetMessage(103);
                    break;

                case 77:
                    SetMessage(104);
                    break;

                case 78:
                    SetMessage(105);
                    EventAPI.AddQuestBit(78);
                    SetNpcTopic(72, 2, 81);
                    break;

                case 79:
                    SetMessage(118);
                    break;

                case 80:
                    SetMessage(119);
                    break;

                case 81:
                    if (EventAPI.HaveItem(626))
                    {
                        SetMessage(107);
                        // TODO: Promot to clerics
                        break;
                    }
                    else
                    {
                        SetMessage(106);
                        break;
                    }

                case 82:
                    SetMessage(109);
                    break;

                case 83:
                    SetMessage(110);
                    EventAPI.AddQuestBit(80);
                    SetNpcTopic(75, 1, 90);
                    break;

                case 84:
                    SetMessage(152);
                    break;

                case 85:
                    SetMessage(153);
                    break;

                case 86:
                    SetMessage(113);
                    EventAPI.AddQuestBit(82);
                    SetNpcTopic(74, 0, 89);
                    break;

                case 87:
                    SetMessage(154);
                    break;

                case 88:
                    SetMessage(155);
                    break;

                case 89:
                    // TODO: Promotion to Lich
                    break;

                case 90:
                    // TODO: Promotion to Nosferatu
                    break;

                case 91:
                    SetMessage(120);
                    break;

                case 92:
                    SetMessage(121);
                    break;

                case 93:
                    SetMessage(151);
                    break;

                case 94:
                    SetMessage(156);
                    SetNpcTopic(9, 0, 95);
                    break;

                case 95:
                    if (EventAPI.IsQuestBitSet(28))
                    {
                        SetMessage(158);
                        break;
                    }
                    else
                    {
                        SetMessage(157);
                        EventAPI.AddQuestBit(28);
                        if (EventAPI.IsQuestBitSet(89) || EventAPI.IsQuestBitSet(90))
                        {
                            SetNpcTopic(11, 3, 634);
                        }
                        break;
                    }

                case 96:
                    SetMessage(159);
                    break;

                case 97:
                    // TODO: Verify
                    SetMessage(160);
                    EventAPI.AddQuestBit(89);
                    if (EventAPI.IsQuestBitSet(90))
                    {
                        if (EventAPI.IsQuestBitSet(26) || EventAPI.IsQuestBitSet(28))
                        {
                            SetNpcTopic(11, 3, 634);
                            break;
                        }
                        else
                        {
                            SetNpcTopic(11, 3, 100);
                            break;
                        }
                    }
                    break;

                case 98:
                    // TODO: Verify
                    SetMessage(160);
                    EventAPI.AddQuestBit(90);
                    if (EventAPI.IsQuestBitSet(89))
                    {
                        if (EventAPI.IsQuestBitSet(26) || EventAPI.IsQuestBitSet(28))
                        {
                            SetNpcTopic(11, 3, 634);
                            break;
                        }
                        else
                        {
                            SetNpcTopic(11, 3, 100);
                            break;
                        }
                    }
                    break;

                case 99:
                    SetMessage(162);
                    break;

                case 100:
                    SetMessage(163);
                    break;

                case 101:
                    SetMessage(164);
                    break;

                case 102:
                    SetMessage(165);
                    SetNpcTopic(37, 0, 103);
                    break;

                case 103:
                    if (EventAPI.IsQuestBitSet(27))
                    {
                        SetMessage(168);
                        EventAPI.RemoveQuestBit(26);
                        EventAPI.RemoveQuestBit(14);
                        EventAPI.RemoveQuestBit(15);
                        EventAPI.RemoveQuestBit(28);
                        EventAPI.AddQuestBit(20);
                        SetNpcTopic(37, 0, 0);
                        EventAPI.AddHistory("History9");
                        EventAPI.AddGold(10000);
                        EventAPI.PartyCharacters().ForEach(
                            chr =>
                            {
                                EventAPI.AddExperience(chr, 50000);
                                EventAPI.AddAward(chr, 11);
                            });
                        break;
                    }
                    else if (EventAPI.IsQuestBitSet(26))
                    {
                        SetMessage(167);
                        break;
                    }
                    else
                    {
                        SetMessage(166);
                        EventAPI.AddQuestBit(26);
                        break;
                    }

                case 104:
                    SetMessage(169);
                    break;

                case 105:
                    SetMessage(170);
                    break;

                case 106:
                    SetMessage(171);
                    SetNpcTopic(53, 0, 107);
                    break;

                case 107:
                    if (EventAPI.IsQuestBitSet(37))
                    {
                        SetMessage(173);
                        EventAPI.RemoveQuestBit(36);
                        SetNpcTopic(53, 0, 109);
                        EventAPI.AddGold(10000);
                        EventAPI.AddExperienceToParty(100000);
                        EventAPI.AddAwardToParty(12);
                        break;
                    }
                    else if (EventAPI.IsQuestBitSet(36))
                    {
                        SetMessage(174);
                        break;
                    }
                    else
                    {
                        SetMessage(172);
                        EventAPI.AddQuestBit(36);
                        EventAPI.AddHistory("History12");
                        break;
                    }

                case 108:
                    SetMessage(175);
                    break;

                case 109:
                    SetMessage(176);
                    SetNpcTopic(53, 0, 110);
                    break;

                case 110:
                    if (EventAPI.IsQuestBitSet(91))
                    {
                        SetMessage(178);
                        break;
                    }
                    else
                    {
                        SetMessage(177);
                        EventAPI.AddQuestBit(91);
                        // TODO: EventAPI.MoveNpc(23, 276);
                        // TODO: EventAPI.MoveNpc(77, 0);
                        break;
                    }

                case 111:
                    SetMessage(179);
                    break;

                case 112:
                    SetMessage(180);
                    break;

                case 113:
                    SetMessage(181);
                    break;

                case 114:
                    SetMessage(182);
                    break;

                case 115:
                    SetMessage(183);
                    break;

                case 116:
                    SetMessage(184);
                    break;

                case 117:
                    SetMessage(185);
                    break;

                case 118:
                    SetMessage(186);
                    break;

                case 119:
                    SetMessage(187);
                    break;

                case 120:
                    SetMessage(188);
                    break;

                case 121:
                    SetMessage(189);
                    break;

                case 122:
                    SetMessage(190);
                    break;

                case 123:
                    SetMessage(191);
                    break;

                case 124:
                    SetMessage(192);
                    break;

                case 125:
                    SetMessage(193);
                    break;

                case 126:
                    SetMessage(194);
                    break;

                case 127:
                    SetMessage(195);
                    break;

                case 128:
                    SetMessage(196);
                    break;

                case 129:
                    SetMessage(197);
                    break;

                case 130:
                    SetMessage(198);
                    break;

                case 131:
                    SetMessage(199);
                    break;

                case 132:
                    SetMessage(550);
                    break;

                case 133:
                    SetMessage(551);
                    break;

                case 134:
                    SetMessage(552);
                    break;

                case 135:
                    SetMessage(553);
                    break;

                case 136:
                    SetMessage(554);
                    break;

                case 137:
                    SetMessage(555);
                    break;

                case 138:
                    SetMessage(556);
                    break;

                case 139:
                    SetMessage(557);
                    break;

                case 140:
                    SetMessage(558);
                    break;

                case 141:
                    if (EventAPI.IsQuestBitSet(99))
                    {
                        SetMessage(614);
                        break;
                    }
                    else
                    {
                        SetMessage(559);
                        EventAPI.AddFood(20);
                        EventAPI.AddQuestBit(99);
                        break;
                    }

                case 142:
                    SetMessage(560);
                    break;

                case 143:
                    SetMessage(561);
                    break;

                case 144:
                    SetMessage(562);
                    break;

                case 145:
                    SetMessage(563);
                    break;

                case 146:
                    SetMessage(564);
                    break;

                case 147:
                    SetMessage(565);
                    break;

                case 148:
                    SetMessage(566);
                    break;

                case 149:
                    SetMessage(567);
                    break;

                case 150:
                    SetMessage(568);
                    break;

                case 151:
                    SetMessage(569);
                    break;

                case 152:
                    SetMessage(570);
                    EventAPI.RemoveQuestBit(91);
                    EventAPI.AddQuestBit(92);
                    SetNpcTopic(23, 0, 153);
                    SetNpcTopic(23, 1, 154);
                    break;

                case 153:
                    if (EventAPI.IsQuestBitSet(41))
                    {
                        SetMessage(571);
                        EventAPI.AddQuestBit(41);
                        EventAPI.AddQuestBit(42);
                        EventAPI.AddQuestBit(43);
                        EventAPI.AddQuestBit(44);
                        EventAPI.AddHistory("History14");
                        SetNpcTopic(53, 0, 0);
                        SetNpcTopic(53, 1, 112);
                        SetNpcTopic(65, 0, 119);
                        SetNpcTopic(66, 0, 126);
                        SetNpcTopic(67, 0, 133);
                        SetNpcTopic(68, 0, 140);
                        SetNpcTopic(69, 0, 147);
                        break;
                    }
                    else
                    {
                        if (EventAPI.IsQuestBitSet(606) ||
                            EventAPI.IsQuestBitSet(607) ||
                            EventAPI.IsQuestBitSet(608) ||
                            EventAPI.IsQuestBitSet(609))
                        {
                            SetMessage(573);
                            break;
                        }
                        else
                        {
                            SetMessage(572);
                            break;
                        }
                    }

                case 154:
                    SetMessage(574);
                    break;

                case 155:
                    SetMessage(575);
                    break;

                case 156:
                    SetMessage(576);
                    break;

                case 157:
                    SetMessage(577);
                    break;

                case 158:
                    SetMessage(578);
                    break;

                case 159:
                    SetMessage(579);
                    SetNpcTopic(26, 0, 160);
                    SetNpcTopic(26, 1, 161);
                    SetNpcTopic(26, 2, 162);
                    SetNpcTopic(26, 3, 163);
                    SetNpcTopic(23, 0, 155);
                    SetNpcTopic(23, 1, 156);
                    SetNpcGreeting(23, 112);
                    EventAPI.AddQuestBit(235);
                    EventAPI.SetMapVar("MapVar29", 4);
                    EventAPI.SetMapVar("MapVar30", 4);
                    EventAPI.SetMapVar("MapVar31", 4);
                    break;

                case 160:
                    if (EventAPI.GetMapVar("MapVar29") == 4)
                    {
                        SetMessage(580);
                        EventAPI.SetMapVar("MapVar29", 3);
                        break;
                    }
                    else if (EventAPI.GetMapVar("MapVar29") == 3)
                    {
                        SetMessage(581);
                        EventAPI.SetMapVar("MapVar29", 2);
                        break;
                    }
                    else if (EventAPI.GetMapVar("MapVar29") == 2)
                    {
                        SetMessage(582);
                        EventAPI.SetMapVar("MapVar29", 1);
                        break;
                    }
                    else if (EventAPI.GetMapVar("MapVar29") == 1)
                    {
                        SetMessage(583);
                        EventAPI.SetMapVar("MapVar29", 4);
                        break;
                    }
                    else
                    {
                        SetMessage(580);
                        EventAPI.SetMapVar("MapVar29", 3);
                        break;
                    }

                case 161:
                    if (EventAPI.GetMapVar("MapVar30") == 4)
                    {
                        SetMessage(584);
                        EventAPI.SetMapVar("MapVar30", 3);
                        break;
                    }
                    else if (EventAPI.GetMapVar("MapVar30") == 3)
                    {
                        SetMessage(585);
                        EventAPI.SetMapVar("MapVar30", 2);
                        break;
                    }
                    else if (EventAPI.GetMapVar("MapVar30") == 2)
                    {
                        SetMessage(586);
                        EventAPI.SetMapVar("MapVar30", 1);
                        break;
                    }
                    else if (EventAPI.GetMapVar("MapVar30") == 1)
                    {
                        SetMessage(587);
                        EventAPI.SetMapVar("MapVar30", 4);
                        break;
                    }
                    else
                    {
                        SetMessage(584);
                        EventAPI.SetMapVar("MapVar30", 3);
                        break;
                    }

                case 162:
                    if (EventAPI.GetMapVar("MapVar31") == 4)
                    {
                        SetMessage(588);
                        EventAPI.SetMapVar("MapVar31", 3);
                        break;
                    }
                    else if (EventAPI.GetMapVar("MapVar31") == 3)
                    {
                        SetMessage(589);
                        EventAPI.SetMapVar("MapVar31", 2);
                        break;
                    }
                    else if (EventAPI.GetMapVar("MapVar31") == 2)
                    {
                        SetMessage(590);
                        EventAPI.SetMapVar("MapVar31", 1);
                        break;
                    }
                    else if (EventAPI.GetMapVar("MapVar31") == 1)
                    {
                        SetMessage(591);
                        EventAPI.SetMapVar("MapVar31", 4);
                        break;
                    }
                    else
                    {
                        SetMessage(588);
                        EventAPI.SetMapVar("MapVar31", 3);
                        break;
                    }

                case 163:
                    SetMessage(592);
                    EventAPI.AddQuestBit(98);
                    SetNpcTopic(26, 0, 164);
                    SetNpcTopic(26, 1, 165);
                    SetNpcTopic(26, 2, 166);
                    SetNpcTopic(26, 3, 0);
                    break;

                case 164:
                    // TODO: Question 1
                    break;

                case 165:
                    // TODO: Question 2
                    break;

                case 166:
                    // TODO: Question 3
                    break;

                case 167:
                    if (EventAPI.IsQuestBitSet(51) &&
                        EventAPI.IsQuestBitSet(53) &&
                        EventAPI.IsQuestBitSet(49))
                    {
                        SetMessage(888);
                    }
                    else
                    {
                        SetMessage(608);
                    }
                    EventAPI.AddQuestBit(56);
                    EventAPI.AddQuestBit(55);
                    EventAPI.RemoveQuestBit(54);
                    EventAPI.AddHistory("History17");
                    SetNpcTopic(30, 0, 0);
                    EventAPI.MoveNpc(30, 0);
                    EventAPI.AddExperienceToParty(100000);
                    EventAPI.AddGold(10000);
                    EventAPI.AddAwardToParty(14);
                    break;

                case 168:
                    if (EventAPI.IsQuestBitSet(51) &&
                        EventAPI.IsQuestBitSet(55) &&
                        EventAPI.IsQuestBitSet(49))
                    {
                        SetMessage(889);
                    }
                    else
                    {
                        SetMessage(609);
                    }
                    EventAPI.AddQuestBit(56);
                    EventAPI.AddQuestBit(53);
                    EventAPI.RemoveQuestBit(52);
                    EventAPI.AddHistory("History17");
                    SetNpcTopic(28, 0, 0);
                    EventAPI.MoveNpc(28, 0);
                    EventAPI.AddExperienceToParty(100000);
                    EventAPI.AddGold(10000);
                    EventAPI.AddAwardToParty(15);
                    break;

                case 169:
                    if (EventAPI.IsQuestBitSet(55) &&
                        EventAPI.IsQuestBitSet(53) &&
                        EventAPI.IsQuestBitSet(49))
                    {
                        SetMessage(890);
                    }
                    else
                    {
                        SetMessage(610);
                    }
                    EventAPI.AddQuestBit(56);
                    EventAPI.AddQuestBit(51);
                    EventAPI.RemoveQuestBit(50);
                    EventAPI.AddHistory("History17");
                    SetNpcTopic(29, 0, 0);
                    EventAPI.MoveNpc(29, 0);
                    EventAPI.AddExperienceToParty(100000);
                    EventAPI.AddGold(10000);
                    EventAPI.AddAwardToParty(16);
                    break;

                case 170:
                    if (EventAPI.IsQuestBitSet(55) &&
                        EventAPI.IsQuestBitSet(53) &&
                        EventAPI.IsQuestBitSet(51))
                    {
                        SetMessage(891);
                    }
                    else
                    {
                        SetMessage(611);
                    }
                    EventAPI.AddQuestBit(56);
                    EventAPI.AddQuestBit(49);
                    EventAPI.RemoveQuestBit(48);
                    EventAPI.AddHistory("History17");
                    SetNpcTopic(27, 0, 0);
                    EventAPI.MoveNpc(27, 0);
                    EventAPI.AddExperienceToParty(100000);
                    EventAPI.AddGold(10000);
                    EventAPI.AddAwardToParty(17);
                    break;

                case 171:
                    if (EventAPI.IsQuestBitSet(97))
                    {
                        SetMessage(594);
                        break;
                    }
                    else
                    {
                        SetMessage(593);
                        SetNpcTopic(26, 0, 164);
                        SetNpcTopic(26, 1, 165);
                        SetNpcTopic(26, 2, 166);
                        SetNpcTopic(26, 3, 0);
                        break;
                    }

                case 172:
                    SetMessage(613);
                    break;

                case 173:
                    SetMessage(612);
                    break;

                case 174:
                    SetMessage(619);
                    SetNpcTopic(78, 0, 175);
                    break;

                case 175:
                    SetMessage(620);
                    EventAPI.AddQuestBit(101);
                    SetNpcTopic(78, 0, 176);
                    EventAPI.AddItem(373);
                    EventAPI.AddItem(373);
                    EventAPI.AddItem(373);
                    break;

                case 176:
                    SetMessage(621);
                    EventAPI.RemoveQuestBit(101);
                    EventAPI.AddGold(1500);
                    EventAPI.AddAwardToParty(18);
                    SetNpcTopic(78, 0, 661);
                    break;

                case 177:
                    SetMessage(622);
                    EventAPI.AddQuestBit(109);
                    SetNpcTopic(79, 0, 178);
                    break;

                case 178:
                    if (EventAPI.HaveItem(616))
                    {
                        SetMessage(623);
                        EventAPI.RemoveItem(616);
                        EventAPI.AddExperienceToParty(7500);
                        EventAPI.AddGold(2000);
                        EventAPI.RemoveQuestBit(109);
                        EventAPI.RemoveQuestBit(245);
                        EventAPI.AddQuestBit(110);
                        EventAPI.AddAwardToParty(59);
                        SetNpcTopic(79, 0, 0);
                        break;
                    }
                    else
                    {
                        SetMessage(745);
                        break;
                    }

                case 179:
                    SetMessage(624);
                    EventAPI.AddQuestBit(113);
                    SetNpcTopic(88, 0, 0);
                    break;

                case 180:
                    SetMessage(747);
                    break;

                case 181:
                    // TODO: Elixir ingredients
                    break;

                case 182:
                    SetMessage(627);
                    EventAPI.AddQuestBit(111);
                    SetNpcTopic(89, 0, 183);
                    break;

                case 183:
                    if (EventAPI.HaveItem(630))
                    {
                        SetMessage(628);
                        EventAPI.RemoveItem(630);
                        EventAPI.AddExperienceToParty(7500);
                        EventAPI.AddGold(2000);
                        EventAPI.AddAwardToParty(51);
                        EventAPI.RemoveQuestBit(111);
                        EventAPI.AddQuestBit(112);
                        SetNpcTopic(89, 0, 0);
                        break;
                    }
                    else
                    {
                        SetMessage(746);
                        break;
                    }

                case 184:
                    SetMessage(629);
                    EventAPI.AddQuestBit(139);
                    SetNpcTopic(91, 0, 186);
                    break;

                case 185:
                    SetMessage(630);
                    if (EventAPI.HaveItem(633))
                    {
                        EventAPI.AddGold(250);
                        EventAPI.RemoveItem(633);
                        break;
                    }
                    else
                    {
                        SetMessage(754);
                        break;
                    }

                case 186:
                    SetMessage(753);
                    EventAPI.AddGold(2500);
                    EventAPI.AddExperienceToParty(7500);
                    EventAPI.AddAwardToParty(60);
                    EventAPI.RemoveQuestBit(139);
                    SetNpcTopic(91, 0, 0);
                    break;

                case 187:
                    if (EventAPI.IsQuestBitSet(120))
                    {
                        SetMessage(755);
                        EventAPI.RemoveQuestBit(120);
                        EventAPI.AddExperienceToParty(15000);
                        EventAPI.AddAwardToParty(56);
                        EventAPI.AddGold(10000);
                        SetNpcTopic(4, 2, 0);
                        break;
                    }
                    else
                    {
                        SetMessage(632);
                        EventAPI.AddQuestBit(119);
                        break;
                    }

                case 188:
                    SetMessage(633);
                    EventAPI.AddQuestBit(117);
                    EventAPI.AddQuestBit(282);
                    EventAPI.AddItem(602);
                    SetNpcTopic(4, 1, 189);
                    break;

                case 189:
                    SetMessage(634);
                    EventAPI.AddExperienceToParty(10000);
                    SetNpcTopic(4, 1, 299);
                    break;

                case 190:
                    SetMessage(635);
                    break;

                case 191:
                    SetMessage(636);
                    break;

                case 192:
                    SetMessage(637);
                    if (EventAPI.HaveItem(632))
                    {
                        EventAPI.RemoveItem(632);
                        EventAPI.AddGold(500);
                        break;
                    }
                    else
                    {
                        SetMessage(756);
                        break;
                    }

                case 193:
                    SetMessage(638);
                    EventAPI.AddQuestBit(127);
                    SetNpcTopic(98, 0, 194);
                    break;

                case 194:
                    if (EventAPI.HaveItem(516))
                    {
                        SetMessage(639);
                        EventAPI.AddExperienceToParty(25000);
                        EventAPI.AddGold(2000);
                        EventAPI.RemoveQuestBit(127);
                        EventAPI.RemoveQuestBit(283);
                        EventAPI.AddQuestBit(128);
                        EventAPI.AddAwardToParty(55);
                        SetNpcTopic(98, 0, 703);
                        break;
                    }
                    else
                    {
                        SetMessage(757);
                        break;
                    }

                case 195:
                    SetMessage(766);
                    break;

                case 196:
                    SetMessage(767);
                    break;

                case 197:
                    SetMessage(768);
                    if (EventAPI.HaveItem(635))
                    {
                        EventAPI.RemoveItem(635);
                        EventAPI.AddGold(1000);
                        break;
                    }
                    else
                    {
                        SetMessage(769);
                        break;
                    }

                case 198:
                    if (EventAPI.IsQuestBitSet(130))
                    {
                        SetMessage(645);
                        EventAPI.AddGold(5000);
                        EventAPI.AddExperienceToParty(10000);
                        EventAPI.AddAwardToParty(53);
                        EventAPI.RemoveQuestBit(129);
                        SetNpcTopic(123, 0, 200);
                        break;
                    }
                    else if (EventAPI.IsQuestBitSet(129))
                    {
                        SetMessage(644);
                        break;
                    }
                    else
                    {
                        SetMessage(643);
                        EventAPI.AddQuestBit(643);
                        break;
                    }

                case 199:
                    SetMessage(758);
                    if (EventAPI.HaveItem(653))
                    {
                        EventAPI.AddGold(500);
                        EventAPI.RemoveItem(653);
                        break;
                    }
                    else
                    {
                        SetMessage(759);
                        break;
                    }

                case 200:
                    SetMessage(942);
                    break;

                case 201:
                    SetMessage(646);
                    EventAPI.AddQuestBit(115);
                    SetNpcTopic(99, 0, 0);
                    break;

                case 202:
                    SetMessage(747);
                    break;

                case 203:
                    // TODO: Potion ingredients
                    break;

                case 204:
                    SetMessage(649);
                    break;

                case 205:
                    SetMessage(650);
                    if (EventAPI.HaveItem(654))
                    {
                        EventAPI.AddGold(500);
                        EventAPI.RemoveItem(654);
                        break;
                    }
                    else
                    {
                        SetMessage(760);
                        break;
                    }

                case 206:
                    SetMessage(651);
                    break;

                case 207:
                    SetMessage(652);
                    EventAPI.AddQuestBit(142);
                    EventAPI.AddItem(256);
                    EventAPI.AddItem(256);
                    EventAPI.AddItem(256);
                    SetNpcTopic(186, 1, 208);
                    break;

                case 208:
                    if (EventAPI.IsQuestBitSet(149))
                    {
                        SetMessage(764);
                        EventAPI.AddGold(1500);
                        EventAPI.AddAwardToParty(52);
                        EventAPI.RemoveQuestBit(142);
                        SetNpcTopic(186, 1, 209);
                        break;
                    }
                    else
                    {
                        SetMessage(653);
                        break;
                    }

                case 209:
                    SetMessage(654);
                    break;

                case 210:
                    SetMessage(655);
                    EventAPI.AddQuestBit(121);
                    SetNpcTopic(160, 0, 212);
                    break;

                case 211:
                    SetMessage(747);
                    break;

                case 212:
                    // TODO: Potion ingredients
                    break;

                case 213:



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
                        GameEvents.InvokeEvent_OnRefreshNpcTalk(talkProp);

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
            GameEvents.InvokeEvent_OnRefreshNpcTalk(talkProp);
        }
    }
}
