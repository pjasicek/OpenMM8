﻿using UnityEngine;
using System.Collections;

// This will hold sounds to play + facial expression to make
public enum CharacterReaction
{
    None = 0,
    SmallMonsterKilled = 1,
    BigMonsterKilled = 2,
    PlaceIsClosed = 3,
    OpenChestSuccess = 4,
    OpenChestFail = 5,
    AvoidTrapDamage = 6,
    WeakItem = 7,
    GoodItem = 8,
    CannotIdentifyItem = 9,
    RepairedItem = 10,
    CannotRepairItem = 11,
    SetQuickSpell = 12,
    CannotRestHere = 13,
    SkillUp = 14,
    NoInventoryRoom = 15,
    PotionMixOk = 16,
    PotionMixFail = 17,
    LockedNeedAKey = 18,
    Strained = 19,
    CannotLearnThat = 20,
    LearnOk = 21,
    GoodDay_1 = 22,
    GoodEvening = 23,
    DamagedOww = 24,
    Weakened = 25,
    Feared = 26,
    Poisoned = 27,
    Diseased = 28,
    Insane = 29,
    Cursed = 30,
    Drunk = 31,
    Unconcious = 32,
    Dead = 33,
    Petrified = 34,
    Eradicated = 35,
    SmileGood = 36,
    ReadScroll = 37,
    NotEnoughGold = 38,
    CannotEquipItem = 39,
    ItemBrokenOrStolen = 40,
    GotManaDrained = 41,
    Aged = 42,
    FailedToEnchantItem = 43,
    ReceivedDamage = 44,
    Unknown_45 = 45,
    EnterDungeon = 46,
    LeaveDungeonInCombat = 47,
    ModeratelyInjured = 48,
    CastedSpell = 49,
    ShotBow = 50,
    MonsterKilledGeneric = 51,
    MissedAttack = 52,
    Beg = 53,
    BegFail = 54,
    Unknown_55 = 55,
    ThreatFail = 56,
    SmileHuge = 57,
    BribeFail = 58,
    NpcDontTalk = 59,
    FoundGold = 60,
    HiredNpc = 61,
    Unknown_62 = 62,
    LookUp = 63,
    LookDown = 64,
    YellMove = 65,
    FallingScrem = 66,
    HaveMoreFoodThanYou = 67,
    Unknown_68 = 68,
    Unknown_69 = 69,
    Unknown_70 = 70,
    TravelByCarriage = 71,
    TravelByBoat = 72,
    ShopIdentifiedItem = 73,
    ShopRepairedItem = 74,
    ShopBoughtItem = 75,
    ShopItemAlreadyRepairedOrIdentified = 76,
    ShopSoldItem = 77,
    ShopLearnedSkill = 78,
    ShopWrongActionOnItem = 79,
    ThatWasRude = 80,
    ShopStoredGoldInBank = 81,
    ShopHealedInTemple = 82,
    ShopDonatedInTemple = 83,
    GoodDayHouse = 84,
    JoinedGuild_1 = 85,
    JoinedGuild_2 = 86,
    TrainedToNextLevel = 87,
    Unknown_88 = 88,
    Unknown_89 = 89,
    Unknown_90 = 90,
    ReceivedTemporaryStatBonus = 91,
    ReceivedPermanentStatBonus = 92,
    QuestDone = 93,
    Unknown_94 = 94,
    Unknown_95 = 95,
    ReceivedAward = 96,
    Unknown_97 = 97,
    Unknown_98 = 98,
    BecameZombie = 99,
    Unknown_100 = 100,
    Unknown_101 = 101,
    PickMe = 102,
    WokeUp = 103,
    IdentifyWeakMonster = 104,
    IdentifyStrongMonster = 105,
    CannotIdentifyMonster = 106,
    LastManStanding = 107,
    Hungry = 108,
    FatallyInjured = 109,
    Unknown_110 = 110
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
    _40,
    _41,
    _42,
    _43,
    _44,
    _45,
    Scared = 46, // Falling 
    _47,
    _48,
    _49,
    _50,
    _51,
    _52,
    _53,
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