using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public delegate void DollClicked(DollClickHandler sender);
    public delegate void CharacterAvatarClicked(Character chr);
    public delegate void OutdoorItemInspectStart(Item item);
    public delegate void OutdoorItemInspectEnd(Item item);
    public delegate void NpcInspectStartDlg(Character inspector, BaseNpc npc, MonsterData npcData);
    public delegate void NpcInspecEndDlg(Character inspector, BaseNpc npc, MonsterData npcData);
    public delegate void InventoryCellClicked(int x, int y);
    public delegate void InventoryItemHoverStart(InventoryItem inventoryItem);
    public delegate void InventoryItemHoverEnd(InventoryItem inventoryItem);
    public delegate void InventoryItemClicked(InventoryItem inventoryItem);
    public delegate void CharacterFinishedEvent(Character character);
    public delegate void RefreshNpcTalk(NpcTalkProperties talkProp);
    public delegate void NpcTalkTextChanged(string text);
    public delegate void TalkWithConcreteNpc(NpcTalkProperties talkProp);
    public delegate void NpcLeavingLocation(NpcTalkProperties talkProp);
    public delegate void TalkSceneStart(Character talkerChr, TalkScene talkScene);
    public delegate void TalkSceneEnd(Character talkerChr, TalkScene talkScene);
    public delegate void PauseGame();
    public delegate void UnpauseGame();
    public delegate void GamePausedAction();
    public delegate void GameUnpausedAction();
    public delegate void InitComplete();
    public delegate void MinuteElapsed(GameTime currTime);
    public delegate void HealthChanged(Character chr, int maxHealth, int currHealth, int delta);
    public delegate void ManaChanged(Character chr, int maxMana, int currMana);
    public delegate void Recovered(Character chr);
    public delegate void RecoveryTimeChanged(Character chr, float recoveryTime);
    public delegate void CharConditionChanged(Character chr, Condition newCondition);
    public delegate void CharHitNpc(Character chr, AttackInfo attackInfo, AttackResult result);
    public delegate void CharGotHit(Character chr, AttackInfo attackInfo, AttackResult attackResult);
    public delegate void CharAttack(Character chr, AttackInfo attackInfo);
    public delegate void NpcInspect(Character inspectorChr, MonsterData npcData);
    public delegate void NpcInspectEnd();
    public delegate void ItemInspect(Character inspectorChr, ItemData itemData/*, InspectResult result*/);
    public delegate void ItemEquip(/*Item item, EquipResult equipResult*/);
    public delegate void ItemHold(/*Item item*/);
    public delegate void ItemHoldEnd();
    public delegate void ItemEquipped(Character chr, Item equippedItem, Item replacedItem);
    public delegate void ItemUnequipped(Character chr, Item unequippedItem);
    public delegate void InteractedWithItem(Character chr, Item item, ItemInteractResult interactResult);
    public delegate void CharacterJoinedParty(Character chr, PlayerParty party);
    public delegate void CharacterLeftParty(Character chr, PlayerParty party);
    public delegate void HoverObject(HoverInfo hoverInfo);
    public delegate void GoldChanged(int oldGold, int newGold, int delta);
    public delegate void FoodChanged(int oldFood, int newFood, int delta);
    public delegate void FoundGold(int amount);
    public delegate void PickedUpLoot(Loot loot);
    public delegate void ActiveCharacterChanged(Character newSelChar);
    public delegate void QuestBitAdded(int questId);
    public delegate void MinimapMarkerCreatedDlg(MinimapMarker marker);
    public delegate void MinimapMarkerDestroyedDlg(MinimapMarker marker);
    public delegate void InspectableUiTextHoverStart(InspectableUiText inspectableUiText);
    public delegate void InspectableUiTextHoverEnd(InspectableUiText inspectableUiText);

    public class GameEvents
    {
        static public event DollClicked OnDollClicked;
        static public event CharacterAvatarClicked OnCharacterAvatarClicked;
        static public event OutdoorItemInspectStart OnOutdoorItemInspectStart;
        static public event OutdoorItemInspectEnd OnOutdoorItemInspectEnd;
        static public event NpcInspectStartDlg OnNpcInspectStart;
        static public event NpcInspecEndDlg OnNpcInspectEnd;
        static public event InventoryCellClicked OnInventoryCellClicked;
        static public event InventoryItemHoverStart OnInventoryItemHoverStart;
        static public event InventoryItemHoverEnd OnInventoryItemHoverEnd;
        static public event InventoryItemClicked OnInventoryItemClicked;
        public static event NpcTalkTextChanged OnNpcTalkTextChanged;
        public static event RefreshNpcTalk OnRefreshNpcTalk;
        public static event TalkWithConcreteNpc OnTalkWithConcreteNpc;
        public static event NpcLeavingLocation OnNpcLeavingLocation;
        static public event TalkSceneStart OnTalkSceneStart;
        static public event TalkSceneEnd OnTalkSceneEnd;
        static public event PauseGame OnPauseGame;
        static public event PauseGame OnUnpauseGame;
        static public event InitComplete OnInitComplete;
        static public event HealthChanged OnCharHealthChanged;
        static public event ManaChanged OnCharManaChanged;
        static public event Recovered OnRecovered;
        static public event RecoveryTimeChanged OnRecoveryTimeChanged;
        static public event CharConditionChanged OnConditionChanged;
        static public event CharHitNpc OnCharHitNpc;
        static public event CharGotHit OnCharGotHit;
        static public event CharAttack OnCharAttack;
        static public event NpcInspect OnNpcInspect;
        static public event ItemInspect OnItemInspect;
        static public event ItemEquip OnItemEquip;
        static public event ItemHold OnItemHold;
        static public event ItemHoldEnd OnItemHoldEnd;
        static public event ItemEquipped OnItemEquipped;
        static public event ItemUnequipped onItemUnequipped;
        static public event InteractedWithItem OnInteractedWithItem;
        static public event CharacterJoinedParty OnCharacterJoinedParty;
        static public event CharacterLeftParty OnCharacterLeftParty;
        static public event HoverObject OnHoverObject;
        static public event GoldChanged OnGoldChanged;
        static public event FoodChanged OnFoodChanged;
        static public event FoundGold OnFoundGold;
        static public event PickedUpLoot OnPickedUpLoot;
        static public event ActiveCharacterChanged OnActiveCharacterChanged;
        static public event MinimapMarkerCreatedDlg OnMinimapMarkerCreated;
        static public event MinimapMarkerDestroyedDlg OnMinimapMarkerDestroyed;
        public static event CharacterFinishedEvent OnCharacterFinishedEvent;
        public static event MinuteElapsed OnMinuteElapsed;
        public static event QuestBitAdded OnQuestBitAdded;
        public static event InspectableUiTextHoverStart OnInspectableUiTextHoverStart;
        public static event InspectableUiTextHoverEnd OnInspectableUiTextHoverEnd;

        static public void InvokeEvent_OnInspectableUiTextHoverStart(InspectableUiText inspectableUiText)
        {
            OnInspectableUiTextHoverStart?.Invoke(inspectableUiText);
        }

        static public void InvokeEvent_OnInspectableUiTextHoverEnd(InspectableUiText inspectableUiText)
        {
            OnInspectableUiTextHoverEnd?.Invoke(inspectableUiText);
        }

        static public void InvokeEvent_OnDollClicked(DollClickHandler sender)
        {
            OnDollClicked?.Invoke(sender);
        }

        static public void InvokeEvent_OnCharacterAvatarClicked(Character chr)
        {
            OnCharacterAvatarClicked?.Invoke(chr);
        }

        static public void InvokeEvent_OnOutdoorItemInspectStart(Item item)
        {
            OnOutdoorItemInspectStart?.Invoke(item);
        }

        static public void InvokeEvent_OnOutdoorItemInspectEnd(Item item)
        {
            OnOutdoorItemInspectEnd?.Invoke(item);
        }

        static public void InvokeEvent_OnNpcInspectStart(Character inspector, BaseNpc npc, MonsterData npcData)
        {
            OnNpcInspectStart?.Invoke(inspector, npc, npcData);
        }

        static public void InvokeEvent_OnNpcInspectEnd(Character inspector, BaseNpc npc, MonsterData npcData)
        {
            OnNpcInspectEnd(inspector, npc, npcData);
        }

        static public void InvokeEvent_OnInventoryCellClicked(int x, int y)
        {
            OnInventoryCellClicked?.Invoke(x, y);
        }

        static public void InvokeEvent_OnInventoryItemHoverStart(InventoryItem inventoryItem)
        {
            OnInventoryItemHoverStart?.Invoke(inventoryItem);
        }

        static public void InvokeEvent_OnInventoryItemHoverEnd(InventoryItem inventoryItem)
        {
            OnInventoryItemHoverEnd?.Invoke(inventoryItem);
        }

        static public void InvokeEvent_OnInventoryItemClicked(InventoryItem inventoryItem)
        {
            OnInventoryItemClicked?.Invoke(inventoryItem);
        }

        static public void InvokeEvent_OnCharacterFinishedEvent(Character character)
        {
            OnCharacterFinishedEvent?.Invoke(character);
        }

        static public void InvokeEvent_OnRefreshNpcTalk(NpcTalkProperties talkProp)
        {
            OnRefreshNpcTalk?.Invoke(talkProp);
        }

        static public void InvokeEvent_OnNpcTalkTextChanged(string text)
        {
            OnNpcTalkTextChanged?.Invoke(text);
        }

        static public void InvokeEvent_OnTalkWithConcreteNpc(NpcTalkProperties talkProp)
        {
            OnTalkWithConcreteNpc?.Invoke(talkProp);
        }

        static public void InvokeEvent_OnNpcLeavingLocation(NpcTalkProperties talkProp)
        {
            OnNpcLeavingLocation?.Invoke(talkProp);
        }

        static public void InvokeEvent_OnTalkSceneStart(Character talkerChr, TalkScene talkScene)
        {
            OnTalkSceneStart?.Invoke(talkerChr, talkScene);
        }

        static public void InvokeEvent_OnTalkSceneEnd(Character talkerChr, TalkScene talkScene)
        {
            OnTalkSceneEnd?.Invoke(talkerChr, talkScene);
        }

        static public void InvokeEvent_OnPauseGame()
        {
            OnPauseGame?.Invoke();
        }

        static public void InvokeEvent_OnUnpauseGame()
        {
            OnUnpauseGame?.Invoke();
        }

        static public void InvokeEvent_OnInitComplete()
        {
            OnInitComplete?.Invoke();
        }

        static public void InvokeEvent_OnMinuteElapsed(GameTime currTime)
        {
            OnMinuteElapsed?.Invoke(currTime);
        }

        static public void InvokeEvent_OnCharHealthChanged(Character chr, int maxHealth, int currHealth, int delta)
        {
            OnCharHealthChanged?.Invoke(chr, maxHealth, currHealth, delta);
        }

        static public void InvokeEvent_OnCharManaChanged(Character chr, int maxMana, int currMana)
        {
            OnCharManaChanged?.Invoke(chr, maxMana, currMana);
        }

        static public void InvokeEvent_OnRecovered(Character chr)
        {
            OnRecovered?.Invoke(chr);
        }

        static public void InvokeEvent_OnRecoveryTimeChanged(Character chr, float recoveryTime)
        {
            OnRecoveryTimeChanged?.Invoke(chr, recoveryTime);
        }

        static public void InvokeEvent_OnCharConditionChanged(Character chr, Condition newCondition)
        {
            OnConditionChanged?.Invoke(chr, newCondition);
        }

        static public void InvokeEvent_OnCharHitNpc(Character chr, AttackInfo attackInfo, AttackResult result)
        {
            OnCharHitNpc?.Invoke(chr, attackInfo, result);
        }

        static public void InvokeEvent_OnCharGotHit(Character chr, AttackInfo attackInfo, AttackResult attackResult)
        {
            OnCharGotHit?.Invoke(chr, attackInfo, attackResult);
        }

        static public void InvokeEvent_OnCharAttack(Character chr, AttackInfo attackInfo)
        {
            OnCharAttack?.Invoke(chr, attackInfo);
        }

        static public void InvokeEvent_OnItemInspect(Character inspectorChr, ItemData itemData/*, InspectResult result*/)
        {
            OnItemInspect?.Invoke(inspectorChr, itemData);
        }

        static public void InvokeEvent_OnItemEquipped(Character chr, Item equippedItem, Item replacedItem)
        {
            OnItemEquipped?.Invoke(chr, equippedItem, replacedItem);
        }

        static public void InvokeEvent_OnItemUnequipped(Character chr, Item unequippedItem)
        {
            onItemUnequipped?.Invoke(chr, unequippedItem);
        }

        static public void InvokeEvent_OnInteractedWithItem(Character chr, Item item, ItemInteractResult interactResult)
        {
            OnInteractedWithItem?.Invoke(chr, item, interactResult);
        }

        static public void InvokeEvent_OnCharacterJoinedParty(Character chr, PlayerParty party)
        {
            OnCharacterJoinedParty?.Invoke(chr, party);
        }

        static public void InvokeEvent_OnCharacterLeftParty(Character chr, PlayerParty party)
        {
            OnCharacterLeftParty?.Invoke(chr, party);
        }

        static public void InvokeEvent_OnHoverObject(HoverInfo hoverInfo)
        {
            OnHoverObject?.Invoke(hoverInfo);
        }

        static public void InvokeEvent_OnGoldChanged(int oldGold, int newGold, int delta)
        {
            OnGoldChanged?.Invoke(oldGold, newGold, delta);
        }

        static public void InvokeEvent_OnFoodChanged(int oldFood, int newFood, int delta)
        {
            OnFoodChanged?.Invoke(oldFood, newFood, delta);
        }

        static public void InvokeEvent_OnFoundGold(int amount)
        {
            OnFoundGold?.Invoke(amount);
        }

        static public void InvokeEvent_OnPickedUpLoot(Loot loot)
        {
            OnPickedUpLoot?.Invoke(loot);
        }

        static public void InvokeEvent_OnActiveCharacterChanged(Character newSelChar)
        {
            OnActiveCharacterChanged?.Invoke(newSelChar);
        }

        static public void InvokeEvent_OnQuestBitAdded(int questId)
        {
            OnQuestBitAdded?.Invoke(questId);
        }

        static public void InvokeEvent_OnMinimapMarkerCreated(MinimapMarker marker)
        {
            OnMinimapMarkerCreated?.Invoke(marker);
        }

        static public void InvokeEvent_OnMinimapMarkerDestroyed(MinimapMarker marker)
        {
            OnMinimapMarkerDestroyed?.Invoke(marker);
        }
    }
}
