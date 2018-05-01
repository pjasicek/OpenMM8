using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Assets.OpenMM8.Scripts.Gameplay.Data;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    public delegate void ReturnToGame();
    public delegate void PauseGame();
    /*public delegate void LevelUnloaded(int levelNum);
    public delegate void LevelLoaded(int levelNum);*/

    public delegate void MapButtonPressed();

    class GameMgr : MonoBehaviour //Singleton<GameMgr>
    {
        public static GameMgr Instance;

        // Events
        static public event ReturnToGame OnReturnToGame;
        static public event PauseGame OnPauseGame;
       /* static public event LevelUnloaded OnLevelUnloaded;
        static public event LevelLoaded OnLevelLoaded;*/
        static public event MapButtonPressed OnMapButtonPressed;

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
        public bool IsGamePaused = false;

        // Private
        private Inspectable m_InspectedObj;

        void Awake()
        {
            // Events
            Talkable.OnTalkWithNpc += OnTalkWithNpc;

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

            bool wasInspectEnabled = (m_InspectedObj != null);
            bool isInspectEnabled = false;
            Inspectable inspectedObj = null;

            if (Input.GetButton("InspectObject"))
            {
                if (!IsGamePaused)
                {
                    Time.timeScale = 0;
                }

                RaycastHit hit;
                Ray ray = UiMgr.Instance.GetCrosshairRay();
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
                if (!IsGamePaused)
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

            if (Input.GetButtonDown("Map"))
            {
                if (OnMapButtonPressed != null)
                {
                    OnMapButtonPressed();
                }

                /*if (!(IsGamePaused && !MapQuestNotesUI.Canvas.enabled))
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
                }*/
            }

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

        public void ReturnToGame()
        {
            UnpauseGame();

            if (OnReturnToGame != null)
            {
                OnReturnToGame();
            }
        }

        public void PauseGame()
        {
            Time.timeScale = 0;
            IsGamePaused = true;
            //OnGamePaused();

            if (OnPauseGame != null)
            {
                OnPauseGame();
            }
        }

        public void UnpauseGame()
        {
            Time.timeScale = 1;
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

        //==================================== Events ====================================

        public void OnTalkWithNpc(Character talkerChr, Talkable talkedToObj)
        {
            PauseGame();
        }
    }
}
