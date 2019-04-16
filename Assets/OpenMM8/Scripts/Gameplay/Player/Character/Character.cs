using Assets.OpenMM8.Scripts.Data;
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
        
        public string QuickSpellName = "None";

        // Buffs
        public Dictionary<PlayerEffectType, SpellEffect> PlayerBuffMap = new Dictionary<PlayerEffectType, SpellEffect>();

        //=============== Base Stats ==================//
        public int BirthYear;
        public int AgeModifier; // Special aging - e.g. Divine Intervention spell casts

        public int Level;

        public Dictionary<CharAttribute, int> BaseAttributes = new Dictionary<CharAttribute, int>();
        public Dictionary<SpellElement, int> BaseResistances = new Dictionary<SpellElement, int>();

        public Dictionary<SkillType, Skill> Skills = new Dictionary<SkillType, Skill>();

        //=============================================//

        public List<Award> Awards = new List<Award>();
        public List<Spell> Spells = new List<Spell>();

        public int AttackBonus;
        public int MinAttackDamage;
        public int MaxAttackDamage;

        public int ShootBonus;
        public int MinShootDamage;
        public int MaxShootDamage;

        int AttackRecoveryTime;
        int BeingHitRecoveryTime;


        public Inventory Inventory = new Inventory();
        public List<InventoryItem> EquipSlots = new List<InventoryItem>();

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
            Class = (CharacterClass)CharacterData.DefaultClass;

            Inventory.Owner = this;

            foreach (CharAttribute attr in Enum.GetValues(typeof(CharAttribute)))
            {
                BaseAttributes.Add(attr, 0);
            }

            foreach (SpellElement elem in Enum.GetValues(typeof(SpellElement)))
            {
                BaseResistances.Add(elem, 0);
            }

            foreach (PlayerEffectType effect in Enum.GetValues(typeof(PlayerEffectType)))
            {
                PlayerBuffMap.Add(effect, new SpellEffect());
            }

            // Testing
            SkillPoints = 15;
        }

        // ============================ PUBLIC API ============================ 

        public float GetHealthPercentage()
        {
            return ((float)CurrHitPoints / (float)GetMaxHitPoints()) * 100.0f;
        }

        public float GetManaPercentage()
        {
            return ((float)CurrSpellPoints / (float)GetMaxSpellPoints()) * 100.0f;
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
            int maxHP = GetMaxHitPoints();
            CurrHitPoints = Math.Min(CurrHitPoints + numHitPoints, maxHP);

            GameEvents.InvokeEvent_OnCharHealthChanged(this, maxHP, CurrHitPoints, numHitPoints);
        }

        public void AddCurrSpellPoints(int numSpellPoints)
        {
            int maxMP = GetMaxSpellPoints();
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

        public ItemInteractResult CanEquipItem(Item item)
        {
            ItemType itemType = item.Data.ItemType;
            ItemSkillGroup skillGroup = item.Data.SkillGroup;

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

        public ItemInteractResult InteractWithItem(Item item)
        {
            ItemInteractResult interactResult = ItemInteractResult.Invalid;
            if (item.IsEquippable())
            {
                // Try to equip the item. If success, we may have replaced the item by the old item on doll
                Item replacedItem = null;
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

            UI.StatsUI.Refresh();

            return interactResult;
        }

        public bool HasSkill(SkillType skillType)
        {
            return Skills.ContainsKey(skillType) && 
                Skills[skillType].Mastery != SkillMastery.None;
        }

        public SkillMastery GetSkillMastery(SkillType skillType)
        {
            if (!HasSkill(skillType))
            {
                return SkillMastery.None;
            }

            return Skills[skillType].Mastery;
        }

        public void AddSKill(SkillType skillType, SkillMastery skillMastery, int skillLevel)
        {
            if (HasSkill(skillType))
            {
                Debug.LogWarning(Name + " already knows skill: " + skillType);
                return;
            }

            Skill newSkill = new Skill();
            newSkill.Mastery = skillMastery;
            newSkill.Type = skillType;
            newSkill.Level = skillLevel;

            Skills.Add(skillType, newSkill);

            UI.SkillsUI.AddSkillRow(newSkill);
            UI.SkillsUI.Refresh();
        }

        public void SetSkillMastery(SkillType skillType, SkillMastery skillMastery)
        {
            if (!HasSkill(skillType))
            {
                Debug.LogWarning(Name + " does not have skill: " + skillType);
                return;
            }

            // TODO: Check for Level x Mastery validity ?
            Skills[skillType].Mastery = skillMastery;

            UI.SkillsUI.Refresh();
        }

        public void LearnSkill(SkillType skillType)
        {
            if (HasSkill(skillType))
            {
                Debug.LogWarning(Name + " already knows skill: " + skillType);
                return;
            }

            Skill newSkill = new Skill();
            newSkill.Mastery = SkillMastery.Normal;
            newSkill.Type = skillType;
            newSkill.Level = 1;

            Skills.Add(skillType, newSkill);

            UI.SkillsUI.AddSkillRow(newSkill);
            UI.SkillsUI.Refresh();
        }

        public void OnSkillClicked(SkillType skillType)
        {
            if (!HasSkill(skillType))
            {
                Debug.LogError(Name + ": skill clicked but it isne learned ? " + skillType);
                return;
            }

            Skill skill = Skills[skillType];
            if (SkillPoints > skill.Level)
            {
                SkillPoints -= skill.Level + 1;
                skill.Level++;

                UI.SkillsUI.Refresh();

                // TODO: Separate ?
                CharFaceUpdater.SetAvatar(UiMgr.RandomSprite(UI.Sprites.Smile), 0.75f);
                SoundMgr.PlaySoundByName("Quest");
            }
        }

        //======================================================================

        /*
        * Base = Permanent + Items 
        * Bonus = Magic + Skill bonuses
        * Actual = Base + Bonus + other factors like age
        * 
        * BonusStatType = pretty much anything that can be applied to player to achieve some effect
        * 
        */

        public int GetBaseAge()
        {
            return TimeMgr.Instance.GetCurrentTime().Year - BirthYear;
        }

        public int GetActualAge()
        {
            // Also get age mod from items etc.

            return GetBaseAge() + AgeModifier;
        }

        public int GetBaseResistance(SpellElement resistType)
        {
            int racialBonus = 0;
            switch (resistType)
            {
                case SpellElement.Air:
                    if (Race == CharacterRace.Goblin)
                    {
                        racialBonus = 5;
                    }
                    break;

                case SpellElement.Fire:
                    if (Race == CharacterRace.Goblin)
                    {
                        racialBonus = 5;
                    }
                    else if (Race == CharacterRace.Troll)
                    {
                        racialBonus = 10;
                    }
                    break;

                case SpellElement.Water:
                    // Dwarf, but I have no dwarfs
                    break;

                case SpellElement.Earth:
                    // Dwarf, but I have no dwarfs
                    break;

                case SpellElement.Body:
                    if (Race == CharacterRace.Human)
                    {
                        racialBonus = 5;
                    }
                    break;

                case SpellElement.Mind:
                    if (Race == CharacterRace.Elf)
                    {
                        racialBonus = 10;
                    }
                    break;

                case SpellElement.Spirit:
                    if (Race == CharacterRace.Human)
                    {
                        racialBonus = 5;
                    }
                    break;

                default:
                    break;
            }

            // + Get from items
            int itemBonus = GetItemsBonus(GameMechanics.ResistanceToAttributeBonus(resistType));

            return racialBonus + itemBonus;
        }

        public int GetBaseAttribute(CharAttribute attribute)
        {
            int baseAttribute = BaseAttributes[attribute];

            // + Get from items
            int itemBonus = GetItemsBonus(GameMechanics.CharAttributeToStatBonus(attribute));

            return baseAttribute + itemBonus;
        }

        public int GetItemsBonus(StatBonusType bonusType)
        {
            int bonusAmount = 0;
            foreach (InventoryItem equipSlot in EquipSlots)
            {
                Item item = equipSlot.Item;
                if (item == null)
                {
                    continue;
                }

                // Add native armor's armor class
                if (bonusType == StatBonusType.ArmorClass)
                {
                    if (item.Data.ItemType == ItemType.Armor ||
                        item.Data.ItemType == ItemType.Helmet ||
                        item.Data.ItemType == ItemType.Cloak ||
                        item.Data.ItemType == ItemType.Shield ||
                        item.Data.ItemType == ItemType.Gauntlets ||
                        item.Data.ItemType == ItemType.Boots)
                    {
                        bonusAmount += int.Parse(item.Data.Mod1) + int.Parse(item.Data.Mod2);
                    }
                }

                // Add generic (enchant)
                bonusAmount += item.GetStatBonusAmount(bonusType);
            }

            return bonusAmount;
        }

        public int GetMagicBonus(StatBonusType bonusType)
        {
            int bonusAmount = 0;


            return bonusAmount;
        }

        public int GetSkillsBonus(StatBonusType bonusType)
        {
            int bonusAmount = 0;

            return bonusAmount;
        }

        public int GetActualLevel()
        {
            int itemBonus = GetItemsBonus(StatBonusType.Level);
            int magicBonus = GetMagicBonus(StatBonusType.Level);

            return Level + itemBonus + magicBonus;
        }
        public int GetActualResistance(SpellElement resistType)
        {
            int baseAmount = GetBaseResistance(resistType);
            int resistFromSkills = GetSkillsBonus(GameMechanics.ResistanceToAttributeBonus(resistType));
            int resistFromMagic = GetMagicBonus(GameMechanics.ResistanceToAttributeBonus(resistType));

            return baseAmount + resistFromSkills + resistFromMagic;
        }

        public int GetActualSkillLevel(SkillType skillType)
        {
            if (!HasSkill(skillType))
            {
                return 0;
            }

            int baseAmount = Skills[skillType].Level;
            int itemBonus = GetItemsBonus(GameMechanics.SkillToAttributeBonus(skillType));

            return baseAmount + itemBonus;
        }

        public int GetActualAttribute(CharAttribute attribute)
        {
            if (attribute == CharAttribute.None)
            {
                Debug.LogError("Invalid attribute - none");
                return 0;
            }

            int actualAge = GetActualAge();
            float agingMultiplier = GameMechanics.GetAttributeAgingMultiplier(actualAge, attribute);

            /*float conditionMultiplier = GameMechanics.GetAttributeConditionMultiplier(..)*/
            float conditionMultiplier = 1.0f;

            int baseValue = BaseAttributes[attribute];
            int magicBonus = GetMagicBonus(GameMechanics.CharAttributeToStatBonus(attribute));
            int itemBonus = GetItemsBonus(GameMechanics.CharAttributeToStatBonus(attribute));

            return (int)(baseValue * agingMultiplier * conditionMultiplier) + magicBonus + itemBonus;
        }

        // Helper accessors
        public int GetMaxHitPoints()
        {
            ClassHpSpData classHpSpData = DbMgr.Instance.ClassHpSpDb.Get(Class);

            int hpBase = classHpSpData.HitPointsBase;
            int hpFromLevel = GetActualLevel() * classHpSpData.HitPointsFactor;
            int hpFromEndurance = GameMechanics.GetAttributeEffect(GetActualEndurance()) * classHpSpData.HitPointsFactor;
            int hpFromItems = GetItemsBonus(StatBonusType.HitPoints);
            int hpFromSkills = GetItemsBonus(StatBonusType.HitPoints);

            int maxHealth = hpBase + hpFromLevel + hpFromEndurance + hpFromItems + hpFromSkills;
            if (maxHealth < 0)
            {
                maxHealth = 0;
            }

            return maxHealth;
        }

        public int GetMaxSpellPoints()
        {
            ClassHpSpData classHpSpData = DbMgr.Instance.ClassHpSpDb.Get(Class);

            // Classes like knights dont have any spell points no matter what
            if (!classHpSpData.IsSpellPointsFromIntellect && !classHpSpData.IsSpellPointsFromPersonality)
            {
                return 0;
            }

            int mpBase = classHpSpData.SpellPointsBase;
            int mpFromLevel = GetActualLevel() * classHpSpData.SpellPointsFactor;
            int mpFromIntellect = 0;
            int mpFromPersonality = 0;
             
            if (classHpSpData.IsSpellPointsFromIntellect)
            {
                mpFromIntellect += GameMechanics.GetAttributeEffect(GetActualIntellect()) * classHpSpData.SpellPointsFactor;
            }

            if (classHpSpData.IsSpellPointsFromPersonality)
            {
                mpFromPersonality += GameMechanics.GetAttributeEffect(GetActualPersonality()) * classHpSpData.SpellPointsFactor;
            }

            int mpFromItems = GetItemsBonus(StatBonusType.SpellPoints);
            int mpFromSkills = GetItemsBonus(StatBonusType.SpellPoints);

            int maxMana = mpBase + mpFromLevel + mpFromIntellect + mpFromPersonality + mpFromItems + mpFromSkills;
            if (maxMana < 0)
            {
                maxMana = 0;
            }

            return maxMana;
        }

        public int GetBaseMight()
        {
            return GetBaseAttribute(CharAttribute.Might);
        }

        public int GetBaseAccuracy()
        {
            return GetBaseAttribute(CharAttribute.Accuracy);
        }

        public int GetBaseEndurance()
        {
            return GetBaseAttribute(CharAttribute.Endurance);
        }

        public int GetBaseIntellect()
        {
            return GetBaseAttribute(CharAttribute.Intellect);
        }

        public int GetBaseLuck()
        {
            return GetBaseAttribute(CharAttribute.Luck);
        }
        public int GetBasePersonality()
        {
            return GetBaseAttribute(CharAttribute.Personality);
        }

        public int GetBaseSpeed()
        {
            return GetBaseAttribute(CharAttribute.Speed);
        }

        public int GetActualMight()
        {
            return GetActualAttribute(CharAttribute.Might);
        }

        public int GetActualAccuracy()
        {
            return GetActualAttribute(CharAttribute.Accuracy);
        }

        public int GetActualEndurance()
        {
            return GetActualAttribute(CharAttribute.Endurance);
        }

        public int GetActualIntellect()
        {
            return GetActualAttribute(CharAttribute.Intellect);
        }

        public int GetActualPersonality()
        {
            return GetActualAttribute(CharAttribute.Personality);
        }

        public int GetActualSpeed()
        {
            return GetActualAttribute(CharAttribute.Speed);
        }

        public int GetActualLuck()
        {
            return GetActualAttribute(CharAttribute.Luck);
        }

        public int GetBaseArmorClass()
        {
            int accuracyBonus = GameMechanics.GetAttributeEffect(GetActualAccuracy());
            int itemBonus = GetItemsBonus(StatBonusType.ArmorClass);
            int skillBonus = GetSkillsBonus(StatBonusType.ArmorClass);

            int result = accuracyBonus + itemBonus + skillBonus;
            if (result < 0)
            {
                result = 0;
            }

            return result;
        }

        public int GetActualArmorClass()
        {
            int baseAC = GetBaseArmorClass();
            int bonusAC = GetMagicBonus(StatBonusType.ArmorClass);

            int result = baseAC + bonusAC;
            if (result < 0)
            {
                result = 0;
            }

            return result;
        }

        public int GetMeleeAttack()
        {
            int accuracyBonus = GameMechanics.GetAttributeEffect(GetActualAccuracy());
            int weaponBonus = 0;
            // Main hand
            Item mainhandItem = UI.DollUI.RH_Weapon.Item;
            if (mainhandItem != null)
            {
                ItemType mainhandItemType = mainhandItem.Data.ItemType;
                if (mainhandItemType == ItemType.WeaponOneHanded ||
                    mainhandItemType == ItemType.WeaponTwoHanded)
                {
                    weaponBonus += int.Parse(mainhandItem.Data.Mod2);
                }
            }
            // TODO: Also handle dual wield here

            int skillsBonus = GetSkillsBonus(StatBonusType.MeleeAttack);
            int magicalBonus = GetMagicBonus(StatBonusType.MeleeAttack);


            return accuracyBonus + weaponBonus + skillsBonus + magicalBonus;
        }

        public int GetMeleeDamageMin()
        {
            int mightBonus = GameMechanics.GetAttributeEffect(GetActualMight());



            return 0;
        }

        public int GetMeleeDamageMax()
        {
            return 0;
        }

        public int GetRangedAttack()
        {
            return 0;
        }

        public int GetRangedDamageMin()
        {
            return 0;
        }

        public int GetRangedDamageMax()
        {
            return 0;
        }



        public bool CanTrainToNextLevel()
        {
            return Experience > GameMechanics.GetTotalExperienceRequired(Level + 1);
        }
    }
}
