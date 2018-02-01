using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    [RequireComponent(typeof(HostilityChecker))]
    [RequireComponent(typeof(Damageable))]
    class PlayerParty : MonoBehaviour, ITriggerListener
    {
        public List<Character> Characters = new List<Character>();
        public Character ActiveCharacter;

        [Header("Sounds - Attack")]
        public List<AudioClip> SwordAttacks = new List<AudioClip>();
        public List<AudioClip> AxeAttacks = new List<AudioClip>();
        public List<AudioClip> BluntAttacks = new List<AudioClip>();
        public List<AudioClip> BowAttacks = new List<AudioClip>();
        public List<AudioClip> DragonAttacks = new List<AudioClip>();
        public List<AudioClip> BlasterAttacks = new List<AudioClip>();

        [Header("Sounds - Got Hit")]
        public AudioClip WeaponVsMetal_Light;
        public AudioClip WeaponVsMetal_Medium;
        public AudioClip WeaponVsMetal_Hard;
        public AudioClip WeaponVsLeather_Light;
        public AudioClip WeaponVsLeather_Medium;
        public AudioClip WeaponVsLeather_Hard;

        [SerializeField]
        private int MinutesSinceSleep;

        private HostilityChecker HostilityChecker;
        public AudioSource PlayerAudioSource;

        private List<GameObject> EnemiesInMeleeRange = new List<GameObject>();
        private List<GameObject> EnemiesInAgroRange = new List<GameObject>();

        // Misc
        private float AttackDelayTimeLeft = 0.0f;

        private void Awake()
        {
            
        }

        private void Start()
        {
            HostilityChecker = GetComponent<HostilityChecker>();

            Damageable damageable = GetComponent<Damageable>();
            damageable.OnAttackReceieved += new AttackReceived(OnAttackReceived);
            damageable.OnSpellReceived += new SpellReceived(OnSpellReceived);
            PlayerAudioSource = transform.Find("FirstPersonCharacter").GetComponent<AudioSource>();
        }

        public void Update()
        {
            AttackDelayTimeLeft -= Time.deltaTime;

            foreach (Character character in Characters)
            {
                character.OnUpdate(Time.deltaTime);
            }

            if (ActiveCharacter == null || !ActiveCharacter.IsRecovered())
            {
                foreach (Character character in Characters)
                {
                    if (character.IsRecovered() && ((ActiveCharacter == null) || !ActiveCharacter.IsRecovered()))
                    {
                        ActiveCharacter = character;
                        ActiveCharacter.CharacterUI.SelectionRing.enabled = true;
                    }
                    else
                    {
                        character.CharacterUI.SelectionRing.enabled = false;
                    }
                }
            }

            if (Input.GetButton("Fire1") && (AttackDelayTimeLeft <= 0.0f))
            {
                if (ActiveCharacter != null && ActiveCharacter.IsRecovered())
                {
                    Damageable victim = null;

                    // 1) Try to attack NPC which is being targeted by the crosshair
                    //    - does not have to be enemy, when we are aiming with attack
                    //    directly on NPC, we can attack guard / villager this way
                    RaycastHit hit;
                    Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5F, 0.595F, 0));
                    if (Physics.Raycast(ray, out hit, 2.5f, 1 << LayerMask.NameToLayer("NPC")))
                    {
                        Transform objectHit = hit.collider.transform;
                        if (objectHit.GetComponent<Damageable>() != null)
                        {
                            victim = objectHit.GetComponent<Damageable>();
                        }
                    }

                    // 2) Try to attack enemy which is closest to Player
                    if (EnemiesInMeleeRange.Count > 0)
                    {
                        EnemiesInMeleeRange.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude);
                        foreach (GameObject enemyObject in EnemiesInMeleeRange)
                        {
                            if (enemyObject.GetComponent<Renderer>().isVisible)
                            {
                                victim = enemyObject.GetComponent<Damageable>();
                                break;
                            }
                        }
                    }

                    ActiveCharacter.Attack(victim);
                    ActiveCharacter = null;
                    AttackDelayTimeLeft = 0.1f;
                }
            }

        }

        private void UpdatePartyEffects(int msDiff)
        {

        }

        private void UpdateConditions(int msDiff)
        {

        }

        private void UpdatePartyAgroStatus(AgroState agroState)
        {
            foreach (Character character in Characters)
            {
                character.CharacterUI.SetAgroStatus(agroState);
            }
        }

        public void AddCharacter(Character character)
        {
            character.PlayerParty = this;
            Characters.Add(character);
        }

        // Damageable events
        AttackResult OnAttackReceived(AttackInfo hitInfo, GameObject source)
        {
            Debug.Log("Received damage !");
            Characters[0].ModifyCurrentHitPoints(-1 * UnityEngine.Random.Range(hitInfo.MinDamage, hitInfo.MaxDamage));
            //Characters[0].ModifyCurrentHitPoints(-4);

            return AttackResult.Hit;
        }

        SpellResult OnSpellReceived(SpellInfo hitInfo, GameObject source)
        {
            return SpellResult.Hit;
        }

        //---------------------------------------------------------------------
        // Triggers
        //---------------------------------------------------------------------
        public void OnObjectEnteredMyTrigger(GameObject other, TriggerType triggerType)
        {
            Debug.Log("Entered: " + other.name);
            switch (triggerType)
            {
                case TriggerType.MeleeRange:
                    OnObjectEnteredMeleeRange(other);
                    break;

                case TriggerType.AgroRange:
                    OnObjectEnteredAgroRange(other);
                    break;

                default:
                    Debug.LogError("Unhandled Trigger Type: " + triggerType);
                    break;
            }
        }

        public void OnObjectLeftMyTrigger(GameObject other, TriggerType triggerType)
        {
            switch (triggerType)
            {
                case TriggerType.MeleeRange:
                    OnObjectLeftMeleeRange(other);
                    break;

                case TriggerType.AgroRange:
                    OnObjectLeftAgroRange(other);
                    break;

                default:
                    Debug.LogError("Unhandled Trigger Type: " + triggerType);
                    break;
            }
        }

        public void OnObjectEnteredMeleeRange(GameObject other)
        {
            if (HostilityChecker.IsHostileTo(other))
            {
                EnemiesInMeleeRange.Add(other);
                UpdateAgroStatus();
            }
        }

        public void OnObjectLeftMeleeRange(GameObject other)
        {
            EnemiesInMeleeRange.Remove(other);
            UpdateAgroStatus();
        }

        public void OnObjectEnteredAgroRange(GameObject other)
        {
            if (HostilityChecker.IsHostileTo(other))
            {
                EnemiesInAgroRange.Add(other);
                UpdateAgroStatus();
            }
        }

        public void OnObjectLeftAgroRange(GameObject other)
        {
            EnemiesInAgroRange.Remove(other);
            UpdateAgroStatus();
        }

        private void UpdateAgroStatus()
        {
            if (EnemiesInMeleeRange.Count > 0)
            {
                UpdatePartyAgroStatus(AgroState.HostileClose);
            }
            else if (EnemiesInAgroRange.Count > 0)
            {
                UpdatePartyAgroStatus(AgroState.HostileNearby);
            }
            else
            {
                UpdatePartyAgroStatus(AgroState.Safe);
            }
        }
    }
}
