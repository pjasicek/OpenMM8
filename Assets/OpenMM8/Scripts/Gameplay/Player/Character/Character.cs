using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    // public delegate SpellResult SpellReceived(SpellInfo hitInfo, GameObject source);
    public delegate void HealthChanged(Character chr, int maxHealth, int currHealth);
    public delegate void ManaChanged(Character chr, int maxMana, int currMana);
    public delegate void Recovered(Character chr);
    public delegate void RecoveryTimeChanged(Character chr, float recoveryTime);
    public delegate void CharConditionChanged(Character chr, Condition newCondition);
    public delegate void CharHitNpc(Character chr, AttackInfo attackInfo, AttackResult result);
    public delegate void CharGotHit(Character chr, AttackInfo attackInfo, AttackResult attackResult);
    public delegate void NpcInspect(Character inspectorChr, NpcData npcData);
    public delegate void NpcInspectEnd();
    public delegate void ItemInspect(Character inspectorChr, ItemData itemData/*, InspectResult result*/);
    public delegate void ItemEquip(/*Item item, EquipResult equipResult*/);
    public delegate void ItemHold(/*Item item*/);
    public delegate void ItemHoldEnd();

    public class Character
    {
        public CharacterData Data;
        public CharacterUI UI;
        public PlayerParty Party;
        public CharacterSounds Sounds;
        public CharacterSprites Sprites;

        public CharFaceUpdater CharFaceUpdater;

        // Events
        public event HealthChanged OnHealthChanged;
        public event ManaChanged OnManaChanged;
        public event Recovered OnRecovered;
        public event RecoveryTimeChanged OnRecoveryTimeChanged;
        public event CharConditionChanged OnConditionChanged;
        public event CharHitNpc OnHitNpc;
        public event CharGotHit OnGotHit;
        public event NpcInspect OnNpcInspect;
        public event NpcInspectEnd OnNpcInspectEnd;
        public event ItemInspect OnItemInspect;
        public event ItemEquip OnItemEquip;
        public event ItemHold OnItemHold;
        public event ItemHoldEnd OnItemHoldEnd;


        private float m_TimeUntilRecovery = 0.0f;
        public float TimeUntilRecovery
        {
            get
            {
                return m_TimeUntilRecovery;
            }

            set
            {
                m_TimeUntilRecovery = value;
                OnRecoveryTimeChanged(this, m_TimeUntilRecovery);
            }
        }

        public Character()
        {
            CharFaceUpdater = new CharFaceUpdater(this);
        }

        public static Character Create(CharacterData characterData, CharacterUI characterUI, CharacterType type)
        {
            Character character = new Character();
            character.Data = characterData;
            character.UI = characterUI;
            character.Sounds = GameMgr.Instance.GetCharacterSounds(type);
            character.Sprites = GameMgr.Instance.GetCharacterSprites(type);

            character.UI.PlayerCharacter.sprite = character.Sprites.ConditionToSpriteMap[Condition.Good];

            return character;
        }

        public void OnFixedUpdate(float secDiff)
        {
            CharFaceUpdater.OnFixedUpdate(secDiff);
        }

        public void OnUpdate(float secDiff)
        {
            bool wasRecovered = IsRecovered();
            TimeUntilRecovery -= secDiff;

            if (!wasRecovered && IsRecovered())
            {
                if (OnRecovered != null)
                {
                    OnRecovered(this);
                }
            }

            /*if (!IsRecovered() && CharacterUI.SelectionRing.enabled == true)
            {
                CharacterUI.SelectionRing.enabled = false;
            }
            else if (IsRecovered() && CharacterUI.SelectionRing.enabled == false)
            {
                CharacterUI.SelectionRing.enabled = true;
            }*/
        }

        public bool IsRecovered()
        {
            return TimeUntilRecovery <= 0.0f;
        }

        public bool Attack(Damageable victim)
        {
            if (TimeUntilRecovery > 0.0f)
            {
                return false;
            }

            TimeUntilRecovery = 1.0f;
            Party.PlayerAudioSource.PlayOneShot(Party.SwordAttacks[UnityEngine.Random.Range(0, Party.SwordAttacks.Count)]);

            if (victim)
            {
                AttackInfo attackInfo = new AttackInfo();
                attackInfo.MinDamage = 38;
                attackInfo.MaxDamage = 64;
                attackInfo.AttackMod = 10000;
                attackInfo.DamageType = SpellElement.Physical;

                AttackResult result = victim.ReceiveAttack(attackInfo, Party.gameObject);
                
                if (OnHitNpc != null)
                {
                    OnHitNpc(this, attackInfo, result);
                }
            }

            return true;
        }

        public void OnAttackReceived(AttackInfo attackInfo, AttackResult result)
        {
            if (OnGotHit != null)
            {
                OnGotHit(this, attackInfo, result);
            }
        }

        // Events
        void UnequipItem(ItemData item)
        {

        }

        void EquipItem(ItemData item)
        {

        }

        bool CanEquipItem(ItemData item)
        {
            return true;
        }

        public void OnItemPickedUp(ItemData item, bool fromPartyMember)
        {

        }

        public void AddCurrHitPoints(int numHitPoints)
        {
            int maxHP = Data.DefaultStats.MaxHitPoints + Data.BonusStats.MaxHitPoints;
            Data.CurrHitPoints = Mathf.Min(Data.CurrHitPoints + numHitPoints, maxHP);

            if (OnHealthChanged != null)
            {
                OnHealthChanged(this, maxHP, Data.CurrHitPoints);
            }
        }

        public void AddCurrSpellPoints(int numSpellPoints)
        {
            int maxMP = Data.DefaultStats.MaxSpellPoints + Data.BonusStats.MaxSpellPoints;
            Data.CurrSpellPoints = Mathf.Min(Data.CurrSpellPoints + numSpellPoints, maxMP);

            if (OnManaChanged != null)
            {
                OnManaChanged(this, maxMP, Data.CurrSpellPoints);
            }
        }

        public void AddLevel()
        {

        }

        public void IncreaseSkillLevel(SkillType skillType, int amount = 1)
        {

        }

        public void ModifyAttribute(Attribute attribute, int amount)
        {

        }

        public void ModifyResistance(SpellElement element, int amount)
        {

        }

        // Avatar expressions
        public void Smile()
        {
            UI.PlayerCharacter.sprite =
                Sprites.Smile[UnityEngine.Random.Range(0, Sprites.Smile.Count)];
        }

        public void TakeDamage()
        {
            UI.PlayerCharacter.sprite =
                Sprites.TakeDamage[UnityEngine.Random.Range(0, Sprites.TakeDamage.Count)];
        }
    }
}
