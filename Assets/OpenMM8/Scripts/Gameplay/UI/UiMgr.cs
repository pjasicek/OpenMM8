using Assets.OpenMM8.Scripts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public partial class UiMgr : Singleton<UiMgr>
    {
        public RawImage SceneVideoImage;

        // Public
        [Header("UI - Canvases")]
        private Canvas m_PartyCanvas;
        private Canvas m_PartyBuffsAndButtonsCanvas;
        private Canvas m_PartyInventoryCanvas;
        private Canvas m_HouseCanvas;

        // Private
        private GameObject m_PartyCanvasObj;
        private GameObject m_NpcInfoCanvasObj;
        private PlayerParty m_PlayerParty;
        private float m_TimeSinceLastPartyText = 0.0f;
        private float m_PartyTextLockTime = 0.0f;

        // State
        private UIState m_CurrUIState = null;

        [Header("UI")]
        private InspectNpcUI m_InspectNpcUI;
        private PartyUI m_PartyUI;
        private NpcTalkUI m_NpcTalkUI;
        private Minimap m_Minimap;
        private Image m_MinimapCloseButtonImage;
        private MapQuestNotesUI m_MapQuestNotesUI;
        private List<Image> m_EmptySlotBanners = new List<Image>();
        private CharDetailUI m_CharDetailUI;

        [Header("UI - Map, Quest, Notes, History")]
        int placeholder;

        private Dictionary<int, Sprite> m_NpcAvatarsMap = 
            new Dictionary<int, Sprite>();

        private Dictionary<CharacterType, CharacterSprites> m_CharacterSpritesMap =
            new Dictionary<CharacterType, CharacterSprites>();

        [Header("Sprites")]
        private Sprite[] m_QuestEffectSprites;

        //=================================== Unity Lifecycle ===================================

        private void Awake()
        {
            GameMgr.OnPauseGame += OnPauseGame;

            PlayerParty.OnCharacterJoinedParty += OnCharacterJoinedParty;
            PlayerParty.OnCharacterLeftParty += OnCharacterLeftParty;
            PlayerParty.OnGoldChanged += OnGoldChanged;
            PlayerParty.OnFoodChanged += OnFoodChanged;
            PlayerParty.OnFoundGold += OnFoundGold;
            PlayerParty.OnHoverObject += OnHoverObject;

            Character.OnHealthChanged += OnCharHealthChanged;
            Character.OnManaChanged += OnCharManaChanged;
            Character.OnRecovered += OnCharRecovered;
            Character.OnRecoveryTimeChanged += OnCharRecoveryTimeChanged;
            Character.OnHitNpc += OnCharHitNpc;
            Character.OnGotHit += OnCharGotHit;
            Character.OnAttack += OnCharAttack;

            InspectableNpc.OnNpcInspectStart += OnNpcInspectStart;
            InspectableNpc.OnNpcInspectEnd += OnNpcInspectEnd;

            MinimapMarker.OnMinimapMarkerCreated += OnMinimapMarkerCreated;
            MinimapMarker.OnMinimapMarkerDestroyed += OnMinimapMarkerDestroyed;

            TalkEventMgr.OnTalkSceneStart += OnTalkSceneStart;

            
            EventAPI.OnCharacterFinishedEvent += OnCharacterFinishedEvent;

            QuestMgr.OnQuestBitAdded += OnQuestBitAdded;
        }

        private void Start()
        {
            m_PartyCanvasObj = GameObject.Find("PartyCanvas");
            if (m_PartyCanvasObj != null)
            {
                m_PartyCanvas = m_PartyCanvasObj.GetComponent<Canvas>();
                m_PartyBuffsAndButtonsCanvas = m_PartyCanvasObj.transform.Find("BuffsAndButtonsCanvas").GetComponent<Canvas>();
                m_Minimap = m_PartyCanvasObj.transform.Find("BuffsAndButtonsCanvas").Find("Minimap").GetComponent<Minimap>();
            }
            else
            {
                Debug.LogError("Could not find PartyCanvas gameobject !");
            }

            m_MinimapCloseButtonImage = m_PartyCanvasObj.transform.Find("MinimapCloseButton").GetComponent<Image>();

            m_MapQuestNotesUI = new MapQuestNotesUI();
            GameObject mapQuestNotesObject = m_PartyCanvasObj.transform.Find("MapQuestNotesCanvas").gameObject;
            m_MapQuestNotesUI.Canvas = mapQuestNotesObject.GetComponent<Canvas>();
            m_MapQuestNotesUI.MapNameText = mapQuestNotesObject.transform.Find("MapCanvas").transform.Find("MapNameText").GetComponent<Text>();

            m_NpcTalkUI = new NpcTalkUI();
            GameObject npcTalkCanvasObject = m_PartyCanvasObj.transform.Find("NpcTalkCanvas").gameObject;
            m_NpcTalkUI.NpcTalkCanvas = npcTalkCanvasObject.GetComponent<Canvas>();
            m_NpcTalkUI.NpcTalkObj = npcTalkCanvasObject.transform.Find("NpcResponseBackground").gameObject;
            m_NpcTalkUI.NpcTalkBackgroundImg = npcTalkCanvasObject.transform.Find("NpcResponseBackground").GetComponent<Image>();
            m_NpcTalkUI.NpcResponseText = npcTalkCanvasObject.transform.Find("NpcResponseBackground").Find("NpcResponseText").GetComponent<Text>();
            m_NpcTalkUI.LocationNameText = npcTalkCanvasObject.transform.Find("LocationNameText").GetComponent<Text>();

            m_NpcTalkUI.TalkAvatar = new TalkAvatarUI();
            m_NpcTalkUI.TalkAvatar.Holder = npcTalkCanvasObject.transform.Find("Avatar").gameObject;
            m_NpcTalkUI.TalkAvatar.Avatar = m_NpcTalkUI.TalkAvatar.Holder.transform.Find("AvatarImage").GetComponent<Image>();
            m_NpcTalkUI.TalkAvatar.NpcNameText = m_NpcTalkUI.TalkAvatar.Holder.transform.Find("NpcNameText").GetComponent<Text>();

            //m_SceneVideoPlayer = npcTalkCanvasObject.transform.Find("VideoPlayer").GetComponent<Video>();
            SceneVideoImage = npcTalkCanvasObject.transform.Find("SceneVideoImage").GetComponent<RawImage>();

            m_NpcTalkUI.TopicButtonHolder = npcTalkCanvasObject.transform.Find("TopicsHolder").GetComponent<RectTransform>();
            m_NpcTalkUI.TopicButtonList.Add(m_NpcTalkUI.TopicButtonHolder.Find("Button_1").gameObject);
            m_NpcTalkUI.TopicButtonList.Add(m_NpcTalkUI.TopicButtonHolder.Find("Button_2").gameObject);
            m_NpcTalkUI.TopicButtonList.Add(m_NpcTalkUI.TopicButtonHolder.Find("Button_3").gameObject);
            m_NpcTalkUI.TopicButtonList.Add(m_NpcTalkUI.TopicButtonHolder.Find("Button_4").gameObject);
            m_NpcTalkUI.TopicButtonList.Add(m_NpcTalkUI.TopicButtonHolder.Find("Button_5").gameObject);
            m_NpcTalkUI.TopicButtonList.Add(m_NpcTalkUI.TopicButtonHolder.Find("Button_6").gameObject);

            foreach (GameObject topicBtn in m_NpcTalkUI.TopicButtonList)
            {
                topicBtn.GetComponent<Button>().onClick.AddListener(
                    delegate { TalkEventMgr.Instance.OnTopicClicked(topicBtn.GetComponent<TopicBtnContext>()); });
            }

            m_NpcTalkUI.AvatarBtnHolder = npcTalkCanvasObject.transform.Find("AvatarButtonsHolder").GetComponent<RectTransform>();
            m_NpcTalkUI.AvatarBtnList.Add(m_NpcTalkUI.AvatarBtnHolder.Find("AvatarButton_1").GetComponent<AvatarBtnContext>());
            m_NpcTalkUI.AvatarBtnList.Add(m_NpcTalkUI.AvatarBtnHolder.Find("AvatarButton_2").GetComponent<AvatarBtnContext>());
            m_NpcTalkUI.AvatarBtnList.Add(m_NpcTalkUI.AvatarBtnHolder.Find("AvatarButton_3").GetComponent<AvatarBtnContext>());

            if (m_NpcTalkUI.AvatarBtnHolder == null) { Debug.Log("null av btn holder"); }
            foreach (AvatarBtnContext avatarCtx in m_NpcTalkUI.AvatarBtnList)
            {
                avatarCtx.AvatarButton.onClick.AddListener(
                    delegate { TalkEventMgr.Instance.OnAvatarClicked(avatarCtx); });
            }

            m_PartyUI = new PartyUI();
            m_PartyUI.GoldText = m_PartyCanvasObj.transform.Find("GoldCountText").GetComponent<Text>();
            m_PartyUI.FoodText = m_PartyCanvasObj.transform.Find("FoodCountText").GetComponent<Text>();
            m_PartyUI.HoverInfoText = m_PartyCanvasObj.transform.Find("BaseBarImage").transform.Find("HoverInfoText").GetComponent<Text>();
            

            GameObject objectInfoCanvasObject = GameObject.Find("NpcInfoCanvas");
            Debug.Assert(objectInfoCanvasObject != null);
            Transform npcInfoBackgroundObject = objectInfoCanvasObject.transform.Find("Background");

            m_InspectNpcUI = new InspectNpcUI();
            m_InspectNpcUI.Canvas = objectInfoCanvasObject.GetComponent<Canvas>();

            m_InspectNpcUI.Healthbar_Background = npcInfoBackgroundObject.transform.Find("Healthbar_Background").GetComponent<Image>();
            m_InspectNpcUI.Healthbar = npcInfoBackgroundObject.transform.Find("Healthbar").GetComponent<Image>();
            m_InspectNpcUI.Healthbar_CapLeft = npcInfoBackgroundObject.transform.Find("Healthbar_CapLeft").GetComponent<Image>();
            m_InspectNpcUI.Healthbar_CapRight = npcInfoBackgroundObject.transform.Find("Healthbar_CapRight").GetComponent<Image>();

            m_InspectNpcUI.NpcNameText = npcInfoBackgroundObject.transform.Find("NpcNameText").GetComponent<Text>();
            m_InspectNpcUI.HitPointsText = npcInfoBackgroundObject.transform.Find("HitPointsText").GetComponent<Text>();
            m_InspectNpcUI.ArmorClassText = npcInfoBackgroundObject.transform.Find("ArmorClassText").GetComponent<Text>();
            m_InspectNpcUI.AttackText = npcInfoBackgroundObject.transform.Find("AttackText").GetComponent<Text>();
            m_InspectNpcUI.DamageText = npcInfoBackgroundObject.transform.Find("DamageText").GetComponent<Text>();
            m_InspectNpcUI.SpellText = npcInfoBackgroundObject.transform.Find("SpellText").GetComponent<Text>();
            m_InspectNpcUI.FireResistanceText = npcInfoBackgroundObject.transform.Find("FireResistanceText").GetComponent<Text>();
            m_InspectNpcUI.AirResistanceText = npcInfoBackgroundObject.transform.Find("AirResistanceText").GetComponent<Text>();
            m_InspectNpcUI.WaterResistanceText = npcInfoBackgroundObject.transform.Find("WaterResistanceText").GetComponent<Text>();
            m_InspectNpcUI.EarthResistanceText = npcInfoBackgroundObject.transform.Find("EarthResistanceText").GetComponent<Text>();
            m_InspectNpcUI.MindResistanceText = npcInfoBackgroundObject.transform.Find("MindResistanceText").GetComponent<Text>();
            m_InspectNpcUI.SpiritResistanceText = npcInfoBackgroundObject.transform.Find("SpiritResistanceText").GetComponent<Text>();
            m_InspectNpcUI.BodyResistanceText = npcInfoBackgroundObject.transform.Find("BodyResistanceText").GetComponent<Text>();
            m_InspectNpcUI.LightResistanceText = npcInfoBackgroundObject.transform.Find("LightResistanceText").GetComponent<Text>();
            m_InspectNpcUI.DarkResistanceText = npcInfoBackgroundObject.transform.Find("DarkResistanceText").GetComponent<Text>();
            m_InspectNpcUI.PhysicalResistanceText = npcInfoBackgroundObject.transform.Find("PhysicalResistanceText").GetComponent<Text>();

            m_EmptySlotBanners.Add(m_PartyCanvasObj.transform.Find("PC1_EmptySlot").GetComponent<Image>());
            m_EmptySlotBanners.Add(m_PartyCanvasObj.transform.Find("PC2_EmptySlot").GetComponent<Image>());
            m_EmptySlotBanners.Add(m_PartyCanvasObj.transform.Find("PC3_EmptySlot").GetComponent<Image>());
            m_EmptySlotBanners.Add(m_PartyCanvasObj.transform.Find("PC4_EmptySlot").GetComponent<Image>());
            m_EmptySlotBanners.Add(m_PartyCanvasObj.transform.Find("PC5_EmptySlot").GetComponent<Image>());

            m_InspectNpcUI.PreviewImage = npcInfoBackgroundObject.transform.Find("PreviewImageMask").transform.Find("PreviewImage").GetComponent<Image>();

            m_CharDetailUI = CharDetailUI.Load();
        }

        // Init sequence: DbMgr(1) -> GameMgr(1) -> UiMgr(1) -> GameMgr(2)
        public bool Init()
        {
            m_PartyCanvasObj = GameObject.Find("PartyCanvas");
            m_NpcInfoCanvasObj = GameObject.Find("NpcInfoCanvas");
            m_PlayerParty = GameMgr.Instance.PlayerParty;

            // ------ Load state-changing sprites ------ 
            Dictionary<string, Sprite> agroStSpriteMap = new Dictionary<string, Sprite>();
            string path = "UI/Party/BarsAgroStatus";
            Sprite[] agroStSprites = Resources.LoadAll<Sprite>(path);
            foreach (Sprite sprite in agroStSprites)
            {
                agroStSpriteMap[sprite.name] = sprite;
            }

            CharacterUI.HealthBarSprite_Green = agroStSpriteMap["ManaG"];
            CharacterUI.HealthBarSprite_Yellow = agroStSpriteMap["manaY"];
            CharacterUI.HealthBarSprite_Red = agroStSpriteMap["manar"];
            CharacterUI.AgroStatusSprite_Green = agroStSpriteMap["statG"];
            CharacterUI.AgroStatusSprite_Yellow = agroStSpriteMap["statY"];
            CharacterUI.AgroStatusSprite_Red = agroStSpriteMap["statR"];
            CharacterUI.AgroStatusSprite_Gray = agroStSpriteMap["statBL"];

            InspectNpcUI.HealthbarSprite_Green = agroStSpriteMap["MHP_GRN"];
            InspectNpcUI.HealthbarSprite_Yellow = agroStSpriteMap["MHP_YEL"];
            InspectNpcUI.HealthbarSprite_Red = agroStSpriteMap["MHP_RED"];

            // Load Npc Avatars
            Sprite[] npcAvatarSprites = Resources.LoadAll<Sprite>("UI/NPC/NpcAvatars");
            foreach (Sprite npcSprite in npcAvatarSprites)
            {
                // Get ID from name
                string idStr = npcSprite.name.Substring(3);
                int spriteId = int.Parse(idStr);
                m_NpcAvatarsMap[spriteId] = npcSprite;
            }

            m_QuestEffectSprites = Resources.LoadAll<Sprite>("UI/EffectFaceSprites/FaceEffect_Quest");

            return true;
        }

        public bool PostInit()
        {
            return true;
        }

        private void UpdateEmptySlotBanners(PlayerParty party)
        {
            int numPartyMembers = party.Characters.Count;
            for (int emptySlotIdx = 0; emptySlotIdx < m_EmptySlotBanners.Count; emptySlotIdx++)
            {
                if (emptySlotIdx < numPartyMembers)
                {
                    m_EmptySlotBanners[emptySlotIdx].enabled = false;
                }
                else
                {
                    m_EmptySlotBanners[emptySlotIdx].enabled = true;
                }
            }
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

        //=================================== Static Helpers ===================================


        static public Ray GetCrosshairRay()
        {
            return Camera.main.ViewportPointToRay(Constants.CrosshairScreenRelPos);
        }

        static public float GetTextHeight(Text text)
        {

            Vector2 extents = new Vector2(text.rectTransform.rect.width, 1);

            TextGenerator textGen = new TextGenerator();
            TextGenerationSettings generationSettings =
                text.GetGenerationSettings(extents);
            textGen.GetPreferredHeight(text.text, generationSettings);
            float height = textGen.lineCount * (text.fontSize * 1.15f);

            return height;
        }

        static public Sprite RandomSprite(List<Sprite> sprites)
        {
            return sprites[UnityEngine.Random.Range(0, sprites.Count)];
        }


        //=================================== Methods ===================================


        public bool HandleButtonDown(string button)
        {
            if (m_CurrUIState != null)
            {
                // Actions need to know how to handle it
                return m_CurrUIState.OnActionPressed(button);
            }
            else
            {
                // Check the button corresponds to any UI state
                switch (button)
                {
                    case "Escape":
                        break;

                    case "Map":
                        m_CurrUIState = new MapUIState();
                        m_CurrUIState.EnterState(null);
                        break;

                    case "Quest":
                        break;

                    case "Story":
                        break;

                    case "Notes":
                        break;

                    case "Rest":
                        break;

                    case "Inventory":
                        m_CurrUIState = new CharDetailUIState();
                        m_CurrUIState.EnterState(new CharDetailUIStateArgs(CharDetailState.Inventory));
                        break;

                    case "Spellbook":
                        break;

                    case "Stats":
                        break;

                    default:
                        break;
                }
            }

            return false;
        }

        public bool IsInGameBlockingState()
        {
            return m_CurrUIState != null && m_CurrUIState.IsGameBlocking();
        }

        public Sprite GetNpcAvatarSprite(int pictureId)
        {
            const int PLACEHOLDER_ID = 2200;

            Sprite sprite = null;
            if (m_NpcAvatarsMap.ContainsKey(pictureId))
            {
                sprite = m_NpcAvatarsMap[pictureId];
            }
            else
            {
                sprite = m_NpcAvatarsMap[PLACEHOLDER_ID];
                Debug.LogWarning("Could not find NPC avatar with ID: " + pictureId + ". Setting placeholder.");
            }

            return sprite;
        }

        public void SetupForFullscreenUiState(UIState invoker)
        {
            m_Minimap.enabled = false;
            m_PartyBuffsAndButtonsCanvas.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            foreach (Character chr in m_PlayerParty.Characters)
            {
                chr.UI.Holder.SetActive(false);
            }

            if (invoker.IsGameBlocking())
            {
                GameMgr.Instance.PauseGame();
            }
        }

        public void SetupForPartialUiState(UIState invoker)
        {
            m_Minimap.enabled = false;
            m_PartyBuffsAndButtonsCanvas.enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (invoker.IsGameBlocking())
            {
                GameMgr.Instance.PauseGame();
            }
        }

        private void SetPartyInfoText(string text, bool forceRewrite = true)
        {
            if (!forceRewrite)
            {
                if (m_PartyTextLockTime < 0)
                {
                    m_PartyUI.HoverInfoText.text = text;
                    m_TimeSinceLastPartyText = 0.0f;
                }

                return;
            }

            m_PartyUI.HoverInfoText.text = text;
            m_TimeSinceLastPartyText = 0.0f;
            m_PartyTextLockTime = 2.0f;
        }

        private CharacterSprites GetCharacterSprites(CharacterType type)
        {
            // Caching
            if (m_CharacterSpritesMap.ContainsKey(type) && m_CharacterSpritesMap[type] != null)
            {
                return m_CharacterSpritesMap[type];
            }
            else
            {
                m_CharacterSpritesMap[type] = CharacterSprites.Load(type);
                return m_CharacterSpritesMap[type];
            }
        }

        public void ReturnToGame()
        {
            m_PartyBuffsAndButtonsCanvas.enabled = true;
            m_NpcTalkUI.NpcTalkCanvas.enabled = false;
            m_InspectNpcUI.Canvas.enabled = false;
            m_MapQuestNotesUI.Canvas.enabled = false;

            m_Minimap.enabled = true;
            m_MinimapCloseButtonImage.enabled = false;
            m_PartyBuffsAndButtonsCanvas.enabled = true;

            Debug.Log("Locking");
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            if (m_CurrUIState != null)
            {
                m_CurrUIState.LeaveState();
                m_CurrUIState = null;
            }

            foreach (Character chr in m_PlayerParty.Characters)
            {
                chr.UI.Holder.SetActive(true);
            }

            GameMgr.Instance.UnpauseGame();
        }


        //=================================== Events ===================================


        private void OnCharacterJoinedParty(Character chr, PlayerParty party)
        {
            // Set up UI
            int numPartyMembers = party.Characters.Count;

            CharacterUI chrUI = new CharacterUI();
            //float chrUiOffsetX = (Constants.PC_WidthDelta * m_PartyCanvasObj.transform.localScale.x) * chr.GetPartyIndex();
            GameObject pc = (GameObject)Instantiate(Resources.Load("Prefabs/UI/PC"), m_PartyCanvasObj.transform);
            pc.transform.localPosition = new Vector3(Constants.PC_WidthDelta, 0.0f, 0.0f) * chr.GetPartyIndex();
            //pc.transform.position += new Vector3(Constants.PC_WidthDelta, 0.0f, 0.0f) * chr.Data.PartyIndex;
            pc.name = "PC_" + chr.Data.Name;

            chrUI.Holder = pc;
            chrUI.PlayerCharacter = pc.transform.Find("PC_Avatar").GetComponent<Image>();
            chrUI.SelectionRing = pc.transform.Find("PC_SelectRing").GetComponent<Image>();
            chrUI.AgroStatus = pc.transform.Find("PC_AgroStatus").GetComponent<Image>();
            chrUI.HealthBar = pc.transform.Find("PC_HealthBar").GetComponent<Image>();
            chrUI.ManaBar = pc.transform.Find("PC_ManaBar").GetComponent<Image>();
            chrUI.BlessBuff = pc.transform.Find("PC_BlessBuff").GetComponent<Image>();
            chrUI.FaceOverlayAnimation = pc.transform.Find("PC_AvatarAnim").GetComponent<SpriteAnimation>();

            chr.UI = chrUI;

            chr.CharFaceUpdater = new CharFaceUpdater(chr);
            chr.Sprites = GetCharacterSprites(chr.Data.CharacterType);

            UpdateEmptySlotBanners(party);
        }

        private void OnCharacterLeftParty(Character removedChr, PlayerParty party)
        {
            Destroy(removedChr.UI.Holder);

            foreach (Character chr in party.Characters)
            {
                chr.UI.Holder.transform.localPosition =
                    new Vector3(Constants.PC_WidthDelta, 0.0f, 0.0f) * chr.GetPartyIndex();
            }

            UpdateEmptySlotBanners(party);
        }

        public void OnCharHealthChanged(Character chr, int maxHealth, int currHealth, int delta)
        {
            float healthPercent = ((float)currHealth / (float)maxHealth) * 100.0f;
            chr.UI.SetHealth(healthPercent);

            //Debug.Log(maxHealth + " " + currHealth);
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
            m_PartyUI.GoldText.text = newGold.ToString();
        }

        public void OnFoodChanged(int oldFood, int newFood, int delta)
        {
            m_PartyUI.FoodText.text = newFood.ToString();
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
            chr.CharFaceUpdater.ResetTimer();

            string hitText = "";
            switch (result.Type)
            {
                case AttackResultType.Hit:
                    hitText = chr.Data.Name + " hits " + result.VictimName + 
                        " for " + result.DamageDealt + " damage";
                    break;

                case AttackResultType.Kill:
                    hitText = chr.Data.Name + " inflicts " + result.DamageDealt + 
                        " points killing " + result.VictimName;
                    chr.CharFaceUpdater.SetAvatar(RandomSprite(chr.Sprites.Smile), 0.75f);
                    break;

                case AttackResultType.Miss:
                    hitText = chr.Data.Name + " missed attack on " + result.VictimName;
                    chr.CharFaceUpdater.SetAvatar(RandomSprite(chr.Sprites.FailAction), 0.75f);
                    break;

                default:
                    Logger.LogError("Unknown attack result type: " + result.Type);
                    break;
            }

            SetPartyInfoText(hitText);
        }

        public void OnCharGotHit(Character chr, AttackInfo attackInfo, AttackResult attackResult)
        {
            if (attackResult.Type == AttackResultType.Hit)
            {
                chr.CharFaceUpdater.SetAvatar(RandomSprite(chr.Sprites.TakeDamage), 0.5f);
            }
        }

        private void OnCharAttack(Character chr, AttackInfo attackInfo)
        {
            chr.CharFaceUpdater.ResetTimer();
        }

        public void OnHoverObject(HoverInfo hoverInfo)
        {
            SetPartyInfoText(hoverInfo.HoverText, false);
        }

        public void OnNpcInspectStart(Character inspector, BaseNpc npc, MonsterData npcData)
        {
            m_InspectNpcUI.Canvas.enabled = true;
            m_InspectNpcUI.NpcNameText.text = npcData.Name;
            m_InspectNpcUI.HitPointsText.text = npc.CurrentHitPoints.ToString() + "/" + npcData.HitPoints;
            m_InspectNpcUI.ArmorClassText.text = npcData.ArmorClass.ToString();
            m_InspectNpcUI.AttackText.text = npcData.Attack1.DamageType.ToString();
            m_InspectNpcUI.DamageText.text = npcData.AttackAmountText;
            m_InspectNpcUI.SpellText.text = npcData.SpellAttack1.SpellName == "" ? "None" : npcData.SpellAttack1.SpellName;
            m_InspectNpcUI.FireResistanceText.text = npcData.Resistances[SpellElement.Fire].ToString();
            m_InspectNpcUI.AirResistanceText.text = npcData.Resistances[SpellElement.Air].ToString();
            m_InspectNpcUI.WaterResistanceText.text = npcData.Resistances[SpellElement.Water].ToString();
            m_InspectNpcUI.EarthResistanceText.text = npcData.Resistances[SpellElement.Earth].ToString();
            m_InspectNpcUI.MindResistanceText.text = npcData.Resistances[SpellElement.Mind].ToString();
            m_InspectNpcUI.SpiritResistanceText.text = npcData.Resistances[SpellElement.Spirit].ToString();
            m_InspectNpcUI.BodyResistanceText.text = npcData.Resistances[SpellElement.Body].ToString();
            m_InspectNpcUI.LightResistanceText.text = npcData.Resistances[SpellElement.Light].ToString();
            m_InspectNpcUI.DarkResistanceText.text = npcData.Resistances[SpellElement.Dark].ToString();
            m_InspectNpcUI.PhysicalResistanceText.text = npcData.Resistances[SpellElement.Physical].ToString();

            m_InspectNpcUI.PreviewImage.sprite = npc.PreviewImage;

            m_InspectNpcUI.Healthbar.fillAmount = (float)npc.CurrentHitPoints / (float)npcData.HitPoints;
            if (m_InspectNpcUI.Healthbar.fillAmount > 0.66f)
            {
                m_InspectNpcUI.Healthbar.sprite = InspectNpcUI.HealthbarSprite_Green;
            }
            else if (m_InspectNpcUI.Healthbar.fillAmount > 0.33f)
            {
                m_InspectNpcUI.Healthbar.sprite = InspectNpcUI.HealthbarSprite_Yellow;
            }
            else
            {
                m_InspectNpcUI.Healthbar.sprite = InspectNpcUI.HealthbarSprite_Red;
            }
        }

        public void OnNpcInspectEnd(Character inspector, BaseNpc npc, MonsterData npcData)
        {
            m_InspectNpcUI.Canvas.enabled = false;
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

        public void OnMinimapMarkerCreated(MinimapMarker marker)
        {
            m_Minimap.MinimapMarkers.Add(marker);
        }

        public void OnMinimapMarkerDestroyed(MinimapMarker marker)
        {
            m_Minimap.MinimapMarkers.Remove(marker);
        }

        /*
         *  Called when Player talks with Talkable object (House, NPC, ...) for the FIRST time
         *  All the rest converstaion takes place inside TalkUIState
         */
        public void OnTalkSceneStart(Character talkerChr, TalkScene talkScene)
        {
            if (m_CurrUIState != null)
            {
                Debug.LogError("Talk attempt when already talking. Not supported now !");
                return;
            }

            m_CurrUIState = new TalkUIState();

            SetupForPartialUiState(m_CurrUIState);
            m_MinimapCloseButtonImage.enabled = true;

            m_CurrUIState.EnterState(new TalkUIStateArgs(talkerChr, talkScene, m_NpcTalkUI));
        }

        private void OnQuestBitAdded(int questId)
        {
            Character chr = m_PlayerParty.GetActiveCharacter();
            if (chr == null)
            {
                chr = m_PlayerParty.GetFirstCharacter();
            }

            SpriteAnimation FaceOverlayAnim = chr.UI.FaceOverlayAnimation;
            FaceOverlayAnim.AnimationSprites = m_QuestEffectSprites;
            FaceOverlayAnim.Play();
            chr.CharFaceUpdater.SetAvatar(RandomSprite(chr.Sprites.Smile), 1.0f);
        }

        private void OnCharacterFinishedEvent(Character chr)
        {
            SpriteAnimation FaceOverlayAnim = chr.UI.FaceOverlayAnimation;
            FaceOverlayAnim.AnimationSprites = m_QuestEffectSprites;
            FaceOverlayAnim.Play();
            chr.CharFaceUpdater.SetAvatar(RandomSprite(chr.Sprites.Smile), 1.0f);
        }

        // =========== Game states

        public void OnPauseGame()
        {

        }

        // =========== Buttons

        public void OnCharDetailButtonPressed(string action)
        {
            if ((m_CurrUIState != null) && (m_CurrUIState.GetType() == typeof(CharDetailUIState)))
            {
                m_CurrUIState.OnActionPressed(action);
            }
        }
    }
}
