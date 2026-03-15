using System.Collections.Generic;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    [CreateAssetMenu(
        fileName = "DefaultGameStartupConfig",
        menuName = "OpenMM8/Bootstrap/Game Startup Config")]
    public class GameStartupConfig : ScriptableObject
    {
        [Header("Startup")]
        [SerializeField] private bool preloadStartupSpriteSheets = true;
        [SerializeField] private bool seedDebugPartyOnStart = true;
        [SerializeField] private List<string> startupSpriteSheets = new List<string>();
        [SerializeField] private List<int> debugPartyCharacterIds = new List<int>();
        [SerializeField] private List<int> debugStartingItemIds = new List<int>();

        public bool PreloadStartupSpriteSheets => preloadStartupSpriteSheets;
        public bool SeedDebugPartyOnStart => seedDebugPartyOnStart;
        public IReadOnlyList<string> StartupSpriteSheets => startupSpriteSheets;
        public IReadOnlyList<int> DebugPartyCharacterIds => debugPartyCharacterIds;
        public IReadOnlyList<int> DebugStartingItemIds => debugStartingItemIds;
    }
}
