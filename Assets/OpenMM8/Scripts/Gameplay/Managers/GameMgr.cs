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

        // Databases
        public ItemDb ItemDb;
        public ItemEnchantDb ItemEnchantDb;
        public NpcDb NpcDb;

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

        // UI Canvases
        [Header("UI - Canvases")]
        public Canvas PartyCanvas;
        public Canvas PartyBuffsAndButtonsCanvas;
        public Canvas PartyInventoryCanvas;
        public Canvas HouseCanvas;
        public Canvas NpcTalkCanvas;
        public Canvas ObjectInfoCanvas;

        // TODO: Get rid of this somehow
        [Header("UI - Character")]
        public Sprite GreenHealthBarSprite;
        public Sprite YellowHealthBarSprite;
        public Sprite RedHealthBarSprite;

        public Sprite GreenAgroStatusSprite;
        public Sprite YelloqAgroStatusSprite;
        public Sprite RedAgroStatusSprite;

        /*static GameMgr()
        {
            
        }*/

        void Awake()
        {
            UnityEngine.Assertions.Assert.IsTrue(Instance == null);
            Instance = this;

            ItemDb = new ItemDb();
            ItemEnchantDb = new ItemEnchantDb();
            NpcDb = new NpcDb();

            GameState = GameState.Ingame;
            MapType = MapType.Outdoor;
        }

        void Start()
        {
            // 1) Gather relevant game objects

            PlayerParty = GameObject.Find("Player").GetComponent<PlayerParty>();
            Debug.Assert(PlayerParty != null);

            GameObject ObjectInfoCanvasObject = GameObject.Find("ObjectInfoCanvas");
            Debug.Assert(ObjectInfoCanvasObject != null);
            ObjectInfoCanvas = ObjectInfoCanvasObject.GetComponent<Canvas>();

            GameObject PartyCanvasObject = GameObject.Find("PartyCanvas");
            if (PartyCanvasObject != null)
            {
                PartyCanvas = PartyCanvasObject.GetComponent<Canvas>();
                PartyBuffsAndButtonsCanvas = PartyCanvasObject.transform.Find("BuffsAndButtonsCanvas").GetComponent<Canvas>();
            }
            else
            {
                Debug.LogError("Could not find PartyCanvas gameobject !");
            }

            // 2) Initialize Player's party
            CharacterModel characterModel1 = new CharacterModel();
            characterModel1.CharacterAvatarId = 27;
            characterModel1.PartyIndex = 1;
            characterModel1.Name = "Tyrkys";
            characterModel1.Class = Class.Necromancer;
            characterModel1.Experience = 0;
            characterModel1.SkillPoints = 0;
            characterModel1.CurrHitPoints = 50;
            characterModel1.CurrSpellPoints = 50;
            characterModel1.Condition = Condition.Good;
            characterModel1.DefaultStats.Age = 30;
            characterModel1.DefaultStats.Level = 1;
            characterModel1.DefaultStats.MaxHitPoints = 50;
            characterModel1.DefaultStats.MaxSpellPoints = 50;

            CharacterUI characterUI1 = new CharacterUI();
            characterUI1.PlayerCharacter = PartyCanvasObject.transform.Find("PC1_Avatar").GetComponent<Image>();
            characterUI1.SelectionRing = PartyCanvasObject.transform.Find("PC1_SelectRing").GetComponent<Image>();
            characterUI1.AgroStatus = PartyCanvasObject.transform.Find("PC1_AgroStatus").GetComponent<Image>();
            characterUI1.HealthBar = PartyCanvasObject.transform.Find("PC1_HealthBar").GetComponent<Image>();
            characterUI1.ManaBar = PartyCanvasObject.transform.Find("PC1_ManaBar").GetComponent<Image>();
            characterUI1.EmptySlot = PartyCanvasObject.transform.Find("EmptySlot_Pos1").GetComponent<Image>();

            characterUI1.GreenHealthBarSprite = GreenHealthBarSprite;
            characterUI1.YellowHealthBarSprite = YellowHealthBarSprite;
            characterUI1.RedHealthBarSprite = RedHealthBarSprite;

            PlayerParty.AddCharacter(Character.Create(characterModel1, characterUI1));
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameState == GameState.Ingame)
                {
                    PauseGame();
                    GameState = GameState.IngamePaused;
                }
                else
                {
                    UnpauseGame();
                    GameState = GameState.Ingame;
                }
            }

            if (Input.GetKeyDown(KeyCode.Q))
            {
                PartyBuffsAndButtonsCanvas.enabled = false;
            }

            if (Input.GetButton("InspectObject"))
            {
                ObjectInfoCanvas.enabled = true;
            }
            else
            {
                ObjectInfoCanvas.enabled = false;
            }
        }

        void PauseGame()
        {
            Time.timeScale = 0;
            AudioListener.pause = true;

            //OnGamePaused();
        }

        void UnpauseGame()
        {
            Time.timeScale = 1;
            AudioListener.pause = false;

            //OnGameUnpaused();
        }

        public void ChangeGameState(GameState newState)
        {

        }
    }
}
