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
        DarkElf,
        Dragon,
        Vampire,
        Trade,
        RepairItem,
        IdentifyItem,
        IdentifyMonster,
        Meditation,
        Alchemy,
        Perception,
        Regeneration,
        DisarmTrap,
        Bodybuilding,
        Armsmaster,
        Learning
    }

    public enum SkillMastery
    {
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

    public enum Attribute
    {
        Might,
        Intellect,
        Personality,
        Endurance,
        Accuracy,
        Speed,
        Luck
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

    public enum Class
    {
        DarkElf,
        Knight,
        Troll,
        Vampire,
        Minotaur,
        Dragon,
        Necromancer,
        Cleric,
        None
    }

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

    public enum EquipType
    {
        WeaponOneHanded,
        WeaponTwoHanded,
        WeaponDualWield,
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
        NotAvailable
    }

    public enum SkillGroup
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

    public enum ItemInteractResult
    {
        Equipped,
        CannotEquip,
        Learned,
        CannotLearn,
        AlreadyLearned,
        Consumed,
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

    public enum LootItemLevel
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
