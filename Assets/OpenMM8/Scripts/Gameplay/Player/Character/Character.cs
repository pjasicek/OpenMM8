using Assets.OpenMM8.Scripts.Gameplay.Items;
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
    public delegate void ItemEquipped(Character chr, BaseItem equippedItem, BaseItem replacedItem);
    public delegate void InteractedWithItem(Character chr, BaseItem item, ItemInteractResult interactResult);

    public class Character
    {
        public CharacterData Data;
        public CharacterUI UI;
        public PlayerParty Party;
        public CharacterSounds Sounds;
        public CharFaceUpdater CharFaceUpdater;

        public Inventory Inventory = new Inventory();

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
        static public event ItemEquipped OnItemEquipped;
        static public event InteractedWithItem OnInteractedWithItem;


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
            Inventory.Owner = this;
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

        public bool IsFemale()
        {
            CharacterType chrType = Data.CharacterType;

            if (chrType == CharacterType.ClericFemale_1 ||
                chrType == CharacterType.ClericFemale_2 ||
                chrType == CharacterType.DarkElfFemale_1 ||
                chrType == CharacterType.DarkElfFemale_2 ||
                chrType == CharacterType.KnightFemale_1 ||
                chrType == CharacterType.KnightFemale_2 ||
                chrType == CharacterType.LichFemale_1 ||
                chrType == CharacterType.NecromancerFemale_1 ||
                chrType == CharacterType.NecromancerFemale_2 ||
                chrType == CharacterType.VampireFemale_1 ||
                chrType == CharacterType.VampireFemale_2)
            {
                return true;
            }

            return false;
        }

        public bool IsMale()
        {
            CharacterType chrType = Data.CharacterType;

            if (chrType == CharacterType.Cleric_1 ||
                chrType == CharacterType.Cleric_2||
                chrType == CharacterType.DarkElf_1 ||
                chrType == CharacterType.DarkElf_2 ||
                chrType == CharacterType.Knight_1 ||
                chrType == CharacterType.Knight_2 ||
                chrType == CharacterType.Lich_1 ||
                chrType == CharacterType.Necromancer_1 ||
                chrType == CharacterType.Necromancer_2 ||
                chrType == CharacterType.Vampire_1 ||
                chrType == CharacterType.Vampire_2)
            {
                return true;
            }

            return false;
        }

        public bool IsTroll()
        {
            CharacterType chrType = Data.CharacterType;

            if (chrType == CharacterType.Troll_1 ||
                chrType == CharacterType.Troll_2)
            {
                return true;
            }

            return false;
        }

        public bool IsMinotaur()
        {
            CharacterType chrType = Data.CharacterType;

            if (chrType == CharacterType.Minotaur_1 ||
                chrType == CharacterType.Minotaur_2)
            {
                return true;
            }

            return false;
        }

        public bool IsDragon()
        {
            CharacterType chrType = Data.CharacterType;

            if (chrType == CharacterType.Dragon_1 ||
                chrType == CharacterType.Dragon_2)
            {
                return true;
            }

            return false;
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

        public ItemInteractResult CanEquipItem(BaseItem item)
        {
            EquipType itemType = item.Data.EquipType;
            SkillGroup skillGroup = item.Data.SkillGroup;

            if (!item.IsEquippable())
            {
                return ItemInteractResult.Invalid;
            }

            // For now I do not support shields
            if (itemType == EquipType.Shield)
            {
                return ItemInteractResult.Invalid;
            }

            // Filter races
            if (IsMinotaur())
            {
                if (itemType == EquipType.Boots ||
                    itemType == EquipType.Helmet)
                {
                    return ItemInteractResult.Invalid;
                }
            }
            if (IsDragon())
            {
                if (itemType == EquipType.Amulet ||
                    itemType == EquipType.Ring)
                {
                    return ItemInteractResult.Equipped;
                }
                else
                {
                    return ItemInteractResult.Invalid;
                }
            }

            // Filter Item x Skill
            /*if (skillGroup == SkillGroup.Sword && Data.Skills[SkillType.Sword] <= 0)
            {
                return ItemInteractResult.CannotEquip;
            }
            if (skillGroup == SkillGroup.Dagger && Data.Skills[SkillType.Dagger] <= 0)
            {
                return ItemInteractResult.CannotEquip;
            }
            if (skillGroup == SkillGroup.Axe && Data.Skills[SkillType.Axe] <= 0)
            {
                return ItemInteractResult.CannotEquip;
            }
            if (skillGroup == SkillGroup.Spear && Data.Skills[SkillType.Spear] <= 0)
            {
                return ItemInteractResult.CannotEquip;
            }
            if (skillGroup == SkillGroup.Bow && Data.Skills[SkillType.Bow] <= 0)
            {
                return ItemInteractResult.CannotEquip;
            }
            if (skillGroup == SkillGroup.Mace && Data.Skills[SkillType.Mace] <= 0)
            {
                return ItemInteractResult.CannotEquip;
            }
            if (skillGroup == SkillGroup.Staff && Data.Skills[SkillType.Staff] <= 0)
            {
                return ItemInteractResult.CannotEquip;
            }
            if (skillGroup == SkillGroup.Leather && Data.Skills[SkillType.LeatherArmor] <= 0)
            {
                return ItemInteractResult.CannotEquip;
            }
            if (skillGroup == SkillGroup.Chain && Data.Skills[SkillType.ChainArmor] <= 0)
            {
                return ItemInteractResult.CannotEquip;
            }
            if (skillGroup == SkillGroup.Plate && Data.Skills[SkillType.PlateArmor] <= 0)
            {
                return ItemInteractResult.CannotEquip;
            }
            if (skillGroup == SkillGroup.Shield && Data.Skills[SkillType.Shield] <= 0)
            {
                return ItemInteractResult.CannotEquip;
            }*/


            return ItemInteractResult.Equipped;
        }

        public ItemInteractResult InteractWithItem(BaseItem item)
        {
            ItemInteractResult interactResult = ItemInteractResult.Invalid;
            if (item.IsEquippable())
            {
                // Try to equip the item. If success, we may have replaced the item by the old item on doll
                BaseItem replacedItem = null;
                interactResult = Inventory.TryEquipItem(item, out replacedItem);
                if (interactResult == ItemInteractResult.Equipped)
                {
                    /*// Held item was equipped by the doll - destroy it
                    GameObject.Destroy(m_HeldItem.gameObject);
                    m_HeldItem = null;

                    if (replacedItem != null)
                    {
                        // If we replaced an item which was equipped by doll, then we have to hold this item
                        SetHeldItem(replacedItem);
                    }*/
                    if (OnItemEquipped != null)
                    {
                        OnItemEquipped(this, item, replacedItem);
                    }
                }
            }
            else if (item.IsCastable())
            {
                interactResult = ItemInteractResult.Casted;
            }
            else if (item.IsConsumable())
            {
                interactResult = ItemInteractResult.Consumed;
            }
            else if (item.IsLearnable())
            {
                interactResult = ItemInteractResult.Learned;
            }
            else if (item.IsReadable())
            {
                interactResult = ItemInteractResult.Read;
            }

            if (OnInteractedWithItem != null)
            {
                OnInteractedWithItem(this, item, interactResult);
            }

            return interactResult;
        }
    }
}
