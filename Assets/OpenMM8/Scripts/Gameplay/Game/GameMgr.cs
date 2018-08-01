using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Assets.OpenMM8.Scripts.Gameplay.Data;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public delegate void PauseGame();
    public delegate void UnpauseGame();
    /*public delegate void LevelUnloaded(int levelNum);
    public delegate void LevelLoaded(int levelNum);*/

    public delegate void MapButtonPressed();

    class GameMgr : MonoBehaviour //Singleton<GameMgr>
    {
        public static GameMgr Instance;

        // Events
        static public event PauseGame OnPauseGame;
        static public event PauseGame OnUnpauseGame;
        /* static public event LevelUnloaded OnLevelUnloaded;
         static public event LevelLoaded OnLevelLoaded;*/

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

        [Header("Sounds")]
        public AudioClip BackgroundMusic;

        [HideInInspector]
        public bool m_IsGamePaused = false;

        // Private
        private Inspectable m_InspectedObj;

        void Awake()
        {
            // Events
            TalkEventMgr.OnTalkSceneStart += OnTalkStart;

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

            //CharacterSprites.Load(CharacterType.Dragon_1);

            return true;
        }

        public bool PostInit()
        {
            AddRandChar();
            AddRandChar();
            AddRandChar();
            AddRandChar();

            return true;
        }

        void Start()
        {

        }

        void Update()
        {
            // IDEA: When game is paused then maybe UiMgr should check if it can consume the event first ?
            if (Input.GetButtonDown("Escape"))
            {
                UiMgr.Instance.HandleButtonDown("Escape");
            }
            if (Input.GetButtonDown("Map"))
            {
                UiMgr.Instance.HandleButtonDown("Map");
            }
            if (Input.GetButtonDown("Inventory"))
            {
                UiMgr.Instance.HandleButtonDown("Inventory");
            }
            if (Input.GetButtonDown("NextPlayer"))
            {
                UiMgr.Instance.HandleButtonDown("NextPlayer");
            }
            /*if (Input.GetButtonDown("Queust"))
            {
                UiMgr.Instance.HandleButtonDown("Queust");
            }
            if (Input.GetButtonDown("Notes"))
            {
                UiMgr.Instance.HandleButtonDown("Notes");
            }
            if (Input.GetButtonDown("Rest"))
            {
                UiMgr.Instance.HandleButtonDown("Rest");
            }
            */

            bool wasInspectEnabled = (m_InspectedObj != null);
            bool isInspectEnabled = false;
            Inspectable inspectedObj = null;

            if (Input.GetButton("InspectObject") && !UiMgr.Instance.IsInGameBlockingState())
            {
                if (!m_IsGamePaused)
                {
                    Time.timeScale = 0;
                }

                RaycastHit hit;
                Ray ray = UiMgr.GetCrosshairRay();
                //ray.origin -= 100 * ray.direction.normalized;

                int layerMask = ~((1 << LayerMask.NameToLayer("NpcRangeTrigger")) | (1 << LayerMask.NameToLayer("Player")));
                if (Physics.Raycast(ray, out hit, 1000.0f, layerMask))
                {
                    Transform objectHit = hit.collider.transform;
                    if (objectHit.GetComponent<Inspectable>() != null)
                    {
                        inspectedObj = objectHit.GetComponent<Inspectable>();
                        isInspectEnabled = true;
                    }
                }

                //Debug.DrawRay(ray.origin, ray.direction, Color.green);
            }
            else
            {
                if (!m_IsGamePaused)
                {
                    Time.timeScale = 1;
                }
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

            if (Input.GetKeyDown(KeyCode.F2))
            {
                PlayerParty.RemoveCharacter(PlayerParty.Characters[0]);
            }

            if (Input.GetKeyDown(KeyCode.F1))
            {
                if (PlayerParty.Characters.Count == 5)
                {
                    PlayerParty.RemoveCharacter(PlayerParty.Characters[0]);
                }
                AddRandChar();
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                TimeMgr.Instance.AddMinutes(30);
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                TimeMgr.Instance.AddMinutes(TimeMgr.DAY_IN_MINUTES / 2);
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                if (PlayerParty.ActiveCharacter != null)
                {
                    PlayerParty.ActiveCharacter.Inventory.AddItem(538);
                }
            }
            if (Input.GetKeyDown(KeyCode.F6))
            {
                if (PlayerParty.ActiveCharacter != null)
                {
                    var randomEntry = DbMgr.Instance.ItemDb.Data.ElementAt(
                        UnityEngine.Random.Range(0, DbMgr.Instance.ItemDb.Data.Count));

                    PlayerParty.ActiveCharacter.Inventory.AddItem(randomEntry.Key);
                }
            }
        }

        public void AddRosterNpcToParty(int rosterId)
        {
            if (PlayerParty.IsFull())
            {
                // Add to the Adventurerer's Inn
            }
            else
            {
                AddRandChar();
            }
        }

        public void AddRandChar()
        {
            var chrType = (CharacterType)UnityEngine.Random.Range(1, (int)Enum.GetValues(typeof(CharacterType)).Cast<CharacterType>().Max());

            CharacterData charData = new CharacterData();
            charData.CharacterType = chrType;
            charData.Name = "Tyrkys";
            charData.Class = Class.Knight;
            charData.Experience = 0;
            charData.SkillPoints = 0;
            charData.CurrHitPoints = 500;
            charData.CurrSpellPoints = 50;
            charData.Condition = Condition.Good;
            charData.DefaultStats.Age = 30;
            charData.DefaultStats.Level = 1;
            charData.DefaultStats.MaxHitPoints = 500;
            charData.DefaultStats.MaxSpellPoints = 50;

            foreach (Attribute attr in Enum.GetValues(typeof(Attribute)))
            {
                charData.DefaultStats.Attributes[attr] = 0;
                charData.BonusStats.Attributes[attr] = 0;
            }

            foreach (SpellElement resist in Enum.GetValues(typeof(SpellElement)))
            {
                charData.DefaultStats.Resistances[resist] = 0;
                charData.BonusStats.Resistances[resist] = 0;
            }

            Character chr = new Character(charData);

            PlayerParty.AddCharacter(chr);
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
            Time.timeScale = 0;
            m_IsGamePaused = true;
            //OnGamePaused();

            if (OnPauseGame != null)
            {
                OnPauseGame();
            }
        }

        public void UnpauseGame()
        {
            Time.timeScale = 1;
            m_IsGamePaused = false;
            //OnGameUnpaused();

            if (OnUnpauseGame != null)
            {
                OnUnpauseGame();
            }
        }

        public void ChangeGameState(GameState newState)
        {

        }

        public static void Log(string msg)
        {
            Debug.Log(msg);
        }

        //==================================== Events ====================================

        public void OnTalkStart(Character talkerChr, TalkScene talkScene)
        {
            PauseGame();
        }
    }
}
