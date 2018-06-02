using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    // public delegate SpellResult SpellReceived(SpellInfo hitInfo, GameObject source);
    public delegate void HealthChanged(Character chr, int maxHealth, int currHealth, int delta);
    public delegate void ManaChanged(Character chr, int maxMana, int currMana);
    public delegate void Recovered(Character chr);
    public delegate void RecoveryTimeChanged(Character chr, float recoveryTime);
    public delegate void CharConditionChanged(Character chr, Condition newCondition);
    public delegate void CharHitNpc(Character chr, AttackInfo attackInfo, AttackResult result);
    public delegate void CharGotHit(Character chr, AttackInfo attackInfo, AttackResult attackResult);
    public delegate void CharAttack(Character chr, AttackInfo attackInfo);
    public delegate void NpcInspect(Character inspectorChr, MonsterData npcData);
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

        public CharFaceUpdater CharFaceUpdater;

        // Events
        static public event HealthChanged OnHealthChanged;
        static public event ManaChanged OnManaChanged;
        static public event Recovered OnRecovered;
        static public event RecoveryTimeChanged OnRecoveryTimeChanged;
        static public event CharConditionChanged OnConditionChanged;
        static public event CharHitNpc OnHitNpc;
        static public event CharGotHit OnGotHit;
        static public event CharAttack OnAttack;
        static public event NpcInspect OnNpcInspect;
        static public event NpcInspectEnd OnNpcInspectEnd;
        static public event ItemInspect OnItemInspect;
        static public event ItemEquip OnItemEquip;
        static public event ItemHold OnItemHold;
        static public event ItemHoldEnd OnItemHoldEnd;  


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

        public Character(CharacterData charData)
        {
            Data = charData;
        }

        // ============================ PUBLIC API ============================ 

        public float GetHealthPercentage()
        {
            return ((float)Data.CurrHitPoints / (float)GetMaxHealth()) * 100.0f;
        }

        public int GetMaxHealth()
        {
            return Data.DefaultStats.MaxHitPoints + Data.BonusStats.MaxHitPoints;
        }

        public int GetPartyIndex()
        {
            return Party.Characters.FindIndex(ch => ch == this);
        }

        // ====================================================================

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

            TimeUntilRecovery = UnityEngine.Random.Range(1.25f, 2.0f);

            AttackInfo attackInfo = new AttackInfo();
            attackInfo.MinDamage = 38;
            attackInfo.MaxDamage = 64;
            attackInfo.AttackMod = 10000;
            attackInfo.DamageType = SpellElement.Physical;

            if (OnAttack != null)
            {
                OnAttack(this, attackInfo);
            }

            if (victim)
            {
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
            if (OnGotHit != null &&
                (result.Type == AttackResultType.Hit || result.Type == AttackResultType.Kill))
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
                OnHealthChanged(this, maxHP, Data.CurrHitPoints, numHitPoints);
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
    }
}
