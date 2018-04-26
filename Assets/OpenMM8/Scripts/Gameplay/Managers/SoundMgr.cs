using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay.Managers
{
    class SoundMgr : Singleton<SoundMgr>
    {
        // Public

        // Private
        private AudioSource AudioSource;
        private AudioClip BackgroundMusic;

        private Dictionary<CharacterType, CharacterSounds> CharacterSoundsMap =
            new Dictionary<CharacterType, CharacterSounds>();

        //=================================== Unity Lifecycle ===================================

        private void Awake()
        {
            // Events
            GameMgr.OnReturnToGame += OnReturnToGame;
            GameMgr.OnPauseGame += OnGamePaused;

            PlayerParty.OnCharacterJoinedParty += OnCharacterJoinedParty;
            PlayerParty.OnCharacterLeftParty += OnCharacterLeftParty;
            PlayerParty.OnGoldChanged += OnGoldChanged;
            PlayerParty.OnFoodChanged += OnFoodChanged;

            Character.OnHitNpc += OnCharHitNpc;
        }

        // Init sequence: DbMgr(1) -> GameMgr(1) -> *Mgr(1) -> GameMgr(2)
        public bool Init()
        {
            AudioSource = gameObject.AddComponent<AudioSource>();
            BackgroundMusic = Resources.Load<AudioClip>("Music/Music_DaggerWoundIsland");

            AudioSource.clip = BackgroundMusic;
            AudioSource.loop = true;
            AudioSource.volume = 0.33f;
            AudioSource.Play();

            return true;
        }

        private void OnCharacterJoinedParty(Character chr, PlayerParty party)
        {
            chr.Sounds = GetCharacterSounds(chr.Data.CharacterType);
        }

        private void OnCharacterLeftParty(Character chr, PlayerParty party)
        {

        }

        private void Start()
        {

        }

        private void OnLevelWasLoaded(int level)
        {
            
            Debug.Log("Loaded !");
        }

        //=================================== Methods ===================================

        public static AudioClip PlayRandomSound(List<AudioClip> sounds)
        {
            return PlayRandomSound(sounds, SoundMgr.Instance.AudioSource);
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

        //=================================== Events ===================================

        public void OnReturnToGame()
        {
            AudioSource.UnPause();
        }

        public void OnGamePaused()
        {
            AudioSource.Pause();
        }

        public void OnGoldChanged(int oldGold, int newGold, int delta)
        {

        }

        public void OnFoodChanged(int oldFood, int newFood, int delta)
        {

        }

        public void OnCharConditionChanged(Character chr, Condition newCondition)
        {

        }

        public void OnCharHitNpc(Character chr, AttackInfo attackInfo, AttackResult result)
        {
         
        }

        public void OnCharGotHit(Character chr, AttackInfo attackInfo, AttackResult attackResult)
        {

        }

        public void OnNpcInspect(Character inspectorChr, NpcData npcData)
        {

        }

        public void OnItemInspect(Character inspectorChr, ItemData itemData/*, InspectResult result*/)
        {

        }

        public void OnItemEquip(/*Item item, EquipResult equipResult*/)
        {

        }
    }
}
