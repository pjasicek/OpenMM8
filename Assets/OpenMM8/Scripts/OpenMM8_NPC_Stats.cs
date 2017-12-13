using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMM8_NPC_Stats : MonoBehaviour
{
    public enum AIType { Coward, Normal, Aggressive, Suicidal };
    public enum MovementSpeedType { Low, Medium, High };
    public enum DamageType { Physical, Fire, Air, Water, Earth, Mind, Spirit, Body, Light, Dark, True };
    public enum MissileType { None, Arrow, FireArrow, Fire, Water, Earth, Air };
    public enum ItemLevel { L1, L2, L3, L4, L5, L6, L7 };
    public enum ItemType { Sword, Club, Axe, Spear, Dagger, Bow, Wand, Ring, Amulet, Gauntlets, Boots, Leather, Chain, Plate, Ore, Gem, Potion };
    public enum SkillMastery { Normal, Expert, Master, Grandmaster };

    public struct AttackDef
    {
        float useChance; // 100% if it is MainAttack
        float minDamage;
        float maxDamage;
        DamageType damageType;
        MissileType missileType;
    }

    public struct SpellAttackDef
    {
        float useChance;
        string spellName;
        SkillMastery skillMastery;
        int skillLevel;
    }

    public struct TreasureDropDef
    {
        int minGold;
        int maxGold;
        float itemChance;
        ItemLevel itemLevel;
        ItemType itemType;
    }

    public struct CombatBonusDef
    {
        /*TODO*/
        int type;
        int level;
        int multiplier;
    }

    public struct RestistanceDef
    {
        DamageType damageResistType;
        int amount;
    }

    //public string m_Name;

    public int m_Level;
    public float m_Health;
    public float m_ArmorClass;
    public bool m_CanFly;
    public /*TODO*/ MovementSpeedType m_MovementSpeedType;
    public /*TODO*/ int m_PreferredTargetClass;
    public int m_Experience;
    public TreasureDropDef m_TreasureDrop;
    public bool m_DropsQuestItem;
    public /*TODO*/ int m_MoveType;
    public AIType m_AIType;
    public /*TODO*/ int m_HostilityType;

    public float m_Speed;
    public /*???TODO???*/ int m_Rec;
    public /*TODO*/ int m_PreferredTarget;
    public /*TODO*/ CombatBonusDef m_CombatBonus;

    // Attacks
    public AttackDef m_MainAttack;
    public AttackDef m_SecondaryAttack;
    public RestistanceDef[] m_ResistanceMap;/* =
    {
        { DamageType.Physical, 0 },
        { DamageType.Fire, 0 },
        { DamageType.Air, 0 },
        { DamageType.Water, 0 },
        { DamageType.Earth, 0 },
        { DamageType.Mind, 0 },
        { DamageType.Spirit, 0 },
        { DamageType.Body, 0 },
        { DamageType.Light, 0 },
        { DamageType.Dark, 0 },
    };*/

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
