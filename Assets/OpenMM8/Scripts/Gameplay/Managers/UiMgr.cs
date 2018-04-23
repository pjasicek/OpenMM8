using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay.Managers
{
    public class UiMgr : Singleton<UiMgr>
    {
        // Public
        public CharacterUI CharacterUI = new CharacterUI();
        public InspectNpcUI InspectNpcUI = new InspectNpcUI();
        public MapQuestNotesUI MapQuestNotesUI = new MapQuestNotesUI();
        public NpcTalkUI NpcTalkUI = new NpcTalkUI();
        public PartyUI PartyUI = new PartyUI();

        // Private
        private GameObject m_PartyCanvasObj;
        private GameObject m_NpcInfoCanvasObj;
        private PlayerParty m_PlayerParty;
        private float m_TimeSinceLastPartyText = 0.0f;
        private float m_PartyTextLockTime = 0.0f;

        //=================================== Unity Lifecycle ===================================

        private void Awake()
        {
            
        }

        // Init sequence: DbMgr(1) -> GameMgr(1) -> UiMgr(1) -> GameMgr(2)
        public bool Init()
        {
            m_PartyCanvasObj = GameObject.Find("PartyCanvas");
            m_NpcInfoCanvasObj = GameObject.Find("NpcInfoCanvas");
            m_PlayerParty = GameMgr.Instance.PlayerParty;

            // Events
            m_PlayerParty.OnCharacterJoinedParty += OnCharacterJoinedParty;
            m_PlayerParty.OnCharacterLeftParty += OnCharacterLeftParty;
            m_PlayerParty.OnGoldChanged += OnGoldChanged;
            m_PlayerParty.OnFoodChanged += OnFoodChanged;
            m_PlayerParty.OnFoundGold += OnFoundGold;
            m_PlayerParty.OnHoverObject += OnHoverObject;

            return true;
        }

        private void OnCharacterJoinedParty(Character chr, PlayerParty party)
        {
            // Hook events
            chr.OnHealthChanged += OnCharHealthChanged;
            chr.OnManaChanged += OnCharManaChanged;
            chr.OnRecovered += OnCharRecovered;
            chr.OnRecoveryTimeChanged += OnCharRecoveryTimeChanged;
            chr.OnHitNpc += OnCharHitNpc;
        }

        private void OnCharacterLeftParty(Character chr, PlayerParty party)
        {

        }

        private void Start()
        {
            
        }

        private void Update()
        {
            m_TimeSinceLastPartyText += Time.deltaTime;
            if (m_TimeSinceLastPartyText > 2.0f)
            {
                SetPartyInfoText("", false);
            }

            m_PartyTextLockTime -= Time.deltaTime;
        }

        //=================================== Methods ===================================

        Ray GetCrosshairRay()
        {
            return Camera.main.ViewportPointToRay(Constants.CrosshairScreenRelPos);
        }

        private void SetPartyInfoText(string text, bool forceRewrite = true)
        {
            if (!forceRewrite)
            {
                if (m_PartyTextLockTime < 0)
                {
                    GameMgr.Instance.PartyUI.HoverInfoText.text = text;
                    m_TimeSinceLastPartyText = 0.0f;
                }

                return;
            }

            GameMgr.Instance.PartyUI.HoverInfoText.text = text;
            m_TimeSinceLastPartyText = 0.0f;
            m_PartyTextLockTime = 2.0f;
        }

        //=================================== Events ===================================

        public void OnCharHealthChanged(Character chr, int maxHealth, int currHealth)
        {
            float healthPercent = ((float)currHealth / (float)maxHealth) * 100.0f;
            chr.UI.SetHealth(healthPercent);

            Debug.Log(maxHealth + " " + currHealth);
        }

        public void OnCharManaChanged(Character chr, int maxMana, int currMana)
        {

        }

        public void OnFoundGold(int amount)
        {
            SetPartyInfoText("You found " + amount.ToString() + " gold !");
        }

        public void OnGoldChanged(int oldGold, int newGold, int delta)
        {
            
        }

        public void OnFoodChanged(int oldFood, int newFood, int delta)
        {

        }

        public void OnCharRecovered(Character chr)
        {
            chr.UI.AgroStatus.enabled = true;
        }

        public void OnCharRecoveryTimeChanged(Character chr, float recoveryTime)
        {
            if (recoveryTime > 0.0f)
            {
                chr.UI.AgroStatus.enabled = false;
            }
        }

        public void OnCharConditionChanged(Character chr, Condition newCondition)
        {

        }

        public void OnCharHitNpc(Character chr, AttackInfo attackInfo, AttackResult result)
        {
            string hitText = "";
            switch (result.Type)
            {
                case AttackResultType.Hit:
                    hitText = chr.Data.Name + " hits " + result.HitObjectName + " for " + result.DamageDealt + " damage";
                    break;

                case AttackResultType.Kill:
                    hitText = chr.Data.Name + " inflicts " + result.DamageDealt + " points killing " + result.HitObjectName;
                    break;

                case AttackResultType.Miss:
                    hitText = chr.Data.Name + " missed attack on " + result.HitObjectName;
                    break;

                default:
                    Logger.LogError("Unknown attack result type: " + result.Type);
                    break;
            }

            SetPartyInfoText(hitText);
        }

        public void OnCharGotHit(Character chr, AttackInfo attackInfo, AttackResult attackResult)
        {

        }

        public void OnHoverObject(HoverInfo hoverInfo)
        {
            SetPartyInfoText(hoverInfo.HoverText, false);
        }

        public void OnNpcInspect(Character inspectorChr, NpcData npcData)
        {

        }

        public void OnNpcInspectEnd()
        {

        }

        public void OnItemInspect(Character inspectorChr, ItemData itemData/*, InspectResult result*/)
        {

        }

        public void OnItemInspectEnd()
        {

        }

        public void OnItemEquip(/*Item item, EquipResult equipResult*/)
        {

        }

        public void OnItemHold(ItemData item)
        {

        }

        public void OnItemHoldEnd()
        {

        }
    }
}
