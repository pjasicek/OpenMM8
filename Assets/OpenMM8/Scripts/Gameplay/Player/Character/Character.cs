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
            return ((float)CurrHitPoints / (float)GetMaxHealth()) * 100.0f;
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
            int maxHP = Stats.HitPoints + BonusStats.HitPoints;
            CurrHitPoints = Mathf.Min(CurrHitPoints + numHitPoints, maxHP);

            GameEvents.InvokeEvent_OnCharHealthChanged(this, maxHP, CurrHitPoints, numHitPoints);
        }

        public void AddCurrSpellPoints(int numSpellPoints)
        {
            int maxMP = Stats.SpellPoints + BonusStats.SpellPoints;
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

            RecalculateStats();

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
                RecalculateStats();

                // TODO: Separate ?
                CharFaceUpdater.SetAvatar(UiMgr.RandomSprite(UI.Sprites.Smile), 0.75f);
                SoundMgr.PlaySoundByName("Quest");
            }
        }

        public void RecalculateStats()
        {
            System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            // This method is stateless - it takes into consideration all factors (base stats, equipment, skills, buffs, etc)
            //    and recalculates all stats

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

            BaseItem mainHandWeapon = UI.DollUI.RH_Weapon?.Item;
            BaseItem offHandWeapon = UI.DollUI.LF_Weapon?.Item;
            BaseItem bowWeapon = UI.DollUI.Bow.Item;

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

            /*
            foreach (SpellEffecct spellEffect in this.SpellEffects)
            {
                spellEffect.Apply(this);
            }
            */


            // After all stat modifiers are applied
            int totalMight = Stats.Attributes[CharAttribute.Might] + BonusStats.Attributes[CharAttribute.Might];
            int totalIntellect = Stats.Attributes[CharAttribute.Intellect] + BonusStats.Attributes[CharAttribute.Intellect];
            int totalEndurance = Stats.Attributes[CharAttribute.Endurance] + BonusStats.Attributes[CharAttribute.Endurance];
            int totalPersonality = Stats.Attributes[CharAttribute.Personality] + BonusStats.Attributes[CharAttribute.Personality];
            int totalSpeed = Stats.Attributes[CharAttribute.Speed] + BonusStats.Attributes[CharAttribute.Speed];
            int totalAccuracy = Stats.Attributes[CharAttribute.Accuracy] + BonusStats.Attributes[CharAttribute.Accuracy];
            int totalLuck = Stats.Attributes[CharAttribute.Luck] + BonusStats.Attributes[CharAttribute.Luck];

            // TODO: Handle immunities (vampire = immune to mind, lich = immune to ?)
            int totalFireResist = Stats.Resistances[SpellElement.Fire] + BonusStats.Resistances[SpellElement.Fire];
            int totalAirResist = Stats.Resistances[SpellElement.Air] + BonusStats.Resistances[SpellElement.Air];
            int totalWaterResist = Stats.Resistances[SpellElement.Water] + BonusStats.Resistances[SpellElement.Water];
            int totalEarthResist = Stats.Resistances[SpellElement.Earth] + BonusStats.Resistances[SpellElement.Earth];
            int totalMindResist = Stats.Resistances[SpellElement.Mind] + BonusStats.Resistances[SpellElement.Mind];
            int totalBodyResist = Stats.Resistances[SpellElement.Body] + BonusStats.Resistances[SpellElement.Body];

            int mightEffect = GameMechanics.GetAttributeEffect(totalMight);

            AttackBonus = 0;
            MinAttackDamage = MaxAttackDamage = 0;
            ShootBonus = 0;
            MinShootDamage = MaxShootDamage = 0;

            if (mainHandWeapon != null)
            {
                int baseDmg = int.Parse(mainHandWeapon.Data.Mod2);
                string[] diceCountAndSides = mainHandWeapon.Data.Mod1.Split('d');
                int minVariableDmg = int.Parse(diceCountAndSides[0]);
                int maxVariableDmg = int.Parse(diceCountAndSides[1]) * minVariableDmg;

                MinAttackDamage += minVariableDmg + baseDmg;
                MaxAttackDamage += maxVariableDmg + baseDmg;

                AttackBonus += baseDmg;
            }

            MinAttackDamage += mightEffect + 1;
            MaxAttackDamage += mightEffect + 3;

            if (bowWeapon != null)
            {
                int baseDmg = int.Parse(bowWeapon.Data.Mod2);
                string[] diceCountAndSides = bowWeapon.Data.Mod1.Split('d');
                int minVariableDmg = int.Parse(diceCountAndSides[0]);
                int maxVariableDmg = int.Parse(diceCountAndSides[1]) * minVariableDmg;

                MinShootDamage += minVariableDmg + baseDmg;
                MaxShootDamage += maxVariableDmg + baseDmg;

                ShootBonus += baseDmg;
            }

            ClassHpSpData classHpSpData = DbMgr.Instance.ClassHpSpDb.Get(Class);
            int enduranceEffect = GameMechanics.GetAttributeEffect(totalEndurance);
            int hpFromEndurance = enduranceEffect * classHpSpData.HitPointsFactor;
            int hpFromLevel = classHpSpData.HitPointsBase + (Stats.Level + BonusStats.Level) * classHpSpData.HitPointsFactor;

            // TODO: Bodybuilding + Item HP bonuses
            Stats.HitPoints = hpFromLevel + hpFromEndurance;

            Stats.SpellPoints = 0;
            Stats.SpellPoints += classHpSpData.SpellPointsBase + (Stats.Level + BonusStats.Level) * classHpSpData.SpellPointsFactor;
            if (classHpSpData.IsSpellPointsFromIntellect)
            {
                int intellectEffect = GameMechanics.GetAttributeEffect(totalIntellect);
                Stats.SpellPoints += intellectEffect * classHpSpData.SpellPointsFactor;
            }

            if (classHpSpData.IsSpellPointsFromPersonality)
            {
                int personalityEffect = GameMechanics.GetAttributeEffect(totalPersonality);
                Stats.SpellPoints += personalityEffect * classHpSpData.SpellPointsFactor;
            }

            // TODO: + mana from items, + mana from meditation

            int speedEffect = GameMechanics.GetAttributeEffect(totalSpeed);
            Stats.ArmorClass += speedEffect;

            // TODO: + armor class from items

            BeingHitRecoveryTime = Math.Max(20 - enduranceEffect, 0);

            // TODO: Calculate armor penalty
            // TODO: "Swift" enchant
            // int armorRecoveryPenalty = ...
            AttackRecoveryTime = 100 - speedEffect; /* + armorRecoveryPenalty*/

            // Recovery time cannot go lower than 30
            // TODO: Bows and blasters are exception
            if (AttackRecoveryTime < 30)
            {
                AttackRecoveryTime = 30;
            }

            int accuracyEffect = GameMechanics.GetAttributeEffect(totalAccuracy);

            AttackBonus += accuracyEffect;
            ShootBonus += accuracyEffect;
            // TODO: Add skill coefficients / bonuses

            // TODO: Calculate/Add resist bonuses - buffs etc


            int totalHitPoints = GetMaxHealth();
            int totalSpellPoints = GetMaxSpellPoints();

            // -------------------------------
            // Clamping / Overrides - some stats cannot be below certain threshold
            // -------------------------------
            AttackBonus = Math.Max(AttackBonus, 0);
            ShootBonus = Math.Max(ShootBonus, 0);

            MinAttackDamage = Math.Max(MinAttackDamage, 1);
            MaxAttackDamage = Math.Max(MaxAttackDamage, 1);
            MinShootDamage = Math.Max(MinShootDamage, 1);
            MaxShootDamage = Math.Max(MaxShootDamage, 1);

            CurrHitPoints = Math.Min(CurrHitPoints, totalHitPoints);
            CurrSpellPoints = Math.Min(CurrSpellPoints, totalSpellPoints);

            Stats.ArmorClass = Math.Max(Stats.ArmorClass, 0);
            BonusStats.ArmorClass = Math.Max(BonusStats.ArmorClass, 0);
            int totalArmorClass = Stats.ArmorClass + BonusStats.ArmorClass;

            if (Race == CharacterRace.Vampire)
            {
                Stats.Resistances[SpellElement.Mind] = int.MaxValue;
            }

            // Maybe handle this elsewhere ?
            StatsUI ui = UI.StatsUI;
            ui.NameText.text = Name;

            string skillPointsStr = SkillPoints.ToString();
            if (SkillPoints > 0)
            {
                skillPointsStr = "<color=#00ff00ff>" + SkillPoints + "</color>";
            }
            ui.SkillPointsText.text = "Skill Points: " + skillPointsStr;

            ui.MightText.text = GenStatTextPair(totalMight, Stats.Attributes[CharAttribute.Might]);
            ui.IntellectText.text = GenStatTextPair(totalIntellect, Stats.Attributes[CharAttribute.Intellect]);
            ui.PersonalityText.text = GenStatTextPair(totalPersonality, Stats.Attributes[CharAttribute.Personality]);
            ui.EnduranceText.text = GenStatTextPair(totalEndurance, Stats.Attributes[CharAttribute.Endurance]);
            ui.AccuracyText.text = GenStatTextPair(totalAccuracy, Stats.Attributes[CharAttribute.Accuracy]);
            ui.SpeedText.text = GenStatTextPair(totalSpeed, Stats.Attributes[CharAttribute.Speed]);
            ui.LuckText.text = GenStatTextPair(totalLuck, Stats.Attributes[CharAttribute.Luck]);

            // hp/sp:
            // < 20% = red
            // < 100% = yellow
            if (GetHealthPercentage() < 20.0f)
            {
                ui.HitPointsText.text = "<color=red>" + CurrHitPoints + "</color> / " + totalHitPoints;
            }
            else if (GetHealthPercentage() < 100.0f)
            {
                ui.HitPointsText.text = "<color=yellow>" + CurrHitPoints + "</color> / " + totalHitPoints;
            }
            else
            {
                ui.HitPointsText.text = CurrHitPoints + " / " + totalHitPoints;
            }

            if (GetManaPercentage() < 20.0f)
            {
                ui.SpellPointsText.text = "<color=red>" + CurrSpellPoints + "</color> / " + totalSpellPoints;
            }
            else if (GetManaPercentage() < 100.0f)
            {
                ui.SpellPointsText.text = "<color=yellow>" + CurrSpellPoints + "</color> / " + totalSpellPoints;
            }
            else
            {
                ui.SpellPointsText.text = CurrSpellPoints + " / " + totalSpellPoints;
            }

            ui.ArmorClassText.text = GenStatTextPair(totalArmorClass, Stats.ArmorClass);

            ui.ConditionText.text = Condition.ToString();
            ui.QuickSpellText.text = QuickSpellName;

            ui.AgeText.text = GenStatTextPair(BonusStats.Age + Stats.Age, Stats.Age);
            ui.LevelText.text = GenStatTextPair(BonusStats.Level + Stats.Level, Stats.Level);

            // Check if can reach next level
            if (Experience > GameMechanics.GetTotalExperienceRequired(Stats.Level + 1))
            {
                ui.ExperienceText.text = "<color=green>" + Experience + "</color>";
            }
            else
            {
                ui.ExperienceText.text = Experience.ToString();
            }

            string attackBonusStr = AttackBonus.ToString();
            if (AttackBonus >= 0)
            {
                attackBonusStr = "+" + AttackBonus;
            }
            string attackDamageStr = MinAttackDamage + " - " + MaxAttackDamage;

            string shootBonusStr = ShootBonus.ToString();
            if (ShootBonus >= 0)
            {
                shootBonusStr = "+" + ShootBonus;
            }
            string shootDamageStr = MinShootDamage + " - " + MaxShootDamage;
            if (UI.DollUI.Bow.Item == null)
            {
                shootDamageStr = "N/A";
            }


            ui.AttackText.text = attackBonusStr;
            ui.AttackDamageText.text = attackDamageStr;
            ui.ShootText.text = shootBonusStr;
            ui.ShootDamageText.text = shootDamageStr;



            Debug.Log("recalc time: " + sw.ElapsedMilliseconds);
            sw.Stop();

        }

        private string GenResistTextPair(int currValue, int baseValue)
        {
            if (baseValue == int.MaxValue)
            {
                return "Immune";
            }
            else
            {
                return GenStatTextPair(currValue, baseValue);
            }
        }

        private string GenStatTextPair(int currValue, int baseValue)
        {
            if (currValue > baseValue)
            {
                return "<color=green>" + currValue + "</color> / " + baseValue;
            }
            else if (currValue < baseValue)
            {
                return "<color=red>" + currValue + "</color> / " + baseValue;
            }
            else
            {
                return currValue + " / " + baseValue; 
            }
        }

        //======================================================================

        public int GetBaseAge()
        {
            return TimeMgr.Instance.GetCurrentTime().Year - BirthYear;
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
            int itemBonus = 0;

            return racialBonus + itemBonus;
        }

        public int GetBaseAttribute(CharAttribute attribute)
        {
            int baseAttribute = BaseAttributes[attribute];

            // + Get from items
            int itemBonus = 0;

            return baseAttribute + itemBonus;
        }

        public int GetActualLevel()
        {
            // + level from items / buffs - e.g. well in MM6 Kriegspire added 30 or 50 levels
            return Level;
        }

        public int GetActualAge()
        {
            // Also get age mod from items etc.

            return GetBaseAge() + AgeModifier;
        }

        public int GetActualResistance(SpellElement resistType)
        {
            int baseAmount = GetBaseResistance(resistType);
            int resistFromSkills = 0; // TODO
            int resistFromMagic = 0; // TODO

            return baseAmount + resistFromSkills + resistFromMagic;
        }

        public int GetActualSkillLevel(SkillType skillType)
        {
            if (!HasSkill(skillType))
            {
                return 0;
            }

            int baseAmount = Skills[skillType].Level;
            int itemBonus = 0;  // TODO

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

            // TODO
            int baseValue = BaseAttributes[attribute];
            int magicBonus = 0;
            int itemBonus = 0;

            return (int)(baseValue * agingMultiplier * conditionMultiplier) + magicBonus + itemBonus;
        }

        // Helper accessors
        public int GetMaxHealth()
        {
            ClassHpSpData classHpSpData = DbMgr.Instance.ClassHpSpDb.Get(Class);

            int hpBase = classHpSpData.HitPointsBase;
            int hpFromLevel = GetActualLevel() * classHpSpData.HitPointsFactor;
            int hpFromEndurance = GameMechanics.GetAttributeEffect(GetActualEndurance()) * classHpSpData.HitPointsFactor;
            int hpFromItems = 0; // TODO
            int hpFromSkills = 0; // TODO - Bodybuilding

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

            int mpFromItems = 0; // TODO
            int mpFromSkills = 0; // TODO

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
    }
}
