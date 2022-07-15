using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using Assets.OpenMM8.Scripts.Gameplay.Data;
using UnityEngine.UI;
using Assets.OpenMM8.Scripts.Data;

using IngameDebugConsole;
using Assets.OpenMM8.Scripts.Gameplay.Items;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    
    /*public delegate void LevelUnloaded(int levelNum);
    public delegate void LevelLoaded(int levelNum);*/

    public delegate void MapButtonPressed();

    class GameCore : MonoBehaviour //Singleton<GameMgr>
    {
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

        [HideInInspector]
        public bool m_IsGamePaused = false;

        public List<BaseNpc> NpcList = new List<BaseNpc>();
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


            //......
            SpriteObjectRegistry.LoadSpritesheet("Decals");
            SpriteObjectRegistry.LoadSpritesheet("RocksTreesFlowers");
            SpriteObjectRegistry.LoadSpritesheet("SpellsProjectiles");
            SpriteObjectRegistry.LoadSpritesheet("m401");
            SpriteObjectRegistry.LoadSpritesheet("m409");
            SpriteObjectRegistry.LoadSpritesheet("m413");
            SpriteObjectRegistry.LoadSpritesheet("m417");
            SpriteObjectRegistry.LoadSpritesheet("m421");
            SpriteObjectRegistry.LoadSpritesheet("m549");

            SpriteObject testAnim = SpriteObjectRegistry.GetSpriteObject("spell57");
            foreach (Sprite animSprite in testAnim.BackSprites)
            {
                Debug.Log(animSprite.name);
            }
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

        public bool PostInit()
        {
            AddChar(21);
            AddChar(3);
            AddChar(16);
            AddChar(1);
            
            PlayerParty.Characters[0].Inventory.AddItem(513);
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
            if (Input.GetButtonDown("Console"))
            {
                UiMgr.Instance.HandleButtonDown("Console");
            }
            if (Input.GetButtonDown("Spellbook"))
            {
                UiMgr.Instance.HandleButtonDown("Spellbook");
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

            // TODO: Ingame command console
            /*if (Input.GetKeyDown(KeyCode.Semicolon))
            {
                
            }*/

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
                TimeMgr.Instance.AddMinutes(12 * 60);
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

            if (Input.GetKeyDown(KeyCode.E))
            {
                /*GameObject arrow = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/PlaceholderProjectile"));
                Projectile projectile = arrow.GetComponent<Projectile>();

                // Test
                projectile.AttackInfo = new AttackInfo();
                projectile.AttackInfo.AttackMod = 1;
                projectile.AttackInfo.MaxDamage = 2;
                projectile.AttackInfo.MinDamage = 1;
                projectile.AttackInfo.DamageType = SpellElement.Physical;
                projectile.Owner = GetParty().gameObject;

                projectile.IsTargetPlayer = false;

                Ray ray = UiMgr.GetCrosshairRay();
                projectile.ShootFromParty(PlayerParty, ray.direction);*/
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                /*GameObject arrow = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/TestSpellProjectile"));
                arrow.GetComponent<SpriteBillboardAnimator>().SetAnimation(SpriteObjectRegistry.GetSpriteObject("spell57"));

                Projectile projectile = arrow.GetComponent<Projectile>();

                // Test
                projectile.AttackInfo = new AttackInfo();
                projectile.AttackInfo.AttackMod = 1;
                projectile.AttackInfo.MaxDamage = 2;
                projectile.AttackInfo.MinDamage = 1;
                projectile.AttackInfo.DamageType = SpellElement.Physical;
                projectile.Owner = GetParty().gameObject;

                projectile.IsTargetPlayer = false;

                Ray ray = UiMgr.GetCrosshairRay();
                projectile.ShootFromParty(PlayerParty, ray.direction);*/

                ProjectileInfo projectileInfo = new ProjectileInfo();
                projectileInfo.Shooter = PlayerParty.GetActiveCharacter();
                projectileInfo.ShooterTransform = PlayerParty.transform;
                //projectileInfo.TargetDirection = UiMgr.GetCrosshairRay().direction;
                projectileInfo.TargetPosition = UiMgr.GetCrosshairRay().GetPoint(100.0f);
                projectileInfo.DisplayData = DbMgr.Instance.ObjectDisplayDb.Get(6030);
                projectileInfo.ImpactObject = DbMgr.Instance.ObjectDisplayDb.Get(6031);

                Projectile.Spawn(projectileInfo);
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

        // This adds initial character with initial attributes and skills to party
        // Fails if party is full
        // TODO: Make a dedicated class for these party-invitation actions
        public void AddChar(int characterId)
        {
            Character chr = new Character(characterId, PlayerParty);

            StartingStatsData startingStats = DbMgr.Instance.StartingStatsDb.Get(chr.Race);
            ClassStartingSkillsData startingSkills = DbMgr.Instance.ClassStartingSkillsDb.Get(chr.Class);

            chr.Name = "Tyrkys_" + characterId;
            chr.Level = 1;
            chr.BirthYear = 1152; // Current is 1172
            foreach (CharAttribute attr in Enum.GetValues(typeof(CharAttribute)))
            {
                if (attr == CharAttribute.None)
                {
                    continue;
                }
                chr.BaseAttributes[attr] = startingStats.DefaultStats[attr];
            }

            foreach (var skillAvailPair in startingSkills.SkillAvailabilityMap)
            {
                if (skillAvailPair.Value == StartingSkillAvailability.HasByDefault)
                {
                    chr.LearnSkill(skillAvailPair.Key);
                    if (chr.Race == CharacterRace.Vampire)
                    {
                        chr.LearnSpell(SpellType.Vampire_Lifedrain);
                    }
                }
            }
            chr.LearnSkill(SkillType.FireMagic);
            chr.LearnSkill(SkillType.WaterMagic);
            chr.LearnSkill(SkillType.AirMagic);
            chr.LearnSkill(SkillType.EarthMagic);
            chr.LearnSkill(SkillType.SpiritMagic);
            chr.LearnSkill(SkillType.MindMagic);
            chr.LearnSkill(SkillType.BodyMagic);
            chr.LearnSkill(SkillType.DarkMagic);
            chr.LearnSkill(SkillType.LightMagic);
            chr.LearnSkill(SkillType.DarkElfAbility);
            chr.LearnSkill(SkillType.VampireAbility);
            chr.LearnSkill(SkillType.DragonAbility);

            chr.AddSKill(SkillType.Meditation, SkillMastery.Grandmaster, 20);

            // Learn all spells
            foreach (SpellType spell in Enum.GetValues(typeof(SpellType)))
            {
                chr.LearnSpell(spell);
            }

            //chr.LearnSkill(SkillType.Staff);
            //chr.LearnSkill(SkillType.Blaster);

            chr.CurrHitPoints = chr.GetMaxHitPoints();
            chr.CurrSpellPoints = chr.GetMaxSpellPoints();
            PlayerParty.AddCharacter(chr);
        }

        public void AddRandChar()
        {
            int characterId = UnityEngine.Random.Range(1, 28);
            AddChar(characterId);
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
            GameObject outdoorItem = (GameObject)GameObject.Instantiate(Resources.Load("Prefabs/Objects/OutdoorItem"),
                transform.position + (transform.forward * 2.5f), transform.rotation);

            outdoorItem.GetComponent<SpriteRenderer>().sprite = item.Data.OutdoorSprite;
            outdoorItem.GetComponent<Lootable>().Loot.Item = item;
            outdoorItem.GetComponent<InspectableItem>().Item = item;

            Debug.Log("[ThrowItem] Id: " + item.Data.Id);

            Vector3 speed = UiMgr.GetCrosshairRay().direction * 5.0f;
            outdoorItem.GetComponent<Rigidbody>().velocity = speed;
        }

        static public SpriteObject GetSpriteObject(string name, string fromSpritesheet = "")
        {
            return SpriteObjectRegistry.GetSpriteObject(name, fromSpritesheet);
        }
    }
}
