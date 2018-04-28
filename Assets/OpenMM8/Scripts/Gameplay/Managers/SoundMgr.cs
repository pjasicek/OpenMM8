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
        private PlayerParty PlayerParty;

        private Dictionary<CharacterType, CharacterSounds> CharacterSoundsMap =
            new Dictionary<CharacterType, CharacterSounds>();

        [Header("Sounds - Attack")]
        private List<AudioClip> SwordAttacks = new List<AudioClip>();
        private List<AudioClip> AxeAttacks = new List<AudioClip>();
        private List<AudioClip> BluntAttacks = new List<AudioClip>();
        private List<AudioClip> BowAttacks = new List<AudioClip>();
        private List<AudioClip> DragonAttacks = new List<AudioClip>();
        private List<AudioClip> BlasterAttacks = new List<AudioClip>();

        [Header("Sounds - Got Hit")]
        private AudioClip WeaponVsMetal_Light;
        private AudioClip WeaponVsMetal_Medium;
        private AudioClip WeaponVsMetal_Hard;
        private AudioClip WeaponVsLeather_Light;
        private AudioClip WeaponVsLeather_Medium;
        private AudioClip WeaponVsLeather_Hard;

        private List<AudioClip> CharGotHit_Leather = new List<AudioClip>();
        private List<AudioClip> CharGotHit_Metal = new List<AudioClip>();


        [Header("Sounds - Gold")]
        public AudioClip GoldChanged;

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
            Character.OnGotHit += OnCharGotHit;
            Character.OnAttack += OnCharAttack;
        }

        // Init sequence: DbMgr(1) -> GameMgr(1) -> *Mgr(1) -> GameMgr(2)
        public bool Init()
        {
            // Load sounds / music
            BackgroundMusic = Resources.Load<AudioClip>("Music/Music_DaggerWoundIsland");

            // Character weapon attack sounds
            SwordAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Sword_1"));
            SwordAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Sword_2"));
            AxeAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Axe_1"));
            AxeAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Axe_2"));
            BluntAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Blunt_1"));
            BluntAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Blunt_2"));
            BowAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Bow_1"));
            DragonAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_DragonBreath_1"));
            BlasterAttacks.Add(Resources.Load<AudioClip>("Player/Sounds/PlayerAttack/PlayerAttack_Blaster_1"));

            // Character getting hit sounds - based on character's armor
            WeaponVsMetal_Light = Resources.Load<AudioClip>("Player/Sounds/PlayerGotHit/Wood_vs_Metal_Light");
            WeaponVsMetal_Medium = Resources.Load<AudioClip>("Player/Sounds/PlayerGotHit/Wood_vs_Metal_Medium");
            WeaponVsMetal_Hard = Resources.Load<AudioClip>("Player/Sounds/PlayerGotHit/Wood_vs_Metal_Heavy");
            WeaponVsLeather_Light = Resources.Load<AudioClip>("Player/Sounds/PlayerGotHit/Wood_vs_Leather_Light");
            WeaponVsLeather_Medium = Resources.Load<AudioClip>("Player/Sounds/PlayerGotHit/Wood_vs_Leather_Medium");
            WeaponVsLeather_Hard = Resources.Load<AudioClip>("Player/Sounds/PlayerGotHit/Wood_vs_Leather_Heavy");

            CharGotHit_Leather.Add(WeaponVsLeather_Light);
            CharGotHit_Leather.Add(WeaponVsLeather_Medium);
            CharGotHit_Leather.Add(WeaponVsLeather_Hard);

            CharGotHit_Metal.Add(WeaponVsMetal_Light);
            CharGotHit_Metal.Add(WeaponVsMetal_Medium);
            CharGotHit_Metal.Add(WeaponVsMetal_Hard);

            // UI sounds
            GoldChanged = Resources.Load<AudioClip>("Player/Sounds/UI/GoldChanged");


            AudioSource = gameObject.AddComponent<AudioSource>();
            AudioSource.clip = BackgroundMusic;
            AudioSource.loop = true;
            AudioSource.volume = 0.33f;
            AudioSource.Play();

            PlayerParty = GameMgr.Instance.PlayerParty;

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
            return PlayRandomSound(sounds, Instance.AudioSource);
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
            AudioSource.PlayOneShot(GoldChanged, 1.5f);
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
            PlayRandomSound(SwordAttacks);
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
                            NpcData npcData = result.Victim.GetComponent<BaseNpc>().NpcData;
                            if (chr.Data.DefaultStats.Level > (npcData.Level))
                            {
                                PlayRandomSound(chr.Sounds.KilledWeakMonster, chr.Party.PlayerAudioSource);
                            }
                            else
                            {
                                PlayRandomSound(chr.Sounds.KilledStrongMonster, chr.Party.PlayerAudioSource);
                            }
                        }
                    }
                }
            }
        }

        public void OnCharGotHit(Character chr, AttackInfo attackInfo, AttackResult attackResult)
        {
            float damagePercentage = ((float)attackResult.DamageDealt / (float)chr.GetMaxHealth()) * 100.0f;
            Debug.Log("DamagePerc: " + damagePercentage);
            if (attackInfo.DamageType == SpellElement.Physical)
            {
                PlayRandomSound(CharGotHit_Leather, chr.Party.PlayerAudioSource);

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
