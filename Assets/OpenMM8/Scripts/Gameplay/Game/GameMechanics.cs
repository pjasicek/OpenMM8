using Assets.OpenMM8.Scripts.Gameplay.Data;
using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    static public class GameMechanics
    {
        static public bool WillPlayerHitMonster(Character character, Monster monster)
        {
            int monsterArmor = monster.MonsterData.ArmorClass;
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
            float distance = (monster.transform.position - character.Party.transform.position).sqrMagnitude;
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
                attackBonus = character.GetMeleeAttack();
            }
            else
            {
                attackBonus = character.GetRangedAttack();
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

        static public bool WillMonsterHitPlayer(Monster monster, Character victim)
        {
            int hitBonus = 0;
            if (monster.BuffMap[MonsterBuffType.HourOfPower].IsActive())
            {
                hitBonus = monster.BuffMap[MonsterBuffType.HourOfPower].Power;
            }
            if (monster.BuffMap[MonsterBuffType.Bless].IsActive() &&
                monster.BuffMap[MonsterBuffType.Bless].Power > hitBonus)
            {
                hitBonus = monster.BuffMap[MonsterBuffType.Bless].Power;
            }
            if (monster.BuffMap[MonsterBuffType.Fate].IsActive())
            {
                hitBonus += monster.BuffMap[MonsterBuffType.Fate].Power;
                monster.BuffMap[MonsterBuffType.Fate].Reset();
            }

            int attackNegativeMod = victim.GetActualArmorClass() + 2 * monster.MonsterData.Level + 10;
            int attackPositiveMod = hitBonus + UnityEngine.Random.Range(0, attackNegativeMod);

            return attackPositiveMod > (attackNegativeMod + 5);
        }

        static public bool WillMonsterHitMonster(Monster monster, Monster victim)
        {
            return false;
        }

        static public void DamagePlayerFromMonster(Monster monster, PlayerParty playerParty)
        {
            Character victim = SelectMonsterAttackVictim(monster, playerParty);
            if (victim == null)
            {
                Debug.LogError("No available victim for monster attack");
                return;
            }


        }

        static public Character SelectMonsterAttackVictim(Monster monster, PlayerParty playerParty)
        {
            List<Character> victimList = new List<Character>();

            foreach (Character chr in playerParty.Characters)
            {
                AttackPreferenceMask attackPreferenceMask = monster.MonsterData.AttackPreferenceMask;

                if (attackPreferenceMask.HasFlag(AttackPreferenceMask.ClassCleric) &&
                    (chr.Class == CharacterClass.Cleric ||
                     chr.Class == CharacterClass.Priest))
                {
                    victimList.Add(chr);
                    continue;
                }
                if (attackPreferenceMask.HasFlag(AttackPreferenceMask.ClassKnight) &&
                    (chr.Class == CharacterClass.Knight ||
                     chr.Class == CharacterClass.Champion))
                {
                    victimList.Add(chr);
                    continue;
                }
                if (attackPreferenceMask.HasFlag(AttackPreferenceMask.ClassNecromancer) &&
                    (chr.Class == CharacterClass.Necromancer ||
                     chr.Class == CharacterClass.Lich))
                {
                    victimList.Add(chr);
                    continue;
                }
                // Race classes wont be handled here, like troll, dark elf etc

                if (attackPreferenceMask.HasFlag(AttackPreferenceMask.GenderMale) &&
                    chr.IsMale())
                {
                    victimList.Add(chr);
                    continue;
                }
                if (attackPreferenceMask.HasFlag(AttackPreferenceMask.GenderFemale) &&
                    chr.IsFemale())
                {
                    victimList.Add(chr);
                    continue;
                }

                if (attackPreferenceMask.HasFlag(AttackPreferenceMask.RaceDarkElf) &&
                    chr.Race == CharacterRace.DarkElf)
                {
                    victimList.Add(chr);
                    continue;
                }
                if (attackPreferenceMask.HasFlag(AttackPreferenceMask.RaceGoblin) &&
                    chr.Race == CharacterRace.Goblin)
                {
                    victimList.Add(chr);
                    continue;
                }
                if (attackPreferenceMask.HasFlag(AttackPreferenceMask.RaceElf) &&
                    chr.Race == CharacterRace.Elf)
                {
                    victimList.Add(chr);
                    continue;
                }
                if (attackPreferenceMask.HasFlag(AttackPreferenceMask.RaceDragon) &&
                    chr.Race == CharacterRace.Dragon)
                {
                    victimList.Add(chr);
                    continue;
                }
                if (attackPreferenceMask.HasFlag(AttackPreferenceMask.RaceMinotaur) &&
                    chr.Race == CharacterRace.Minotaur)
                {
                    victimList.Add(chr);
                    continue;
                }
                if (attackPreferenceMask.HasFlag(AttackPreferenceMask.RaceTroll) &&
                    chr.Race == CharacterRace.Troll)
                {
                    victimList.Add(chr);
                    continue;
                }
                if (attackPreferenceMask.HasFlag(AttackPreferenceMask.RaceUndead) &&
                    chr.Race == CharacterRace.Undead)
                {
                    victimList.Add(chr);
                    continue;
                }
                if (attackPreferenceMask.HasFlag(AttackPreferenceMask.RaceVampire) &&
                    chr.Race == CharacterRace.Vampire)
                {
                    victimList.Add(chr);
                    continue;
                }
            }

            // Remove all characters which are not in attackable state
            victimList.RemoveAll(chr =>
            {
                return chr.IsParalyzed() ||
                    chr.IsUnconcious() ||
                    chr.IsDead() ||
                    chr.IsPetrified() ||
                    chr.IsEradicated();
            });

            // If monster has some specific victims, use them
            if (victimList.Count > 0)
            {
                return victimList[UnityEngine.Random.Range(0, victimList.Count)];
            }

            // If none, build it
            foreach (Character chr in playerParty.Characters)
            {
                if (!chr.IsParalyzed() ||
                    chr.IsUnconcious() ||
                    chr.IsDead() ||
                    chr.IsPetrified() ||
                    chr.IsEradicated())
                {
                    victimList.Add(chr);
                }
            }

            if (victimList.Count == 0)
            {
                Debug.LogError("No valid attack target");
                return null;
            }

            return victimList[UnityEngine.Random.Range(0, victimList.Count)];
        }

        static public int MeleeDamageFromPlayerToMonster(Character dmgDealer, Monster victim)
        {
            int mainhandWeaponDamage = 0;
            int offhandWeaponDamage = 0;

            if (dmgDealer.IsUnarmed())
            {
                mainhandWeaponDamage = UnityEngine.Random.Range(0, 3) + 1;
            }
            else
            {
                Item mainhandItem = dmgDealer.GetItemAtSlot(EquipSlot.MainHand);
                Item offhandItem = dmgDealer.GetItemAtSlot(EquipSlot.OffHand);

                if (mainhandItem != null)
                {
                    bool holdsSpearWithNoShield = mainhandItem.Data.SkillGroup == ItemSkillGroup.Spear &&
                        (offhandItem == null || offhandItem.Data.SkillGroup != ItemSkillGroup.Shield);

                    mainhandWeaponDamage = CalculateMeleeDamageToMonsterWithWeapon(
                        dmgDealer, mainhandItem, victim, holdsSpearWithNoShield);
                }

                if (offhandItem != null)
                {
                    if (offhandItem.Data.ItemType != ItemType.Shield)
                    {
                        offhandWeaponDamage = CalculateMeleeDamageToMonsterWithWeapon(
                            dmgDealer, offhandItem, victim, false);
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

        static public int CalculateMeleeDamageToMonsterWithWeapon(Character dmgDealer, 
            Item weapon, 
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

            if (dmgDealer.GetSkillMastery(SkillType.Dagger) >= SkillMastery.Master &&
                weapon.Data.SkillGroup == ItemSkillGroup.Dagger)
            {
                if (UnityEngine.Random.Range(0, 100) < 10)
                {
                    totalDamage *= 3;
                }
            }

            return totalDamage;
        }

        static public int CalculatePlayerRangedDamage(Character dmgDealer, Monster victim)
        {
            if (!dmgDealer.WearsItemAtSlot(EquipSlot.Bow))
            {
                return 0;
            }

            Item bow = dmgDealer.GetItemAtSlot(EquipSlot.Bow);
            int damage = bow.GetMod() + GetDiceResult(bow.GetDiceRolls(), bow.GetDiceSides());

            if (victim != null)
            {
                // Handle x2 damage enchants
            }

            return damage + dmgDealer.GetSkillsBonus(StatType.RangedDamageBonus);
        }

        static public int CalculateSpellDamage(SpellType spellType, SkillMastery skillMastery, int skillLevel, int currentHp)
        {
            SpellData spellData = DbMgr.Instance.SpellDataDb.Get(spellType);
            if (spellData == null)
            {
                Debug.LogError("No such spell data for: " + spellType);
                return 0;
            }

            int damage = 0;
            switch (spellType)
            {
                case SpellType.Fire_FireSpike:
                    int numDiceSides = 0;
                    switch (skillMastery)
                    {
                        case SkillMastery.Normal:
                        case SkillMastery.Expert:
                            numDiceSides = 6;
                            break;
                        case SkillMastery.Master:
                            numDiceSides = 8;
                            break;
                        case SkillMastery.Grandmaster:
                            numDiceSides = 10;
                            break;
                        default:
                            numDiceSides = 0;
                            break;
                    }
                    damage = GetDiceResult(skillLevel, numDiceSides);
                    break;

                case SpellType.Earth_MassDistortion:
                    damage = currentHp * (spellData.BaseDamage + 2 * skillLevel) / 100;
                    break;

                default:
                    damage = spellData.BaseDamage + GetDiceResult(skillLevel, spellData.DamageDiceSides);
                    break;
            }

            return damage;
        }




    

        public static int GetDiceResult(int numRolls, int numDiceSides)
        {
            int result = 0;
            for (int i = 0; i < numRolls; i++)
            {
                result += UnityEngine.Random.Range(0, numDiceSides) + 1;
            }

            return result;
        }

        /*
         * When your character gets hit by magic, there is 1 - 30/(30 + Resistance + LuckEffect) chance of reducing damage on each 'dice drop'. Here's what happens:
         *  Dice is dropped. If you are unlucky, you get full 100% damage.
         *  If you are lucky, dice is dropped again. If you are unlucky this time, you get 1/2 (50%) damage.
         *  If you are lucky, dice is dropped again. If you are unlucky this time, you get 1/4 (25%) damage.
         *  If you are lucky, dice is dropped again. If you are unlucky this time, you get 1/8 (12.5%) damage.
         *  If you are lucky, you get 1/16 (6.25%) damage.
         */
        static public float GetResistanceReductionCoeff(SpellElement element, int resistanceAmount, int luck)
        {
            // Only magic
            if (element == SpellElement.None || element == SpellElement.Physical)
            {
                return 1.0f;
            }

            int luckEffect = GetAttributeEffect(luck);

            float reductionChance = 1.0f - 30.0f / (30.0f + resistanceAmount + luckEffect);
            if (reductionChance <= 0.0f)
            {
                return 1.0f;
            }

            float reductionCoeff = 1.0f;

            // 5 dice rolls down to 1/16 chance
            for (int i = 0; i < 5; i++)
            {
                float diceResult = UnityEngine.Random.Range(0.0f, 1.0f);
                if (diceResult > reductionChance)
                {
                    // Dice roll failed;
                    break;
                }

                // Dice roll successful - Halve the damage
                reductionCoeff /= 2.0f;
            }

            return reductionCoeff;
        }

        static public int GetAttributeEffect(int attributeAmount)
        {
            if (attributeAmount >= 500)
            {
                return 30;
            }
            else if (attributeAmount >= 400)
            {
                return 25;
            }
            else if (attributeAmount >= 350)
            {
                return 20;
            }
            else if (attributeAmount >= 300)
            {
                return 19;
            }
            else if (attributeAmount >= 275)
            {
                return 18;
            }
            else if (attributeAmount >= 250)
            {
                return 17;
            }
            else if (attributeAmount >= 225)
            {
                return 16;
            }
            else if (attributeAmount >= 200)
            {
                return 15;
            }
            else if (attributeAmount >= 175)
            {
                return 14;
            }
            else if (attributeAmount >= 150)
            {
                return 13;
            }
            else if (attributeAmount >= 125)
            {
                return 12;
            }
            else if (attributeAmount >= 100)
            {
                return 11;
            }
            else if (attributeAmount >= 75)
            {
                return 10;
            }
            else if (attributeAmount >= 50)
            {
                return 9;
            }
            else if (attributeAmount >= 40)
            {
                return 8;
            }
            else if (attributeAmount >= 35)
            {
                return 7;
            }
            else if (attributeAmount >= 30)
            {
                return 6;
            }
            else if (attributeAmount >= 25)
            {
                return 5;
            }
            else if (attributeAmount >= 21)
            {
                return 4;
            }
            else if (attributeAmount >= 19)
            {
                return 3;
            }
            else if (attributeAmount >= 17)
            {
                return 2;
            }
            else if (attributeAmount >= 15)
            {
                return 1;
            }
            else if (attributeAmount >= 13)
            {
                return 0;
            }
            else if (attributeAmount >= 11)
            {
                return -1;
            }
            else if (attributeAmount >= 9)
            {
                return -2;
            }
            else if (attributeAmount >= 7)
            {
                return -3;
            }
            else if (attributeAmount >= 5)
            {
                return -4;
            }
            else if (attributeAmount >= 3)
            {
                return -5;
            }
            else
            {
                return -6;
            }
        }

        // e.g. how much total experience to reach level 5
        static public int GetTotalExperienceRequired(int level)
        {
            return level * (level - 1) * 500;
        }

        // e.g. how many xp is required from 1 to 2
        static public int GetExperienceToNextLevel(int currLevel)
        {
            return currLevel * 1000;
        }

        /*static public SkillGroupType GetSkillGroup(SkillType skillType)
        {
            switch (skillType)
            {
                case SkillType.Staff:
                case SkillType.Sword:
                case SkillType.Dagger:
                case SkillType.Axe:
                case SkillType.Spear:
                case SkillType.Bow:
                case SkillType.Blaster:
                    return SkillGroupType.Weapon;

                case SkillType.LeatherArmor:
                case SkillType.ChainArmor:
                case SkillType.PlateArmor:
                case SkillType.Shield:
                    return SkillGroupType.Armor;

                case SkillType.FireMagic:
                case SkillType.WaterMagic:
                case SkillType.AirMagic:
                case SkillType.EarthMagic:
                case SkillType.MindMagic:
                case SkillType.BodyMagic:
                case SkillType.DragonAbility:
                case SkillType.DarkElfAbility:
                case SkillType.VampireAbility:
                case SkillType.DarkMagic:
                case SkillType.LightMagic:
                    return SkillGroupType.Magic;

                default:
                    return SkillGroupType.Misc;
            }
        }*/

        // e.g. currentClass == Knight ? return Champion
        //      currentClass == Necromancer ? return Lich
        // When currentClass is already the final promotion, CharacterClass.None is returned
        static public CharacterClass GetNextClassPromotion(CharacterClass currentClass)
        {
            switch (currentClass)
            {
                case CharacterClass.Cleric: return CharacterClass.Priest;
                case CharacterClass.DarkElf: return CharacterClass.Patriarch;
                case CharacterClass.Dragon: return CharacterClass.GreatWyrm;
                case CharacterClass.Knight: return CharacterClass.Champion;
                case CharacterClass.Minotaur: return CharacterClass.MinotaurLord;
                case CharacterClass.Troll: return CharacterClass.WarTroll;
                case CharacterClass.Vampire: return CharacterClass.Nosferatu;
                case CharacterClass.Necromancer: return CharacterClass.Lich;
            }

            return CharacterClass.None;
        }

        static public float GetAttributeAgingMultiplier(int age, CharAttribute attribute)
        {
            switch (attribute)
            {
                case CharAttribute.Might:
                    if (age >= 150)
                    {
                        return 0.1f;
                    }
                    else if (age >= 100)
                    {
                        return 0.4f;
                    }
                    else if (age >= 50)
                    {
                        return 0.75f;
                    }
                    else
                    {
                        return 1.0f;
                    }

                case CharAttribute.Intellect:
                    if (age >= 150)
                    {
                        return 0.1f;
                    }
                    else if (age >= 100)
                    {
                        return 1.0f;
                    }
                    else if (age >= 50)
                    {
                        return 1.5f;
                    }
                    else
                    {
                        return 1.0f;
                    }

                // This was "Willpower" in MM7 - im not sure if a person between 50 and 100 years should
                // have better personality ?
                case CharAttribute.Personality:
                    if (age >= 150)
                    {
                        return 0.1f;
                    }
                    else if (age >= 100)
                    {
                        return 1.0f;
                    }
                    else if (age >= 50)
                    {
                        return 1.5f;
                    }
                    else
                    {
                        return 1.0f;
                    }

                case CharAttribute.Endurance:
                    if (age >= 150)
                    {
                        return 0.1f;
                    }
                    else if (age >= 100)
                    {
                        return 0.4f;
                    }
                    else if (age >= 50)
                    {
                        return 0.75f;
                    }
                    else
                    {
                        return 1.0f;
                    }

                case CharAttribute.Accuracy:
                    if (age >= 150)
                    {
                        return 0.1f;
                    }
                    else if (age >= 100)
                    {
                        return 0.4f;
                    }
                    else if (age >= 50)
                    {
                        return 1.0f;
                    }
                    else
                    {
                        return 1.0f;
                    }

                case CharAttribute.Speed:
                    if (age >= 150)
                    {
                        return 0.1f;
                    }
                    else if (age >= 100)
                    {
                        return 0.4f;
                    }
                    else if (age >= 50)
                    {
                        return 1.0f;
                    }
                    else
                    {
                        return 1.0f;
                    }

                case CharAttribute.Luck:
                    if (age >= 150)
                    {
                        return 1.0f;
                    }
                    else if (age >= 100)
                    {
                        return 1.0f;
                    }
                    else if (age >= 50)
                    {
                        return 1.0f;
                    }
                    else
                    {
                        return 1.0f;
                    }
            }

            return 1.0f;
        }

        public static StatType CharAttributeToStatBonus(CharAttribute attribute)
        {
            switch (attribute)
            {
                case CharAttribute.Accuracy: return StatType.Accuracy;
                case CharAttribute.Endurance: return StatType.Endurance;
                case CharAttribute.Might: return StatType.Might;
                case CharAttribute.Intellect: return StatType.Intellect;
                case CharAttribute.Personality: return StatType.Personality;
                case CharAttribute.Luck: return StatType.Luck;
                case CharAttribute.Speed: return StatType.Speed;
            }

            return StatType.None;
        }

        public static StatType SkillToAttributeBonus(SkillType skillType)
        {
            switch (skillType)
            {
                case SkillType.Staff: return StatType.Staff;
                case SkillType.Sword: return StatType.Sword;
                case SkillType.Dagger: return StatType.Dagger;
                case SkillType.Axe: return StatType.Axe;
                case SkillType.Spear: return StatType.Spear;
                case SkillType.Bow: return StatType.Bow;
                case SkillType.Mace: return StatType.Mace;
                case SkillType.Blaster: return StatType.Blaster;
                case SkillType.Shield: return StatType.Shield;
                case SkillType.LeatherArmor: return StatType.LeatherArmor;
                case SkillType.ChainArmor: return StatType.ChainArmor;
                case SkillType.PlateArmor: return StatType.PlateArmor;
                case SkillType.FireMagic: return StatType.FireMagic;
                case SkillType.AirMagic: return StatType.AirMagic;
                case SkillType.WaterMagic: return StatType.WaterMagic;
                case SkillType.EarthMagic: return StatType.EarthMagic;
                case SkillType.SpiritMagic: return StatType.SpiritMagic;
                case SkillType.MindMagic: return StatType.MindMagic;
                case SkillType.BodyMagic: return StatType.BodyMagic;
                case SkillType.LightMagic: return StatType.LightMagic;
                case SkillType.DarkMagic: return StatType.DarkMagic;
                case SkillType.DarkElfAbility: return StatType.DarkElfAbility;
                case SkillType.DragonAbility: return StatType.DragonAbility;
                case SkillType.VampireAbility: return StatType.VampireAbility;
                case SkillType.Merchant: return StatType.Merchant;
                case SkillType.RepairItem: return StatType.RepairItem;
                case SkillType.IdentifyItem: return StatType.IdentifyItem;
                case SkillType.IdentifyMonster: return StatType.IdentifyMonster;
                case SkillType.Meditation: return StatType.Meditation;
                case SkillType.Alchemy: return StatType.Alchemy;
                case SkillType.Perception: return StatType.Perception;
                case SkillType.Regeneration: return StatType.Regeneration;
                case SkillType.DisarmTraps: return StatType.DisarmTraps;
                case SkillType.Bodybuilding: return StatType.Bodybuilding;
                case SkillType.Armsmaster: return StatType.Armsmaster;
                case SkillType.Learning: return StatType.Learning;
                case SkillType.Dodging: return StatType.Dodging;
                case SkillType.Unarmed: return StatType.Unarmed;
                case SkillType.Stealing: return StatType.Stealing;
            }

            return StatType.None;
        }

        public static StatType ResistanceToAttributeBonus(SpellElement resistance)
        {
            switch (resistance)
            {
                case SpellElement.Fire: return StatType.FireResistance;
                case SpellElement.Water: return StatType.WaterResistance;
                case SpellElement.Air: return StatType.AirResistance;
                case SpellElement.Earth: return StatType.EarthResistance;
                case SpellElement.Spirit: return StatType.BodyResistance;
                case SpellElement.Mind: return StatType.MindResistance;
                case SpellElement.Body: return StatType.BodyResistance;
            }

            return StatType.None;
        }

        public static SkillType SpellSchoolToSkillType(SpellSchool spellSchool)
        {
            switch (spellSchool)
            {
                case SpellSchool.Air: return SkillType.AirMagic;
                case SpellSchool.Earth: return SkillType.EarthMagic;
                case SpellSchool.Water: return SkillType.WaterMagic;
                case SpellSchool.Fire: return SkillType.FireMagic;
                case SpellSchool.Spirit: return SkillType.SpiritMagic;
                case SpellSchool.Mind: return SkillType.MindMagic;
                case SpellSchool.Body: return SkillType.BodyMagic;
                case SpellSchool.Light: return SkillType.LightMagic;
                case SpellSchool.Dark: return SkillType.DarkMagic;
                case SpellSchool.Dragon: return SkillType.DragonAbility;
                case SpellSchool.Vampire: return SkillType.VampireAbility;
                case SpellSchool.DarkElf: return SkillType.DarkElfAbility;
            }

            return SkillType.None;
        }

        public static SpellSchool SkillTypeToSpellSchool(SkillType skillType)
        {
            switch (skillType)
            {
                case SkillType.AirMagic: return SpellSchool.Air;
                case SkillType.EarthMagic: return SpellSchool.Earth;
                case SkillType.WaterMagic: return SpellSchool.Water;
                case SkillType.FireMagic: return SpellSchool.Fire;
                case SkillType.SpiritMagic: return SpellSchool.Spirit;
                case SkillType.MindMagic: return SpellSchool.Mind;
                case SkillType.BodyMagic: return SpellSchool.Body;
                case SkillType.LightMagic: return SpellSchool.Light;
                case SkillType.DarkMagic: return SpellSchool.Dark;
                case SkillType.DragonAbility: return SpellSchool.Dragon;
                case SkillType.VampireAbility: return SpellSchool.Vampire;
                case SkillType.DarkElfAbility: return SpellSchool.DarkElf;
            }

            return SpellSchool.None;
        }

        static public float ConvertToUnitySpeed(float mmSpeed)
        {
            return mmSpeed / 100.0f;
        }

        static public float ConvertToUnitySize(float mmSizeUnits)
        {
            return mmSizeUnits / 40.0f;
        }
    }
}
