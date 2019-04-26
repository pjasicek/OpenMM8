using Assets.OpenMM8.Scripts.Gameplay.Data;
using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    class SoundMgr : Singleton<SoundMgr>
    {
        // Public

        // Private
        private AudioSource m_AudioSource;
        private AudioClip m_BackgroundMusic;
        private PlayerParty m_PlayerParty;

        [Header("Sounds - Attack")]
        private List<AudioClip> m_SwordAttacks = new List<AudioClip>();
        private List<AudioClip> m_AxeAttacks = new List<AudioClip>();
        private List<AudioClip> m_BluntAttacks = new List<AudioClip>();
        private List<AudioClip> m_BowAttacks = new List<AudioClip>();
        private List<AudioClip> m_DragonAttacks = new List<AudioClip>();
        private List<AudioClip> m_BlasterAttacks = new List<AudioClip>();

        [Header("Sounds - Got Hit")]
        private AudioClip m_WeaponVsMetal_Light;
        private AudioClip m_WeaponVsMetal_Medium;
        private AudioClip m_WeaponVsMetal_Hard;
        private AudioClip m_WeaponVsLeather_Light;
        private AudioClip m_WeaponVsLeather_Medium;
        private AudioClip m_WeaponVsLeather_Hard;

        private List<AudioClip> m_CharGotHit_Leather = new List<AudioClip>();
        private List<AudioClip> m_CharGotHit_Metal = new List<AudioClip>();


        [Header("Sounds")]
        private AudioClip m_GoldChanged;
        private AudioClip m_Quest;
        private AudioClip m_Error;

        private Dictionary<string, AudioClip> m_SoundMap = new Dictionary<string, AudioClip>();
        private Dictionary<int, AudioClip> m_SoundIdMap = new Dictionary<int, AudioClip>();

        //=================================== Unity Lifecycle ===================================

        private void Awake()
        {
            // Events
            SceneManager.sceneLoaded += OnSceneLoaded;

            GameEvents.OnPauseGame += OnGamePaused;
            GameEvents.OnUnpauseGame += OnGameUnpaused;

            GameEvents.OnCharacterLeftParty += OnCharacterLeftParty;
            GameEvents.OnGoldChanged += OnGoldChanged;
            GameEvents.OnFoodChanged += OnFoodChanged;
            GameEvents.OnPickedUpLoot += OnPickedUpLoot;

            GameEvents.OnCharHealthChanged += OnHealthChanged;
            GameEvents.OnCharHitNpc += OnCharHitNpc;
            GameEvents.OnCharGotHit += OnCharGotHit;
            GameEvents.OnCharAttack += OnCharAttack;
            GameEvents.OnInteractedWithItem += OnInteractedWithItem;

            GameEvents.OnTalkSceneStart += OnTalkSceneStart;
            GameEvents.OnTalkSceneEnd += OnTalkSceneEnd;

            GameEvents.OnQuestBitAdded += OnQuestBitAdded;

            GameEvents.OnCharacterFinishedEvent += OnCharacterFinishedEvent;
        }

        // Init sequence: DbMgr(1) -> GameMgr(1) -> *Mgr(1) -> GameMgr(2)
        public bool Init()
        {
            // Load sounds / music
            m_BackgroundMusic = Resources.Load<AudioClip>("Music/1");

            // Character weapon attack sounds
            m_SwordAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Sword_1"));
            m_SwordAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Sword_2"));
            m_AxeAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Axe_1"));
            m_AxeAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Axe_2"));
            m_BluntAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Blunt_1"));
            m_BluntAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Blunt_2"));
            m_BowAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Bow_1"));
            m_DragonAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_DragonBreath_1"));
            m_BlasterAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Blaster_1"));

            // Character getting hit sounds - based on character's armor
            m_WeaponVsMetal_Light = Resources.Load<AudioClip>("Player/Sounds/PlayerGotHit/Wood_vs_Metal_Light");
            m_WeaponVsMetal_Medium = Resources.Load<AudioClip>("Player/Sounds/PlayerGotHit/Wood_vs_Metal_Medium");
            m_WeaponVsMetal_Hard = Resources.Load<AudioClip>("Player/Sounds/PlayerGotHit/Wood_vs_Metal_Heavy");
            m_WeaponVsLeather_Light = Resources.Load<AudioClip>("Player/Sounds/PlayerGotHit/Wood_vs_Leather_Light");
            m_WeaponVsLeather_Medium = Resources.Load<AudioClip>("Player/Sounds/PlayerGotHit/Wood_vs_Leather_Medium");
            m_WeaponVsLeather_Hard = Resources.Load<AudioClip>("Player/Sounds/PlayerGotHit/Wood_vs_Leather_Heavy");

            m_CharGotHit_Leather.Add(m_WeaponVsLeather_Light);
            m_CharGotHit_Leather.Add(m_WeaponVsLeather_Medium);
            m_CharGotHit_Leather.Add(m_WeaponVsLeather_Hard);

            m_CharGotHit_Metal.Add(m_WeaponVsMetal_Light);
            m_CharGotHit_Metal.Add(m_WeaponVsMetal_Medium);
            m_CharGotHit_Metal.Add(m_WeaponVsMetal_Hard);

            // UI sounds
            m_GoldChanged = Resources.Load<AudioClip>("Player/Sounds/UI/GoldChanged");
            m_Quest = Resources.Load<AudioClip>("Sounds/Quest");
            m_Error = Resources.Load<AudioClip>("Sounds/Error");

            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.clip = m_BackgroundMusic;
            m_AudioSource.loop = true;
            m_AudioSource.volume = 0.33f;
            //m_AudioSource.Play();

            m_PlayerParty = GameMgr.Instance.PlayerParty;

            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            /*AudioClip[] sounds = Resources.LoadAll<AudioClip>("Sounds");
            foreach (AudioClip sound in sounds)
            {
                m_SoundMap[sound.name] = sound;
            }*/

            sw.Stop();
            //UnityEngine.Debug.LogError("Sound load elapsed: " + sw.ElapsedMilliseconds + " ms");

            return true;
        }

        private void Start()
        {

        }


        //=================================== Methods ===================================
        
        // TODO: Handle sound caching or something, this way most sounds get lazy loaded
        //       but the never get unloaded once they are loaded
        // TODO: Maybe use existing SoundData database to store and cache sounds

        public static void PlaySoundById(int soundId, AudioSource audioSource = null)
        {
            if (audioSource == null)
            {
                audioSource = Instance.m_AudioSource;
            }

            SoundData soundData = DbMgr.Instance.SoundDb.Get(soundId);
            if (soundData == null)
            {
                Debug.LogError("No sound data for ID: " + soundId);
                return;
            }

            string soundName = soundData.SoundName;
            PlaySoundByName(soundName, audioSource);
        }

        public static void PlaySoundByName(string soundName, AudioSource audioSource = null)
        {
            if (audioSource == null)
            {
                audioSource = Instance.m_AudioSource;
            }

            AudioClip sound = null;
            if (!Instance.m_SoundMap.ContainsKey(soundName))
            {
                // Try to load it
                sound = Resources.Load<AudioClip>("Sounds/" + soundName);
                if (sound == null)
                {
                    Debug.LogError("Failed to load sound: " + soundName);
                    return;
                }

                Instance.m_SoundMap.Add(soundName, sound);
            }
            else
            {
                sound = Instance.m_SoundMap[soundName];
            }

            audioSource.PlayOneShot(sound);
        }

        public static void PlayRandomSound(List<int> soundIds)
        {
            PlayRandomSound(soundIds, Instance.m_AudioSource);
        }

        public static void PlayRandomSound(List<int> soundIds, AudioSource audioSource)
        {
            if (soundIds.Count == 0)
            {
                return;
            }

            int soundId = soundIds[UnityEngine.Random.Range(0, soundIds.Count)];

            PlaySoundById(soundId);
        }
    
        public static AudioClip PlayRandomSound(List<AudioClip> sounds)
        {
            return PlayRandomSound(sounds, Instance.m_AudioSource);
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

        //=================================== Events ===================================

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            
        }

        private void OnQuestBitAdded(int questId)
        {
            m_AudioSource.PlayOneShot(m_Quest);
        }

        private void OnCharacterFinishedEvent(Character chr)
        {
            m_AudioSource.PlayOneShot(m_Quest);
        }

        private void OnHealthChanged(Character chr, int maxHealth, int currHealth, int delta)
        {
            float prevHealthPerc = ((float)(currHealth - delta) / (float)maxHealth) * 100.0f;
            float currHealthPerc = chr.GetHealthPercentage();

            if (prevHealthPerc > 20.0f && currHealthPerc < 20.0f)
            {
                //PlayRandomSound(chr.Sounds.BadlyWounded, chr.Party.PlayerAudioSource);
            }
        }

        private void OnCharacterLeftParty(Character chr, PlayerParty party)
        {

        }

        public void OnGamePaused()
        {
            
        }

        private void OnGameUnpaused()
        {
            if (!m_AudioSource.isPlaying)
            {
                m_AudioSource.UnPause();
            }
        }

        public void OnGoldChanged(int oldGold, int newGold, int delta)
        {
            //m_AudioSource.PlayOneShot(m_GoldChanged, 1.5f);

            PlaySoundById(GameMgr.Instance.PlayerParty.ActiveCharacter.VoiceData.LearnSpell[0]);
        }

        public void OnFoodChanged(int oldFood, int newFood, int delta)
        {

        }

        public void OnPickedUpLoot(Loot loot)
        {
            m_AudioSource.PlayOneShot(m_GoldChanged, 1.5f);

            //PlaySoundById(GameMgr.Instance.PlayerParty.ActiveCharacter.CharacterVoiceData.LearnSpell[0]);
        }

        public void OnCharConditionChanged(Character chr, Condition newCondition)
        {
            
        }

        public void OnInteractedWithItem(Character chr, Item item, ItemInteractResult interactResult)
        {
            switch (interactResult)
            {
                case ItemInteractResult.AlreadyLearned:
                    PlayRandomSound(chr.VoiceData.CantLearn);
                    break;

                case ItemInteractResult.Equipped:
                    break;

                case ItemInteractResult.CannotEquip:
                    PlayRandomSound(chr.VoiceData.CantEquip);
                    break;

                case ItemInteractResult.Learned:
                    PlayRandomSound(chr.VoiceData.LearnSpell);
                    break;

                case ItemInteractResult.CannotLearn:
                    PlayRandomSound(chr.VoiceData.CantLearn);
                    break;

                case ItemInteractResult.Consumed:
                    
                    break;

                case ItemInteractResult.Casted:

                    break;

                case ItemInteractResult.Read:
                    
                    break;

                case ItemInteractResult.Invalid:
                    PlaySoundByName("Error");
                    break;
            }
        }

        private void OnCharAttack(Character chr, AttackInfo attackInfo)
        {
            // TODO: Handle attack sounds based on character's equpped weapon
            PlayRandomSound(m_SwordAttacks);
            //Party.PlayerAudioSource.PlayOneShot(Party.SwordAttacks[UnityEngine.Random.Range(0, Party.SwordAttacks.Count)]);
        }

        public void OnCharHitNpc(Character chr, AttackInfo attackInfo, AttackResult result)
        {
            if ((result.Type == AttackResultType.Kill) && (result.Victim != null))
            {
                bool shouldComment = UnityEngine.Random.Range(0, 2) == 0;
                if (shouldComment)
                {
                    bool isVictimNpc = result.Victim.GetComponent<BaseNpc>() != null;
                    if (isVictimNpc)
                    {
                        bool isVictimInMeleeRange = 
                            Vector3.Distance(result.Victim.transform.position, chr.Party.GetPosition()) < Constants.MeleeRangeDistance;
                        if (isVictimInMeleeRange)
                        {
                            MonsterData npcData = result.Victim.GetComponent<BaseNpc>().NpcData;
                            bool isNpcStrong = npcData.Level > chr.Level;
                            if (isNpcStrong)
                            {
                                //PlayRandomSound(chr.Sounds.KilledStrongMonster, chr.Party.PlayerAudioSource);
                            }
                            else
                            {
                                //PlayRandomSound(chr.Sounds.KilledWeakMonster, chr.Party.PlayerAudioSource);
                            }
                        }
                    }
                }
            }
        }

        public void OnCharGotHit(Character chr, AttackInfo attackInfo, AttackResult attackResult)
        {
            float damagePercentage = ((float)attackResult.DamageDealt / (float)chr.GetMaxHitPoints()) * 100.0f;
            if (attackInfo.DamageType == SpellElement.Physical)
            {
                PlayRandomSound(m_CharGotHit_Leather, chr.Party.PlayerAudioSource);

                /*if (damagePercentage > 20.0f)
                {
                    chr.Party.PlayerAudioSource.PlayOneShot(WeaponVsLeather_Hard);
                }
                else if (damagePercentage > 10.0f)
                {
                    chr.Party.PlayerAudioSource.PlayOneShot(WeaponVsLeather_Hard);
                }
                else
                {
                    Debug.Log("Playing");
                    if (WeaponVsLeather_Light == null) Debug.Log("Null");
                    chr.Party.PlayerAudioSource.PlayOneShot(WeaponVsLeather_Light);
                }*/
            }

            // TODO - handle based on character's armor
        }

        public void OnNpcInspect(Character inspectorChr, MonsterData npcData)
        {

        }

        public void OnTalkSceneStart(Character talkerChr, TalkScene talkScene)
        {
            /*if (talkedToObj.IsBuilding)
            {
                // Pause background music
                m_AudioSource.Pause();

                TalkableBuilding building = (TalkableBuilding)talkedToObj;
                if (building.EnterSound)
                {
                    building.AudioSource.PlayOneShot(building.EnterSound, building.SoundVolume);
                }

                if (building.GreetSound)
                {
                    building.AudioSource.PlayOneShot(building.GreetSound, building.SoundVolume);
                }
            }
            else
            {
                PlayRandomSound(talkerChr.Sounds.Greeting, talkerChr.Party.PlayerAudioSource);
            }*/

            if (!talkScene.IsBuilding)
            {
                PlayRandomSound(talkerChr.VoiceData.GoodDay);
            }
        }

        private void OnTalkSceneEnd(Character talkerChr, TalkScene talkScene)
        {
            /*if (talkedToObj.IsBuilding)
            {
                TalkableBuilding building = (TalkableBuilding)talkedToObj;
                if (building.LeaveSound)
                {
                    building.AudioSource.PlayOneShot(building.LeaveSound, building.SoundVolume);
                }
            }*/
        }

        public void OnItemInspect(Character inspectorChr, ItemData itemData/*, InspectResult result*/)
        {

        }


        public void OnItemEquip(/*Item item, EquipResult equipResult*/)
        {

        }
    }
}
