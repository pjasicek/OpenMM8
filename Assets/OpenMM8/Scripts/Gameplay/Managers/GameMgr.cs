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
    public delegate void LevelUnloaded(int levelNum);
    public delegate void LevelLoaded(int levelNum);

    public delegate void MapButtonPressed();

    class GameMgr : MonoBehaviour //Singleton<GameMgr>
    {
        public static GameMgr Instance;

        // Events
        static public event ReturnToGame OnReturnToGame;
        static public event PauseGame OnPauseGame;
        static public event LevelUnloaded OnLevelUnloaded;
        static public event LevelLoaded OnLevelLoaded;
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

        private AudioSource AudioSource;
        private Dictionary<CharacterType, CharacterSounds> CharacterSoundsMap =
            new Dictionary<CharacterType, CharacterSounds>();

        private Dictionary<CharacterType, CharacterSprites> CharacterSpritesMap =
            new Dictionary<CharacterType, CharacterSprites>();

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
            AudioSource = gameObject.AddComponent<AudioSource>();
            AudioSource.clip = BackgroundMusic;
            AudioSource.loop = true;
            AudioSource.volume = 0.33f;
            AudioSource.Play();
            // 1) Gather relevant game objects

            PlayerParty = GameObject.Find("Player").GetComponent<PlayerParty>();
            Debug.Assert(PlayerParty != null);

            //CharacterSprites.Load(CharacterType.Dragon_1);

            return true;
        }

        public bool PostInit()
        {
            CharacterData charData = new CharacterData();
            charData.CharacterType = CharacterType.Lich_1;
            charData.CharacterAvatarId = 27;
            charData.PartyIndex = 1;
            charData.Name = "Tyrkys";
            charData.Class = Class.Necromancer;
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

            /*CharacterUI characterUI1 = new CharacterUI();
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

            PlayerParty.AddCharacter(Character.Create(charData, characterUI1, CharacterType.Lich_1));*/

            if (OnLevelLoaded != null)
            {
                OnLevelLoaded(1);
            }

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
            AudioSource.Pause();
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

        //==================================== Events ====================================

        public void OnTalkWithNpc(Character talkerChr, Talkable talkedToObj)
        {
            PauseGame();
        }
    }
}
