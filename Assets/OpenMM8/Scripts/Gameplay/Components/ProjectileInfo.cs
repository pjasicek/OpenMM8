using Assets.OpenMM8.Scripts.Gameplay;
using Assets.OpenMM8.Scripts.Gameplay.Data;
using Assets.OpenMM8.Scripts.Gameplay.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class ProjectileInfo
{
    // null = No particular owner, e.g. random meteor shower to crater
    // Character = Character spawned this projectile
    // Monster = Monster spawned this projectile
    public object Shooter = null;

    // null = No particular target
    // PlayerParty = party is the target
    // Monster = Monster is the target
    public object Target = null;

    public Transform ShooterTransform;
    public Vector3 TargetPosition;
    //public Vector3 TargetDirection;

    // Display data of the projectile - CANNOT be null
    public ObjectDisplayData DisplayData;

    // Not all projectiles have impact object - can be null
    public ObjectDisplayData ImpactObject;

    // Item with which this projectile was shot with - can be null
    public Item ContainingItem;

    // Either this
    public SpellType SpellType = SpellType.None;
    public SkillMastery SkillMastery = SkillMastery.None;
    public int SkillLevel = 0;

    // Or this
    public int DamageDiceRolls = 0;
    public int DamageDiceSides = 0;
    public int DamageBonus = 0;

    // Or if shooter is monster also this
    public MonsterAttackType MonsterAttackType = MonsterAttackType.None;
}