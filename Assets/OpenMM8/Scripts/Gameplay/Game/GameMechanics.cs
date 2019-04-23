using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    // Thanks to: https://grayface.github.io/mm/mechanics/
    static public class GameMechanics
    {
        static public AttackResult DamageFromPlayerToNpc(AttackInfo hitInfo,
            Dictionary<SpellElement, int> npcResistances,
            int npcArmorClass)
        {
            AttackResult result = new AttackResult();

            float chanceBeingHit = (float)(15 + hitInfo.AttackMod * 2) / (float)(30 + hitInfo.AttackMod * 2 + npcArmorClass);
            if (UnityEngine.Random.Range(0.0f, 1.0f) > chanceBeingHit)
            {
                result.Type = AttackResultType.Miss;
                result.DamageDealt = 0;
                return result;
            }
            else
            {
                result.Type = AttackResultType.Hit;
            }

            //float damageDealt = (Gaussian.Random() * (hitInfo.MaxDamage - hitInfo.MinDamage)) + hitInfo.MinDamage;
            float damageDealt = Gaussian.RandomRange(hitInfo.MinDamage, hitInfo.MaxDamage);

            // Apply resistances
            int attackResistance = npcResistances[hitInfo.DamageType];
            damageDealt *= GetResistanceReductionCoeff(hitInfo.DamageType, attackResistance, 0);

            result.DamageDealt = Mathf.RoundToInt(damageDealt);

            return result;
        }

        static public AttackResult DamageFromNpcToPlayer(AttackInfo hitInfo,
            Dictionary<SpellElement, int> playerResistances,
            int playerArmorClass,
            int playerLuck)
        {
            AttackResult result = new AttackResult();

            float chanceBeingHit = (float)(5 + hitInfo.SourceLevel * 2) / (float)(10 + hitInfo.SourceLevel * 2 + playerArmorClass);
            if (UnityEngine.Random.Range(0.0f, 1.0f) > chanceBeingHit)
            {
                result.Type = AttackResultType.Miss;
                result.DamageDealt = 0;
                return result;
            }
            else
            {
                result.Type = AttackResultType.Hit;
            }

            float damageDealt = Gaussian.RandomRange(hitInfo.MinDamage, hitInfo.MaxDamage);// (Gaussian.Random() * (hitInfo.MaxDamage - hitInfo.MinDamage)) + hitInfo.MinDamage;

            // Apply resistances
            int attackResistance = playerResistances[hitInfo.DamageType];
            damageDealt *= GetResistanceReductionCoeff(hitInfo.DamageType, attackResistance, playerLuck);

            result.DamageDealt = Mathf.RoundToInt(damageDealt);

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
    }
}
