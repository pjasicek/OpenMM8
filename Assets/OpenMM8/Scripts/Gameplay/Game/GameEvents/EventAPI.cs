using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Assets.OpenMM8.Scripts.Data;
using Assets.OpenMM8.Scripts.Gameplay.Items;
using UnityStandardAssets.Characters.FirstPerson;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public enum EventDoorState
    {
        Closed,
        Open,
        Toggle,
    }

    public enum EventFacetBit
    {
        Invisible,
        Untouchable,
    }

    public enum EventMonsterBit
    {
        Invisible,
    }

    public class EventAPI
    {
        private static readonly HashSet<int> s_Autonotes = new HashSet<int>();
        private static readonly HashSet<string> s_History = new HashSet<string>();
        private static readonly Dictionary<string, int> s_MapVars = new Dictionary<string, int>();
        private static readonly Dictionary<int, EventDoorState> s_DoorStates = new Dictionary<int, EventDoorState>();
        private static readonly Dictionary<int, HashSet<EventFacetBit>> s_FacetBits =
            new Dictionary<int, HashSet<EventFacetBit>>();
        private static readonly Dictionary<int, HashSet<EventMonsterBit>> s_MonsterGroupBits =
            new Dictionary<int, HashSet<EventMonsterBit>>();
        private static readonly Dictionary<int, int> s_NpcGroupNews = new Dictionary<int, int>();
        private static readonly Dictionary<int, bool> s_LightStates = new Dictionary<int, bool>();
        private static readonly Dictionary<int, string> s_TextureOverrides = new Dictionary<int, string>();
        private static readonly Dictionary<int, string> s_SpriteOverrides = new Dictionary<int, string>();
        private static readonly Dictionary<string, GameTime> s_Counters = new Dictionary<string, GameTime>();
        private static readonly Dictionary<int, HashSet<int>> s_PlayerBits = new Dictionary<int, HashSet<int>>();
        private static readonly Dictionary<int, Dictionary<CharAttribute, int>> s_AttributeBonusOverrides =
            new Dictionary<int, Dictionary<CharAttribute, int>>();
        private static readonly Dictionary<int, Dictionary<SpellElement, int>> s_ResistanceBonusOverrides =
            new Dictionary<int, Dictionary<SpellElement, int>>();
        private static int s_BankGold;

        static public Character GetCurrentCharacter()
        {
            PlayerParty party = Party();
            if (party == null)
            {
                return null;
            }

            return party.GetActiveOrFirstCharacter();
        }
        
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
            if (character == null)
            {
                return;
            }

            if (HasAward(character, awardId))
            {
                return;
            }

            character.Awards.Add(new Award()
            {
                AwardId = awardId,
                Type = AwardType.NormalQuest,
                Description = string.Empty,
            });

            character.PlayEventReaction(CharacterReaction.ReceivedAward);
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
            if (chr == null)
            {
                return false;
            }

            return chr.Awards.Any(award => award.AwardId == awardId);
        }

        // Check if any member in party has the award
        static public bool HasAward(int awardId)
        {
            return PartyCharacters().Any(chr => HasAward(chr, awardId));
        }

        static public void AddAutonote(int autonoteId)
        {
            s_Autonotes.Add(autonoteId);
        }

        static public void RemoveAutonote(int autonoteId)
        {
            s_Autonotes.Remove(autonoteId);
        }

        static public void AddHistory(string historyType)
        {
            if (!string.IsNullOrEmpty(historyType))
            {
                s_History.Add(historyType);
            }
        }

        static public List<Character> PartyCharacters()
        {
            PlayerParty party = Party();
            if (party == null)
            {
                return new List<Character>();
            }

            return party.Characters;
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
            return PartyCharacters().Any(chr => HasItem(chr, itemId));
        }

        static public void RemoveItem(int itemId)
        {
            RemoveItems(itemId, 1);
        }

        static public void AddItem(Character chr, int itemId)
        {
            if (chr == null)
            {
                return;
            }

            // TODO: Handle if inventory is full ?
            Logger.LogDebug("add item: " + itemId);
            chr.Inventory.AddItem(itemId);
        }

        static public void AddItem(int itemId)
        {
            PlayerParty party = Party();
            if (party == null)
            {
                return;
            }

            AddItem(party.GetActiveOrFirstCharacter(), itemId);
        }

        static public void AddGold(int numGold)
        {
            PlayerParty party = Party();
            if (party == null)
            {
                return;
            }

            party.AddGold(numGold);
        }

        static public void AddFood(int numFood)
        {
            PlayerParty party = Party();
            if (party == null)
            {
                return;
            }

            party.AddFood(numFood);
        }

        static public void AddExperience(Character chr, int numExperience)
        {
            if (chr == null)
            {
                return;
            }

            chr.Experience += numExperience;
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
            return PartyCharacters().Any(chr => chr.CharacterId == characterId);
        }

        static public void SetMapVar(string mapVar, int value)
        {
            if (string.IsNullOrEmpty(mapVar))
            {
                return;
            }

            s_MapVars[mapVar] = value;
        }

        static public int GetMapVar(string mapVar)
        {
            if (string.IsNullOrEmpty(mapVar))
            {
                return 0;
            }

            if (!s_MapVars.ContainsKey(mapVar))
            {
                s_MapVars[mapVar] = 0;
            }

            return s_MapVars[mapVar];
        }

        static public void MoveNpc(int npcId, int newHouseId)
        {
            NpcTalkProperties talkProp = TalkEventMgr.Instance.GetNpcTalkProperties(npcId);
            if (talkProp == null)
            {
                return;
            }

            if (newHouseId == 0)
            {
                talkProp.IsPresent = false;
                talkProp.NestedTopicIds.Clear();
            }
            else
            {
                talkProp.IsPresent = true;
                Debug.LogWarning("EventAPI.MoveNpc stores destination house only logically for now: NPC "
                    + npcId + " -> house " + newHouseId);
            }
        }

        static public void MoveToMap(float x, float y, float z, int direction = 0, int lookAngle = 0,
            int speedZ = 0, int houseId = 0, int icon = 0, string mapName = "0")
        {
            PlayerParty party = Party();
            if (party == null)
            {
                return;
            }

            if (!IsSameMapTeleport(mapName))
            {
                Debug.LogWarning("EventAPI.MoveToMap cross-map transition is not implemented yet. Target: "
                    + mapName + " @ (" + x + ", " + y + ", " + z + ")");
                return;
            }

            Vector3 unityPosition = MmWorldUtil.ToUnityCoordinates(new Vector3(x, y, z));
            float unityYawDegrees = MmWorldUtil.ToUnityYawDegrees(direction);

            FirstPersonController controller = party.Controller;
            CharacterController characterController = controller != null ? controller.m_CharacterController : null;
            if (characterController != null)
            {
                characterController.enabled = false;
            }

            party.transform.SetPositionAndRotation(
                unityPosition,
                Quaternion.Euler(0.0f, unityYawDegrees, 0.0f));

            if (controller != null)
            {
                controller.m_MoveDir = Vector3.zero;
                controller.m_Jump = false;
                controller.IsSpellJumpQueued = false;
                controller.ResumeFromUi();
            }

            if (characterController != null)
            {
                characterController.enabled = true;
            }
        }

        static private bool IsSameMapTeleport(string mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName) || mapName == "0")
            {
                return true;
            }

            string normalizedTarget = NormalizeMapName(mapName);
            string normalizedActiveScene = NormalizeMapName(SceneManager.GetActiveScene().name);
            return normalizedTarget == normalizedActiveScene;
        }

        static private string NormalizeMapName(string mapName)
        {
            if (string.IsNullOrWhiteSpace(mapName))
            {
                return string.Empty;
            }

            return mapName.Trim().ToLowerInvariant().Replace(".blv", string.Empty);
        }

        static public bool OpenChest(int chestId)
        {
            Debug.LogWarning("EventAPI.OpenChest is not implemented yet. Chest ID: " + chestId);
            return false;
        }

        static public void SetDoorState(int doorId, EventDoorState state)
        {
            s_DoorStates[doorId] = state;
            Debug.LogWarning("EventAPI.SetDoorState is not implemented yet. Door ID: " + doorId);
        }

        static public void StopDoor(int doorId)
        {
            Debug.LogWarning("EventAPI.StopDoor is not implemented yet. Door ID: " + doorId);
        }

        static public void SetFacetBit(int facetId, EventFacetBit bit, bool on)
        {
            SetBitState(s_FacetBits, facetId, bit, on);
            Debug.LogWarning("EventAPI.SetFacetBit is not implemented yet. Facet ID: " + facetId
                + ", Bit: " + bit);
        }

        static public void SetMonGroupBit(int groupId, EventMonsterBit bit, bool on)
        {
            SetBitState(s_MonsterGroupBits, groupId, bit, on);
            Debug.LogWarning("EventAPI.SetMonGroupBit is not implemented yet. Group ID: " + groupId
                + ", Bit: " + bit);
        }

        static public void SetNPCGroupNews(int groupId, int newsId)
        {
            s_NpcGroupNews[groupId] = newsId;
        }

        static public int GetNPCGroupNews(int groupId)
        {
            if (!s_NpcGroupNews.ContainsKey(groupId))
            {
                return 0;
            }

            return s_NpcGroupNews[groupId];
        }

        static public void SetNPCTopic(int npcId, int topicIndex, int topicId)
        {
            NpcTalkProperties talkProp = TalkEventMgr.Instance.GetNpcTalkProperties(npcId);
            if (talkProp == null || topicIndex < 0)
            {
                return;
            }

            while (talkProp.TopicIds.Count <= topicIndex)
            {
                talkProp.TopicIds.Add(0);
            }

            talkProp.TopicIds[topicIndex] = topicId;
        }

        static public void SetNPCGreeting(int npcId, int greetingId)
        {
            NpcTalkProperties talkProp = TalkEventMgr.Instance.GetNpcTalkProperties(npcId);
            if (talkProp == null)
            {
                return;
            }

            talkProp.GreetId = greetingId;
        }

        static public void StatusText(string text, bool overrideExisting = true, float duration = 2.0f)
        {
            GameCore.SetStatusBarText(text, overrideExisting, duration);
        }

        static public void SetMessage(string message)
        {
            GameEvents.InvokeEvent_OnNpcTalkTextChanged(message);
        }

        static public void SetMessage(int textId)
        {
            string message = "Missing message: " + textId;
            NpcTextData textData = DbMgr.Instance.NpcTextDb.Get(textId);
            if (textData != null)
            {
                message = textData.Text;
            }

            SetMessage(message);
        }

        static public void PlaySound(int soundId)
        {
            SoundMgr.PlaySoundById(soundId);
        }

        static public void PlaySound(string soundName)
        {
            SoundMgr.PlaySoundByName(soundName);
        }

        static public void ShowMovie(string movieName)
        {
            Debug.LogWarning("EventAPI.ShowMovie is not implemented yet. Movie: " + movieName);
        }

        static public void SetTexture(int objectId, string textureName)
        {
            s_TextureOverrides[objectId] = textureName;
            Debug.LogWarning("EventAPI.SetTexture is not implemented yet. Object ID: " + objectId);
        }

        static public void SetSprite(int spriteId, bool hidden, string spriteName = "")
        {
            s_SpriteOverrides[spriteId] = spriteName;
            Debug.LogWarning("EventAPI.SetSprite is not implemented yet. Sprite ID: " + spriteId);
        }

        static public void SetLight(int lightId, bool enabled)
        {
            s_LightStates[lightId] = enabled;
            Debug.LogWarning("EventAPI.SetLight is not implemented yet. Light ID: " + lightId);
        }

        static public void CastSpell(int spellId, int skillLevel, int skillMastery,
            Vector3 fromPosition, Vector3 targetPosition)
        {
            Debug.LogWarning("EventAPI.CastSpell is not implemented yet. Spell ID: " + spellId);
        }

        static public void SummonMonsters(int typeIndexInMapStats, int level, int count,
            Vector3 position, int npcGroup, int uniqueNameId = 0)
        {
            Debug.LogWarning("EventAPI.SummonMonsters is not implemented yet. Group: " + npcGroup
                + ", Count: " + count);
        }

        static public void SummonObject(int objectId, Vector3 position, float speed = 0.0f,
            int count = 1, bool randomAngle = true)
        {
            Debug.LogWarning("EventAPI.SummonObject is not implemented yet. Object ID: " + objectId
                + ", Count: " + count);
        }

        static public bool CheckMonstersKilled(int checkType, int id, int count, bool invisibleAsDead = true)
        {
            List<Monster> monsters = GameCore.Instance.MonsterList
                .Where(monster => monster.Group == id)
                .ToList();

            if (monsters.Count == 0)
            {
                return count <= 0;
            }

            int defeatedCount = monsters.Count(monster => IsMonsterDefeated(monster, invisibleAsDead));
            int aliveCount = monsters.Count - defeatedCount;

            switch (checkType)
            {
                case 1:
                    return aliveCount <= count;

                case 4:
                    return defeatedCount >= count;

                default:
                    Debug.LogWarning("EventAPI.CheckMonstersKilled uses unsupported check type: " + checkType);
                    return defeatedCount >= count;
            }
        }

        static public bool CheckItemsCount(int itemId, int count)
        {
            return CountItems(itemId) >= count;
        }

        static public bool RemoveItems(int itemId, int count)
        {
            int itemsLeftToRemove = count;
            foreach (Character chr in PartyCharacters())
            {
                while (itemsLeftToRemove > 0)
                {
                    Item item = chr.Inventory.InventoryItems.FirstOrDefault(invItem => invItem.Data.Id == itemId);
                    if (item == null)
                    {
                        break;
                    }

                    chr.Inventory.RemoveItem(item);
                    itemsLeftToRemove--;
                }

                if (itemsLeftToRemove <= 0)
                {
                    return true;
                }
            }

            return itemsLeftToRemove <= 0;
        }

        static public void FaceAnimation(Character character, CharacterExpression expression)
        {
            if (character == null)
            {
                return;
            }

            character.PlayCharacterExpression(expression);
        }

        static public bool Question(string questionText)
        {
            Debug.LogWarning("EventAPI.Question is not implemented yet. Question: " + questionText);
            return false;
        }

        static public bool IsTotalBountyInRange(int minValue, int maxValue)
        {
            Debug.LogWarning("EventAPI.IsTotalBountyInRange is not implemented yet.");
            return false;
        }

        static public int GetVar(string variableName, Character character)
        {
            if (variableName.StartsWith("MapVar", StringComparison.Ordinal))
            {
                return GetMapVar(variableName);
            }
            if (variableName == "Gold")
            {
                return GetGold();
            }
            if (variableName == "BankGold")
            {
                return s_BankGold;
            }
            if (variableName == "Food")
            {
                return GetFood();
            }
            if (variableName == "Experience")
            {
                return character?.Experience ?? 0;
            }
            if (variableName == "SkillPoints")
            {
                return character?.SkillPoints ?? 0;
            }
            if (variableName == "RepairSkill")
            {
                return character?.GetActualSkillLevel(SkillType.RepairItem) ?? 0;
            }
            if (variableName == "ClassIs")
            {
                return character != null ? (int)character.Class : 0;
            }
            if (variableName == "Invisible")
            {
                PlayerParty party = Party();
                return party != null && party.IsInvisible() ? 1 : 0;
            }
            if (variableName == "BaseLuck")
            {
                return character?.GetBaseLuck() ?? 0;
            }
            if (variableName == "BaseMight")
            {
                return character?.GetBaseMight() ?? 0;
            }
            if (variableName == "BaseIntellect")
            {
                return character?.GetBaseIntellect() ?? 0;
            }
            if (variableName == "BasePersonality")
            {
                return character?.GetBasePersonality() ?? 0;
            }
            if (variableName == "BaseEndurance")
            {
                return character?.GetBaseEndurance() ?? 0;
            }
            if (variableName == "BaseAccuracy")
            {
                return character?.GetBaseAccuracy() ?? 0;
            }
            if (variableName == "BaseSpeed")
            {
                return character?.GetBaseSpeed() ?? 0;
            }
            if (variableName == "CurrentMight")
            {
                return character != null ? character.GetActualMight() + GetAttributeBonusOverride(character, CharAttribute.Might) : 0;
            }
            if (variableName == "CurrentIntellect")
            {
                return character != null ? character.GetActualIntellect() + GetAttributeBonusOverride(character, CharAttribute.Intellect) : 0;
            }
            if (variableName == "CurrentPersonality")
            {
                return character != null ? character.GetActualPersonality() + GetAttributeBonusOverride(character, CharAttribute.Personality) : 0;
            }
            if (variableName == "CurrentEndurance")
            {
                return character != null ? character.GetActualEndurance() + GetAttributeBonusOverride(character, CharAttribute.Endurance) : 0;
            }
            if (variableName == "CurrentAccuracy")
            {
                return character != null ? character.GetActualAccuracy() + GetAttributeBonusOverride(character, CharAttribute.Accuracy) : 0;
            }
            if (variableName == "CurrentSpeed")
            {
                return character != null ? character.GetActualSpeed() + GetAttributeBonusOverride(character, CharAttribute.Speed) : 0;
            }
            if (variableName == "CurrentLuck")
            {
                return character != null ? character.GetActualLuck() + GetAttributeBonusOverride(character, CharAttribute.Luck) : 0;
            }
            if (variableName == "MightBonus")
            {
                return character == null ? 0 : GetAttributeBonusValue(character, CharAttribute.Might);
            }
            if (variableName == "IntellectBonus")
            {
                return character == null ? 0 : GetAttributeBonusValue(character, CharAttribute.Intellect);
            }
            if (variableName == "PersonalityBonus")
            {
                return character == null ? 0 : GetAttributeBonusValue(character, CharAttribute.Personality);
            }
            if (variableName == "HasFullHP")
            {
                return CharacterHasFullHP(character) ? 1 : 0;
            }
            if (variableName == "HasFullSP")
            {
                return CharacterHasFullSP(character) ? 1 : 0;
            }
            if (variableName == "Dead")
            {
                return CharacterHasCondition(character, Condition.Dead) ? 1 : 0;
            }
            if (variableName == "Drunk")
            {
                return CharacterHasCondition(character, Condition.Drunk) ? 1 : 0;
            }
            if (variableName == "PoisonedGreen")
            {
                return CharacterHasCondition(character, Condition.PoisonWeak) ? 1 : 0;
            }
            if (variableName == "PoisonedYellow")
            {
                return CharacterHasCondition(character, Condition.PoisonMedium) ? 1 : 0;
            }
            if (variableName == "DiseasedGreen")
            {
                return CharacterHasCondition(character, Condition.DiseaseWeak) ? 1 : 0;
            }
            if (variableName == "DiseasedYellow")
            {
                return CharacterHasCondition(character, Condition.DiseaseMedium) ? 1 : 0;
            }
            if (variableName == "DayOfWeekIs")
            {
                return (int)TimeMgr.Instance.CurrentTime.GetDayOfWeek();
            }
            if (variableName == "FireResistance")
            {
                return character?.GetBaseResistance(SpellElement.Fire) ?? 0;
            }
            if (variableName == "AirResistance")
            {
                return character?.GetBaseResistance(SpellElement.Air) ?? 0;
            }
            if (variableName == "WaterResistance")
            {
                return character?.GetBaseResistance(SpellElement.Water) ?? 0;
            }
            if (variableName == "EarthResistance")
            {
                return character?.GetBaseResistance(SpellElement.Earth) ?? 0;
            }
            if (variableName == "FireResBonus")
            {
                return character == null ? 0 : GetResistanceBonusValue(character, SpellElement.Fire);
            }

            Debug.LogWarning("EventAPI.GetVar does not support variable yet: " + variableName);
            return 0;
        }

        static public void AddVar(string variableName, int value, Character character = null)
        {
            if (variableName == "QBits")
            {
                AddQuestBit(value);
                return;
            }
            if (variableName == "AutonotesBits")
            {
                AddAutonote(value);
                return;
            }
            if (variableName == "Gold")
            {
                AddGold(value);
                return;
            }
            if (variableName == "Food")
            {
                AddFood(value);
                return;
            }
            if (variableName == "Experience")
            {
                AddExperience(character, value);
                return;
            }
            if (variableName == "SkillPoints" && character != null)
            {
                character.SkillPoints += value;
                return;
            }
            if (variableName == "Inventory")
            {
                AddItem(character, value);
                return;
            }
            if (variableName == "Awards")
            {
                AddAward(character, value);
                return;
            }
            if (variableName == "HP" && character != null)
            {
                character.AddCurrHitPoints(value);
                return;
            }
            if (variableName == "SP" && character != null)
            {
                character.AddCurrSpellPoints(value);
                return;
            }
            if (variableName == "BaseLuck" && character != null)
            {
                character.BaseAttributes[CharAttribute.Luck] += value;
                return;
            }
            if (variableName == "BaseMight" && character != null)
            {
                character.BaseAttributes[CharAttribute.Might] += value;
                return;
            }
            if (variableName == "BaseIntellect" && character != null)
            {
                character.BaseAttributes[CharAttribute.Intellect] += value;
                return;
            }
            if (variableName == "BasePersonality" && character != null)
            {
                character.BaseAttributes[CharAttribute.Personality] += value;
                return;
            }
            if (variableName == "BaseEndurance" && character != null)
            {
                character.BaseAttributes[CharAttribute.Endurance] += value;
                return;
            }
            if (variableName == "BaseAccuracy" && character != null)
            {
                character.BaseAttributes[CharAttribute.Accuracy] += value;
                return;
            }
            if (variableName == "BaseSpeed" && character != null)
            {
                character.BaseAttributes[CharAttribute.Speed] += value;
                return;
            }
            if (variableName == "MightBonus" && character != null)
            {
                AddAttributeBonusOverride(character, CharAttribute.Might, value);
                return;
            }
            if (variableName == "IntellectBonus" && character != null)
            {
                AddAttributeBonusOverride(character, CharAttribute.Intellect, value);
                return;
            }
            if (variableName == "PersonalityBonus" && character != null)
            {
                AddAttributeBonusOverride(character, CharAttribute.Personality, value);
                return;
            }
            if (variableName == "FireResistance" && character != null)
            {
                character.BaseResistances[SpellElement.Fire] += value;
                return;
            }
            if (variableName == "AirResistance" && character != null)
            {
                character.BaseResistances[SpellElement.Air] += value;
                return;
            }
            if (variableName == "WaterResistance" && character != null)
            {
                character.BaseResistances[SpellElement.Water] += value;
                return;
            }
            if (variableName == "EarthResistance" && character != null)
            {
                character.BaseResistances[SpellElement.Earth] += value;
                return;
            }
            if (variableName == "FireResBonus" && character != null)
            {
                AddResistanceBonusOverride(character, SpellElement.Fire, value);
                return;
            }
            if (variableName == "PlayerBits" && character != null)
            {
                SetPlayerBit(character, value, true);
                return;
            }
            if (variableName.StartsWith("Counter", StringComparison.Ordinal))
            {
                StampCounter(variableName);
                return;
            }
            if (variableName.StartsWith("MapVar", StringComparison.Ordinal))
            {
                SetMapVar(variableName, GetMapVar(variableName) + value);
                return;
            }
            if (variableName.StartsWith("History", StringComparison.Ordinal))
            {
                AddHistory(variableName);
                return;
            }

            Debug.LogWarning("EventAPI.AddVar does not support variable yet: " + variableName);
        }

        static public void SubtractVar(string variableName, int value, Character character = null)
        {
            if (variableName == "QBits")
            {
                RemoveQuestBit(value);
                return;
            }
            if (variableName == "Inventory")
            {
                RemoveItems(character, value, 1);
                return;
            }
            if (variableName == "AutonotesBits")
            {
                RemoveAutonote(value);
                return;
            }
            if (variableName == "Awards")
            {
                RemoveAward(character, value);
                return;
            }
            if (variableName == "Gold")
            {
                AddGold(-value);
                return;
            }
            if (variableName == "Food")
            {
                AddFood(-value);
                return;
            }
            if (variableName.StartsWith("MapVar", StringComparison.Ordinal))
            {
                SetMapVar(variableName, GetMapVar(variableName) - value);
                return;
            }
            if (variableName == "PlayerBits" && character != null)
            {
                SetPlayerBit(character, value, false);
                return;
            }

            Debug.LogWarning("EventAPI.SubtractVar does not support variable yet: " + variableName);
        }

        static public void SetVar(string variableName, int value, Character character = null)
        {
            if (variableName.StartsWith("MapVar", StringComparison.Ordinal))
            {
                SetMapVar(variableName, value);
                return;
            }
            if (variableName == "QBits")
            {
                AddQuestBit(value);
                return;
            }
            if (variableName == "AutonotesBits")
            {
                AddAutonote(value);
                return;
            }
            if (variableName == "ClassIs" && character != null)
            {
                character.Class = (CharacterClass)value;
                return;
            }
            if (variableName.StartsWith("Counter", StringComparison.Ordinal))
            {
                StampCounter(variableName);
                return;
            }
            if (variableName == "Dead" && character != null)
            {
                SetConditionState(character, Condition.Dead, value != 0);
                return;
            }
            if (variableName == "Drunk" && character != null)
            {
                SetConditionState(character, Condition.Drunk, value != 0);
                return;
            }
            if (variableName == "PoisonedGreen" && character != null)
            {
                SetConditionState(character, Condition.PoisonWeak, value != 0);
                return;
            }
            if (variableName == "PoisonedYellow" && character != null)
            {
                SetConditionState(character, Condition.PoisonMedium, value != 0);
                return;
            }
            if (variableName == "DiseasedGreen" && character != null)
            {
                SetConditionState(character, Condition.DiseaseWeak, value != 0);
                return;
            }
            if (variableName == "DiseasedYellow" && character != null)
            {
                SetConditionState(character, Condition.DiseaseMedium, value != 0);
                return;
            }
            if (variableName == "Gold")
            {
                PlayerParty party = Party();
                if (party != null)
                {
                    party.Gold = value;
                }
                return;
            }
            if (variableName == "Food")
            {
                PlayerParty party = Party();
                if (party != null)
                {
                    party.Food = value;
                }
                return;
            }
            if (variableName == "BankGold")
            {
                s_BankGold = value;
                return;
            }

            Debug.LogWarning("EventAPI.SetVar does not support variable yet: " + variableName);
        }

        static private PlayerParty Party()
        {
            if (GameCore.Instance == null)
            {
                return null;
            }

            return GameCore.Instance.PlayerParty;
        }

        static public bool HasItem(Character chr, int itemId)
        {
            if (chr == null)
            {
                return false;
            }

            if (chr.Inventory.InventoryItems.Any(item => item.Data.Id == itemId))
            {
                return true;
            }

            PlayerParty party = Party();
            return party != null && party.GetHeldItem() != null && party.GetHeldItem().Data.Id == itemId;
        }

        static private int CountItems(int itemId)
        {
            return PartyCharacters().Sum(chr => chr.Inventory.InventoryItems.Count(item => item.Data.Id == itemId));
        }

        static private int GetGold()
        {
            PlayerParty party = Party();
            return party != null ? party.Gold : 0;
        }

        static private int GetFood()
        {
            PlayerParty party = Party();
            return party != null ? party.Food : 0;
        }

        static private bool CharacterHasFullHP(Character character)
        {
            return character != null && character.CurrHitPoints >= character.GetMaxHitPoints();
        }

        static private bool CharacterHasFullSP(Character character)
        {
            return character != null && character.CurrSpellPoints >= character.GetMaxSpellPoints();
        }

        static private bool IsMonsterDefeated(Monster monster, bool invisibleAsDead)
        {
            if (monster == null)
            {
                return true;
            }

            if (monster.AIState == MonsterState.Dead ||
                monster.AIState == MonsterState.Dying ||
                monster.AIState == MonsterState.Disabled ||
                monster.AIState == MonsterState.Removed)
            {
                return true;
            }

            if (invisibleAsDead && monster.SpriteRenderer != null && !monster.SpriteRenderer.enabled)
            {
                return true;
            }

            return false;
        }

        static private void RemoveAward(Character character, int awardId)
        {
            if (character == null)
            {
                return;
            }

            Award award = character.Awards.FirstOrDefault(a => a.AwardId == awardId);
            if (award != null)
            {
                character.Awards.Remove(award);
            }
        }

        static private bool RemoveItems(Character character, int itemId, int count)
        {
            if (character == null || count <= 0)
            {
                return false;
            }

            int removed = 0;
            while (removed < count)
            {
                Item item = character.Inventory.InventoryItems.FirstOrDefault(invItem => invItem.Data.Id == itemId);
                if (item == null)
                {
                    break;
                }

                character.Inventory.RemoveItem(item);
                removed++;
            }

            return removed == count;
        }

        static public bool HasAutonote(int autonoteId)
        {
            return s_Autonotes.Contains(autonoteId);
        }

        static public bool HasHistoryEntry(string historyName)
        {
            return !string.IsNullOrEmpty(historyName) && s_History.Contains(historyName);
        }

        static public bool HasPlayerBit(Character character, int bitId)
        {
            return character != null &&
                s_PlayerBits.ContainsKey(character.CharacterId) &&
                s_PlayerBits[character.CharacterId].Contains(bitId);
        }

        static public bool IsCounterElapsed(string counterName, int hours)
        {
            if (!s_Counters.ContainsKey(counterName) || !s_Counters[counterName].IsValid())
            {
                return false;
            }

            long targetSeconds = s_Counters[counterName].GetSeconds() + GameTime.FromHours(hours).GetSeconds();
            return TimeMgr.GetCurrentTime().GetSeconds() >= targetSeconds;
        }

        static public void StampCounter(string counterName)
        {
            s_Counters[counterName] = GameTime.FromCurrentTime(0);
        }

        static private void SetPlayerBit(Character character, int bitId, bool on)
        {
            if (character == null)
            {
                return;
            }

            if (!s_PlayerBits.ContainsKey(character.CharacterId))
            {
                s_PlayerBits[character.CharacterId] = new HashSet<int>();
            }

            if (on)
            {
                s_PlayerBits[character.CharacterId].Add(bitId);
            }
            else
            {
                s_PlayerBits[character.CharacterId].Remove(bitId);
            }
        }

        static private int GetAttributeBonusOverride(Character character, CharAttribute attribute)
        {
            if (character == null ||
                !s_AttributeBonusOverrides.ContainsKey(character.CharacterId) ||
                !s_AttributeBonusOverrides[character.CharacterId].ContainsKey(attribute))
            {
                return 0;
            }

            return s_AttributeBonusOverrides[character.CharacterId][attribute];
        }

        static private void AddAttributeBonusOverride(Character character, CharAttribute attribute, int value)
        {
            if (character == null)
            {
                return;
            }

            if (!s_AttributeBonusOverrides.ContainsKey(character.CharacterId))
            {
                s_AttributeBonusOverrides[character.CharacterId] =
                    new Dictionary<CharAttribute, int>();
            }

            if (!s_AttributeBonusOverrides[character.CharacterId].ContainsKey(attribute))
            {
                s_AttributeBonusOverrides[character.CharacterId][attribute] = 0;
            }

            s_AttributeBonusOverrides[character.CharacterId][attribute] += value;
        }

        static private int GetAttributeBonusValue(Character character, CharAttribute attribute)
        {
            if (character == null)
            {
                return 0;
            }

            return GetAttributeBonusOverride(character, attribute);
        }

        static private int GetResistanceBonusOverride(Character character, SpellElement resistance)
        {
            if (character == null ||
                !s_ResistanceBonusOverrides.ContainsKey(character.CharacterId) ||
                !s_ResistanceBonusOverrides[character.CharacterId].ContainsKey(resistance))
            {
                return 0;
            }

            return s_ResistanceBonusOverrides[character.CharacterId][resistance];
        }

        static private void AddResistanceBonusOverride(Character character, SpellElement resistance, int value)
        {
            if (character == null)
            {
                return;
            }

            if (!s_ResistanceBonusOverrides.ContainsKey(character.CharacterId))
            {
                s_ResistanceBonusOverrides[character.CharacterId] =
                    new Dictionary<SpellElement, int>();
            }

            if (!s_ResistanceBonusOverrides[character.CharacterId].ContainsKey(resistance))
            {
                s_ResistanceBonusOverrides[character.CharacterId][resistance] = 0;
            }

            s_ResistanceBonusOverrides[character.CharacterId][resistance] += value;
        }

        static private int GetResistanceBonusValue(Character character, SpellElement resistance)
        {
            if (character == null)
            {
                return 0;
            }

            return GetResistanceBonusOverride(character, resistance);
        }

        static private bool CharacterHasCondition(Character character, Condition condition)
        {
            if (character == null || !character.Conditions.ContainsKey(condition))
            {
                return false;
            }

            return character.Conditions[condition].IsValid();
        }

        static private void SetConditionState(Character character, Condition condition, bool on)
        {
            if (character == null)
            {
                return;
            }

            if (on)
            {
                character.SetCondition(condition, false);
                character.Condition = condition;
                return;
            }

            character.RemoveCondition(condition);

            if (condition == Condition.Dead)
            {
                character.Condition = Condition.Good;
                if (character.CurrHitPoints <= 0)
                {
                    character.CurrHitPoints = 1;
                }
            }
        }

        static private bool IsBooleanVar(string variableName)
        {
            return variableName == "Invisible" ||
                variableName == "HasFullHP" ||
                variableName == "HasFullSP" ||
                variableName == "Dead" ||
                variableName == "Drunk" ||
                variableName == "PoisonedGreen" ||
                variableName == "PoisonedYellow" ||
                variableName == "DiseasedGreen" ||
                variableName == "DiseasedYellow";
        }

        static private void SetBitState<TBit>(Dictionary<int, HashSet<TBit>> map, int id, TBit bit, bool on)
        {
            if (!map.ContainsKey(id))
            {
                map[id] = new HashSet<TBit>();
            }

            if (on)
            {
                map[id].Add(bit);
            }
            else
            {
                map[id].Remove(bit);
            }
        }
    }
}
