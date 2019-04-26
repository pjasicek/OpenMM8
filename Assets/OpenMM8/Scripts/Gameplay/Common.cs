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

    // This will hold sounds to play + facial expression to make
    public enum CharacterAction
    {

    }

    public enum CharacterExpression
    {
        None = 0,
        Good = 1,
        Cursed = 2,
        Weak = 3,
        Sleep = 4,
        Fear = 5,
        Drunk = 6,
        Insane = 7,
        Poisoned = 8,
        Diseased = 9,
        Paralyzed = 10,
        Unconcious = 11,
        Petrified = 12,
        Idle_1 = 13,
        Idle_2 = 14,
        Idle_3 = 15,
        Idle_4 = 16,
        Idle_5 = 17,
        Idle_6 = 18,
        Idle_7 = 19,
        Idle_8 = 20,
        _21,
        _22,
        _23,
        _24,
        _25,
        _26,
        _27,
        _28,
        Idle_9 = 29,
        Idle_10 = 30,
        _31,
        _32,
        _33,
        DamageReceiveMinor = 34,
        DamageReceiveModerate = 35,
        DamageReceiveMajor = 36,
        WaterWalkOk = 37,
        _38,
        _39,

        // ?

        Scared = 46, // Falling 

        // ?
        Idle_11 = 54,
        Idle_12 = 55,
        Idle_13 = 56,
        Idle_14 = 57,
        Falling = 58, // ??

        // ??

        Dead,
        Eradicated
    }

    public enum CharacterSpeech
    {
        TrapDisarmed = 1,
        DoorIsClosed = 2,
        FailedDisarm = 3,
        ChooseMe = 4,
        BadItem = 5,
        GoodItem = 6,
        CantIdentify = 7,
        ItemRepaired = 8,
        CannotRepairItem = 9,
        IdentifiedWeakMonster = 10,
        IdentifiedStrongMonster = 11,
        CantIdentifyMonster = 12,
        QuickSpellWasSet = 13,
        Hungry = 14,
        SoftInjured = 15,
        Injured = 16,
        FatallyInjured = 17,
        Drunk = 18,
        Insane = 19,
        Poisoned = 20,
        Cursed = 21,
        Fear = 22,
        CannotRestHere = 23,
        NeedMoreGold = 24,
        InventoryFull = 25,
        PotionMixed = 26,
        FailedPotionMixing = 27,
        NeedAKey = 28,
        LearnedSpell = 29,
        CannotLearnSpell = 30,
        CannotEquipItem = 31,
        GoodDay = 32,
        GoodEvening = 34,
        Win = 37,
        Heh = 38,
        LastManStanding = 39,
        HardFightEnded = 40,
        EnteredDungeon = 41,
        Yes = 42,
        Thanks = 43,
        SomeoneWasRude = 44,
        Move = 47
    }

    public enum Condition
    {
        Cursed = 0,
        Weak,
        Sleep,
        Fear,
        Drunk,
        Insane,
        PoisonWeak,
        DiseaseWeak,
        PoisonMedium,
        DiseaseMedium,
        PoisonSevere,
        DiseaseSevere,
        Paralyzed,
        Unconcious,
        Dead,
        Petrified,
        Eradicated,
        Zombie,
        Good
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

        //OfProtection,	// (+10) on all four resistances
        //OfGods,	// (+10) on all 7 stats
        OfCarnage,	// projectile explodes (fireball radius,dmg=weapdmg)
        OfCold,	// (3-4) cold damage
        OfFrost,	// (6-8) cold damage
        OfIce,	// (9-12) cold damage
        OfSparks,	// (2-5) elec damage
        OfLightning,	// (4-10) elec damage
        OfThunderbolts,	// (6-15) elec damage
        OfFire,	// (1-6) fire damage
        OfFlame,	// (2-12) fire damage
        OfInfernos,	// (3-18) fire damage
        OfPoison,	// (5) pois damage
        OfVenom,	// (8) pois damage
        OfAcid,	// (12) pois damage
        Vampiric,	// Heal 20% of damage done on attack
        OfRecovery,	// (-10) pts on recovery from being hit
        //OfImmunity,	// Immune to desease condition
        //OfSanity,	// immune to insanity condition
        //OfFreedom,	// immune to paralize condition
        //OfAntidotes,	// immune to poison condition
        //OfAlarms,	// immune to sleep condition
        //OfMedusa,	// immune to stone condition
        OfForce,	// Increase knockback effect (X10)
        //OfPower,	// (+5) char level
        OfAirMagic,	// 50% increase of spell effect and duration
        OfBodyMagic,	// 50% increase of spell effect and duration
        OfDarkMagic,	// 50% increase of spell effect and duration
        OfEarthMagic,	// 50% increase of spell effect and duration
        OfFireMagic,	// 50% increase of spell effect and duration
        OfLightMagic,	// 50% increase of spell effect and duration
        OfMindMagic,	// 50% increase of spell effect and duration
        OfSpiritMagic,	// 50% increase of spell effect and duration
        OfWaterMagic,	// 50% increase of spell effect and duration
        OfThievery,	// double chance to perform lockpick and steal skill
        OfShielding,	// (1/2)dmg from all missle attacks (not cumulative)
        OfOgreSlaying,	// X 2 damage vs Ogres, Ogre Magi, Trolls, Troll Peasants, and Cyclops
        OfDragonSlaying,	// X 2 damage vs all dragons
        OfDarkness,	// (-20) pts on weapon speed, Vampiric
        //OfDoom,	// (+1) 7stats,hpts,spts,AC,4 resistances
        //OfEarth,	// End (+10), AC (+10), HP (+10)
        //OfLife,	// HP (+10), Regen hpts
        //Rogues,	// Speed (+5), Acc (+5)
        OfTheDragon,	// (10-20) Fire damage, Might (+25)
        //OfTheEclipse,	// SP (+10), Regen spts
        //OfTheGolem,	// End (+15), AC (+5)
        //OfTheMoon,	// Luck (+10), Int (+10)
        //OfThePhoenix,	// Fire Res (+30), Regen hpts
        //OfTheSky,	// Int (+10), Spd (+10), SP (+10)
        //OfTheStars,	// End (+10), Acc (+10)
        //OfTheSun,	// Might(+10), Per (+10)
        //OfTheTroll,	// End (+15), Regen hpts
        //OfTheUnicorn,	// Luck (+15), Regen spts
        //Warriors,	// Might (+5), End (+5)
        //Wizards,	// Intelect (+5), Per (+5)
        //Antique,	// Gold value increased
        Swift,	// (-20) pts on weapon speed
        //Monks,	// +3 to Dodging and Unarmed skill
        //Thieves,	// +3 to Stealing and Disarm skill
        //OfIdentifying,	// +3 to ID Monster and ID Item skill
        OfElementalSlaying,	// x2 damage vs Fire, Earth, Water, and Air elementals
        OfUndeadSlaying,	// x2 damage on Liches, Vampires, Zombies, Skeletons, Ghosts, and Wights
        OfDavid,	// UNUSED x2 damage on all Titans
        //OfPlenty,	// Regenerate 1 hp/x and 1 sp/x while walking, etc.
        //Assasins,	// (5) pois damage and +2 Disarm skill
        //Barbarians,	// (6-8) cold damage and +5 AC
        //OfTheStorm,	// +20 Air Resistance and 1/2 damage from missile attacks
        //OfTheOcean,	// +10 Water Resistance and +2 Alchemy skill
        OfWaterWalking,	// Character with item takes no damage from drowning
        OfFeatherFalling,	// Character with item takes no damage from falling

        // Shared among special item enchants / artefacts
        HpRegeneration,
        SpRegeneration,
        HpDrain,  // HP is being drained from the wearer continously
        SpDrain,// HP is being drained from the wearer continously

        OfSlowTarget,

        FearImmunity,
        DiseaseImmunity,
        InsanityImmunity,
        ParalyzeImmunity,
        PoisonImmunity,
        SleepImmunity,
        StoneImmunity, // Stoned (medusa etc)

        // Artifacts/Relics
        Elderaxe, // 6-12 cold damage
        Volcano,
        Guardian,
        Foulfang,
        Breaker,
        Finality,

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
        None
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

    public enum TreasureLevel
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
