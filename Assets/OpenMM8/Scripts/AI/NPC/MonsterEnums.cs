using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum MonsterState
{
    Standing,
    Walking,
    AttackingMelee,
    AttackingRanged1,
    AttackingRanged2,
    AttackingRanged3,
    AttackingRanged4,
    Dying,
    Dead,
    Pursuing,
    Fidgeting,
    Interacting,
    Fleeing,
    Stunned,
    Stoned,
    Paralyzed,
    Removed,
    Resurrected,
    Summoned,
    Disabled
}

public enum MonsterAnimationType
{
    Standing,
    Walking,
    AttackMelee,
    AttackRanged,
    Gothit,
    Dying,
    Dead,
    Bored
}

[Flags]
public enum MonsterFlags
{
    Unknown_1 = 0x1,
    Unknown_2 = 0x2,
    Unknown_3 = 0x4,
    Unknown_4 = 0x8,
    Unknown_5 = 0x10,
    Unknown_6 = 0x20,
    Unknown_7 = 0x40,
    StandInQueue = 0x80, // ??
    Unknown_9 = 0x100,
    Unknown_10 = 0x200,
    InUpdateRange = 0x400,
    // ...
    ActorNearby = 0x8000, // How is this different from @InUpdateRange ?
    Unknown_11 = 0x10000,
    // ...
    Fleeing = 0x00020000,
    Aggressor = 0x00080000,
    Animating = 0x00200000,
    HoldingItem = 0x00800000,
    Hostile = 0x01000000,
}

public enum MonsterBuffType
{
    Charm = 1,
    Summoned = 2,
    Shrink = 3,
    Afraid = 4,
    Stoned = 5,
    Paralyzed = 6,
    Slowed = 7,
    HalvedArmorClass = 8,
    Berserk = 9,
    Distortion = 10,
    Fate = 11,
    Enslaved = 12,
    DayOfProtection = 13,
    HourOfPower = 14,
    Shield = 15,
    Stoneskin = 16,
    Bless = 17,
    Heroism = 18,
    Haste = 19,
    PainReflection = 20,
    Hammerhands = 21
}

public enum HostilityType
{
    Friendly = 0,
    HostileClose = 1,
    HostileShort = 2,
    HostileMedium = 3,
    HostileLong = 4
}

public enum MonsterAggresivityType
{
    Wimp,
    Normal,
    Agressive,
    Suicidal
}

public enum MonsterMoveType
{
    Stationary,
    Short,
    Medium,
    Long,
    Global,
    Free
}

public enum MonsterAttackType
{
    Attack1,
    Attack2,
    Spell1,
    Spell2,
    Special,
    None
}

public enum SpecialAbilityType
{
    None,
    Shot,
    Summon,
    Explode
}

[Flags]
public enum AttackPreferenceMask
{
    None = 0x0,

    // Class
    ClassCleric      = 0x1, // c
    ClassDarkElf     = 0x2,
    ClassDragon      = 0x4, 
    ClassKnight      = 0x8, // k
    ClassMinotaur    = 0x10,
    ClassTroll       = 0x20,
    ClassVampire     = 0x40,
    ClassNecromancer = 0x80, // n

    // Gender
    GenderMale   = 0x1000,   // x
    GenderFemale = 0x2000,   // o

    // Race
    RaceVampire  = 0x010000, // v
    RaceDarkElf  = 0x020000, // de
    RaceMinotaur = 0x040000, // m
    RaceTroll    = 0x080000, // t
    RaceDragon   = 0x100000, // d
    RaceUndead   = 0x200000, // u
    RaceElf      = 0x400000, // e
    RaceGoblin   = 0x800000  // g
}

/*
 * Human,
        Vampire,
        DarkElf,
        Minotaur,
        Troll,
        Dragon,
        Undead,
        Elf,
        Goblin,
        None
*/