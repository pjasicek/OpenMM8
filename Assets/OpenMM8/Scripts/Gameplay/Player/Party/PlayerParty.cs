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

        [SerializeField]
        private int MinutesSinceSleep;

        private HostilityChecker HostilityChecker;

        private List<GameObject> EnemiesInMeleeRange = new List<GameObject>();
        private List<GameObject> EnemiesInAgroRange = new List<GameObject>();

        private void Awake()
        {
            
        }

        private void Start()
        {
            HostilityChecker = GetComponent<HostilityChecker>();

            Damageable damageable = GetComponent<Damageable>();
            damageable.OnAttackReceieved += new AttackReceived(OnAttackReceived);
            damageable.OnSpellReceived += new SpellReceived(OnSpellReceived);
        }

        public void Update()
        {

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
