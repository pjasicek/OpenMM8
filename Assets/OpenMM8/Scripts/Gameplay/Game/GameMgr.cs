using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Assets.OpenMM8.Scripts.Gameplay.Data;
using UnityEngine.UI;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    
    /*public delegate void LevelUnloaded(int levelNum);
    public delegate void LevelLoaded(int levelNum);*/

    public delegate void MapButtonPressed();

    class GameMgr : MonoBehaviour //Singleton<GameMgr>
    {
        public static GameMgr Instance;

        // Events
        
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
            GameEvents.OnTalkSceneStart += OnTalkStart;

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
            /*AddChar(CharacterType.KnightFemale_1);
            AddRandChar();
            AddRandChar();
            AddRandChar();
            AddRandChar();*/
            AddChar(21);
            AddChar(3);
            AddChar(16);
            AddChar(1);
            
            PlayerParty.Characters[0].Inventory.AddItem(513);
            /*PlayerParty.Characters[0].Inventory.AddItem(84);
            PlayerParty.Characters[0].Inventory.AddItem(85);
            PlayerParty.Characters[0].Inventory.AddItem(86);
            PlayerParty.Characters[0].Inventory.AddItem(87);
            PlayerParty.Characters[0].Inventory.AddItem(88);
            PlayerParty.Characters[0].Inventory.AddItem(89);
            PlayerParty.Characters[0].Inventory.AddItem(90);
            PlayerParty.Characters[0].Inventory.AddItem(91);
            PlayerParty.Characters[0].Inventory.AddItem(92);
            PlayerParty.Characters[0].Inventory.AddItem(93);
            PlayerParty.Characters[0].Inventory.AddItem(94);
            PlayerParty.Characters[0].Inventory.AddItem(95);
            PlayerParty.Characters[0].Inventory.AddItem(96);
            PlayerParty.Characters[0].Inventory.AddItem(97);
            PlayerParty.Characters[0].Inventory.AddItem(98);
            PlayerParty.Characters[0].Inventory.AddItem(514);*/
            /*AddChar(CharacterType.VampireFemale_2);
            AddChar(CharacterType.VampireFemale_2);
            AddChar(CharacterType.VampireFemale_2);
            AddChar(CharacterType.VampireFemale_2);*/
            PlayerParty.Characters[0].Inventory.AddItem(514);
            PlayerParty.Characters[0].Inventory.AddItem(117);
            PlayerParty.Characters[0].Inventory.AddItem(132);
            PlayerParty.Characters[0].Inventory.AddItem(522);
            PlayerParty.Characters[0].Inventory.AddItem(522);
            PlayerParty.Characters[0].Inventory.AddItem(522);
            PlayerParty.Characters[0].Inventory.AddItem(512);
            PlayerParty.Characters[0].Inventory.AddItem(532);
            PlayerParty.Characters[0].Inventory.AddItem(529);
            PlayerParty.Characters[0].Inventory.AddItem(115);
            PlayerParty.Characters[0].Inventory.AddItem(536);

            PlayerParty.Characters[0].Inventory.AddItem(151);
            PlayerParty.Characters[0].Inventory.AddItem(517);

            PlayerParty.Characters[0].Inventory.AddItem(141);
            PlayerParty.Characters[0].Inventory.AddItem(141);
            PlayerParty.Characters[0].Inventory.AddItem(141);
            PlayerParty.Characters[0].Inventory.AddItem(141);
            PlayerParty.Characters[0].Inventory.AddItem(141);
            PlayerParty.Characters[0].Inventory.AddItem(141);
            PlayerParty.Characters[0].Inventory.AddItem(141);
            PlayerParty.Characters[0].Inventory.AddItem(519);

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
            if (Input.GetKeyDown(KeyCode.F11))
            {
                if (PlayerParty.ActiveCharacter != null)
                {
                    var randomEntry = PlayerParty.ActiveCharacter.Inventory.InventoryItems.ElementAt(
                        UnityEngine.Random.Range(0, PlayerParty.ActiveCharacter.Inventory.InventoryItems.Count));

                    if (randomEntry != null)
                    {
                        PlayerParty.ActiveCharacter.Inventory.RemoveItem(randomEntry);
                    }
                }
            }
            if (Input.GetKeyDown(KeyCode.F8))
            {
                if (PlayerParty.ActiveCharacter != null)
                {
                    for (int i = 0; i < 100; i++)
                    {
                        var randomEntry = DbMgr.Instance.ItemDb.Data.ElementAt(
                        UnityEngine.Random.Range(0, DbMgr.Instance.ItemDb.Data.Count));

                        PlayerParty.ActiveCharacter.Inventory.AddItem(randomEntry.Key);
                    }
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

        public void AddChar(int characterId)
        {
            Character chr = new Character(characterId);

            chr.Name = "Tyrkys";
            chr.Class = CharacterClass.Knight;
            chr.Experience = 0;
            chr.SkillPoints = 0;
            chr.CurrHitPoints = 500;
            chr.CurrSpellPoints = 50;
            chr.Condition = Condition.Good;
            chr.DefaultStats.Age = 30;
            chr.DefaultStats.Level = 1;
            chr.DefaultStats.MaxHitPoints = 500;
            chr.DefaultStats.MaxSpellPoints = 50;

            foreach (CharAttribute attr in Enum.GetValues(typeof(CharAttribute)))
            {
                chr.DefaultStats.Attributes[attr] = 0;
                chr.BonusStats.Attributes[attr] = 0;
            }

            foreach (SpellElement resist in Enum.GetValues(typeof(SpellElement)))
            {
                chr.DefaultStats.Resistances[resist] = 0;
                chr.BonusStats.Resistances[resist] = 0;
            }

            

            PlayerParty.AddCharacter(chr);
        }

        public void AddRandChar()
        {
            var chrType = (CharacterType)UnityEngine.Random.Range(1, (int)Enum.GetValues(typeof(CharacterType)).Cast<CharacterType>().Max());

            int characterId = UnityEngine.Random.Range(1, 28);

            Character chr = new Character(characterId);

            chr.Name = "Tyrkys";
            chr.Class = CharacterClass.Knight;
            chr.Experience = 0;
            chr.SkillPoints = 0;
            chr.CurrHitPoints = 500;
            chr.CurrSpellPoints = 50;
            chr.Condition = Condition.Good;
            chr.DefaultStats.Age = 30;
            chr.DefaultStats.Level = 1;
            chr.DefaultStats.MaxHitPoints = 500;
            chr.DefaultStats.MaxSpellPoints = 50;

            foreach (CharAttribute attr in Enum.GetValues(typeof(CharAttribute)))
            {
                chr.DefaultStats.Attributes[attr] = 0;
                chr.DefaultStats.Attributes[attr] = 0;
            }

            foreach (SpellElement resist in Enum.GetValues(typeof(SpellElement)))
            {
                chr.DefaultStats.Resistances[resist] = 0;
                chr.DefaultStats.Resistances[resist] = 0;
            }

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
