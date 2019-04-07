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

        static public void AddAwardToParty(int awardId)
        {
            PartyCharacters().ForEach(
                chr =>
                {
                    AddAward(chr, awardId);
                });
        }

        static public CharacterClass GetClass(Character chr)
        {
            return chr.Class;
        }

        static public void SetClass(Character chr, CharacterClass newClass)
        {
            chr.Class = newClass;
        }

        // Check if specific member has the award
        static public bool HasAward(Character chr, int awardId)
        {
            return false;
        }

        // Check if any member in party has the award
        static public bool HasAward(int awardId)
        {
            return false;
        }

        static public void AddAutonote(int autonoteId)
        {

        }

        static public void RemoveAutonote(int autonoteId)
        {

        }

        static public void AddHistory(string historyType)
        {
            //TODO
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
            return false;
        }

        static public void RemoveItem(int itemId)
        {

        }

        static public void AddItem(Character chr, int itemId)
        {
            // TODO: Handle if inventory is full ?
            chr.Inventory.AddItem(itemId);
        }

        static public void AddItem(int itemId)
        {
            // TODO: Handle if inventory is full ?
            //GameMgr.Instance.PlayerParty.
        }

        static public void AddGold(int numGold)
        {

        }

        static public void AddFood(int numFood)
        {

        }

        static public void AddExperience(Character chr, int numExperience)
        {
            // TODO: Add the experience
        }

        static public void AddExperienceToParty(int numExperience)
        {
            PartyCharacters().ForEach(
                chr =>
                {
                    AddExperience(chr, numExperience);
                });
        }

        static public void AddTimer(Timer t)
        {
            TimeMgr.Instance.AddTimer(t);
        }

        static public bool IsCharacterInParty(int characterId)
        {
            return false;
        }

        static public void SetMapVar(string mapVar, int value)
        {

        }

        static public int GetMapVar(string mapVar)
        {
            return 0;
        }

        static public void MoveNpc(int npcId, int newHouseId)
        {

        }
    }
}
