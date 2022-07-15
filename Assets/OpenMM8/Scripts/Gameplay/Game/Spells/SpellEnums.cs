using UnityEngine;
using UnityEditor;

public enum SpellSchool
{
    Fire,
    Air,
    Water,
    Earth,
    Spirit,
    Mind,
    Body,
    Light,
    Dark,
    DarkElf,
    Vampire,
    Dragon,
    None = 10000
}

public enum SpellType
{
    None = 0,

    // Fire school
    Fire_TorchLight,
    Fire_FireBolt,
    Fire_ProtectionFromFire,
    Fire_FireAura,
    Fire_Haste,
    Fire_Fireball,
    Fire_FireSpike,
    Fire_Immolation,
    Fire_MeteorShower,
    Fire_Inferno,
    Fire_Incinerate,

    // Air school
    Air_WizardEye,
    Air_FeatherFall,
    Air_ProtectionFromAir,
    Air_Sparks,
    Air_Jump,
    Air_Shield,
    Air_LightningBolt,
    Air_Invisibility,
    Air_Implosion,
    Air_Fly,
    Air_Startburst,

    // Water school
    Water_Awaken,
    Water_PoisonSpray,
    Water_ProtectionFromWater,
    Water_IceBolt,
    Water_WaterWalk,
    Water_RechargeItem,
    Water_AcidBurst,
    Water_EnchantItem,
    Water_TownPortal,
    Water_IceBlast,
    Water_LloydsBeacon,

    // Earth school
    Earth_Stun,
    Earth_Slow,
    Earth_ProtectionFromEarth,
    Earth_DeadlySwarm,
    Earth_Stoneskin,
    Earth_Blades,
    Earth_StoneToFlesh,
    Earth_RockBlast,
    Earth_Telekinesis,
    Earth_DeathBlossom,
    Earth_MassDistortion,

    // Spirit school
    Spirit_DetectLife,
    Spirit_Bless,
    Spirit_Fate,
    Spirit_TurnUndead,
    Spirit_RemoveCurse,
    Spirit_Preservation,
    Spirit_Heroism,
    Spirit_SpiritLash,
    Spirit_RaiseDead,
    Spirit_SharedLife,
    Spirit_Ressurection,

    // Mind school
    Mind_Telepathy,
    Mind_RemoveFear,
    Mind_ProtectionFromMind,
    Mind_MindBlast,
    Mind_Charm,
    Mind_CureParalysis,
    Mind_Berserk,
    Mind_MassFear,
    Mind_CureInsanity,
    Mind_PsychicShock,
    Mind_Enslave,

    // Body school
    Body_CureWeakness,
    Body_FirstAid,
    Body_ProtectionFromBody,
    Body_Harm,
    Body_Regeneration,
    Body_CurePoison,
    Body_Hammerhands,
    Body_CureDisease,
    Body_ProtectionFromMagic,
    Body_FlyingFist,
    Body_PowerCure,

    // Light school
    Light_LightBolt,
    Light_DestroyUndead,
    Light_DispelMagic,
    Light_Paralyze,
    Light_SummonElemental,
    Light_DayOfTheGods,
    Light_PrismaticLight,
    Light_DayOfProtection,
    Light_HourOfPower,
    Light_Sunray,
    Light_DivineIntervention,

    // Dark school
    Dark_Reanimate,
    Dark_ToxicCloud,
    Dark_VampiricWeapon,
    Dark_ShrinkingRay,
    Dark_Sharpmetal,
    Dark_ControlUndead,
    Dark_PainReflection,
    Dark_DarkGrasp,
    Dark_DragonBreath,
    Dark_Armageddon,
    Dark_Souldrinker,

    // Dark elf abilities
    DarkElf_Glamour,
    DarkElf_TravelersBoon,
    DarkElf_Blind,
    DarkElf_DarkfireBolt,
    DarkElf_UNUSED_5,
    DarkElf_UNUSED_6,
    DarkElf_UNUSED_7,
    DarkElf_UNUSED_8,
    DarkElf_UNUSED_9,
    DarkElf_UNUSED_10,
    DarkElf_UNUSED_11,

    // Vampire abilities
    Vampire_Lifedrain,
    Vampire_Levitate,
    Vampire_Charm,
    Vampire_Mistform,
    Vampire_UNUSED_5,
    Vampire_UNUSED_6,
    Vampire_UNUSED_7,
    Vampire_UNUSED_8,
    Vampire_UNUSED_9,
    Vampire_UNUSED_10,
    Vampire_UNUSED_11,

    // Dragon abilities
    Dragon_Fear,
    Dragon_FlameBlast,
    Dragon_Flight,
    Dragon_WingBuffer,
    Dragon_UNUSED_5,
    Dragon_UNUSED_6,
    Dragon_UNUSED_7,
    Dragon_UNUSED_8,
    Dragon_UNUSED_9,
    Dragon_UNUSED_10,
    Dragon_UNUSED_11,


    // Misc - not spellbook spells but not normal attacks either
    //        all of these should have some projectile
    Misc_Arrow = 1000,
    Misc_Blaster = 1001,
    Misc_DragonBreath = 1002,

    Misc_Disease = 1100,
    Misc_QuestCompleted = 1101,
}

// Buffs affecting the whole party
public enum PartyEffectType
{
    ResistFire,
    ResistWater,
    ResistAir,
    ResistEarth,
    ResistBody,
    ResistMind,
    ProtectionFromMagic,
    DayOfTheGods,
    Shield,
    StoneSkin,
    Torchlight,
    WizardEye,
    Invisibility,
    Immolation,
    Fly,
    WaterWalk,
    Heroism,
    Haste,
    DetectLife,
    FeatherFall,
    Levitate
}

// Buffs affecting specific player - cummulative with PartyEffect if applicable
public enum PlayerEffectType
{
    ResistFire,
    ResistWater,
    ResistAir,
    ResistEarth,
    ResistBody,
    ResistMind,
    Bless,
    Fate,
    Hammerhands,
    Haste,
    Heroism,
    PainReflection,
    Preservation,
    Regeneration,
    Shield,
    Stoneskin,
    Accuracy,
    Endurance,
    Intelligence,
    Luck,
    Might,
    Personality,
    Speed,
    WaterWalk
}