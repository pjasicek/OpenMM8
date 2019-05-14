using Assets.OpenMM8.Scripts.Gameplay;
using Assets.OpenMM8.Scripts.Gameplay.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public partial class Monster
{
    //=============================================================================================
    // AI
    //=============================================================================================

    public void AI_RandomMove(Transform target, float radius, float actionLength)
    {
        if (UnityEngine.Random.Range(0, 100) < 25)
        {
            AI_StandWhile();
            return;
        }

        if (AIState != MonsterState.Walking)
        {
            SetNavMeshAgentEnabled(true);

            //radius = Mathf.Sqrt(radius);
            CurrentDestination = InitialPosition + new Vector3(
                UnityEngine.Random.Range(-radius * 0.5f, radius * 0.5f),
                0,
                UnityEngine.Random.Range(-radius * 0.5f, radius * 0.5f));
            NavMeshAgent.ResetPath();

            NavMeshAgent.SetDestination(CurrentDestination);

            Vector3 direction = (CurrentDestination - transform.position).normalized;
            transform.rotation = Quaternion.LookRotation(direction);

            CurrentActionLength = 4.0f + UnityEngine.Random.Range(0.0f, 1.5f);
            CurrentActionTime = 0.0f;

            AIState = MonsterState.Walking;
            UpdateAnimation();

            if (UnityEngine.Random.Range(0, 100) < 2)
            {
                SoundMgr.PlaySoundById(SoundIdFidget, AudioSource);
            }
        }
        else
        {
            AI_Stand(transform.forward, 2.0f + UnityEngine.Random.Range(0.0f, 2.0f));
        }
    }

    public void AI_StandWhile()
    {
        CurrentActionLength = 1.0f + UnityEngine.Random.Range(0.0f, 1.0f);
        CurrentActionTime = 0.0f;
        AIState = MonsterState.Standing;
        UpdateAnimation();

        SetNavMeshAgentEnabled(false);
    }

    public void AI_Stand(Vector3 lookDirection, float timeLength = 0.0f)
    {
        if (timeLength == 0.0f)
        {
            timeLength = 2.0f + UnityEngine.Random.Range(0.0f, 2.0f);
        }

        CurrentActionLength = timeLength;
        CurrentActionTime = 0.0f;
        transform.rotation = Quaternion.LookRotation(lookDirection);
        AIState = MonsterState.Standing;
        UpdateAnimation();

        SetNavMeshAgentEnabled(false);
    }

    public void AI_Bored(Vector3 lookDirection)
    {
        CurrentActionLength = AnimationFidget.TotalAnimationLengthSeconds;
        CurrentActionTime = 0.0f;

        if (SpriteRenderer.isVisible)
        {
            AIState = MonsterState.Fidgeting;
            transform.rotation = Quaternion.LookRotation(lookDirection);
            UpdateAnimation();

            if (UnityEngine.Random.Range(0, 100) < 5)
            {
                SoundMgr.PlaySoundById(SoundIdFidget, AudioSource);
            }

            SetNavMeshAgentEnabled(false);
        }
        else
        {
            // If not visible on camera, just stand
            AI_Stand(lookDirection, CurrentActionLength);
        }
    }

    public void AI_FaceObject(Transform targetTransform)
    {
        Vector3 lookDirection = (targetTransform.position - transform.position).normalized;


        if (UnityEngine.Random.Range(0, 100) >= 5)
        {
            AIState = MonsterState.Interacting;
            CurrentActionLength = 2.0f;
            CurrentActionTime = 0.0f;
            transform.rotation = Quaternion.LookRotation(lookDirection);
            UpdateAnimation();

            SetNavMeshAgentEnabled(false);
        }
        else
        {
            AI_Bored(lookDirection);
        }
    }

    public void AI_StandOrBored(Vector3 lookDirection, float duration)
    {
        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            AI_Bored(lookDirection);
        }
        else
        {
            AI_Stand(lookDirection, duration);
        }
    }

    public void AI_Flee(Transform aggressor, float duration = 0.0f)
    {
        if (!CanAct())
        {
            return;
        }

        if (aggressor == null)
        {
            Debug.LogError("No aggressor yet have to flee");
            return;
        }

        Vector3 fleeDirection = (aggressor.transform.position - transform.position).normalized;

        float randRotMod = UnityEngine.Random.Range(-20.0f, 20.0f);
        fleeDirection = Quaternion.AngleAxis(randRotMod, Vector3.up) * fleeDirection;

        SetNavMeshAgentEnabled(true);
        CurrentDestination = transform.position - fleeDirection * 6.0f;
        NavMeshAgent.SetDestination(CurrentDestination);

        Vector3 lookDirection = (CurrentDestination - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(lookDirection);

        if (duration > 2.0f || duration == 0.0f)
        {
            duration = 2.0f;
        }

        CurrentActionLength = duration;
        CurrentActionTime = 0.0f;
        AIState = MonsterState.Fleeing;
        UpdateAnimation();
    }

    public void AI_PursueKiteRanged(Transform target, float duration)
    {
        float distanceSqr = (target.transform.position - transform.position).sqrMagnitude;
        Vector3 pursueDirection = (target.transform.position - transform.position).normalized;

        if (distanceSqr < MAX_MELEE_DISTANCE_SQR)
        {
            CurrentActionLength = duration;
            AI_Stand(pursueDirection, 2.0f);
            return;
        }

        if (Speed == 0.0f)
        {
            AI_Stand(pursueDirection, 2.0f);
            return;
        }

        if (duration == 0.0f)
        {
            duration = 1.0f;
        }
        CurrentActionLength = duration;
        CurrentActionTime = 0.0f;

        // Add noise to pursue direction
        // TODO: Try to find out how much
        float rotationNoiseDegrees = 3.0f;
        if ((UnityEngine.Random.Range(0, MonsterId) % 2) == 0)
        {
            rotationNoiseDegrees *= -1.0f;
        }
        rotationNoiseDegrees += 90.0f;
        pursueDirection = Quaternion.AngleAxis(rotationNoiseDegrees, Vector3.up) * pursueDirection;

        // Make this rather large. TODO: Check if its really okay
        Vector3 pursueDestination = transform.position + pursueDirection * 10.0f;

        SetNavMeshAgentEnabled(true);
        NavMeshAgent.SetDestination(pursueDestination);

        AIState = MonsterState.Pursuing;
        UpdateAnimation();
    }

    // This seems to be the case where monsters with melee attacks are pursuing
    public void AI_PursueToMeleeDirect(Transform target, float duration)
    {
        float distanceSqr = (target.transform.position - transform.position).sqrMagnitude;
        Vector3 pursueDirection = (target.transform.position - transform.position).normalized;

        //Debug.Log("DistanceSqr: " + distanceSqr);
        if (distanceSqr < MAX_MELEE_DISTANCE_SQR)
        {
            AI_StandOrBored(pursueDirection, duration);
            return;
        }

        if (Speed == 0.0f)
        {
            AI_Stand(pursueDirection, duration);
            return;
        }

        SetNavMeshAgentEnabled(true);

        if (duration == 0.0f)
        {
            duration = 2.0f;
        }
        CurrentActionLength = duration;
        CurrentActionTime = 0.0f;

        // Make this rather large. TODO: Check if its really okay
        Vector3 pursueDestination = target.position;

        NavMeshAgent.SetDestination(pursueDestination);

        AIState = MonsterState.Pursuing;
        UpdateAnimation();
    }

    public void AI_PursueToMeleeFarAway(Transform target, float duration)
    {
        float distanceSqr = (target.transform.position - transform.position).sqrMagnitude;
        Vector3 pursueDirection = (target.transform.position - transform.position).normalized;

        if (distanceSqr < MAX_MELEE_DISTANCE_SQR)
        {
            CurrentActionLength = duration;
            AI_Stand(pursueDirection);
            return;
        }

        if (Speed == 0.0f)
        {
            AI_Stand(pursueDirection, duration);
            return;
        }

        if (duration == 0.0f)
        {
            duration = 2.0f;
        }
        CurrentActionLength = duration + UnityEngine.Random.Range(0.0f, duration);
        CurrentActionTime = 0.0f;

        // Add noise to pursue direction
        // TODO: Try to find out how much
        float rotationNoiseDegrees = 45.0f;
        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            rotationNoiseDegrees *= -1.0f;
        }
        pursueDirection = Quaternion.AngleAxis(rotationNoiseDegrees, Vector3.up) * pursueDirection;

        // Make this rather large. TODO: Check if its really okay
        Vector3 pursueDestination = transform.position + pursueDirection * 10.0f;

        SetNavMeshAgentEnabled(true);
        NavMeshAgent.SetDestination(pursueDestination);

        AIState = MonsterState.Pursuing;
        UpdateAnimation();

        if (UnityEngine.Random.Range(0, 100) < 2)
        {
            // ???
            SoundMgr.PlaySoundById(SoundIdGetHit, AudioSource);
        }
    }


    public void AI_MeleeAttack(Transform target)
    {
        if (CanDirectlySeeTarget(target))
        {
            Vector3 direction = (target.position - transform.position).normalized;

            CurrentActionLength = AnimationShoot.TotalAnimationLengthSeconds;
            CurrentActionTime = 0.0f;
            RecoveryTimeLeft = Data.RecoveryTime;

            transform.rotation = Quaternion.LookRotation(direction);
            AIState = MonsterState.AttackingMelee;
            UpdateAnimation();

            SoundMgr.PlaySoundById(SoundIdAttack, AudioSource);

            SetNavMeshAgentEnabled(false);
        }
        else
        {
            AI_PursueKiteRanged(target, 0.5f);
        }
    }

    public void AI_MissileAttack(Transform target, MonsterAttackType missileAttackType)
    {
        if (missileAttackType != MonsterAttackType.Attack1 &&
            missileAttackType != MonsterAttackType.Attack2)
        {
            Debug.LogError("Invalid missileAttackType: " + missileAttackType);
            return;
        }

        if (CanDirectlySeeTarget(target))
        {
            Vector3 direction = (target.position - transform.position).normalized;

            CurrentActionLength = AnimationAttack.TotalAnimationLengthSeconds;
            CurrentActionTime = 0.0f;
            RecoveryTimeLeft = Data.RecoveryTime + CurrentActionLength;

            if (missileAttackType == MonsterAttackType.Attack1)
            {
                AIState = MonsterState.AttackingRanged1;
            }
            else
            {
                AIState = MonsterState.AttackingRanged2;
            }

            SetNavMeshAgentEnabled(false);

            SoundMgr.PlaySoundById(SoundIdAttack, AudioSource);
            transform.rotation = Quaternion.LookRotation(direction);

            //Debug.LogError("RecoveryTime: " + RecoveryTimeLeft);

            UpdateAnimation();


        }
        else
        {
            AI_PursueKiteRanged(target, 0.5f);
        }
    }

    public void AI_SpellAttack(Transform target, MonsterAttackType spellAttackType)
    {
        if (spellAttackType != MonsterAttackType.Spell1 &&
            spellAttackType != MonsterAttackType.Spell2)
        {
            Debug.LogError("Invalid spellAttackType: " + spellAttackType);
            return;
        }

        if (CanDirectlySeeTarget(target))
        {
            Vector3 direction = (target.position - transform.position).normalized;

            CurrentActionLength = AnimationAttack.TotalAnimationLengthSeconds;
            CurrentActionTime = 0.0f;
            RecoveryTimeLeft = Data.RecoveryTime + CurrentActionLength;

            SpellType spell = Data.Spell1_SpellType;
            AIState = MonsterState.AttackingRanged3;
            if (spellAttackType == MonsterAttackType.Spell2)
            {
                spell = Data.Spell2_SpellType;
                AIState = MonsterState.AttackingRanged4;
            }

            SetNavMeshAgentEnabled(false);

            SoundMgr.PlaySoundById(SoundIdAttack, AudioSource);
            transform.rotation = Quaternion.LookRotation(direction);

            // This makes projectile spells casted sooner than other spells
            // What is the reasoning behind this I have no idea
            // It is hard to believe it is a bug
            bool shouldPlayFidgetAnim = ShouldPlayFidgetAttackAnim(spell);
            if (shouldPlayFidgetAnim)
            {
                MonsterState currState = AIState;
                AIState = MonsterState.Fidgeting;
                CurrentActionLength = AnimationFidget.TotalAnimationLengthSeconds;
                UpdateAnimation();

                AIState = currState;
            }
            else
            {
                UpdateAnimation();
            }
        }
        else
        {
            AI_PursueKiteRanged(target, 0.5f);
        }
    }

    public void AI_CastSpell(Transform target, SpellType spellType, int skillLevel, SkillMastery skillMastery)
    {
        SpellData spellData = DbMgr.Instance.SpellDataDb.Get(spellType);
        if (spellData == null)
        {
            Debug.LogError("Failed to retrieve spell data for : " + spellType);
            return;
        }

        ProjectileInfo projectileInfo = null;

        object projectileTarget = null;
        if (target.GetComponent<PlayerParty>() != null)
        {
            projectileTarget = target.GetComponent<PlayerParty>();
        }
        else if (target.GetComponent<Monster>() != null)
        {
            projectileTarget = target.GetComponent<Monster>();
        }

        int buffDurationMins = 0;
        int spellPower = 0;
        PlayerParty party = GameCore.GetParty();

        switch (spellType)
        {
            // Single target projectiles
            case SpellType.Fire_FireBolt:
            case SpellType.Fire_Fireball:
            case SpellType.Fire_Incinerate:
            case SpellType.Air_LightningBolt:
            case SpellType.Water_IceBolt:
            case SpellType.Water_AcidBurst:
            case SpellType.Earth_Blades:
            case SpellType.Earth_RockBlast:
            case SpellType.Mind_MindBlast:
            case SpellType.Mind_PsychicShock:
            case SpellType.Body_Harm:
            case SpellType.Light_LightBolt:
            case SpellType.Dark_ToxicCloud:
            case SpellType.Dark_DragonBreath:
                projectileInfo = new ProjectileInfo();
                projectileInfo.Shooter = this;
                projectileInfo.ShooterTransform = transform;
                projectileInfo.Target = projectileTarget;
                projectileInfo.TargetPosition = target.position;

                projectileInfo.DisplayData = DbMgr.Instance.ObjectDisplayDb.Get(spellData.DisplayObjectId);
                projectileInfo.ImpactObject = DbMgr.Instance.ObjectDisplayDb.Get(spellData.ImpactDisplayObjectId);

                projectileInfo.SpellType = spellType;
                projectileInfo.SkillMastery = skillMastery;
                projectileInfo.SkillLevel = skillLevel;

                SoundMgr.PlaySoundById(spellData.EffectSoundId, AudioSource);
                Projectile.Spawn(projectileInfo);
                break;

            // Buffs

            case SpellType.Fire_Haste:
                if (skillMastery == SkillMastery.Normal || skillMastery == SkillMastery.Expert)
                {
                    buffDurationMins = 60 + skillLevel;
                }
                else if (skillMastery == SkillMastery.Master)
                {
                    buffDurationMins = 60 + skillLevel * 2;
                }
                else if (skillMastery == SkillMastery.Grandmaster)
                {
                    buffDurationMins = 60 + skillLevel * 3;
                }

                BuffMap[MonsterBuffType.Haste].Apply(skillMastery, skillLevel,
                    GameTime.FromCurrentTime(60 * buffDurationMins));
                // Render some monster buff effect
                SoundMgr.PlaySoundById(spellData.EffectSoundId, AudioSource);
                break;

            case SpellType.Air_Shield:
                if (skillMastery == SkillMastery.Normal || skillMastery == SkillMastery.Expert)
                {
                    buffDurationMins = 64 + skillLevel * 5;
                }
                else if (skillMastery == SkillMastery.Master)
                {
                    buffDurationMins = 64 + skillLevel * 15;
                }
                else if (skillMastery == SkillMastery.Grandmaster)
                {
                    buffDurationMins = 64 + skillLevel * 60;
                }

                BuffMap[MonsterBuffType.Shield].Apply(skillMastery, skillLevel,
                    GameTime.FromCurrentTime(60 * buffDurationMins));
                // Render some monster buff effect
                SoundMgr.PlaySoundById(spellData.EffectSoundId, AudioSource);
                break;

            case SpellType.Earth_Stoneskin:
                if (skillMastery == SkillMastery.Normal || skillMastery == SkillMastery.Expert)
                {
                    buffDurationMins = 64 + skillLevel * 5;
                }
                else if (skillMastery == SkillMastery.Master)
                {
                    buffDurationMins = 64 + skillLevel * 15;
                }
                else if (skillMastery == SkillMastery.Grandmaster)
                {
                    buffDurationMins = 64 + skillLevel * 60;
                }

                // AC = skillLevel + 5
                BuffMap[MonsterBuffType.Stoneskin].Apply(skillMastery, skillLevel + 5,
                    GameTime.FromCurrentTime(60 * buffDurationMins));
                // Render some monster buff effect
                SoundMgr.PlaySoundById(spellData.EffectSoundId, AudioSource);
                break;

            case SpellType.Spirit_Bless:
                if (skillMastery == SkillMastery.Normal || skillMastery == SkillMastery.Expert)
                {
                    buffDurationMins = 64 + skillLevel * 5;
                }
                else if (skillMastery == SkillMastery.Master)
                {
                    buffDurationMins = 64 + skillLevel * 15;
                }
                else if (skillMastery == SkillMastery.Grandmaster)
                {
                    buffDurationMins = 64 + skillLevel * 20;
                }

                // Attack Bonus = skillLevel + 5
                BuffMap[MonsterBuffType.Bless].Apply(skillMastery, skillLevel + 5,
                    GameTime.FromCurrentTime(60 * buffDurationMins));
                // Render some monster buff effect
                SoundMgr.PlaySoundById(spellData.EffectSoundId, AudioSource);
                break;

            case SpellType.Spirit_Fate:
                if (skillMastery == SkillMastery.Normal || skillMastery == SkillMastery.Expert)
                {
                    spellPower = 2 * skillLevel + 40;
                }
                else if (skillMastery == SkillMastery.Master)
                {
                    spellPower = 3 * skillLevel + 60;
                }
                else if (skillMastery == SkillMastery.Grandmaster)
                {
                    spellPower = 6 * skillLevel + 120;
                }

                // Attack Bonus = skillLevel + 5
                BuffMap[MonsterBuffType.Fate].Apply(skillMastery, spellPower,
                    GameTime.FromCurrentTime(60 * 5));
                // Render some monster buff effect
                SoundMgr.PlaySoundById(spellData.EffectSoundId, AudioSource);
                break;

            case SpellType.Spirit_Heroism:
                if (skillMastery == SkillMastery.Normal || skillMastery == SkillMastery.Expert)
                {
                    buffDurationMins = 64 + skillLevel * 5;
                }
                else if (skillMastery == SkillMastery.Master)
                {
                    buffDurationMins = 64 + skillLevel * 15;
                }
                else if (skillMastery == SkillMastery.Grandmaster)
                {
                    buffDurationMins = 64 + skillLevel * 20;
                }

                // Attack Bonus = skillLevel + 5
                BuffMap[MonsterBuffType.Heroism].Apply(skillMastery, skillLevel + 5,
                    GameTime.FromCurrentTime(60 * buffDurationMins));
                // Render some monster buff effect
                SoundMgr.PlaySoundById(spellData.EffectSoundId, AudioSource);
                break;

            case SpellType.Body_Hammerhands:
                buffDurationMins = 60 * skillLevel;

                BuffMap[MonsterBuffType.Heroism].Apply(skillMastery, skillLevel,
                    GameTime.FromCurrentTime(60 * buffDurationMins));
                // Render some monster buff effect
                SoundMgr.PlaySoundById(spellData.EffectSoundId, AudioSource);
                break;

            case SpellType.Body_PowerCure:
                CurrentHp += 5 * skillLevel + 10;
                if (CurrentHp > Data.HitPoints)
                {
                    CurrentHp = Data.HitPoints;
                }

                // Render some monster buff effect
                SoundMgr.PlaySoundById(14020, AudioSource);
                break;

            case SpellType.Light_DispelMagic:
                foreach (SpellEffect partyBuff in party.PartyBuffMap.Values)
                {
                    partyBuff.Reset();
                }

                party.Characters.ForEach(chr =>
                {
                    int intellectBonus = GameMechanics.GetAttributeEffect(chr.GetActualIntellect());
                    int luckBonus = GameMechanics.GetAttributeEffect(chr.GetActualLuck());
                    int dispelResistance = intellectBonus + luckBonus + 30;

                    if (UnityEngine.Random.Range(0, dispelResistance) < 30)
                    {
                        foreach (SpellEffect playerBuff in chr.PlayerBuffMap.Values)
                        {
                            playerBuff.Reset();
                        }
                        // TODO: I dont know what is the actual one for dispel, maybe none
                        //SpellFxRenderer.SetPlayerBuffAnim("sp108", chr);
                    }
                });
                SoundMgr.PlaySoundById(spellData.EffectSoundId, AudioSource);
                break;

            case SpellType.Light_DayOfProtection:
                if (skillMastery == SkillMastery.Normal || skillMastery == SkillMastery.Expert)
                {
                    buffDurationMins = 64 + skillLevel * 5;
                }
                else if (skillMastery == SkillMastery.Master)
                {
                    buffDurationMins = 64 + skillLevel * 15;
                }
                else if (skillMastery == SkillMastery.Grandmaster)
                {
                    buffDurationMins = 64 + skillLevel * 20;
                }

                BuffMap[MonsterBuffType.DayOfProtection].Apply(skillMastery, skillLevel,
                    GameTime.FromCurrentTime(60 * buffDurationMins));
                // Render some monster buff effect
                SoundMgr.PlaySoundById(spellData.EffectSoundId, AudioSource);
                break;

            case SpellType.Light_HourOfPower:
                if (skillMastery == SkillMastery.Normal || skillMastery == SkillMastery.Expert)
                {
                    buffDurationMins = 64 + skillLevel * 5;
                }
                else if (skillMastery == SkillMastery.Master)
                {
                    buffDurationMins = 64 + skillLevel * 15;
                }
                else if (skillMastery == SkillMastery.Grandmaster)
                {
                    buffDurationMins = 64 + skillLevel * 20;
                }

                BuffMap[MonsterBuffType.HourOfPower].Apply(skillMastery, skillLevel + 5,
                    GameTime.FromCurrentTime(60 * buffDurationMins));
                // Render some monster buff effect
                SoundMgr.PlaySoundById(spellData.EffectSoundId, AudioSource);
                break;

            case SpellType.Dark_PainReflection:
                if (skillMastery == SkillMastery.Normal ||
                    skillMastery == SkillMastery.Expert ||
                    skillMastery == SkillMastery.Master)
                {
                    buffDurationMins = 64 + skillLevel * 3;
                }
                else if (skillMastery == SkillMastery.Grandmaster)
                {
                    buffDurationMins = 64 + skillLevel * 15;
                }

                BuffMap[MonsterBuffType.HourOfPower].Apply(skillMastery, skillLevel,
                    GameTime.FromCurrentTime(60 * buffDurationMins));
                // Render some monster buff effect
                SoundMgr.PlaySoundById(spellData.EffectSoundId, AudioSource);
                break;

            case SpellType.Dark_Sharpmetal:
            case SpellType.Fire_MeteorShower:
            case SpellType.Air_Sparks:

            default:
                Debug.LogError("Attempted to cast unimplemented spell: " + spellType);
                break;
        }
    }

    public void AI_SpawnMissile(Transform target)
    {
        string missileName = "";
        MonsterAttackType monsterAttackType = MonsterAttackType.None;
        if (AIState == MonsterState.AttackingRanged1)
        {
            missileName = Data.Attack1_Missile;
            monsterAttackType = MonsterAttackType.Attack1;
        }
        else if (AIState == MonsterState.AttackingRanged2)
        {
            missileName = Data.Attack2_Missile;
            monsterAttackType = MonsterAttackType.Attack2;
        }
        else
        {
            Debug.LogError("Invalid AI state when trying to spawn missile: " + AIState);
            return;
        }

        int missileDisplayId = 0;
        switch (missileName.ToLower())
        {
            case "arrow": missileDisplayId = 545; break;
            case "arrowf": missileDisplayId = 550; break;
            case "fire": missileDisplayId = 510; break;
            case "air": missileDisplayId = 500; break;
            case "water": missileDisplayId = 515; break;
            case "earth": missileDisplayId = 505; break;
            case "spirit": missileDisplayId = 530; break;
            case "mind": missileDisplayId = 525; break;
            case "body": missileDisplayId = 520; break;
            case "light": missileDisplayId = 535; break;
            case "dark": missileDisplayId = 540; break;
            case "ener": missileDisplayId = 555; break;
            default: break;
        }

        ObjectDisplayData missileObjectData = DbMgr.Instance.ObjectDisplayDb.Get(missileDisplayId);
        if (missileObjectData == null || missileDisplayId == 0)
        {
            Debug.LogError("Invalid ObjectDisplayData for missile: " + missileName);
            return;
        }

        ProjectileInfo projectileInfo = new ProjectileInfo();
        projectileInfo.Shooter = this;
        projectileInfo.ShooterTransform = transform;
        projectileInfo.TargetPosition = target.position;
        projectileInfo.DisplayData = missileObjectData;
        projectileInfo.MonsterAttackType = monsterAttackType;

        Projectile.Spawn(projectileInfo);
    }

    public void AI_Stun(bool stunRegardlessOfState)
    {
        if (AIState == MonsterState.Fleeing)
        {
            Flags |= MonsterFlags.Fleeing;
        }

        if (Data.Hostility != 4)
        {
            Flags &= ~MonsterFlags.Unknown_4;
        }

        if (BuffMap[MonsterBuffType.Charm].IsActive())
        {
            BuffMap[MonsterBuffType.Charm].Reset();
        }
        if (BuffMap[MonsterBuffType.Afraid].IsActive())
        {
            BuffMap[MonsterBuffType.Afraid].Reset();
        }

        if (stunRegardlessOfState ||
            AIState != MonsterState.Stunned &&
            AIState != MonsterState.AttackingRanged1 &&
            AIState != MonsterState.AttackingRanged2 &&
            AIState != MonsterState.AttackingRanged3 &&
            AIState != MonsterState.AttackingRanged4 &&
            AIState != MonsterState.AttackingMelee)
        {
            SetNavMeshAgentEnabled(false);

            AIState = MonsterState.Stunned;
            CurrentActionLength = AnimationStunned.TotalAnimationLengthSeconds;
            CurrentActionTime = 0;
            SoundMgr.PlaySoundById(SoundIdGetHit);
            UpdateAnimation();
        }
    }

    public void AI_Die()
    {
        foreach (SpellEffect buff in BuffMap.Values)
        {
            buff.Reset();
        }

        AIState = MonsterState.Dying;
        CurrentActionLength = AnimationDie.TotalAnimationLengthSeconds;
        CurrentActionTime = 0;
        UpdateAnimation();
        SoundMgr.PlaySoundById(SoundIdDie);

        // TODO: Handle other stuff like dropping items, hunting rewards etc
    }
}
