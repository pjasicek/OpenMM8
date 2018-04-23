using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Assets.OpenMM8.Scripts.Gameplay.Data;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    class GameMgr : MonoBehaviour //Singleton<GameMgr>
    {
        public static GameMgr Instance;

        // States
        [Header("Game states")]
        public GameState GameState;
        public MapType MapType;

        // Player
        [Header("Player")]
        public PlayerParty PlayerParty;

        // Events
        public delegate void GamePausedAction();
        public delegate void GameUnpausedAction();

        /*public event GamePausedAction OnGamePaused;
        public event GameUnpausedAction OnGameUnpaused;*/

        [Header("UI")]
        public InspectNpcUI InspectNpcUI;
        public PartyUI PartyUI;
        public NpcTalkUI NpcTalkUI;
        public Minimap Minimap;
        public Image MinimapCloseButtonImage;
        public MapQuestNotesUI MapQuestNotesUI;

        // UI Canvases
        [Header("UI - Canvases")]
        public Canvas PartyCanvas;
        public Canvas PartyBuffsAndButtonsCanvas;
        public Canvas PartyInventoryCanvas;
        public Canvas HouseCanvas;

        // TODO: Get rid of this somehow
        [Header("UI - Character")]
        public Sprite GreenHealthBarSprite;
        public Sprite YellowHealthBarSprite;
        public Sprite RedHealthBarSprite;

        public Sprite GreenAgroStatusSprite;
        public Sprite YelloqAgroStatusSprite;
        public Sprite RedAgroStatusSprite;

        [Header("UI - Inspect NPC")]
        public Sprite GreenInspectNpcHealthbar;
        public Sprite YellowInspectNpcHealthbar;
        public Sprite RedInspectNpcHealthbar;

        [Header("UI - Map, Quest, Notes, History")]


        [Header("Sounds")]
        public AudioClip BackgroundMusic;

        [Header("Misc")]
        private Canvas InspectedCanvas = null;

        [HideInInspector]
        public bool IsGamePaused = false;

        private AudioSource AudioSource;
        private Dictionary<CharacterType, CharacterSounds> CharacterSoundsMap = 
            new Dictionary<CharacterType, CharacterSounds>();

        private Dictionary<CharacterType, CharacterSprites> CharacterSpritesMap =
            new Dictionary<CharacterType, CharacterSprites>();

        /*static GameMgr()
        {
            
        }*/

        void Awake()
        {
            UnityEngine.Assertions.Assert.IsTrue(Instance == null);
            Instance = this;

            DontDestroyOnLoad(this);

            GameState = GameState.Ingame;
            MapType = MapType.Outdoor;
        }

        public bool Init()
        {
            AudioSource = gameObject.AddComponent<AudioSource>();
            AudioSource.clip = BackgroundMusic;
            AudioSource.loop = true;
            AudioSource.volume = 0.33f;
            AudioSource.Play();
            // 1) Gather relevant game objects

            PlayerParty = GameObject.Find("Player").GetComponent<PlayerParty>();
            Debug.Assert(PlayerParty != null);

            /*GameObject ObjectInfoCanvasObject = GameObject.Find("NpcInfoCanvas");
            Debug.Assert(ObjectInfoCanvasObject != null);
            ObjectInfoCanvas = ObjectInfoCanvasObject.GetComponent<Canvas>();*/

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

            //CharacterSprites.Load(CharacterType.Dragon_1);

            return true;
        }

        public bool PostInit()
        {
            CharacterData characterModel1 = new CharacterData();
            characterModel1.CharacterAvatarId = 27;
            characterModel1.PartyIndex = 1;
            characterModel1.Name = "Tyrkys";
            characterModel1.Class = Class.Necromancer;
            characterModel1.Experience = 0;
            characterModel1.SkillPoints = 0;
            characterModel1.CurrHitPoints = 500;
            characterModel1.CurrSpellPoints = 50;
            characterModel1.Condition = Condition.Good;
            characterModel1.DefaultStats.Age = 30;
            characterModel1.DefaultStats.Level = 1;
            characterModel1.DefaultStats.MaxHitPoints = 500;
            characterModel1.DefaultStats.MaxSpellPoints = 50;

            foreach (Attribute attr in Enum.GetValues(typeof(Attribute)))
            {
                characterModel1.DefaultStats.Attributes[attr] = 0;
                characterModel1.BonusStats.Attributes[attr] = 0;
            }

            foreach (SpellElement resist in Enum.GetValues(typeof(SpellElement)))
            {
                characterModel1.DefaultStats.Resistances[resist] = 0;
                characterModel1.BonusStats.Resistances[resist] = 0;
            }

            CharacterUI characterUI1 = new CharacterUI();
            GameObject partyCanvasObject = GameObject.Find("PartyCanvas");
            characterUI1.PlayerCharacter = partyCanvasObject.transform.Find("PC1_Avatar").GetComponent<Image>();
            characterUI1.SelectionRing = partyCanvasObject.transform.Find("PC1_SelectRing").GetComponent<Image>();
            characterUI1.AgroStatus = partyCanvasObject.transform.Find("PC1_AgroStatus").GetComponent<Image>();
            characterUI1.HealthBar = partyCanvasObject.transform.Find("PC1_HealthBar").GetComponent<Image>();
            characterUI1.ManaBar = partyCanvasObject.transform.Find("PC1_ManaBar").GetComponent<Image>();
            characterUI1.EmptySlot = partyCanvasObject.transform.Find("EmptySlot_Pos1").GetComponent<Image>();

            characterUI1.GreenHealthBarSprite = GreenHealthBarSprite;
            characterUI1.YellowHealthBarSprite = YellowHealthBarSprite;
            characterUI1.RedHealthBarSprite = RedHealthBarSprite;

            PlayerParty.AddCharacter(Character.Create(characterModel1, characterUI1, CharacterType.Lich_1));

            return true;
        }

        void Start()
        {

        }

        void Update()
        {
            if (Input.GetButtonDown("Escape"))
            {
                if (IsGamePaused)
                {
                    ReturnToGame();
                }
                else
                {

                }
            }

            bool wasInspectEnabled = (InspectedCanvas != null) && (InspectedCanvas.enabled == true);
            bool isInspectEnabled = false;

            if (Input.GetButton("InspectObject"))
            {
                if (!IsGamePaused)
                {
                    Time.timeScale = 0;
                }

                RaycastHit hit;
                Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.595F, 0.0F));
                //ray.origin -= 100 * ray.direction.normalized;

                int layerMask = ~((1 << LayerMask.NameToLayer("NpcRangeTrigger")) | (1 << LayerMask.NameToLayer("Player")));
                if (Physics.Raycast(ray, out hit, 1000.0f, layerMask))
                {
                    Transform objectHit = hit.collider.transform;
                    if (objectHit.GetComponent<Inspectable>() != null)
                    {
                        InspectedCanvas = objectHit.GetComponent<Inspectable>().SetupInspectCanvas();
                        isInspectEnabled = true;
                    }
                }

                //Debug.DrawRay(ray.origin, ray.direction, Color.green);
            }
            else
            {
                if (!IsGamePaused)
                {
                    Time.timeScale = 1;
                }
            }

            if (!wasInspectEnabled && isInspectEnabled)
            {
                InspectedCanvas.enabled = true;
            }
            else if (wasInspectEnabled && !isInspectEnabled)
            {
                InspectedCanvas.enabled = false;
                InspectedCanvas = null;
            }

            if (Input.GetButtonDown("Map"))
            {
                if (!(IsGamePaused && !MapQuestNotesUI.Canvas.enabled))
                {
                    if (MapQuestNotesUI.Canvas.enabled)
                    {
                        ReturnToGame();
                    }
                    else
                    {
                        Cursor.lockState = CursorLockMode.None;
                        Cursor.visible = true;
                        PauseGame();
                        MapQuestNotesUI.Canvas.enabled = true;
                        GameMgr.Instance.Minimap.enabled = false;
                    }
                }
            }
        }

        public void ReturnToGame()
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
            UnpauseGame();
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            AudioSource.Pause();
            IsGamePaused = true;
            //OnGamePaused();
        }

        public void UnpauseGame()
        {
            Time.timeScale = 1;
            AudioSource.UnPause();
            IsGamePaused = false;
            //OnGameUnpaused();
        }

        public static AudioClip PlayRandomSound(List<AudioClip> sounds, AudioSource audioSource)
        {
            if (sounds.Count == 0)
            {
                return null;
            }

            AudioClip sound = sounds[UnityEngine.Random.Range(0, sounds.Count)];
            audioSource.PlayOneShot(sound);

            return sound;
        }

        public static AudioClip PlayRandomSound(List<AudioClip> sounds)
        {
            return PlayRandomSound(sounds, GameMgr.Instance.AudioSource);
        }

        public CharacterSounds GetCharacterSounds(CharacterType type)
        {
            // Caching
            if (CharacterSoundsMap.ContainsKey(type) && CharacterSoundsMap[type] != null)
            {
                return CharacterSoundsMap[type];
            }
            else
            {
                CharacterSoundsMap[type] = CharacterSounds.Load(type);
                return CharacterSoundsMap[type];
            }
        }

        public CharacterSprites GetCharacterSprites(CharacterType type)
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

        public void ChangeGameState(GameState newState)
        {

        }

        public static void Log(string msg)
        {
            Debug.Log(msg);
        }

        /*public static bool CrosshairRaycast()
        {

        }*/
    }
}
