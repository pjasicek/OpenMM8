using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    class InitMgr : MonoBehaviour
    {
        private const string DefaultStartupConfigResourcePath = "Configs/DefaultGameStartupConfig";

        [Header("Startup")]
        [SerializeField] private GameStartupConfig startupConfig;

        private bool didLogMissingStartupConfig;

        private void Awake()
        {
            DbMgr.Instance.Init();

            if (ShouldPreloadStartupSpriteSheets())
            {
                PreloadStartupSpriteSheets();
            }
        }

        private void Start()
        {
            InitializeManagers();
            RunPostInitialization();
        }

        private void InitializeManagers()
        {
            ItemGenerator.Instance.Init();

            TimeMgr.Instance.Init();
            UiMgr.Instance.Init();
            GameCore.Instance.Init();
            SoundMgr.Instance.Init();
            QuestMgr.Instance.Init();
            GameEventMgr.Instance.Init();
            TalkEventMgr.Instance.Init();
        }

        private void RunPostInitialization()
        {
            UiMgr.Instance.PostInit();

            if (ShouldSeedDebugPartyOnStart())
            {
                SeedDebugParty();
            }

            GameEvents.InvokeEvent_OnInitComplete();
        }

        private void PreloadStartupSpriteSheets()
        {
            foreach (string spritesheet in GetConfiguredSpriteSheets())
            {
                SpriteObjectRegistry.LoadSpritesheet(spritesheet);
            }
        }

        private void SeedDebugParty()
        {
            PartyRosterService.SeedDebugParty(
                GameCore.Instance.PlayerParty,
                GetConfiguredDebugPartyCharacterIds(),
                GetConfiguredDebugStartingItemIds());
        }

        private IEnumerable<string> GetConfiguredSpriteSheets()
        {
            GameStartupConfig config = ResolveStartupConfig();
            return config != null ? config.StartupSpriteSheets : Array.Empty<string>();
        }

        private IEnumerable<int> GetConfiguredDebugPartyCharacterIds()
        {
            GameStartupConfig config = ResolveStartupConfig();
            return config != null ? config.DebugPartyCharacterIds : Array.Empty<int>();
        }

        private IEnumerable<int> GetConfiguredDebugStartingItemIds()
        {
            GameStartupConfig config = ResolveStartupConfig();
            return config != null ? config.DebugStartingItemIds : Array.Empty<int>();
        }

        private bool ShouldPreloadStartupSpriteSheets()
        {
            GameStartupConfig config = ResolveStartupConfig();
            return config != null && config.PreloadStartupSpriteSheets;
        }

        private bool ShouldSeedDebugPartyOnStart()
        {
            GameStartupConfig config = ResolveStartupConfig();
            return config != null && config.SeedDebugPartyOnStart;
        }

        private GameStartupConfig ResolveStartupConfig()
        {
            if (startupConfig == null)
            {
                startupConfig = Resources.Load<GameStartupConfig>(DefaultStartupConfigResourcePath);
            }

            if (startupConfig == null && !didLogMissingStartupConfig)
            {
                Debug.LogError("[InitMgr] Missing startup config at Resources/" + DefaultStartupConfigResourcePath);
                didLogMissingStartupConfig = true;
            }

            return startupConfig;
        }
    }
}
