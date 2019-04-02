using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    

    public class EventAPI
    {
        static public void EnterHouse(int houseId)
        {
            TalkEventMgr.Instance.EnterBuilding(houseId);
        }

        static public void TalkWithNPC(int npcId)
        {
            TalkEventMgr.Instance.TalkWithNPC(npcId);
        }

        static public void TalkNPCNews(int npcId, int npcNewsId)
        {
            TalkEventMgr.Instance.TalkNPCNews(npcId, npcNewsId);
        }

        static public void AddAward(Character character, int awardId)
        {
            GameEvents.InvokeEvent_OnCharacterFinishedEvent(character);
        }

        static public void AddAutonote(int autonoteId)
        {

        }

        static public void RemoveAutonote(int autonoteId)
        {

        }

        static public List<Character> PartyCharacters()
        {
            return GameMgr.Instance.PlayerParty.Characters;
        }

        static public void AddQuestBit(int questId)
        {
            QuestMgr.Instance.SetQuestBit(questId, 1);
        }

        static public void RemoveQuestBit(int questId)
        {
            QuestMgr.Instance.SetQuestBit(questId, 0);
        }

        static public bool IsQuestBitSet(int questId)
        {
            return QuestMgr.Instance.IsQuestBitSet(questId);
        }

        static public bool HaveItem(int itemId)
        {
            return true;
        }

        static public void RemoveItem(int itemId)
        {

        }

        static public void AddItem(int itemId)
        {

        }

        static public void AddGold(int numGold)
        {

        }

        static public void AddExperience(Character chr, int numExperience)
        {
            // TODO: Add the experience
        }

        static public void AddTimer(Timer t)
        {
            TimeMgr.Instance.AddTimer(t);
        }
    }
}
