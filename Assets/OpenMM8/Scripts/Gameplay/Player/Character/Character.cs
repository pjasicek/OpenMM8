using Assets.OpenMM8.Scripts.Gameplay.Data;
using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    // public delegate SpellResult SpellReceived(SpellInfo hitInfo, GameObject source);
    

    public class Character
    {
        // ID corresponding to CHARACTER_DATA table ID. MM8 characters are 1-28
        public int CharacterId;

        //public int CharacterAvatarId;
        //public int PartyIndex;

        // Data from DB
        public CharacterData CharacterData;
        public DollTypeData DollTypeData;
        public CharacterVoiceData CharacterVoiceData;

        // Gameplay Data
        public string Name;
        public CharacterRace Race;
        public CharacterClass Class;
        public int Experience;
        public int SkillPoints;
        public int CurrHitPoints;
        public int CurrSpellPoints;
        public Condition Condition;

        public string QuickSpellName = "";

        public CharacterStats Stats = new CharacterStats();
        public CharacterStats BonusStats = new CharacterStats();
        public Dictionary<SkillType, int> Skills = new Dictionary<SkillType, int>();
        public Dictionary<SkillType, int> SkillBonuses = new Dictionary<SkillType, int>();
        public List<Award> Awards = new List<Award>();
        public List<Spell> Spells = new List<Spell>();

        public int AttackBonus;
        public RangeInt AttackDamage;
        public int ShootBonus;
        public RangeInt ShootDamage;

        public Inventory Inventory = new Inventory();

        public List<BaseItem> EquippedItems = new List<BaseItem>();

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
                GameEvents.InvokeEvent_OnRecoveryTimeChanged(this, m_TimeUntilRecovery);
            }
        }

        // UI Data
        public CharacterUI UI;
        public PlayerParty Party;
        public CharacterSounds Sounds;
        public CharFaceUpdater CharFaceUpdater;

        public Character(int characterId)
        {
            CharacterData = DbMgr.Instance.CharacterDataDb.Get(characterId);
            DollTypeData = DbMgr.Instance.DollTypeDb.Get(CharacterData.DollId);
            CharacterVoiceData = DbMgr.Instance.CharacterVoiceDb.Get(CharacterData.DefaultVoice);

            CharacterId = CharacterData.Id;
            Race = CharacterData.Race;

            Inventory.Owner = this;
        }

        // ============================ PUBLIC API ============================ 

        public float GetHealthPercentage()
        {
            return ((float)CurrHitPoints / (float)GetMaxHealth()) * 100.0f;
        }

        public int GetMaxHealth()
        {
            return Stats.MaxHitPoints + BonusStats.MaxHitPoints;
        }

        public int GetPartyIndex()
        {
            return Party.Characters.FindIndex(ch => ch == this);
        }

        public bool IsFemale()
        {
            return CharacterData.DefaultSex == 1;
        }

        public bool IsMale()
        {
            return CharacterData.DefaultSex == 0;
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
                GameEvents.InvokeEvent_OnRecovered(this);
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

            GameEvents.InvokeEvent_OnCharAttack(this, attackInfo);

            if (victim)
            {
                AttackResult result = victim.ReceiveAttack(attackInfo, Party.gameObject);
                
                GameEvents.InvokeEvent_OnCharHitNpc(this, attackInfo, result);
            }

            return true;
        }

        public void OnAttackReceived(AttackInfo attackInfo, AttackResult result)
        {
            if (result.Type == AttackResultType.Hit || result.Type == AttackResultType.Kill)
            {
                GameEvents.InvokeEvent_OnCharGotHit(this, attackInfo, result);
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
            int maxHP = Stats.MaxHitPoints + BonusStats.MaxHitPoints;
            CurrHitPoints = Mathf.Min(CurrHitPoints + numHitPoints, maxHP);

            GameEvents.InvokeEvent_OnCharHealthChanged(this, maxHP, CurrHitPoints, numHitPoints);
        }

        public void AddCurrSpellPoints(int numSpellPoints)
        {
            int maxMP = Stats.MaxSpellPoints + BonusStats.MaxSpellPoints;
            CurrSpellPoints = Mathf.Min(CurrSpellPoints + numSpellPoints, maxMP);
    
            GameEvents.InvokeEvent_OnCharManaChanged(this, maxMP, CurrSpellPoints);
        }

        public void AddLevel()
        {

        }

        public void IncreaseSkillLevel(SkillType skillType, int amount = 1)
        {

        }

        public void ModifyAttribute(CharAttribute attribute, int amount)
        {

        }

        public void ModifyResistance(SpellElement element, int amount)
        {

        }

        public ItemInteractResult CanEquipItem(BaseItem item)
        {
            ItemType itemType = item.Data.ItemType;
            SkillGroup skillGroup = item.Data.SkillGroup;

            if (!item.IsEquippable())
            {
                return ItemInteractResult.Invalid;
            }

            // For now I do not support shields
            if (itemType == ItemType.Shield)
            {
                return ItemInteractResult.Invalid;
            }

            if (itemType == ItemType.Boots && !DollTypeData.CanEquipBoots)
            {
                return ItemInteractResult.Invalid;
            }
            else if (itemType == ItemType.Armor && !DollTypeData.CanEquipArmor)
            {
                return ItemInteractResult.Invalid;
            }
            else if (itemType == ItemType.Helmet && !DollTypeData.CanEquipHelm)
            {
                return ItemInteractResult.Invalid;
            }
            else if (itemType == ItemType.Belt && !DollTypeData.CanEquipBelt)
            {
                return ItemInteractResult.Invalid;
            }
            else if (itemType == ItemType.Cloak && !DollTypeData.CanEquipCloak)
            {
                return ItemInteractResult.Invalid;
            }
            else if (itemType == ItemType.WeaponDualWield && !DollTypeData.CanEquipWeapon)
            {
                return ItemInteractResult.Invalid;
            }
            else if (itemType == ItemType.WeaponOneHanded && !DollTypeData.CanEquipWeapon)
            {
                return ItemInteractResult.Invalid;
            }
            else if (itemType == ItemType.WeaponTwoHanded && !DollTypeData.CanEquipWeapon)
            {
                return ItemInteractResult.Invalid;
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
                    GameEvents.InvokeEvent_OnItemEquipped(this, item, replacedItem);
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

            GameEvents.InvokeEvent_OnInteractedWithItem(this, item, interactResult);

            return interactResult;
        }

        public void RecalculateStats()
        {
            // This method is stateless - it takes into consideration all factors in one shanpshot

            List<BaseItem> equippedArmorItems = new List<BaseItem>();
            // Armor = everything besides weapon(s)
            equippedArmorItems.Add(UI.DollUI.Armor.Item);
            equippedArmorItems.Add(UI.DollUI.Helmet.Item);
            equippedArmorItems.Add(UI.DollUI.Belt.Item);
            equippedArmorItems.Add(UI.DollUI.Boots.Item);
            equippedArmorItems.Add(UI.DollUI.Cloak.Item);
            equippedArmorItems.Add(UI.DollUI.Gauntlets.Item);
            equippedArmorItems.Add(UI.DollUI.Necklace.Item);
            equippedArmorItems.Add(UI.DollUI.Ring_1.Item);
            equippedArmorItems.Add(UI.DollUI.Ring_2.Item);
            equippedArmorItems.Add(UI.DollUI.Ring_3.Item);
            equippedArmorItems.Add(UI.DollUI.Ring_4.Item);
            equippedArmorItems.Add(UI.DollUI.Ring_5.Item);
            equippedArmorItems.Add(UI.DollUI.Ring_6.Item);
            //TODO: equippedArmor.Add(UI.DollUI.Shield.Item);

            // Only take into consideration equipped items
            equippedArmorItems.RemoveAll(item => item == null);

            BaseItem mainHandWeapon = UI.DollUI.RH_Weapon.Item;
            BaseItem offHandWeapon = UI.DollUI.LF_Weapon.Item;

            // Base armor class = character base + armor base
            Stats.ArmorClass = 0;

            BonusStats = new CharacterStats();
            foreach (BaseItem armorItem in equippedArmorItems)
            {
                switch (armorItem.Data.ItemType)
                {
                    case ItemType.Armor:
                    case ItemType.Helmet:
                    case ItemType.Cloak:
                    case ItemType.Shield:
                    case ItemType.Gauntlets:
                    case ItemType.Boots:
                        int armorClass = int.Parse(armorItem.Data.Mod1) + int.Parse(armorItem.Data.Mod2);
                        Stats.ArmorClass += armorClass;
                        break;
                }

                // TODO: Add enchant bonuses
                /*if (armorItem.Enchant != null)
                {

                }*/
            }



            // After all stat modifiers are applied

            // TODO: Do this generically, this is PoC
            int attrEffect = 0;
            int totalMight = Stats.Attributes[CharAttribute.Might] + BonusStats.Attributes[CharAttribute.Might];
            if (totalMight >= 500)
            {
                attrEffect = 30;
            }
            else if (totalMight >= 400)
            {
                attrEffect = 25;
            }
            else
            {
                // .....
            }

            AttackDamage.start = attrEffect;
            AttackDamage.length = 0;
            if (mainHandWeapon != null)
            {
                int baseDmg = int.Parse(mainHandWeapon.Data.Mod2);
                string[] diceCountAndSides = mainHandWeapon.Data.Mod1.Split('d');
                int minVariableDmg = int.Parse(diceCountAndSides[0]);
                int maxVariableDmg = int.Parse(diceCountAndSides[1]) * minVariableDmg;

                AttackDamage.start = attrEffect + minVariableDmg;
                AttackDamage.length = attrEffect + maxVariableDmg;

                if (AttackDamage.start <= 0)
                {
                    AttackDamage.start = 1;
                }
                if (AttackDamage.length <= 0)
                {
                    AttackDamage.length = 1;
                }
            }
        }
    }
}
