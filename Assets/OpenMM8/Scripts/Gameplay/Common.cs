using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

namespace Assets.OpenMM8.Scripts.Gameplay
{
    // Events
    public delegate AttackResult AttackReceived(AttackInfo hitInfo, GameObject source);
    public delegate SpellResult SpellReceived(SpellInfo hitInfo, GameObject source);

    // Global constants
    public static class Constants
    {
        public static float MeleeRangeDistance = 3.0f;
        public static Vector3 CrosshairScreenRelPos = new Vector3(0.5f, 0.595f, 0.0f);

        public static float PC_WidthDelta = 97;
        public static Vector2Int PC1_EmptySlot = new Vector2Int(-272, -432);
        public static Vector2Int PC1_Avatar = new Vector2Int(-269, -428);
        public static Vector2Int PC1_SelectionRing = new Vector2Int(-269, -428);
        public static Vector2Int PC1_AgroStatus = new Vector2Int(-300, -466);
        public static Vector2Int PC1_HealthBar = new Vector2Int(-236, -453);
        public static Vector2Int PC1_ManaBar = new Vector2Int(-232, -453);
        public static Vector2Int PC1_HealthManaFrame = new Vector2Int(-234, -453);
    }

    // Enums

    public enum GameState
    {
        Menu,
        Ingame,
        IngamePaused,
    }

    public enum MapType
    {
        Indoor,
        Outdoor,
        None
    }

    public enum SkillType
    {
        Staff,
        Sword,
        Dagger,
        Axe,
        Spear,
        Bow,
        Mace,
        Blaster,
        Shield,
        LeatherArmor,
        ChainArmor,
        PlateArmor,
        FireMagic,
        AirMagic,
        WaterMagic,
        EarthMagic,
        SpiritMagic,
        MindMagic,
        BodyMagic,
        LightMagic,
        DarkMagic,
        DarkElfAbility,
        DragonAbility,
        VampireAbility,
        Merchant,
        RepairItem,
        IdentifyItem,
        IdentifyMonster,
        Meditation,
        Alchemy,
        Perception,
        Regeneration,
        DisarmTraps,
        Bodybuilding,
        Armsmaster,
        Learning,
        Dodging,
        Unarmed,
        Stealing,
        None
    }

    public enum SkillGroupType
    {
        Weapon,
        Armor,
        Magic,
        Misc,
        None
    }

    public enum SkillMastery
    {
        None,
        Normal,
        Expert,
        Master,
        Grandmaster
    }

    public enum SpellElement
    {
        Fire,
        Air,
        Water,
        Earth,
        Mind,
        Body,
        Spirit,
        Dark,
        Light,
        Physical,
        None
    }

    public enum CharAttribute
    {
        Might,
        Intellect,
        Personality,
        Endurance,
        Accuracy,
        Speed,
        Luck,
        None
    }

    public enum Condition
    {
        Good,
        Weak,
        Insane,
        Poisoned,
        Diseased,
        Unconsious,
        Dead,
        Eradicated,
        Stoned,
        Paralyzed,
        Sleeping,
        Drunk
    }

    public enum CharacterClass
    {
        Cleric = 4,
        Priest = 5,
        DarkElf = 8,
        Patriarch = 9,
        Dragon = 10,
        GreatWyrm = 11,
        Knight = 16,
        Champion = 19,
        Minotaur = 20,
        MinotaurLord = 21,
        Troll = 38,
        WarTroll = 39,
        Vampire = 40,
        Nosferatu = 41,
        Necromancer = 44,
        Lich = 45,
        None
    }

    // NOTUSED
    public enum CharacterType
    {
        None = -1,
        Knight_1 = 1,
        KnightFemale_1,
        Knight_2,
        KnightFemale_2,
        Cleric_1,
        ClericFemale_1,
        Cleric_2,
        ClericFemale_2,
        Necromancer_1,
        NecromancerFemale_1,
        Necromancer_2,
        NecromancerFemale_2,
        Vampire_1,
        VampireFemale_1,
        Vampire_2,
        VampireFemale_2,
        DarkElf_1,
        DarkElfFemale_1,
        DarkElf_2,
        DarkElfFemale_2,
        Minotaur_1,
        Minotaur_2,
        Troll_1,
        Troll_2,
        Dragon_1,
        Dragon_2,
        Lich_1,
        LichFemale_1,
    }

    // This contains every attribute - CharAttribute, SkillType, SpellElement
    // Mapping from this to actual attribute needs to be done manually
    public enum StatType
    {
        // Attributes
        Might,
        Intellect,
        Personality,
        Endurance,
        Accuracy,
        Speed,
        Luck,
        HitPoints,
        SpellPoints,
        ArmorClass,
        FireResistance,
        AirResistance,
        WaterResistance,
        EarthResistance,
        MindResistance,
        BodyResistance,
        Age,
        Level,
        RecoveryTime,

        // Skills
        Staff,
        Sword,
        Dagger,
        Axe,
        Spear,
        Bow,
        Mace,
        Blaster,
        Shield,
        LeatherArmor,
        ChainArmor,
        PlateArmor,
        FireMagic,
        AirMagic,
        WaterMagic,
        EarthMagic,
        SpiritMagic,
        MindMagic,
        BodyMagic,
        LightMagic,
        DarkMagic,
        DarkElfAbility,
        DragonAbility,
        VampireAbility,
        Merchant,
        RepairItem,
        IdentifyItem,
        IdentifyMonster,
        Meditation,
        Alchemy,
        Perception,
        Regeneration,
        DisarmTraps,
        Bodybuilding,
        Armsmaster,
        Learning,
        Dodging,
        Unarmed,
        Stealing,

        // Other "Stats"
        MeleeAttack,
        MeleeDamageBonus,
        MeleeDamageMin,
        MeleeDamageMax,

        RangedAttack,
        RangedDamageBonus,
        RangedDamageMin,
        RangedDamageMax,

        // Special bonuses - they do not need to have amount, they are rather 0 or 1
        //   (0 if they are not present on the item, 1 if they are)
        // They should not be stackable I think
        OfFireMagic,
        OfWaterMagic,
        OfAirMagic,
        OfEarthMagic,
        OfSpiritMagic,
        OfBodyMagic,
        OfMindMagic,

        None
    }

    public enum SpellEffectType
    {
        None
    }



    public enum CharacterRace
    {
        Human,
        Vampire,
        DarkElf,
        Minotaur,
        Troll,
        Dragon,
        Undead,
        Elf,
        Goblin,
        None
    }

    public enum ItemType
    {
        WeaponOneHanded,
        WeaponTwoHanded,
        Wand,
        Missile,
        Armor,
        Shield,
        Helmet,
        Belt,
        Cloak,
        Gauntlets,
        Boots,
        Ring,
        Amulet,
        Gem,
        Gold,
        Reagent,
        Bottle,
        SpellScroll,
        SpellBook,
        Misc,
        Ore,
        MessageScroll,
        NotAvailable,
        None
    }

    public enum EquipSlot
    {
        OffHand,  // Left hand
        MainHand, // Right hand
        Bow,
        Armor,
        Helmet,
        Boots,
        Belt,
        Cloak,
        Gauntlets,
        Necklace,
        Ring_1,
        Ring_2,
        Ring_3,
        Ring_4,
        Ring_5,
        Ring_6,
    }

    public enum ItemSkillGroup
    {
        Sword,
        Dagger,
        Axe,
        Spear,
        Bow,
        Mace,
        Club,
        Staff,
        Leather,
        Chain,
        Plate,
        Shield,
        Misc
    }

    public enum SpecialEnchantType
    {
        None = 0,
        OfProtection = 1,
        OfGods,
        OfCarnage,
        OfCold,
        OfFrost,
        OfIce,
        OfSparks,
        OfLightning,
        OfThunderbolts,
        OfFire,
        OfFlame,
        OfInfernos,
        OfPoison,
        OfVenom,
        OfAcid,
        Vampiric,
        OfRecovery,
        OfImmunity,
        OfSanity,
        OfFreedom,
        OfAntidotes,
        OfAlarms,
        OfMedusa,
        OfForce,
        OfPower,
        OfAirMagic,
        OfBodyMagic,
        OfDarkMagic,
        OfEarthMagic,
        OfFireMagic,
        OfLightMagic,
        OfMindMagic,
        OfSpiritMagic,
        OfWaterMagic,
        OfThievery,
        OfShielding,
        OfRegeneration,
        OfMana,
        OfOgreSlaying,
        OfDragonSlaying,
        OfDarkness,
        OfDoom,
        OfEarth,
        OfLife,
        Rogues,
        OfTheDragon,
        OfTheEclipse,
        OfTheGolem,
        OfTheMoon,
        OfThePhoenix,
        OfTheSky,
        OfTheStars,
        OfTheSun,
        OfTheTroll,
        OfTheUnicorn,
        Warriors,
        Wizards,
        Antique,
        Swift,
        Monks,
        Thieves,
        OfIdentifying,
        OfElementalSlaying,
        OfUndeadSlaying,
        OfDavid,
        OfPlenty,
        Assasins,
        Barbarians,
        OfTheStorm,
        OfTheOcean,
        OfWaterWalking,
        OfFeatherFalling,
        Max
    }

    public enum ItemInteractResult
    {
        Equipped,
        CannotEquip,
        Learned,
        CannotLearn,
        AlreadyLearned,
        Consumed,
        Casted,
        Read,
        Invalid
    }

    public enum PlayerState
    {
        Normal,
        Weak,
        Cursed,
        Asleep,
        Afraid,
        Drunk,
        Insane,
        Piosoned,
        Diseased,
        Paralyzed,
        Unconscious,
        Dead,
        Stoned,
        Eradicated,
        Zombie
    }

    public enum AgroState
    {
        Safe, // Green
        HostileNearby, // Yellow
        HostileClose // Red
    }

    public enum AttackResultType
    {
        Kill,
        Hit,
        Resist,
        Miss,
        None
    }

    public enum SpellResultType
    {
        Hit,
        Resist,
        Kill,
        None
    }

    public enum ItemLevel
    {
        L1,
        L2,
        L3,
        L4,
        L5,
        L6,
        None
    }
    public enum NpcAgressivityType
    {
        Wimp,
        Normal,
        Agressive,
        Suicidal
    }
}
