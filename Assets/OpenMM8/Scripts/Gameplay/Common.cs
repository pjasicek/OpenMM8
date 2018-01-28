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

    // Enums

    public enum GameState
    {
        Menu,
        Ingame,
        IngamePaused
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
        Unconsious,
        Dead,
        Eradicated,
        Stoned,
        Paralyzed
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

    public enum AttackResult
    {
        Kill,
        Hit,
        Resist,
        Miss,
        None
    }

    public enum SpellResult
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
