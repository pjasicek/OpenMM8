using Assets.OpenMM8.Scripts.Data;
using Assets.OpenMM8.Scripts.Gameplay.Data;
using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Assertions;

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
        public CharacterVoiceData VoiceData;

        // Gameplay Data
        public string Name;
        public CharacterRace Race;
        public CharacterClass Class;
        public int Experience;
        public int SkillPoints;
        public int CurrHitPoints;
        public int CurrSpellPoints;
        public Condition Condition = Condition.Good;

        public SpellSchool LastSpellbookPage = SpellSchool.Fire;
        public string QuickSpellName = "None";

        // Buffs
        public Dictionary<PlayerEffectType, SpellEffect> PlayerBuffMap = new Dictionary<PlayerEffectType, SpellEffect>();

        // Conditions
        public Dictionary<Condition, GameTime> Conditions = new Dictionary<Condition, GameTime>();

        // Expressions
        public CharacterExpression CurrExpression = CharacterExpression.Good;
        // WARNING: This is in seconds
        public float CurrExpressionTimePassed = 0;
        public float CurrExpressionTimeLength = 0;

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
        public CharFaceUpdater CharFaceUpdater;

        public Character(int characterId, PlayerParty playerParty)
        {
            Party = playerParty;

            CharacterData = DbMgr.Instance.CharacterDataDb.Get(characterId);
            DollTypeData = DbMgr.Instance.DollTypeDb.Get(CharacterData.DollId);
            VoiceData = DbMgr.Instance.CharacterVoiceDb.Get(CharacterData.DefaultVoice);

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

            foreach (Condition condition in Enum.GetValues(typeof(Condition)))
            {
                Conditions.Add(condition, new GameTime(0));
            }

            // Testing
            SkillPoints = 15;

            CurrHitPoints = GetMaxHitPoints();
            CurrSpellPoints = GetMaxSpellPoints();

            //=====================================================================================
            // CharacterUI
            //=====================================================================================
            UI = CharacterUI.Create(this);
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
            UI.Refresh();
        }

        public void OnUpdate(float secDiff)
        {
            bool wasRecovered = IsRecovered();
            TimeUntilRecovery -= secDiff;

            if (CanAct() && !wasRecovered && IsRecovered())
            {
                GameEvents.InvokeEvent_OnRecovered(this);
            }

            // Handle buff expiration
            foreach (var playerBuffPair in PlayerBuffMap)
            {
                SpellEffect buff = playerBuffPair.Value;
                PlayerEffectType buffType = playerBuffPair.Key;

                if (buff.IsAppliedAndExpired())
                {
                    switch (buffType)
                    {
                        case PlayerEffectType.Haste:
                            SetCondition(Condition.Weak, false);
                            break;
                    }

                    buff.Reset();
                }
            }
        }

        public bool IsRecovered()
        {
            return TimeUntilRecovery <= 0.0f;
        }

        public bool Attack(Monster victim)
        {
            if (!CanAct() || TimeUntilRecovery > 0.0f)
            {
                return false;
            }

            // TODO: 
            TimeUntilRecovery = UnityEngine.Random.Range(1.25f, 2.0f);

            if (victim)
            {
                //AttackResult result = victim.ReceiveAttack(attackInfo, Party.gameObject);

                victim.ReceiveDamageFromPlayer(this, null);
                
                //GameEvents.InvokeEvent_OnCharHitNpc(this, attackInfo, result);
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
                    Party.SetHeldItem(replacedItem);
                    //GameEvents.InvokeEvent_OnItemEquipped(this, item, replacedItem);
                }
                else
                {
                    PlayEventReaction(CharacterReaction.CannotEquipItem);
                }
            }
            else if (item.IsCastable())
            {
                interactResult = ItemInteractResult.Casted;

                Party.SetHeldItem(null);
            }
            else if (item.IsConsumable())
            {
                interactResult = ItemInteractResult.Consumed;
                SoundMgr.PlaySoundById(SoundType.Drink);
                PlayEventReaction(CharacterReaction.SmileGood);

                Party.SetHeldItem(null);
            }
            else if (item.IsLearnable())
            {
                interactResult = ItemInteractResult.Learned;
                PlayEventReaction(CharacterReaction.LearnOk);

                Party.SetHeldItem(null);
            }
            else if (item.IsReadable())
            {
                interactResult = ItemInteractResult.Read;
                PlayEventReaction(CharacterReaction.ReadScroll);
            }
            else
            {
                SoundMgr.PlaySoundById(SoundType.Error);
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
                //CharFaceUpdater.SetAvatar(UiMgr.RandomSprite(UI.Sprites.Smile), 0.75f);
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
            return (int)(TimeMgr.START_YEAR + TimeMgr.GetCurrentTime().GetYears()) - BirthYear;
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

        //=============================================================================================================

        //{16, 15, 14, 17, 13, 2, 12, 11, 10, 9, 8, 7, 6, 5, 4, 3, 1, 0}};

        private Condition[] m_ConditionImportancyTable =
        {
            Condition.Eradicated, // Most severe
            Condition.Petrified,
            Condition.Dead,
            Condition.Zombie,
            Condition.Unconcious,
            Condition.Sleep,
            Condition.Paralyzed,
            Condition.DiseaseSevere,
            Condition.PoisonSevere,
            Condition.DiseaseMedium,
            Condition.PoisonMedium,
            Condition.DiseaseWeak,
            Condition.PoisonWeak,
            Condition.Insane,
            Condition.Drunk,
            Condition.Fear,
            Condition.Weak,
            Condition.Cursed, // Least severe
        };

        public Condition GetWorstCondition()
        {
            foreach (Condition testCond in m_ConditionImportancyTable)
            {
                if (Conditions[testCond].IsValid())
                {
                    return testCond;
                }
            }

            return Condition.Good;
        }

        public bool IsWeak()
        {
            return Conditions[Condition.Weak].IsValid();
        }

        public bool IsDead()
        {
            return Conditions[Condition.Dead].IsValid();
        }

        public bool IsEradicated()
        {
            return Conditions[Condition.Eradicated].IsValid();
        }

        public bool IsZombie()
        {
            return Conditions[Condition.Zombie].IsValid();
        }

        public bool IsCursed()
        {
            return Conditions[Condition.Cursed].IsValid();
        }

        public bool IsPetrified()
        {
            return Conditions[Condition.Petrified].IsValid();
        }

        public bool IsUnconcious()
        {
            return Conditions[Condition.Unconcious].IsValid();
        }

        public bool IsAsleep()
        {
            return Conditions[Condition.Sleep].IsValid();
        }

        public bool IsParalyzed()
        {
            return Conditions[Condition.Paralyzed].IsValid();
        }

        public bool IsDrunk()
        {
            return Conditions[Condition.Drunk].IsValid();
        }

        public bool CanAct()
        {
            if (IsAsleep() ||
                IsParalyzed() ||
                IsUnconcious() ||
                IsDead() ||
                IsPetrified() ||
                IsEradicated())
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool CanBeAffectedByCondition(Condition condition, bool isBlockable)
        {
            if (!isBlockable)
            {
                return true;
            }

            // Protection from Magic buff
            SpellEffect protFromMagicBuff = Party.PartyBuffMap[PartyEffectType.ProtectionFromMagic];
            if (protFromMagicBuff.IsActive())
            {
                bool isBlockedByProtMagic = false;
                switch (condition)
                {
                    case Condition.PoisonMedium:
                    case Condition.PoisonSevere:
                    case Condition.PoisonWeak:
                    case Condition.DiseaseMedium:
                    case Condition.DiseaseSevere:
                    case Condition.DiseaseWeak:
                    case Condition.Weak:
                    case Condition.Paralyzed:
                    case Condition.Petrified:
                        isBlockedByProtMagic = true;
                        break;
                    case Condition.Dead:
                    case Condition.Eradicated:
                        if (protFromMagicBuff.SkillMastery == SkillMastery.Grandmaster)
                        {
                            isBlockedByProtMagic = true;
                        }
                        break;
                }

                if (isBlockedByProtMagic)
                {
                    // Remove charge
                    protFromMagicBuff.Power--;
                    if (protFromMagicBuff.Power <= 0)
                    {
                        protFromMagicBuff.Reset();
                    }
                    // Player will not be affected - intercepted by protection from magic
                    return false;
                }
            }

            // Check protection from items
            switch (condition)
            {
                case Condition.PoisonMedium:
                case Condition.PoisonSevere:
                case Condition.PoisonWeak:
                    if (WearsItemWithBonusStat(StatType.PoisonImmunity))
                    {
                        return false;
                    }
                    break;

                case Condition.DiseaseMedium:
                case Condition.DiseaseSevere:
                case Condition.DiseaseWeak:
                    if (WearsItemWithBonusStat(StatType.DiseaseImmunity))
                    {
                        return false;
                    }
                    break;

                case Condition.Fear:
                    if (WearsItemWithBonusStat(StatType.FearImmunity))
                    {
                        return false;
                    }
                    break;

                case Condition.Insane:
                    if (WearsItemWithBonusStat(StatType.InsanityImmunity))
                    {
                        return false;
                    }
                    break;

                case Condition.Paralyzed:
                    if (WearsItemWithBonusStat(StatType.ParalyzeImmunity))
                    {
                        return false;
                    }
                    break;

                case Condition.Petrified:
                    if (WearsItemWithBonusStat(StatType.PetrifyImmunity))
                    {
                        return false;
                    }
                    break;

                case Condition.Sleep:
                    if (WearsItemWithBonusStat(StatType.SleepImmunity))
                    {
                        return false;
                    }
                    break;
            }

            return true;
        }

        public void SetCondition(Condition condition, bool isBlockable)
        {
            if (Conditions[condition].IsValid())
            {
                return;
            }

            if (!CanBeAffectedByCondition(condition, isBlockable))
            {
                return;
            }

            switch (condition)
            {
                case Condition.Cursed:
                    PlayEventReaction(CharacterReaction.Cursed);
                    break;
                case Condition.Weak:
                    PlayEventReaction(CharacterReaction.Weakened);
                    break;
                case Condition.Sleep:
                    break;
                case Condition.Fear:
                    PlayEventReaction(CharacterReaction.Feared);
                    break;
                case Condition.Drunk:
                    PlayEventReaction(CharacterReaction.Drunk);
                    break;
                case Condition.Insane:
                    PlayEventReaction(CharacterReaction.Insane);
                    break;

                case Condition.PoisonWeak:
                case Condition.PoisonMedium:
                case Condition.PoisonSevere:
                    PlayEventReaction(CharacterReaction.Poisoned);
                    break;

                case Condition.DiseaseWeak:
                case Condition.DiseaseMedium:
                case Condition.DiseaseSevere:
                    PlayEventReaction(CharacterReaction.Diseased);
                    break;

                case Condition.Paralyzed:
                    break;

                case Condition.Unconcious:
                    PlayEventReaction(CharacterReaction.Unconcious);
                    if (CurrHitPoints > 0)
                    {
                        CurrHitPoints = 0;
                    }
                    break;

                case Condition.Dead:
                    PlayEventReaction(CharacterReaction.Dead);
                    if (CurrHitPoints > 0)
                    {
                        CurrHitPoints = 0;
                    }
                    if (CurrSpellPoints > 0)
                    {
                        CurrSpellPoints = 0;
                    }
                    break;

                case Condition.Petrified:
                    PlayEventReaction(CharacterReaction.Petrified);
                    break;

                case Condition.Eradicated:
                    PlayEventReaction(CharacterReaction.Eradicated);
                    if (CurrHitPoints > 0)
                    {
                        CurrHitPoints = 0;
                    }
                    if (CurrSpellPoints > 0)
                    {
                        CurrSpellPoints = 0;
                    }
                    break;

                case Condition.Zombie:
                    // Cannot even happen in MM8
                    Debug.LogError("Became zombie ?");
                    break;
            }

            // Check how many people could act before setting this condition
            int playersBefore = 0;
            Party.Characters.ForEach(chr =>
            {
                if (chr.CanAct())
                {
                    playersBefore++;
                }
            });

            // Set the condition for this player
            Conditions[condition] = TimeMgr.Instance.CurrentTime;

            // Check how many players can act after this player has this condition set
            int playersNow = 0;
            Character lastPlayer = null;
            Party.Characters.ForEach(chr =>
            {
                if (chr.CanAct())
                {
                    playersNow++;
                    lastPlayer = chr;
                }
            });

            bool isLastManStanding = playersBefore == 2 && playersNow == 1;
            if (isLastManStanding)
            {
                lastPlayer.PlayEventReaction(CharacterReaction.LastManStanding);
            }
        }

        // This is general-purpose reaction to all game events
        // It will try to play sound + facial avatar animation
        public void PlayEventReaction(CharacterReaction characterReaction)
        {
            CharacterReactionData reactionData = DbMgr.Instance.CharacterReactionDb.Get(characterReaction);

            int numSpeechVariants = reactionData.SpeechVariants.Length;
            if (numSpeechVariants > 0)
            {
                int rndSpeechIndex = UnityEngine.Random.Range(0, numSpeechVariants);
                CharacterSpeech speechVariant = reactionData.SpeechVariants[rndSpeechIndex];
                PlayCharacterSpeech(speechVariant);
            }

            int numExpressionVariants = reactionData.ExpressionVariants.Length;
            if (numExpressionVariants > 0)
            {
                int rndExpressionIndex = UnityEngine.Random.Range(0, numExpressionVariants);
                CharacterExpression expressionVariant = reactionData.ExpressionVariants[rndExpressionIndex];
                Debug.Log("Len: " + numExpressionVariants + ", Chosen: " + expressionVariant);
                PlayCharacterExpression(expressionVariant);
            }
        }

        public void PlayCharacterExpression(CharacterExpression newExpression)
        {
            // Check if newExpression is applicable
            if (CurrExpression == CharacterExpression.Dead ||
                CurrExpression == CharacterExpression.Eradicated)
            {
                // Cannot really react when I am dead
                return;
            }
            else if (CurrExpression == CharacterExpression.Petrified && 
                     newExpression == CharacterExpression.Falling)
            {
                // Cannot yell that I am falling when I am petrified
                return;
            }
            else
            {
                // I am not sleeping nor falling
                if (CurrExpression != CharacterExpression.Sleep ||
                    CurrExpression != CharacterExpression.Falling)
                {
                    // I have some bad condition but I am not poisoned
                    // and also I am not animating receiving damage
                    // ????????
                    if (CurrExpression >= CharacterExpression.Cursed && 
                        CurrExpression <= CharacterExpression.Unconcious &&
                        CurrExpression != CharacterExpression.Poisoned &&
                        !(newExpression == CharacterExpression.DamageReceiveMinor ||
                          newExpression == CharacterExpression.DamageReceiveModerate ||
                          newExpression == CharacterExpression.DamageReceiveMajor))
                    {
                        return;
                    }
                }
            }

            CharacterExpressionData expressionData = DbMgr.Instance.CharacterFaceExpressionDb.Get(newExpression);
            CurrExpressionTimePassed = 0.0f;
            CurrExpressionTimeLength = expressionData.AnimDurationSeconds;
            CurrExpression = newExpression;
        }

        public void PlayCharacterSpeech(CharacterSpeech characterSpeech)
        {
            switch (characterSpeech)
            {
                case CharacterSpeech.TrapDisarmed:
                    SoundMgr.PlayRandomSound(VoiceData.TrapDisarmedOk);
                    break;
                case CharacterSpeech.FailedDisarm:
                    SoundMgr.PlayRandomSound(VoiceData.TrapDisarmedFail);
                    break;
                case CharacterSpeech.DoorIsClosed:
                    SoundMgr.PlayRandomSound(VoiceData.DoorsClosed);
                    break;
                case CharacterSpeech.ChooseMe:
                    SoundMgr.PlayRandomSound(VoiceData.ChooseMe);
                    break;
                case CharacterSpeech.BadItem:
                    SoundMgr.PlayRandomSound(VoiceData.BadItem);
                    break;
                case CharacterSpeech.GoodItem:
                    SoundMgr.PlayRandomSound(VoiceData.GoodItem);
                    break;
                case CharacterSpeech.CantIdentify:
                    SoundMgr.PlayRandomSound(VoiceData.CantIdentify);
                    break;
                case CharacterSpeech.ItemRepaired:
                    SoundMgr.PlayRandomSound(VoiceData.Repaired);
                    break;
                case CharacterSpeech.CannotRepairItem:
                    SoundMgr.PlayRandomSound(VoiceData.CantRepair);
                    break;
                case CharacterSpeech.IdentifiedWeakMonster:
                    SoundMgr.PlayRandomSound(VoiceData.EasyFight);
                    break;
                case CharacterSpeech.IdentifiedStrongMonster:
                    SoundMgr.PlayRandomSound(VoiceData.HardFight);
                    break;
                case CharacterSpeech.CantIdentifyMonster:
                    SoundMgr.PlayRandomSound(VoiceData.CantIdMonster);
                    break;
                case CharacterSpeech.QuickSpellWasSet:
                    SoundMgr.PlayRandomSound(VoiceData.QuickSpell);
                    break;
                case CharacterSpeech.Hungry:
                    SoundMgr.PlayRandomSound(VoiceData.Hungry);
                    break;
                case CharacterSpeech.SoftInjured:
                    SoundMgr.PlayRandomSound(VoiceData.SoftInjured);
                    break;
                case CharacterSpeech.Injured:
                    SoundMgr.PlayRandomSound(VoiceData.Injured);
                    break;
                case CharacterSpeech.FatallyInjured:
                    SoundMgr.PlayRandomSound(VoiceData.HardInjured);
                    break;
                case CharacterSpeech.Drunk:
                    SoundMgr.PlayRandomSound(VoiceData.Drunk);
                    break;
                case CharacterSpeech.Insane:
                    SoundMgr.PlayRandomSound(VoiceData.Insane);
                    break;
                case CharacterSpeech.Poisoned:
                    SoundMgr.PlayRandomSound(VoiceData.Poisoned);
                    break;
                case CharacterSpeech.Cursed:
                    SoundMgr.PlayRandomSound(VoiceData.Misc);
                    break;
                case CharacterSpeech.Fear:
                    SoundMgr.PlayRandomSound(VoiceData.Fall);
                    break;
                case CharacterSpeech.CannotRestHere:
                    SoundMgr.PlayRandomSound(VoiceData.CantRestHere);
                    break;
                case CharacterSpeech.NeedMoreGold:
                    SoundMgr.PlayRandomSound(VoiceData.NeedMoreGold);
                    break;
                case CharacterSpeech.InventoryFull:
                    SoundMgr.PlayRandomSound(VoiceData.InventoryFull);
                    break;
                case CharacterSpeech.PotionMixed:
                    SoundMgr.PlayRandomSound(VoiceData.PotionMixed);
                    break;
                case CharacterSpeech.FailedPotionMixing:
                    SoundMgr.PlayRandomSound(VoiceData.FailMixing);
                    break;
                case CharacterSpeech.NeedAKey:
                    SoundMgr.PlayRandomSound(VoiceData.NeedAKey);
                    break;
                case CharacterSpeech.LearnedSpell:
                    SoundMgr.PlayRandomSound(VoiceData.LearnSpell);
                    break;
                case CharacterSpeech.CannotLearnSpell:
                    SoundMgr.PlayRandomSound(VoiceData.CantLearn);
                    break;
                case CharacterSpeech.CannotEquipItem:
                    SoundMgr.PlayRandomSound(VoiceData.CantEquip);
                    break;
                case CharacterSpeech.GoodDay:
                    SoundMgr.PlayRandomSound(VoiceData.GoodDay);
                    break;
                case CharacterSpeech.GoodEvening:
                    SoundMgr.PlayRandomSound(VoiceData.GoodEvening);
                    break;
                case CharacterSpeech.Win:
                    SoundMgr.PlayRandomSound(VoiceData.Win);
                    break;
                case CharacterSpeech.Heh:
                    SoundMgr.PlayRandomSound(VoiceData.Heh);
                    break;
                case CharacterSpeech.LastManStanding:
                    SoundMgr.PlayRandomSound(VoiceData.LastStanding);
                    break;
                case CharacterSpeech.HardFightEnded:
                    SoundMgr.PlayRandomSound(VoiceData.HardFightEnd);
                    break;
                case CharacterSpeech.EnteredDungeon:
                    SoundMgr.PlayRandomSound(VoiceData.EnterDungeon);
                    break;
                case CharacterSpeech.Yes:
                    SoundMgr.PlayRandomSound(VoiceData.Yes);
                    break;
                case CharacterSpeech.Thanks:
                    SoundMgr.PlayRandomSound(VoiceData.Thanks);
                    break;
                case CharacterSpeech.SomeoneWasRude:
                    SoundMgr.PlayRandomSound(VoiceData.SomeoneWasRude);
                    break;
                case CharacterSpeech.Move:
                    SoundMgr.PlayRandomSound(VoiceData.Move);
                    break;
                default:
                    break;
            }
        }

        //=========================================================================================
        // Receiving and dealing damage
        //=========================================================================================


        // Damage @damageAmount reduced by respective resistances to @spellElement
        public int CalculateIncomingDamage(int damageAmount, SpellElement spellElement)
        {
            // Lich is immune to Mind/Body/Spirit
            if (Class == CharacterClass.Lich &&
                (spellElement == SpellElement.Mind ||
                 spellElement == SpellElement.Spirit ||
                 spellElement == SpellElement.Body))
            {
                return 0;
            }

            int resistAmount = GetActualResistance(spellElement);
            int luckAmount = GetActualLuck();
            float resistReductionCoeff = GameMechanics.GetResistanceReductionCoeff(spellElement, resistAmount, luckAmount);

            if (spellElement == SpellElement.Physical)
            {
                Item armorItem = GetItemAtSlot(EquipSlot.Armor);
                if (armorItem != null && !armorItem.IsBroken)
                {
                    if (armorItem.Data.SkillGroup == ItemSkillGroup.Plate)
                    {
                        if (GetSkillMastery(SkillType.PlateArmor) >= SkillMastery.Master)
                        {
                            resistReductionCoeff /= 2.0f;
                        }
                    }

                    if (armorItem.Data.SkillGroup == ItemSkillGroup.Chain)
                    {
                        if (GetSkillMastery(SkillType.ChainArmor) == SkillMastery.Grandmaster)
                        {
                            resistReductionCoeff *= 2.0f / 3.0f;
                        }
                    }
                }
            }

            return (int)(damageAmount * resistReductionCoeff);
        }


        // This damage is already probably partly reduced by effects like Shield
        //    - but is further reduced by resistances to @damageElement
        public int ReceiveDamage(int damageAmount, SpellElement damageElement)
        {
            Conditions[Condition.Sleep].Reset();

            int damageTaken = CalculateIncomingDamage(damageAmount, damageElement);
            CurrHitPoints -= damageTaken;

            // Player too hurt - unconcious or dead
            if (CurrHitPoints <= 0)
            {
                // High endurance or preservation buff prevents character from dying
                //   - he will rather stay in unconcious state
                if ((CurrHitPoints + GetBaseEndurance()) > 0 ||
                    PlayerBuffMap[PlayerEffectType.Preservation].IsActive())
                {
                    SetCondition(Condition.Unconcious, false);
                }
                else
                {
                    SetCondition(Condition.Dead, false);
                }

                // Break armor if health dropped below -10
                if (CurrHitPoints <= -10)
                {
                    Item armorItem = GetItemAtSlot(EquipSlot.Armor);
                    if (armorItem != null)
                    {
                        armorItem.IsBroken = true;
                    }
                }
            }

            if (damageTaken > 0 && CanAct())
            {
                // TODO: Each character should have his own audio source
                //PlayEventReaction(CharacterReaction.DamagedOww);
            }

            return damageTaken;
        }


        // Calculates melee damage with specific weapon to specific monster
        public int CalculateMeleeDamageToMonsterWithWeapon(Item weapon,
            Monster monster,
            bool addOneDice = false)
        {
            int diceCount = weapon.GetDiceRolls();
            if (addOneDice)
            {
                diceCount++;
            }

            int diceResult = 0;
            for (int i = 0; i < diceCount; i++)
            {
                diceResult += UnityEngine.Random.Range(0, weapon.GetDiceSides()) + 1;
            }

            int totalDamage = weapon.GetMod() + diceResult;

            if (monster != null)
            {
                // TODO: Check double damage to ogres, titans, dragons, etc.
            }

            if (GetSkillMastery(SkillType.Dagger) >= SkillMastery.Master &&
                weapon.Data.SkillGroup == ItemSkillGroup.Dagger)
            {
                if (UnityEngine.Random.Range(0, 100) < 10)
                {
                    totalDamage *= 3;
                }
            }

            return totalDamage;
        }


        // Calculates ranged damage to specific monster
        public int CalculateRangedDamage(Monster victim)
        {
            if (!WearsItemAtSlot(EquipSlot.Bow))
            {
                return 0;
            }

            Item bow = GetItemAtSlot(EquipSlot.Bow);
            int damage = bow.GetMod() + GameMechanics.GetDiceResult(bow.GetDiceRolls(), bow.GetDiceSides());

            if (victim != null)
            {
                // Handle x2 damage enchants
            }

            return damage + GetSkillsBonus(StatType.RangedDamageBonus);
        }


        // Generates true/false whether the attack will miss or hit
        public bool WillHitMonster(Monster monster)
        {
            int monsterArmor = monster.Data.ArmorClass;
            int armorBuff = 0;

            if (monster.BuffMap[MonsterBuffType.HalvedArmorClass].IsActive())
            {
                monsterArmor /= 2;
            }

            if (monster.BuffMap[MonsterBuffType.HourOfPower].IsActive())
            {
                armorBuff = monster.BuffMap[MonsterBuffType.HourOfPower].Power;
            }

            if (monster.BuffMap[MonsterBuffType.Stoneskin].IsActive() &&
                monster.BuffMap[MonsterBuffType.Stoneskin].Power > armorBuff)
            {
                armorBuff = monster.BuffMap[MonsterBuffType.Stoneskin].Power;
            }

            int effectiveArmor = monsterArmor + armorBuff;

            // Distance modificator
            int distanceMod = 0;

            // TODO: Im not really sure
            float distance = (monster.transform.position - Party.transform.position).sqrMagnitude;
            if (distance < Monster.MAX_MELEE_DISTANCE_SQR)
            {
                distanceMod = 0;
            }
            else if (distance < 512)
            {
                distanceMod = 1;
            }
            else if (distance < 1280)
            {
                distanceMod = 2;
            }
            else
            {
                distanceMod = 3;
            }

            int attackBonus;
            if (distanceMod == 0)
            {
                attackBonus = GetMeleeAttack();
            }
            else
            {
                attackBonus = GetRangedAttack();
            }

            int attackPositiveMod = UnityEngine.Random.Range(0,
                effectiveArmor + 2 * attackBonus + 30);

            int attackNegativeMod;
            if (distanceMod == 2)
            {
                attackNegativeMod = ((effectiveArmor + 15) / 2) + effectiveArmor + 15;
            }
            else if (distanceMod == 3)
            {
                attackNegativeMod = 2 * effectiveArmor + 30;
            }
            else
            {
                attackNegativeMod = effectiveArmor + 15;
            }

            return (attackPositiveMod > attackNegativeMod);
        }


        // Generates melee damage to specific monster
        public int CalculateMeleeDamageToMonster(Monster victim)
        {
            int mainhandWeaponDamage = 0;
            int offhandWeaponDamage = 0;

            if (IsUnarmed())
            {
                mainhandWeaponDamage = UnityEngine.Random.Range(0, 3) + 1;
            }
            else
            {
                Item mainhandItem = GetItemAtSlot(EquipSlot.MainHand);
                Item offhandItem = GetItemAtSlot(EquipSlot.OffHand);

                if (mainhandItem != null)
                {
                    bool holdsSpearWithNoShield = mainhandItem.Data.SkillGroup == ItemSkillGroup.Spear &&
                        (offhandItem == null || offhandItem.Data.SkillGroup != ItemSkillGroup.Shield);

                    mainhandWeaponDamage = CalculateMeleeDamageToMonsterWithWeapon(
                        mainhandItem, victim, holdsSpearWithNoShield);
                }

                if (offhandItem != null)
                {
                    if (offhandItem.Data.ItemType != ItemType.Shield)
                    {
                        offhandWeaponDamage = CalculateMeleeDamageToMonsterWithWeapon(
                            offhandItem, victim, false);
                    }
                }
            }

            int damageSum = mainhandWeaponDamage + offhandWeaponDamage;


            if (damageSum < 1)
            {
                damageSum = 1;
            }

            return 200;
            return damageSum;
        }
    }
}
