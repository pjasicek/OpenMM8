using System;
using System.Collections.Generic;
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

        private Dictionary<CharacterType, CharacterSounds> CharacterSoundsMap =
            new Dictionary<CharacterType, CharacterSounds>();

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


        [Header("Sounds - Gold")]
        public AudioClip m_GoldChanged;

        //=================================== Unity Lifecycle ===================================

        private void Awake()
        {
            // Events
            SceneManager.sceneLoaded += OnSceneLoaded;

            GameMgr.OnPauseGame += OnGamePaused;
            GameMgr.OnUnpauseGame += OnGameUnpaused;

            PlayerParty.OnCharacterJoinedParty += OnCharacterJoinedParty;
            PlayerParty.OnCharacterLeftParty += OnCharacterLeftParty;
            PlayerParty.OnGoldChanged += OnGoldChanged;
            PlayerParty.OnFoodChanged += OnFoodChanged;

            Character.OnHealthChanged += OnHealthChanged;
            Character.OnHitNpc += OnCharHitNpc;
            Character.OnGotHit += OnCharGotHit;
            Character.OnAttack += OnCharAttack;

            Talkable.OnTalkStart += OnTalkStart;
            Talkable.OnTalkEnd += OnTalkEnd;
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


            m_AudioSource = gameObject.AddComponent<AudioSource>();
            m_AudioSource.clip = m_BackgroundMusic;
            m_AudioSource.loop = true;
            m_AudioSource.volume = 0.33f;
            m_AudioSource.Play();

            m_PlayerParty = GameMgr.Instance.PlayerParty;

            return true;
        }

        private void Start()
        {

        }


        //=================================== Methods ===================================

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

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            
        }

        private void OnHealthChanged(Character chr, int maxHealth, int currHealth, int delta)
        {
            float prevHealthPerc = ((float)(currHealth - delta) / (float)maxHealth) * 100.0f;
            float currHealthPerc = chr.GetHealthPercentage();

            if (prevHealthPerc > 20.0f && currHealthPerc < 20.0f)
            {
                PlayRandomSound(chr.Sounds.BadlyWounded, chr.Party.PlayerAudioSource);
            }
        }

        private void OnCharacterJoinedParty(Character chr, PlayerParty party)
        {
            chr.Sounds = GetCharacterSounds(chr.Data.CharacterType);
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
            m_AudioSource.PlayOneShot(m_GoldChanged, 1.5f);
        }

        public void OnFoodChanged(int oldFood, int newFood, int delta)
        {

        }

        public void OnCharConditionChanged(Character chr, Condition newCondition)
        {
            
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
                            bool isNpcStrong = npcData.Level > chr.Data.DefaultStats.Level;
                            if (isNpcStrong)
                            {
                                PlayRandomSound(chr.Sounds.KilledStrongMonster, chr.Party.PlayerAudioSource);
                            }
                            else
                            {
                                PlayRandomSound(chr.Sounds.KilledWeakMonster, chr.Party.PlayerAudioSource);
                            }
                        }
                    }
                }
            }
        }

        public void OnCharGotHit(Character chr, AttackInfo attackInfo, AttackResult attackResult)
        {
            float damagePercentage = ((float)attackResult.DamageDealt / (float)chr.GetMaxHealth()) * 100.0f;
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

        public void OnTalkStart(Character talkerChr, Talkable talkedToObj)
        {
            if (talkedToObj.IsBuilding)
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
            }
        }

        private void OnTalkEnd(Talkable talkedToObj)
        {
            if (talkedToObj.IsBuilding)
            {
                TalkableBuilding building = (TalkableBuilding)talkedToObj;
                if (building.LeaveSound)
                {
                    building.AudioSource.PlayOneShot(building.LeaveSound, building.SoundVolume);
                }
            }
        }

        public void OnItemInspect(Character inspectorChr, ItemData itemData/*, InspectResult result*/)
        {

        }


        public void OnItemEquip(/*Item item, EquipResult equipResult*/)
        {

        }
    }
}
