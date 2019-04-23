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

        public SpellSchool LastSpellbookPage = SpellSchool.Fire;
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
        public List<SpellType> LearnedSpells = new List<SpellType>();

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
            else if (itemType == ItemType.WeaponOneHanded && !DollTypeData.CanEquipWeapon)
            {
                return ItemInteractResult.Invalid;
            }
            else if (itemType == ItemType.WeaponTwoHanded && !DollTypeData.CanEquipWeapon)
            {
                return ItemInteractResult.Invalid;
            }

            if (!item.IsEquippableByRace(Race))
            {
                return ItemInteractResult.CannotEquip;
            }

            if (!item.IsEquippableByClass(Class))
            {
                return ItemInteractResult.CannotEquip;
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

        public bool HasSpell(SpellType spell)
        {
            return LearnedSpells.Contains(spell);
        }

        public void LearnSpell(SpellType spell)
        {
            if (!HasSpell(spell))
            {
                LearnedSpells.Add(spell);
            }
        }

        /*public bool CanLearnSpell(SpellType spell)
        {
            
        }*/

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

        public int GetHpScalingFactor()
        {
            ClassHpSpData classHpSpData = DbMgr.Instance.ClassHpSpDb.Get(Class);
            if (classHpSpData != null)
            {
                return classHpSpData.HitPointsFactor;
            }

            return 0;
        }

        public int GetManaScalingFactor()
        {
            ClassHpSpData classHpSpData = DbMgr.Instance.ClassHpSpDb.Get(Class);
            if (classHpSpData != null)
            {
                return classHpSpData.SpellPointsFactor;
            }

            return 0;
        }

        public int GetSkillLevelMultiplier(SkillType skillType, int normalMult, int expertMult, int masterMult, int grandmasterMult)
        {
            if (!HasSkill(skillType))
            {
                return 0;
            }

            SkillMastery skillMastery = GetSkillMastery(skillType);
            switch (skillMastery)
            {
                case SkillMastery.Normal:
                    return normalMult;

                case SkillMastery.Expert:
                    return expertMult;

                case SkillMastery.Master:
                    return masterMult;

                case SkillMastery.Grandmaster:
                    return grandmasterMult;
            }

            return 0;
        }

        //======================================================================

        public Item GetItemAtSlot(EquipSlot equipSlot)
        {
            switch (equipSlot)
            {
                case EquipSlot.MainHand: return UI.DollUI.RH_Weapon.Item;
                //case EquipSlot.OffHand: return UI.DollUI.LH_Weapon.Item;
                case EquipSlot.Bow: return UI.DollUI.Bow.Item;
                case EquipSlot.Armor: return UI.DollUI.Armor.Item;
                case EquipSlot.Belt: return UI.DollUI.Belt.Item;
                case EquipSlot.Cloak: return UI.DollUI.Cloak.Item;
                case EquipSlot.Helmet: return UI.DollUI.Helmet.Item;
                case EquipSlot.Boots: return UI.DollUI.Boots.Item;
                case EquipSlot.Gauntlets: return UI.DollUI.Gauntlets.Item;
                case EquipSlot.Necklace: return UI.DollUI.Necklace.Item;
                case EquipSlot.Ring_1: return UI.DollUI.Ring_1.Item;
                case EquipSlot.Ring_2: return UI.DollUI.Ring_2.Item;
                case EquipSlot.Ring_3: return UI.DollUI.Ring_3.Item;
                case EquipSlot.Ring_4: return UI.DollUI.Ring_4.Item;
                case EquipSlot.Ring_5: return UI.DollUI.Ring_5.Item;
                case EquipSlot.Ring_6: return UI.DollUI.Ring_6.Item;
            }

            return null;
        }

        public bool WearsItemAtSlot(EquipSlot equipSlot)
        {
            return GetItemAtSlot(equipSlot) != null;
        }

        public bool WearsItemType(ItemType itemType)
        {
            foreach (InventoryItem equipSlot in EquipSlots)
            {
                Item item = equipSlot.Item;
                if (item != null && item.Data.ItemType == itemType)
                {
                    // If broken, what then ?
                    return true;
                }
            }

            return false;
        }

        public bool IsUnarmed()
        {
            return !WearsItemType(ItemType.WeaponTwoHanded) && !WearsItemType(ItemType.WeaponOneHanded);
        }

        public bool WearsItemAtSlot(int itemId, EquipSlot equipSlot)
        {
            Item item = GetItemAtSlot(equipSlot);
            if (item != null && item.Data.Id == itemId)
            {
                return true;
            }

            return false;
        }

        public bool WearsItemAnywhere(int itemId)
        {
            foreach (InventoryItem equipSlot in EquipSlots)
            {
                Item item = equipSlot.Item;
                if (item != null && item.Data.Id == itemId)
                {
                    return true;
                }
            }

            return false;
        }

        public bool WearsItemWithEnchant(int enchantId)
        {
            // TODO
            return false;
        }

        public bool WearsItemWithBonusStat(StatType statBonusType)
        {
            foreach (InventoryItem equipSlot in EquipSlots)
            {
                Item item = equipSlot.Item;
                if (item != null && item.Enchant != null)
                {
                    if (item.Enchant.StatBonusMap.ContainsKey(statBonusType) &&
                        item.Enchant.StatBonusMap[statBonusType] > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
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

        public int GetItemsBonus(StatType bonusType)
        {
            int bonusAmount = 0;

            // First non-trivial attribute calculations
            switch (bonusType)
            {
                case StatType.ArmorClass:
                    foreach (InventoryItem equipSlot in EquipSlots)
                    {
                        Item item = equipSlot.Item;
                        if (item == null)
                        {
                            continue;
                        }

                        // Add native armor's armor class
                        if (bonusType == StatType.ArmorClass)
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
                    }
                    break;

                case StatType.MeleeDamageBonus:
                case StatType.MeleeAttack:
                    Item mainhandItem = GetItemAtSlot(EquipSlot.MainHand);
                    Item offhandItem = GetItemAtSlot(EquipSlot.OffHand);

                    if (mainhandItem != null)
                    {
                        ItemType mainhandItemType = mainhandItem.Data.ItemType;
                        if (mainhandItemType == ItemType.WeaponOneHanded ||
                            mainhandItemType == ItemType.WeaponTwoHanded)
                        {
                            bonusAmount += mainhandItem.GetMod();
                        }
                    }

                    // TODO: Handle offhand
                    /*
                    Item offhandItem = UI.DollUI.RH_Weapon.Item;
                    if (offhandItem != null)
                    {
                        ItemType offhandItemType = offhandItem.Data.ItemType;
                        if (offhandItemType == ItemType.WeaponOneHanded ||
                            offhandItemType == ItemType.WeaponTwoHanded)
                        {
                            bonusAmount += int.Parse(mainhandItem.Data.Mod2);
                        }
                    }
                    */

                    break;

                case StatType.MeleeDamageMin:
                    if (IsUnarmed())
                    {
                        bonusAmount += 1;
                        break;
                    }

                    mainhandItem = GetItemAtSlot(EquipSlot.MainHand);
                    offhandItem = GetItemAtSlot(EquipSlot.OffHand);
                    if (mainhandItem != null)
                    {
                        bonusAmount += mainhandItem.GetMod();
                        bonusAmount += mainhandItem.GetDiceRolls();
                    }

                    if (offhandItem != null)
                    {
                        if (offhandItem.Data.ItemType != ItemType.Shield)
                        {
                            bonusAmount += offhandItem.GetMod();
                            bonusAmount += offhandItem.GetDiceRolls();
                        }
                    }

                    break;

                case StatType.MeleeDamageMax:
                    if (IsUnarmed())
                    {
                        bonusAmount += 3;
                        break;
                    }

                    mainhandItem = GetItemAtSlot(EquipSlot.MainHand);
                    offhandItem = GetItemAtSlot(EquipSlot.OffHand);
                    if (mainhandItem != null)
                    {
                        bonusAmount += mainhandItem.GetMod();
                        bonusAmount += mainhandItem.GetDiceRolls() * mainhandItem.GetDiceSides();
                    }

                    if (offhandItem != null)
                    {
                        if (offhandItem.Data.ItemType != ItemType.Shield)
                        {
                            bonusAmount += offhandItem.GetMod();
                            bonusAmount += offhandItem.GetDiceRolls() * offhandItem.GetDiceSides();
                        }
                    }

                    break;

                case StatType.RangedDamageBonus:
                case StatType.RangedAttack:
                    Item bow = GetItemAtSlot(EquipSlot.Bow);
                    if (bow != null)
                    {
                        bonusAmount += bow.GetMod();
                    }
                    break;

                case StatType.RangedDamageMin:
                    bow = GetItemAtSlot(EquipSlot.Bow);
                    if (bow != null)
                    {
                        bonusAmount += bow.GetMod();
                        bonusAmount += bow.GetDiceRolls();
                    }
                    break;

                case StatType.RangedDamageMax:
                    bow = GetItemAtSlot(EquipSlot.Bow);
                    if (bow != null)
                    {
                        bonusAmount += bow.GetMod();
                        bonusAmount += bow.GetDiceRolls() * bow.GetDiceSides();
                    }
                    break;

                default:
                    break;
            }


            // Then trivial calculations - just extract stat bonus from item
            foreach (InventoryItem equipSlot in EquipSlots)
            {
                Item item = equipSlot.Item;
                if (item == null)
                {
                    continue;
                }

                // Add generic (enchant)
                bonusAmount += item.GetStatBonusAmount(bonusType);
            }

            return bonusAmount;
        }

        public int GetMagicBonus(StatType bonusType)
        {
            int bonusAmount = 0;

            switch (bonusType)
            {
                case StatType.FireResistance:
                    bonusAmount += Party.PartyBuffMap[PartyEffectType.ResistFire].Power;
                    bonusAmount += PlayerBuffMap[PlayerEffectType.ResistFire].Power;
                    break;

                case StatType.AirResistance:
                    bonusAmount += Party.PartyBuffMap[PartyEffectType.ResistAir].Power;
                    bonusAmount += PlayerBuffMap[PlayerEffectType.ResistAir].Power;
                    break;

                case StatType.BodyResistance:
                    bonusAmount += Party.PartyBuffMap[PartyEffectType.ResistBody].Power;
                    bonusAmount += PlayerBuffMap[PlayerEffectType.ResistBody].Power;
                    break;

                case StatType.WaterResistance:
                    bonusAmount += Party.PartyBuffMap[PartyEffectType.ResistWater].Power;
                    bonusAmount += PlayerBuffMap[PlayerEffectType.ResistWater].Power;
                    break;

                case StatType.EarthResistance:
                    bonusAmount += Party.PartyBuffMap[PartyEffectType.ResistEarth].Power;
                    bonusAmount += PlayerBuffMap[PlayerEffectType.ResistEarth].Power;
                    break;

                case StatType.MindMagic:
                    bonusAmount += Party.PartyBuffMap[PartyEffectType.ResistMind].Power;
                    bonusAmount += PlayerBuffMap[PlayerEffectType.ResistMind].Power;
                    break;

                case StatType.MeleeAttack:
                case StatType.RangedAttack:
                    bonusAmount += PlayerBuffMap[PlayerEffectType.Bless].Power;
                    break;

                case StatType.MeleeDamageBonus:
                    bonusAmount += Party.PartyBuffMap[PartyEffectType.Heroism].Power;
                    bonusAmount += PlayerBuffMap[PlayerEffectType.Heroism].Power;
                    break;

                case StatType.Might:
                    bonusAmount += Party.PartyBuffMap[PartyEffectType.DayOfTheGods].Power;
                    bonusAmount += PlayerBuffMap[PlayerEffectType.Might].Power;
                    break;

                case StatType.Endurance:
                    bonusAmount += Party.PartyBuffMap[PartyEffectType.DayOfTheGods].Power;
                    bonusAmount += PlayerBuffMap[PlayerEffectType.Endurance].Power;
                    break;

                case StatType.Intellect:
                    bonusAmount += Party.PartyBuffMap[PartyEffectType.DayOfTheGods].Power;
                    bonusAmount += PlayerBuffMap[PlayerEffectType.Intelligence].Power;
                    break;

                case StatType.Personality:
                    bonusAmount += Party.PartyBuffMap[PartyEffectType.DayOfTheGods].Power;
                    bonusAmount += PlayerBuffMap[PlayerEffectType.Personality].Power;
                    break;

                case StatType.Speed:
                    bonusAmount += Party.PartyBuffMap[PartyEffectType.DayOfTheGods].Power;
                    bonusAmount += PlayerBuffMap[PlayerEffectType.Speed].Power;
                    break;

                case StatType.Accuracy:
                    bonusAmount += Party.PartyBuffMap[PartyEffectType.DayOfTheGods].Power;
                    bonusAmount += PlayerBuffMap[PlayerEffectType.Accuracy].Power;
                    break;

                case StatType.Luck:
                    bonusAmount += Party.PartyBuffMap[PartyEffectType.DayOfTheGods].Power;
                    bonusAmount += PlayerBuffMap[PlayerEffectType.Luck].Power;
                    break;

                case StatType.ArmorClass:
                    bonusAmount += Party.PartyBuffMap[PartyEffectType.StoneSkin].Power;
                    bonusAmount += PlayerBuffMap[PlayerEffectType.Stoneskin].Power;
                    break;
            }

            return bonusAmount;
        }

        public int GetSkillsBonus(StatType bonusType)
        {
            int bonusAmount = 0;

            switch (bonusType)
            {
                case StatType.HitPoints:
                    int bodybuildingMult = GetSkillLevelMultiplier(SkillType.Bodybuilding, 1, 2, 3, 5);
                    int bodybuildingEffect = GetActualSkillLevel(SkillType.Bodybuilding) * bodybuildingMult * GetHpScalingFactor();
                    bonusAmount += bodybuildingEffect;
                    break;

                case StatType.SpellPoints:
                    int meditationMult = GetSkillLevelMultiplier(SkillType.Meditation, 1, 2, 3, 5);
                    int meditationEffect = GetActualSkillLevel(SkillType.Meditation) * meditationMult * GetManaScalingFactor();
                    bonusAmount += meditationEffect;
                    break;

                // TODO: Rest...
            }

            return bonusAmount;
        }

        public int GetActualLevel()
        {
            int itemBonus = GetItemsBonus(StatType.Level);
            int magicBonus = GetMagicBonus(StatType.Level);

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
            int hpFromItems = GetItemsBonus(StatType.HitPoints);
            int hpFromSkills = GetSkillsBonus(StatType.HitPoints);

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

            int mpFromItems = GetItemsBonus(StatType.SpellPoints);
            int mpFromSkills = GetSkillsBonus(StatType.SpellPoints);

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
            int itemBonus = GetItemsBonus(StatType.ArmorClass);
            int skillBonus = GetSkillsBonus(StatType.ArmorClass);

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
            int bonusAC = GetMagicBonus(StatType.ArmorClass);

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
            int weaponBonus = GetItemsBonus(StatType.MeleeAttack);
            int skillsBonus = GetSkillsBonus(StatType.MeleeAttack);
            int magicalBonus = GetMagicBonus(StatType.MeleeAttack);

            return accuracyBonus + weaponBonus + skillsBonus + magicalBonus;
        }

        public int GetMeleeDamageMin()
        {
            int mightBonus = GameMechanics.GetAttributeEffect(GetActualMight());
            int weaponBonus = GetItemsBonus(StatType.MeleeDamageMin);
            int skillsBonus = GetSkillsBonus(StatType.MeleeDamageBonus);
            int magicalBonus = GetMagicBonus(StatType.MeleeDamageBonus);

            int result = mightBonus + weaponBonus + skillsBonus + magicalBonus;
            if (result < 1)
            {
                result = 1;
            }

            return result;
        }

        public int GetMeleeDamageMax()
        {
            int mightBonus = GameMechanics.GetAttributeEffect(GetActualMight());
            int weaponBonus = GetItemsBonus(StatType.MeleeDamageMax);
            int skillsBonus = GetSkillsBonus(StatType.MeleeDamageBonus);
            int magicalBonus = GetMagicBonus(StatType.MeleeDamageBonus);

            int result = mightBonus + weaponBonus + skillsBonus + magicalBonus;
            if (result < 1)
            {
                result = 1;
            }

            return result;
        }

        public int GetRangedAttack()
        {
            Item mainhandItem = UI.DollUI.RH_Weapon.Item;
            bool isBlasterEquipped = mainhandItem != null && (mainhandItem.Data.Id == 64 || mainhandItem.Data.Id == 65);
            if (isBlasterEquipped)
            {
                return GetMeleeAttack();
            }

            int accuracyBonus = GameMechanics.GetAttributeEffect(GetActualAccuracy());
            int weaponBonus = GetItemsBonus(StatType.RangedAttack);
            int skillsBonus = GetSkillsBonus(StatType.RangedAttack);
            int magicalBonus = GetMagicBonus(StatType.RangedAttack);

            return accuracyBonus + weaponBonus + skillsBonus + magicalBonus;
        }

        public int GetRangedDamageMin()
        {
            int weaponBonus = GetItemsBonus(StatType.RangedDamageMin);
            int skillsBonus = GetSkillsBonus(StatType.RangedDamageBonus);
            int magicalBonus = GetMagicBonus(StatType.RangedDamageBonus);

            int result = weaponBonus + skillsBonus + magicalBonus;
            if (result < 0)
            {
                result = 0;
            }

            return result;
        }

        public int GetRangedDamageMax()
        {
            int weaponBonus = GetItemsBonus(StatType.RangedDamageMax);
            int skillsBonus = GetSkillsBonus(StatType.RangedDamageBonus);
            int magicalBonus = GetMagicBonus(StatType.RangedDamageBonus);

            int result = weaponBonus + skillsBonus + magicalBonus;
            if (result < 0)
            {
                result = 0;
            }

            return result;
        }



        public bool CanTrainToNextLevel()
        {
            return Experience > GameMechanics.GetTotalExperienceRequired(Level + 1);
        }

        //=============================================================================================================

        public void CastSpell(SpellType spellType)
        {
            int spellSchoolIdx = (int)(spellType - 1) / 11;
            if (!Enum.IsDefined(typeof(SpellSchool), spellSchoolIdx))
            {
                Debug.LogError("Invalid dervied spell school for spell: " + spellType);
                return;
            }

            SpellSchool spellSchool = (SpellSchool)spellSchoolIdx;
            SkillType skillType = GameMechanics.SpellSchoolToSkillType(spellSchool);
            if (skillType == SkillType.None)
            {
                Debug.LogError("Invalid skill type derived from SpellSchool: " + spellSchool);
                return;
            }

            int skillLevel = GetActualSkillLevel(skillType);
            SkillMastery skillMastery = GetSkillMastery(skillType);
            if (skillMastery == SkillMastery.None)
            {
                Debug.LogError("No skill mastery for: " + skillType);
                return;
            }

            SpellData spellData = DbMgr.Instance.SpellDataDb.Get(spellType);
            if (spellData == null)
            {
                Debug.LogError("No spell data for: " + spellType);
                return;
            }

            int requiredMana = int.MaxValue;
            switch (skillMastery)
            {
                case SkillMastery.Normal:
                    requiredMana = spellData.ManaCostNormal;
                    break;
                case SkillMastery.Expert:
                    requiredMana = spellData.ManaCostExpert;
                    break;
                case SkillMastery.Master:
                    requiredMana = spellData.ManaCostMaster;
                    break;
                case SkillMastery.Grandmaster:
                    requiredMana = spellData.ManaCostGrandmaster;
                    break;
            }

            if (requiredMana == int.MaxValue)
            {
                Debug.LogError("Invalid required mana for spell: " + spellType);
                return;
            }

            if (CurrSpellPoints < requiredMana)
            {
                Debug.Log("Not enough mana: " + CurrSpellPoints + " (Required " + requiredMana + ")");
                // Play some sounds / expression
                return;
            }

            // Handle curse - chance to fail the spell

            int duration = 0;
            int amount = 0;

            switch (spellType)
            {
                case SpellType.None:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Fire_TorchLight:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Fire_FireBolt:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Fire_ProtectionFromFire:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Fire_FireAura:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Fire_Haste:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Fire_Fireball:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Fire_FireSpike:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Fire_Immolation:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Fire_MeteorShower:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Fire_Inferno:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Fire_Incinerate:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Air_WizardEye:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Air_FeatherFall:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Air_ProtectionFromAir:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Air_Sparks:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Air_Jump:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Air_Shield:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Air_LightningBolt:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Air_Invisibility:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Air_Implosion:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Air_Fly:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Air_Startburst:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_Awaken:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_PoisonSpray:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_ProtectionFromWater:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_IceBolt:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_WaterWalk:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_RechargeItem:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_AcidBurst:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_EnchantItem:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_TownPortal:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_IceBlast:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Water_LloydsBeacon:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_Stun:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_Slow:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_ProtectionFromEarth:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_DeadlySwarm:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_Stoneskin:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_Blades:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_StoneToFlesh:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_RockBlast:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_Telekinesis:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_DeathBlossom:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Earth_MassDistortion:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Spirit_DetectLife:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Spirit_Bless:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Spirit_Fate:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Spirit_TurnUndead:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Spirit_RemoveCurse:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Spirit_Preservation:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Spirit_Heroism:
                    amount = 5 + skillLevel;
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                        case SkillMastery.Expert:
                            duration = 60 + (skillLevel * 5);
                            break;

                        case SkillMastery.Master:
                            duration = 60 + (skillLevel * 15);
                            break;

                        case SkillMastery.Grandmaster:
                            duration = 60 + (skillLevel * 60);
                            break;
                    }

                    Party.PartyBuffMap[PartyEffectType.Heroism].Apply(skillMastery, amount, GameTime.FromCurrentTime(duration), this);
                    Party.Characters.ForEach(chr => SpellFxRenderer.SetPlayerBuffAnim("sp51", chr));
                    SoundMgr.PlaySoundById(spellData.EffectSoundId);
                    break;

                case SpellType.Spirit_SpiritLash:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break; 

                case SpellType.Spirit_RaiseDead:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Spirit_SharedLife:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Spirit_Ressurection:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_Telepathy:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_RemoveFear:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_ProtectionFromMind:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_MindBlast:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_Charm:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_CureParalysis:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_Berserk:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_MassFear:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_CureInsanity:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_PsychicShock:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Mind_Enslave:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_CureWeakness:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_FirstAid:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_ProtectionFromBody:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_Harm:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_Regeneration:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_CurePoison:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_Hammerhands:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_CureDisease:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_ProtectionFromMagic:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_FlyingFist:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Body_PowerCure:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_LightBold:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_DestroyUndead:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_DispelMagic:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_Paralyze:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_SummonElemental:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_DayOfTheGods:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_PrismaticLight:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_DayOfProtection:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_HourOfPower:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_Sunray:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Light_DivineIntervention:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_Reanimate:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_ToxicCloud:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_VampiricWeapon:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_ShrinkingRay:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_Sharpmetal:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_ControlUndead:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_PainReflection:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_DarkGrasp:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_DragonBreath:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_Armageddon:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dark_Souldrinker:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.DarkElf_Glamour:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.DarkElf_TravelersBoon:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.DarkElf_Blind:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.DarkElf_DarkfireBolt:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.DarkElf_UNUSED_5:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.DarkElf_UNUSED_6:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.DarkElf_UNUSED_7:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.DarkElf_UNUSED_8:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.DarkElf_UNUSED_9:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.DarkElf_UNUSED_10:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.DarkElf_UNUSED_11:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Vampire_Lifedrain:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Vampire_Levitate:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Vampire_Charm:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Vampire_Mistform:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Vampire_UNUSED_5:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Vampire_UNUSED_6:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Vampire_UNUSED_7:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Vampire_UNUSED_8:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Vampire_UNUSED_9:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Vampire_UNUSED_10:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Vampire_UNUSED_11:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dragon_Fear:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dragon_FlameBlast:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dragon_Flight:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dragon_WingBuffer:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dragon_UNUSED_5:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dragon_UNUSED_6:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dragon_UNUSED_7:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dragon_UNUSED_8:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dragon_UNUSED_9:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dragon_UNUSED_10:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                case SpellType.Dragon_UNUSED_11:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

                default:
                    Debug.LogError("Spell not implemented: " + spellType);
                    break;

            }
        }
    }
}
