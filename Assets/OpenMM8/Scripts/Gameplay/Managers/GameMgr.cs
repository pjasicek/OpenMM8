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

        [Header("UI")]
        public InspectNpcUI InspectNpcUI;
        public PartyUI PartyUI;

        // UI Canvases
        [Header("UI - Canvases")]
        public Canvas PartyCanvas;
        public Canvas PartyBuffsAndButtonsCanvas;
        public Canvas PartyInventoryCanvas;
        public Canvas HouseCanvas;
        public Canvas NpcTalkCanvas;

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

        [Header("Misc")]
        private Canvas InspectedCanvas = null;
        public bool IsGamePaused = false;


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

            /*GameObject ObjectInfoCanvasObject = GameObject.Find("NpcInfoCanvas");
            Debug.Assert(ObjectInfoCanvasObject != null);
            ObjectInfoCanvas = ObjectInfoCanvasObject.GetComponent<Canvas>();*/

            GameObject partyCanvasObject = GameObject.Find("PartyCanvas");
            if (partyCanvasObject != null)
            {
                PartyCanvas = partyCanvasObject.GetComponent<Canvas>();
                PartyBuffsAndButtonsCanvas = partyCanvasObject.transform.Find("BuffsAndButtonsCanvas").GetComponent<Canvas>();
            }
            else
            {
                Debug.LogError("Could not find PartyCanvas gameobject !");
            }

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

            // 2) Initialize Player's party
            CharacterModel characterModel1 = new CharacterModel();
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
            characterUI1.PlayerCharacter = partyCanvasObject.transform.Find("PC1_Avatar").GetComponent<Image>();
            characterUI1.SelectionRing = partyCanvasObject.transform.Find("PC1_SelectRing").GetComponent<Image>();
            characterUI1.AgroStatus = partyCanvasObject.transform.Find("PC1_AgroStatus").GetComponent<Image>();
            characterUI1.HealthBar = partyCanvasObject.transform.Find("PC1_HealthBar").GetComponent<Image>();
            characterUI1.ManaBar = partyCanvasObject.transform.Find("PC1_ManaBar").GetComponent<Image>();
            characterUI1.EmptySlot = partyCanvasObject.transform.Find("EmptySlot_Pos1").GetComponent<Image>();

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
        }

        void PauseGame()
        {
            Time.timeScale = 0;
            AudioListener.pause = true;
            IsGamePaused = true;
            //OnGamePaused();
        }

        void UnpauseGame()
        {
            Time.timeScale = 1;
            AudioListener.pause = false;
            IsGamePaused = false;
            //OnGameUnpaused();
        }

        public void ChangeGameState(GameState newState)
        {

        }

        public static void Log(string msg)
        {
            Debug.Log(msg);
        }

        private static float Gaussian()
        {
            float u, v, S;

            do
            {
                u = 2.0f * UnityEngine.Random.Range(0.0f, 1.0f) - 1.0f;
                v = 2.0f * UnityEngine.Random.Range(0.0f, 1.0f) - 1.0f;
                S = u * u + v * v;
            }
            while (S >= 1.0);

            float fac = UnityEngine.Mathf.Sqrt(-2.0f * UnityEngine.Mathf.Log(S) / S);
            return u * fac;
        }

        public static float GaussianRandom()
        {
            float sigma = 1.0f / 6.0f; // or whatever works.
            while (true)
            {
                float z = Gaussian() * sigma + 0.5f;
                if (z >= 0.0 && z <= 1.0)
                {
                    return z;
                }
            }
        }

        /*public static bool CrosshairRaycast()
        {

        }*/
    }
}
