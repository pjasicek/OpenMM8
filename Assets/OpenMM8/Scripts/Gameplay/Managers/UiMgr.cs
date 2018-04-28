using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public class UiMgr : Singleton<UiMgr>
    {
        // Public
        [Header("UI - Canvases")]
        private Canvas PartyCanvas;
        private Canvas PartyBuffsAndButtonsCanvas;
        private Canvas PartyInventoryCanvas;
        private Canvas HouseCanvas;

        // Private
        private GameObject m_PartyCanvasObj;
        private GameObject m_NpcInfoCanvasObj;
        private PlayerParty m_PlayerParty;
        private float m_TimeSinceLastPartyText = 0.0f;
        private float m_PartyTextLockTime = 0.0f;

        [Header("UI")]
        private InspectNpcUI InspectNpcUI;
        private PartyUI PartyUI;
        private NpcTalkUI NpcTalkUI;
        private Minimap Minimap;
        private Image MinimapCloseButtonImage;
        private MapQuestNotesUI MapQuestNotesUI;

        [Header("UI - Map, Quest, Notes, History")]
        int placeholder;

        private Dictionary<CharacterType, CharacterSprites> CharacterSpritesMap =
            new Dictionary<CharacterType, CharacterSprites>();

        //=================================== Unity Lifecycle ===================================

        private void Awake()
        {
            GameMgr.OnPauseGame += OnPauseGame;
            GameMgr.OnReturnToGame += OnReturnToGame;
            GameMgr.OnMapButtonPressed += OnMapButtonPressed;

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

            InspectableNpc.OnNpcInspectStart += OnNpcInspectStart;
            InspectableNpc.OnNpcInspectEnd += OnNpcInspectEnd;

            MinimapMarker.OnMinimapMarkerCreated += OnMinimapMarkerCreated;
            MinimapMarker.OnMinimapMarkerDestroyed += OnMinimapMarkerDestroyed;

            Talkable.OnTalkWithNpc += OnTalkWithNpc;
        }

        private void Start()
        {
            GameObject partyCanvasObject = GameObject.Find("PartyCanvas");
            if (partyCanvasObject != null)
            {
                PartyCanvas = partyCanvasObject.GetComponent<Canvas>();
                PartyBuffsAndButtonsCanvas = partyCanvasObject.transform.Find("BuffsAndButtonsCanvas").GetComponent<Canvas>();
                Minimap = partyCanvasObject.transform.Find("BuffsAndButtonsCanvas").Find("Minimap").GetComponent<Minimap>();
            }
            else
            {
                Debug.LogError("Could not find PartyCanvas gameobject !");
            }

            MinimapCloseButtonImage = partyCanvasObject.transform.Find("MinimapCloseButton").GetComponent<Image>();

            MapQuestNotesUI = new MapQuestNotesUI();
            GameObject mapQuestNotesObject = partyCanvasObject.transform.Find("MapQuestNotesCanvas").gameObject;
            MapQuestNotesUI.Canvas = mapQuestNotesObject.GetComponent<Canvas>();
            MapQuestNotesUI.MapNameText = mapQuestNotesObject.transform.Find("MapCanvas").transform.Find("MapNameText").GetComponent<Text>();

            NpcTalkUI = new NpcTalkUI();
            GameObject npcTalkCanvasObject = partyCanvasObject.transform.Find("NpcTalkCanvas").gameObject;
            NpcTalkUI.NpcTalkCanvas = npcTalkCanvasObject.GetComponent<Canvas>();
            NpcTalkUI.NpcResponseBackground = npcTalkCanvasObject.transform.Find("NpcResponseBackground").GetComponent<Image>();
            NpcTalkUI.NpcResponseText = npcTalkCanvasObject.transform.Find("NpcResponseBackground").Find("NpcResponseText").GetComponent<Text>();
            NpcTalkUI.NpcAvatar = npcTalkCanvasObject.transform.Find("Avatar").GetComponent<Image>();
            NpcTalkUI.LocationNameText = npcTalkCanvasObject.transform.Find("LocationNameText").GetComponent<Text>();
            NpcTalkUI.NpcNameText = npcTalkCanvasObject.transform.Find("NpcNameText").GetComponent<Text>();

            PartyUI = new PartyUI();
            PartyUI.GoldText = partyCanvasObject.transform.Find("GoldCountText").GetComponent<Text>();
            PartyUI.FoodText = partyCanvasObject.transform.Find("FoodCountText").GetComponent<Text>();
            PartyUI.HoverInfoText = partyCanvasObject.transform.Find("BaseBarImage").transform.Find("HoverInfoText").GetComponent<Text>();
            

            GameObject objectInfoCanvasObject = GameObject.Find("NpcInfoCanvas");
            Debug.Assert(objectInfoCanvasObject != null);
            Transform npcInfoBackgroundObject = objectInfoCanvasObject.transform.Find("Background");

            InspectNpcUI = new InspectNpcUI();
            InspectNpcUI.Canvas = objectInfoCanvasObject.GetComponent<Canvas>();

            InspectNpcUI.Healthbar_Background = npcInfoBackgroundObject.transform.Find("Healthbar_Background").GetComponent<Image>();
            InspectNpcUI.Healthbar = npcInfoBackgroundObject.transform.Find("Healthbar").GetComponent<Image>();
            InspectNpcUI.Healthbar_CapLeft = npcInfoBackgroundObject.transform.Find("Healthbar_CapLeft").GetComponent<Image>();
            InspectNpcUI.Healthbar_CapRight = npcInfoBackgroundObject.transform.Find("Healthbar_CapRight").GetComponent<Image>();

            InspectNpcUI.NpcNameText = npcInfoBackgroundObject.transform.Find("NpcNameText").GetComponent<Text>();
            InspectNpcUI.HitPointsText = npcInfoBackgroundObject.transform.Find("HitPointsText").GetComponent<Text>();
            InspectNpcUI.ArmorClassText = npcInfoBackgroundObject.transform.Find("ArmorClassText").GetComponent<Text>();
            InspectNpcUI.AttackText = npcInfoBackgroundObject.transform.Find("AttackText").GetComponent<Text>();
            InspectNpcUI.DamageText = npcInfoBackgroundObject.transform.Find("DamageText").GetComponent<Text>();
            InspectNpcUI.SpellText = npcInfoBackgroundObject.transform.Find("SpellText").GetComponent<Text>();
            InspectNpcUI.FireResistanceText = npcInfoBackgroundObject.transform.Find("FireResistanceText").GetComponent<Text>();
            InspectNpcUI.AirResistanceText = npcInfoBackgroundObject.transform.Find("AirResistanceText").GetComponent<Text>();
            InspectNpcUI.WaterResistanceText = npcInfoBackgroundObject.transform.Find("WaterResistanceText").GetComponent<Text>();
            InspectNpcUI.EarthResistanceText = npcInfoBackgroundObject.transform.Find("EarthResistanceText").GetComponent<Text>();
            InspectNpcUI.MindResistanceText = npcInfoBackgroundObject.transform.Find("MindResistanceText").GetComponent<Text>();
            InspectNpcUI.SpiritResistanceText = npcInfoBackgroundObject.transform.Find("SpiritResistanceText").GetComponent<Text>();
            InspectNpcUI.BodyResistanceText = npcInfoBackgroundObject.transform.Find("BodyResistanceText").GetComponent<Text>();
            InspectNpcUI.LightResistanceText = npcInfoBackgroundObject.transform.Find("LightResistanceText").GetComponent<Text>();
            InspectNpcUI.DarkResistanceText = npcInfoBackgroundObject.transform.Find("DarkResistanceText").GetComponent<Text>();
            InspectNpcUI.PhysicalResistanceText = npcInfoBackgroundObject.transform.Find("PhysicalResistanceText").GetComponent<Text>();

            InspectNpcUI.PreviewImage = npcInfoBackgroundObject.transform.Find("PreviewImageMask").transform.Find("PreviewImage").GetComponent<Image>();
        }

        // Init sequence: DbMgr(1) -> GameMgr(1) -> UiMgr(1) -> GameMgr(2)
        public bool Init()
        {
            m_PartyCanvasObj = GameObject.Find("PartyCanvas");
            m_NpcInfoCanvasObj = GameObject.Find("NpcInfoCanvas");
            m_PlayerParty = GameMgr.Instance.PlayerParty;

            // ------ Load state-changing sprites ------ 
            Dictionary<string, Sprite> spriteMap = new Dictionary<string, Sprite>();
            string path = "UI/BarsAgroStatus";
            Sprite[] sprites = Resources.LoadAll<Sprite>(path);
            foreach (Sprite sprite in sprites)
            {
                spriteMap[sprite.name] = sprite;
            }

            CharacterUI.HealthBarSprite_Green = spriteMap["ManaG"];
            CharacterUI.HealthBarSprite_Yellow = spriteMap["manaY"];
            CharacterUI.HealthBarSprite_Red = spriteMap["manar"];
            CharacterUI.AgroStatusSprite_Green = spriteMap["statG"];
            CharacterUI.AgroStatusSprite_Yellow = spriteMap["statY"];
            CharacterUI.AgroStatusSprite_Red = spriteMap["statR"];
            CharacterUI.AgroStatusSprite_Gray = spriteMap["statBL"];

            InspectNpcUI.HealthbarSprite_Green = spriteMap["MHP_GRN"];
            InspectNpcUI.HealthbarSprite_Yellow = spriteMap["MHP_YEL"];
            InspectNpcUI.HealthbarSprite_Red = spriteMap["MHP_RED"];

            return true;
        }

        private void OnCharacterJoinedParty(Character chr, PlayerParty party)
        {
            // Set up UI
            GameObject partyCanvasObject = GameObject.Find("PartyCanvas");

            CharacterUI chrUI = new CharacterUI();
            chrUI.PlayerCharacter = partyCanvasObject.transform.Find("PC1_Avatar").GetComponent<Image>();
            chrUI.SelectionRing = partyCanvasObject.transform.Find("PC1_SelectRing").GetComponent<Image>();
            chrUI.AgroStatus = partyCanvasObject.transform.Find("PC1_AgroStatus").GetComponent<Image>();
            chrUI.HealthBar = partyCanvasObject.transform.Find("PC1_HealthBar").GetComponent<Image>();
            chrUI.ManaBar = partyCanvasObject.transform.Find("PC1_ManaBar").GetComponent<Image>();
            chrUI.EmptySlot = partyCanvasObject.transform.Find("EmptySlot_Pos1").GetComponent<Image>();
            chr.UI = chrUI;

            chr.CharFaceUpdater = new CharFaceUpdater(chr);
            chr.Sprites = GetCharacterSprites(chr.Data.CharacterType);

            Debug.Log("OK");
        }

        private void OnCharacterLeftParty(Character chr, PlayerParty party)
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

        public Ray GetCrosshairRay()
        {
            return Camera.main.ViewportPointToRay(Constants.CrosshairScreenRelPos);
        }

        private void SetPartyInfoText(string text, bool forceRewrite = true)
        {
            if (!forceRewrite)
            {
                if (m_PartyTextLockTime < 0)
                {
                   PartyUI.HoverInfoText.text = text;
                    m_TimeSinceLastPartyText = 0.0f;
                }

                return;
            }

            PartyUI.HoverInfoText.text = text;
            m_TimeSinceLastPartyText = 0.0f;
            m_PartyTextLockTime = 2.0f;
        }

        private CharacterSprites GetCharacterSprites(CharacterType type)
        {
            // Caching
            if (CharacterSpritesMap.ContainsKey(type) && CharacterSpritesMap[type] != null)
            {
                return CharacterSpritesMap[type];
            }
            else
            {
                CharacterSpritesMap[type] = CharacterSprites.Load(type);
                return CharacterSpritesMap[type];
            }
        }

        private Sprite RandomSprite(List<Sprite> sprites)
        {
            return sprites[UnityEngine.Random.Range(0, sprites.Count)];
        }

        //=================================== Events ===================================

        public void OnCharHealthChanged(Character chr, int maxHealth, int currHealth)
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
            PartyUI.GoldText.text = newGold.ToString();
        }

        public void OnFoodChanged(int oldFood, int newFood, int delta)
        {
            PartyUI.FoodText.text = newFood.ToString();
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

        public void OnHoverObject(HoverInfo hoverInfo)
        {
            SetPartyInfoText(hoverInfo.HoverText, false);
        }

        public void OnNpcInspectStart(Character inspector, BaseNpc npc, NpcData npcData)
        {
            InspectNpcUI.Canvas.enabled = true;
            InspectNpcUI.NpcNameText.text = npcData.Name;
            InspectNpcUI.HitPointsText.text = npc.CurrentHitPoints.ToString() + "/" + npcData.HitPoints;
            InspectNpcUI.ArmorClassText.text = npcData.ArmorClass.ToString();
            InspectNpcUI.AttackText.text = npcData.Attack1.DamageType.ToString();
            InspectNpcUI.DamageText.text = npcData.AttackAmountText;
            InspectNpcUI.SpellText.text = npcData.SpellAttack1.SpellName == "" ? "None" : npcData.SpellAttack1.SpellName;
            InspectNpcUI.FireResistanceText.text = npcData.Resistances[SpellElement.Fire].ToString();
            InspectNpcUI.AirResistanceText.text = npcData.Resistances[SpellElement.Air].ToString();
            InspectNpcUI.WaterResistanceText.text = npcData.Resistances[SpellElement.Water].ToString();
            InspectNpcUI.EarthResistanceText.text = npcData.Resistances[SpellElement.Earth].ToString();
            InspectNpcUI.MindResistanceText.text = npcData.Resistances[SpellElement.Mind].ToString();
            InspectNpcUI.SpiritResistanceText.text = npcData.Resistances[SpellElement.Spirit].ToString();
            InspectNpcUI.BodyResistanceText.text = npcData.Resistances[SpellElement.Body].ToString();
            InspectNpcUI.LightResistanceText.text = npcData.Resistances[SpellElement.Light].ToString();
            InspectNpcUI.DarkResistanceText.text = npcData.Resistances[SpellElement.Dark].ToString();
            InspectNpcUI.PhysicalResistanceText.text = npcData.Resistances[SpellElement.Physical].ToString();

            InspectNpcUI.PreviewImage.sprite = npc.PreviewImage;

            InspectNpcUI.Healthbar.fillAmount = (float)npc.CurrentHitPoints / (float)npcData.HitPoints;
            if (InspectNpcUI.Healthbar.fillAmount > 0.66f)
            {
                InspectNpcUI.Healthbar.sprite = InspectNpcUI.HealthbarSprite_Green;
            }
            else if (InspectNpcUI.Healthbar.fillAmount > 0.33f)
            {
                InspectNpcUI.Healthbar.sprite = InspectNpcUI.HealthbarSprite_Yellow;
            }
            else
            {
                InspectNpcUI.Healthbar.sprite = InspectNpcUI.HealthbarSprite_Red;
            }
        }

        public void OnNpcInspectEnd(Character inspector, BaseNpc npc, NpcData npcData)
        {
            InspectNpcUI.Canvas.enabled = false;
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
            Minimap.MinimapMarkers.Add(marker);
        }

        public void OnMinimapMarkerDestroyed(MinimapMarker marker)
        {
            Minimap.MinimapMarkers.Remove(marker);
        }

        public void OnTalkWithNpc(Character talkerChr, Talkable talkedToObj)
        {
            NpcTalkUI.NpcNameText.text = talkedToObj.Name;
            NpcTalkUI.LocationNameText.text = talkedToObj.Location;
            NpcTalkUI.NpcResponseText.text = talkedToObj.GreetText;
            NpcTalkUI.NpcTalkCanvas.enabled = true;
            NpcTalkUI.NpcAvatar.sprite = talkedToObj.Avatar;
            Minimap.enabled = false;
            MinimapCloseButtonImage.enabled = true;
            PartyBuffsAndButtonsCanvas.enabled = false;

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            TextGenerator textGen = new TextGenerator();
            TextGenerationSettings generationSettings =
                NpcTalkUI.NpcResponseText.GetGenerationSettings(NpcTalkUI.NpcResponseText.rectTransform.rect.size);
            float height = textGen.GetPreferredHeight(talkedToObj.GreetText, generationSettings);

            float textSizeY = (height / 2.0f) / 10.0f;
            Vector2 v = new Vector2(
                NpcTalkUI.NpcResponseBackground.rectTransform.anchoredPosition.x,
                NpcTalkUI.DefaultResponseY + textSizeY + 5.0f);
            NpcTalkUI.NpcResponseBackground.rectTransform.anchoredPosition = v;
        }

        // =========== Game states

        public void OnReturnToGame()
        {
            PartyBuffsAndButtonsCanvas.enabled = true;
            NpcTalkUI.NpcTalkCanvas.enabled = false;
            InspectNpcUI.Canvas.enabled = false;
            MapQuestNotesUI.Canvas.enabled = false;

            Minimap.enabled = true;
            MinimapCloseButtonImage.enabled = false;
            PartyBuffsAndButtonsCanvas.enabled = true;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public void OnPauseGame()
        {

        }

        // =========== Buttons

        private void OnMapButtonPressed()
        {
            if (!(GameMgr.Instance.IsGamePaused && !MapQuestNotesUI.Canvas.enabled))
            {
                if (MapQuestNotesUI.Canvas.enabled)
                {
                    // Close map -> Return back to game
                    GameMgr.Instance.ReturnToGame();
                }
                else
                {
                    // Open map -> Pause game
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                    MapQuestNotesUI.Canvas.enabled = true;
                    Minimap.enabled = false;

                    GameMgr.Instance.PauseGame();
                }
            }
        }
    }
}
