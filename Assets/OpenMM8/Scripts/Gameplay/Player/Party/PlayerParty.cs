using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    [RequireComponent(typeof(Damageable))]
    class PlayerParty : MonoBehaviour
    {
        public List<Character> Characters = new List<Character>();

        [SerializeField]
        private int MinutesSinceSleep;

        private void Awake()
        {
            
        }

        private void Start()
        {
            Damageable damageable = GetComponent<Damageable>();
            damageable.OnDamageReceived += new DamageReceived(OnDamageReceived);
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

        public void AddCharacter(Character character)
        {

        }

        // Damageable events
        DamageResult OnDamageReceived(int amount, GameObject source)
        {
            return DamageResult.Hit;
        }

        SpellResult OnSpellReceived(Spell spell, GameObject source)
        {
            return SpellResult.Hit;
        }
    }
}
