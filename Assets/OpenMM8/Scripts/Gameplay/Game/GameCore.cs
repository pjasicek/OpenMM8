using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Assets.OpenMM8.Scripts.Gameplay.Items;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    
    /*public delegate void LevelUnloaded(int levelNum);
    public delegate void LevelLoaded(int levelNum);*/

    public delegate void MapButtonPressed();

    class GameCore : MonoBehaviour //Singleton<GameMgr>
    {
        private static readonly string[] GlobalUiButtons =
        {
            "Escape",
            "Map",
            "Inventory",
            "NextPlayer",
            "Console",
            "Spellbook",
        };

        public static GameCore Instance;

        // States
        [Header("Game states")]
        public GameState GameState;
        public MapType MapType;

        // Player
        [Header("Player")]
        public PlayerParty PlayerParty;
        public StatusTextBar StatusTextBar;

        [Header("Sounds")]
        public AudioClip BackgroundMusic;

        [Header("Debug")]
        [SerializeField] private bool enableDebugHotkeys = true;

        [HideInInspector]
        public bool m_IsGamePaused = false;

        public List<Monster> MonsterList = new List<Monster>();

        public List<Monster> NearbyMonsterList = new List<Monster>();
        public List<float> NearbyMonsterDistanceList = new List<float>();

        public float TimeSinceMonsterUpdate = 0.0f;

        // Private

        // TODO: Get rid of this
        private Inspectable m_InspectedObj;

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
            // 1) Gather relevant game objects

            PlayerParty = GameObject.Find("Player").GetComponent<PlayerParty>();
            Debug.Assert(PlayerParty != null);
            PlayerParty.Initialize();

            StatusTextBar = new StatusTextBar();
            StatusTextBar.TargetText = PlayerParty.PartyUI.StatusBarText;


            return true;
        }
        //=========================================================================================
        // This will be THE MAIN game update loop
        //=========================================================================================
        void Update()
        {
            //Debug.Log("Alive NPCs: " + NpcList.Count);

            // 1) Display pointed status object string (not sure if necessary to do that here ?)

            StatusTextBar.DoUpdate();

            // 2) PROCESS INPUT - THIS HAS TO BE THE ONLY PLACE WHERE KEYBOARD INPUT IS PROCESSED
            // ProcessInput();

            // 3) Do event loop - not sure if it's applicable here ? I do not do async events, I process
            //    everything directly when it happens

            TimeSinceMonsterUpdate += Time.deltaTime;
            if (TimeSinceMonsterUpdate >= 0.02)
            {
                Monster.UpdateMonsters(TimeSinceMonsterUpdate);

                TimeSinceMonsterUpdate = 0.0f;
            }

            PlayerParty?.DoUpdate(Time.deltaTime);

            // 4) If arcomage is in progress - just update arcomage and continue

            // Timers ? not here
            
            // 5) If playing, update periodic effects

            // 6) 

            HandleGlobalUiInput();
            UpdateInspectionState();

            if (enableDebugHotkeys)
            {
                HandleDebugHotkeys();
            }
        }

        private void HandleGlobalUiInput()
        {
            foreach (string button in GlobalUiButtons)
            {
                if (Input.GetButtonDown(button))
                {
                    UiMgr.Instance.HandleButtonDown(button);
                }
            }
        }

        private void UpdateInspectionState()
        {
            bool wasInspectEnabled = m_InspectedObj != null;
            bool isInspectEnabled = false;
            Inspectable inspectedObj = null;

            if (Input.GetButton("InspectObject") && !UiMgr.Instance.IsInGameBlockingState())
            {
                if (!m_IsGamePaused)
                {
                    Time.timeScale = 0;
                }

                int layerMask = ~((1 << LayerMask.NameToLayer("NpcRangeTrigger")) | (1 << LayerMask.NameToLayer("Player")));
                Ray ray = UiMgr.GetCrosshairRay();

                if (Physics.Raycast(ray, out RaycastHit hit, 1000.0f, layerMask))
                {
                    inspectedObj = hit.collider.transform.GetComponent<Inspectable>();
                    isInspectEnabled = inspectedObj != null;
                }
            }
            else if (!m_IsGamePaused)
            {
                Time.timeScale = 1;
            }

            if (m_InspectedObj == null)
            {
                m_InspectedObj = inspectedObj;
            }

            if (inspectedObj != null && m_InspectedObj != null && m_InspectedObj != inspectedObj)
            {
                m_InspectedObj.EndInspect(PlayerParty.GetMostRecoveredCharacter());
                inspectedObj.StartInspect(PlayerParty.GetMostRecoveredCharacter());
            }
            else if (inspectedObj != null && !wasInspectEnabled && isInspectEnabled)
            {
                m_InspectedObj.StartInspect(PlayerParty.GetMostRecoveredCharacter());
            }
            else if (wasInspectEnabled && !isInspectEnabled)
            {
                m_InspectedObj.EndInspect(PlayerParty.GetMostRecoveredCharacter());
                m_InspectedObj = null;
            }

            m_InspectedObj = inspectedObj;
        }

        private void HandleDebugHotkeys()
        {
            if (Input.GetKeyDown(KeyCode.F2) && PlayerParty.Characters.Count > 0)
            {
                PlayerParty.RemoveCharacter(PlayerParty.Characters[0]);
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (PlayerParty.Characters.Count == 5)
                {
                    PlayerParty.RemoveCharacter(PlayerParty.Characters[0]);
                }

                PartyRosterService.AddRandomCharacter(PlayerParty);
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                TimeMgr.Instance.AddMinutes(30);
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                TimeMgr.Instance.AddMinutes(12 * 60);
            }

            if (Input.GetKeyDown(KeyCode.F5) && PlayerParty.ActiveCharacter != null)
            {
                PlayerParty.ActiveCharacter.Inventory.AddItem(538);
            }

            if (Input.GetKeyDown(KeyCode.F6) && PlayerParty.ActiveCharacter != null)
            {
                var randomEntry = DbMgr.Instance.ItemDb.Data.ElementAt(
                    UnityEngine.Random.Range(0, DbMgr.Instance.ItemDb.Data.Count));

                PlayerParty.ActiveCharacter.Inventory.AddItem(randomEntry.Key);
            }

            if (Input.GetKeyDown(KeyCode.F11) &&
                PlayerParty.ActiveCharacter != null &&
                PlayerParty.ActiveCharacter.Inventory.InventoryItems.Count > 0)
            {
                var randomEntry = PlayerParty.ActiveCharacter.Inventory.InventoryItems.ElementAt(
                    UnityEngine.Random.Range(0, PlayerParty.ActiveCharacter.Inventory.InventoryItems.Count));

                if (randomEntry != null)
                {
                    PlayerParty.ActiveCharacter.Inventory.RemoveItem(randomEntry);
                }
            }

            if (Input.GetKeyDown(KeyCode.F8) && PlayerParty.ActiveCharacter != null)
            {
                for (int i = 0; i < 100; i++)
                {
                    var randomEntry = DbMgr.Instance.ItemDb.Data.ElementAt(
                        UnityEngine.Random.Range(0, DbMgr.Instance.ItemDb.Data.Count));

                    PlayerParty.ActiveCharacter.Inventory.AddItem(randomEntry.Key);
                }
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                ProjectileInfo projectileInfo = new ProjectileInfo();
                projectileInfo.Shooter = PlayerParty.GetActiveCharacter();
                projectileInfo.ShooterTransform = PlayerParty.transform;
                projectileInfo.TargetPosition = UiMgr.GetCrosshairRay().GetPoint(100.0f);
                projectileInfo.DisplayData = DbMgr.Instance.ObjectDisplayDb.Get(6030);
                projectileInfo.ImpactObject = DbMgr.Instance.ObjectDisplayDb.Get(6031);

                Projectile.Spawn(projectileInfo);
            }
        }

        public void PressEscape()
        {
            UiMgr.Instance.HandleButtonDown("Escape");
        }

        public bool IsGamePaused()
        {
            return m_IsGamePaused || UiMgr.Instance.IsInGameBlockingState();
        }

        public void PauseGame()
        {
            Time.timeScale = 0.0f;
            m_IsGamePaused = true;
            //OnGamePaused();

            GameEvents.InvokeEvent_OnPauseGame();
        }

        public void UnpauseGame()
        {
            Time.timeScale = 1;
            m_IsGamePaused = false;
            //OnGameUnpaused();
            
            GameEvents.InvokeEvent_OnUnpauseGame();
        }

        public void ChangeGameState(GameState newState)
        {

        }

        //=========================================================================================
        // STATIC ACCESSORS
        //=========================================================================================

        static public PlayerParty GetParty()
        {
            return Instance.PlayerParty;
        }

        static public void SetStatusBarText(string text, bool overrideExisting = true, float duration = 2.0f)
        {
            Instance.StatusTextBar.SetText(text, overrideExisting, duration);
        }

        static public void ThrowItem(Transform transform, Vector3 direction, Item item)
        {
            if (item == null)
            {
                Debug.LogError("Thrown item is null");
                return;
            }

            GameObject outdoorItemPrefab = Resources.Load<GameObject>("Prefabs/Objects/OutdoorItem");
            if (outdoorItemPrefab == null)
            {
                Debug.LogError("Missing prefab at Resources/Prefabs/Objects/OutdoorItem");
                return;
            }

            GameObject outdoorItem = GameObject.Instantiate(
                outdoorItemPrefab,
                transform.position + (direction.normalized * 2.5f),
                Quaternion.LookRotation(direction));

            SpriteRenderer spriteRenderer = outdoorItem.GetComponent<SpriteRenderer>();
            Lootable lootable = outdoorItem.GetComponent<Lootable>();
            InspectableItem inspectableItem = outdoorItem.GetComponent<InspectableItem>();
            Rigidbody rigidbody = outdoorItem.GetComponent<Rigidbody>();

            if (spriteRenderer == null || lootable == null || inspectableItem == null || rigidbody == null)
            {
                Debug.LogError("OutdoorItem prefab is missing required components");
                GameObject.Destroy(outdoorItem);
                return;
            }

            spriteRenderer.sprite = item.Data.OutdoorSprite;
            lootable.Loot.Item = item;
            inspectableItem.Item = item;

            Debug.Log("[ThrowItem] Id: " + item.Data.Id);

            Vector3 speed = direction.normalized * 5.0f;
            rigidbody.velocity = speed;
        }

        static public SpriteObject GetSpriteObject(string name, string fromSpritesheet = "")
        {
            return SpriteObjectRegistry.GetSpriteObject(name, fromSpritesheet);
        }
    }
}
