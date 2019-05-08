using UnityEngine;
using System.Collections;
using Assets.OpenMM8.Scripts.Gameplay;
using Assets.OpenMM8.Scripts.Gameplay.Data;
using UnityEngine.AI;
using System.Collections.Generic;
using System;

using Assets.OpenMM8.Scripts;

public class Monster : MonoBehaviour
{
    public const int MAX_RANGE_DISTANCE_SQR = 2560;
    public const int MAX_MELEE_DISTANCE_SQR = 13;

    public int MonsterId;
    public string Name;
    public MonsterData MonsterData;
    public MonsterObjectData DisplayData;

    public int Height;
    public int Radius;
    public float Speed;

    public SpriteObject AnimationStand = null;
    public SpriteObject AnimationWalk = null;
    public SpriteObject AnimationAttack = null;
    public SpriteObject AnimationShoot = null;
    public SpriteObject AnimationStunned = null;
    public SpriteObject AnimationDie = null;
    public SpriteObject AnimationDead = null;
    public SpriteObject AnimationFidget = null;

    public int SoundIdAttack;
    public int SoundIdDie;
    public int SoundIdGetHit;
    public int SoundIdFidget;

    // Components
    public SpriteBillboardAnimator SpriteBillboardAnimator;
    public AudioSource AudioSource;
    public NavMeshAgent NavMeshAgent;
    public NavMeshObstacle NavMeshObstacle;
    public SpriteRotator SpriteLookRotator;
    public HoverInfo HoverInfo;
    public SpriteRenderer SpriteRenderer;

    // State
    public MonsterState AIState = MonsterState.Standing;
    public MonsterAnimationType CurrAnimationType = MonsterAnimationType.Standing;
    public SpriteObject CurrAnimation = null; // From SpriteBillboardAnimator

    public Vector3 InitialPosition;
    public Vector3 CurrentDestination;

    public float CurrentActionTime = 0.0f;
    public float CurrentActionLength = 0.0f;
    public float RecoveryTimeLeft = 0.0f;
    public object LastObjectHit = null;

    public MonsterFlags Flags = 0x0;


    public int Group; // Looks like group is always 0, or most of the times. Used e.g. for arena
    public int Ally;  // Ally = e.g. Dragons/Dragon Hunters, it maps to Group

    public HostilityType HostilityType = HostilityType.Friendly;
    public int CurrentHp;

    public Dictionary<MonsterBuffType, SpellEffect> BuffMap = new Dictionary<MonsterBuffType, SpellEffect>();


    private void Awake()
    {
        SpriteBillboardAnimator = GetComponent<SpriteBillboardAnimator>();
        AudioSource = GetComponent<AudioSource>();
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshObstacle = GetComponent<NavMeshObstacle>();
        SpriteLookRotator = GetComponent<SpriteRotator>();
        HoverInfo = GetComponent<HoverInfo>();
        SpriteRenderer = GetComponent<SpriteRenderer>();

        InitialPosition = transform.position;

        foreach (MonsterBuffType monsterBuff in Enum.GetValues(typeof(MonsterBuffType)))
        {
            BuffMap.Add(monsterBuff, new SpellEffect());
        }
    }

    // Use this for initialization
    void Start()
    {
        MonsterData = DbMgr.Instance.MonsterDb.Get(MonsterId);
        DisplayData = DbMgr.Instance.MonsterObjectDb.Get(MonsterId);

        // TODO: Transform to unity scale
        Name = DisplayData.Name;
        Height = DisplayData.Height;
        Radius = DisplayData.Radius;
        Speed = GameMechanics.ConvertToUnitySpeed(MonsterData.Speed); // Scaled to unity units

        NavMeshAgent.speed = Speed;

        string spritesheetName = DisplayData.StandFramesName.Substring(0, 4).ToLower(); // e.g. m401
        AnimationStand = SpriteObjectRegistry.GetSpriteObject(DisplayData.StandFramesName, spritesheetName);
        AnimationWalk = SpriteObjectRegistry.GetSpriteObject(DisplayData.WalkFramesName, spritesheetName);
        AnimationDie = SpriteObjectRegistry.GetSpriteObject(DisplayData.DieFramesName, spritesheetName);
        AnimationFidget = SpriteObjectRegistry.GetSpriteObject(DisplayData.FidgetFramesName, spritesheetName);

        if (DisplayData.DeadFramesName != "null")
        {
            AnimationDead = SpriteObjectRegistry.GetSpriteObject(DisplayData.DeadFramesName, spritesheetName);
        }
        if (DisplayData.AttackFramesName != "null")
        {
            AnimationAttack = SpriteObjectRegistry.GetSpriteObject(DisplayData.AttackFramesName, spritesheetName);
        }
        if (DisplayData.ShootFramesName != "null")
        {
            AnimationShoot = SpriteObjectRegistry.GetSpriteObject(DisplayData.ShootFramesName, spritesheetName);
        }
        if (DisplayData.StunFramesName != "null")
        {
            AnimationStunned = SpriteObjectRegistry.GetSpriteObject(DisplayData.StunFramesName, spritesheetName);
        }

        SoundIdAttack = DisplayData.AttackSoundId;
        SoundIdDie = DisplayData.DieSoundId;
        SoundIdGetHit = DisplayData.GetHitSoundId;
        SoundIdFidget = DisplayData.FidgetSoundId;


        SpriteBillboardAnimator.SetAnimation(AnimationStand);


        // Where else to set this ??????
        int relationIndexParty = 0;
        int relationIndexThis = GetMonsterRelationIndex(MonsterId);
        if (MonsterRelationDb.GetRelation(relationIndexParty, relationIndexThis) > 0)
        {
            Flags |= MonsterFlags.Aggressor;
        }

        CurrentHp = MonsterData.HitPoints;

        GameCore.Instance.MonsterList.Add(this);

        Debug.Log("Fidged id: " + SoundIdFidget);
    }

    private void OnDestroy()
    {
        GameCore.Instance.MonsterList.Remove(this);
    }


    private void UpdateAnimation()
    {
        SpriteObject monsterAnim = null;

        Flags &= ~MonsterFlags.Animating;
        switch (AIState)
        {
            case MonsterState.Walking:
                monsterAnim = AnimationWalk;
                CurrAnimationType = MonsterAnimationType.Walking;
                break;

            case MonsterState.AttackingMelee:
                monsterAnim = AnimationAttack;
                CurrAnimationType = MonsterAnimationType.AttackMelee;
                Flags |= MonsterFlags.Animating;
                break;

            case MonsterState.AttackingRanged1:
            case MonsterState.AttackingRanged2:
            case MonsterState.AttackingRanged3:
            case MonsterState.AttackingRanged4:
                monsterAnim = AnimationAttack;
                CurrAnimationType = MonsterAnimationType.AttackRanged;
                Flags |= MonsterFlags.Animating;
                break;

            case MonsterState.Dying:
            case MonsterState.Resurrected:
                monsterAnim = AnimationDie;
                CurrAnimationType = MonsterAnimationType.Dying;
                Flags |= MonsterFlags.Animating;
                break;

            case MonsterState.Fleeing:
            case MonsterState.Pursuing:
                monsterAnim = AnimationWalk;
                CurrAnimationType = MonsterAnimationType.Walking;
                Flags |= MonsterFlags.Animating;
                break;

            case MonsterState.Stunned:
                monsterAnim = AnimationStunned;
                CurrAnimationType = MonsterAnimationType.Gothit;
                Flags |= MonsterFlags.Animating;
                break;

            case MonsterState.Fidgeting:
                monsterAnim = AnimationFidget;
                CurrAnimationType = MonsterAnimationType.Bored;
                Flags |= MonsterFlags.Animating;
                break;

            case MonsterState.Standing:
            case MonsterState.Interacting:
            case MonsterState.Summoned:
                monsterAnim = AnimationStand;
                CurrAnimationType = MonsterAnimationType.Standing;
                Flags |= MonsterFlags.Animating;
                break;

            case MonsterState.Dead:
                if (AnimationDead == null)
                {
                    AIState = MonsterState.Removed;
                }
                else
                {
                    monsterAnim = AnimationDead;
                    CurrAnimationType = MonsterAnimationType.Dead;
                }
                break;

            case MonsterState.Removed:
            case MonsterState.Disabled:
                return;

            default:
                Debug.LogError("Invalid AI state: " + AIState);
                return;
        }

        if (monsterAnim != null && SpriteBillboardAnimator.SpriteObject != monsterAnim)
        {
            SpriteBillboardAnimator.SetAnimation(monsterAnim);
            if (AIState == MonsterState.AttackingMelee ||
                AIState == MonsterState.AttackingRanged1 ||
                AIState == MonsterState.AttackingRanged2 ||
                AIState == MonsterState.AttackingRanged3 ||
                AIState == MonsterState.AttackingRanged4 ||
                AIState == MonsterState.Dead ||
                AIState == MonsterState.Dying)
            {
                SpriteBillboardAnimator.Loop = false;
            }
            else
            {
                SpriteBillboardAnimator.Loop = true;
            }
        }
    }

    public bool CanAct()
    {
        bool isStoned = BuffMap[MonsterBuffType.Stoned].IsActive();
        bool isParalyzed = BuffMap[MonsterBuffType.Paralyzed].IsActive();

        if (isStoned ||
            isParalyzed ||
            AIState == MonsterState.Dying ||
            AIState == MonsterState.Dead ||
            AIState == MonsterState.Removed ||
            AIState == MonsterState.Summoned ||
            AIState == MonsterState.Disabled)
        {
            return false;
        }

        return true;
    }

    public bool IsTargetable()
    {
        bool isStoned = BuffMap[MonsterBuffType.Stoned].IsActive();

        if (isStoned ||
            AIState == MonsterState.Dying ||
            AIState == MonsterState.Dead ||
            AIState == MonsterState.Removed ||
            AIState == MonsterState.Summoned ||
            AIState == MonsterState.Disabled)
        {
            return false;
        }

        return true;
    }

    // If otherMonster == null, then relation to PlayerParty is assumed
    public int GetRelationTo(Monster otherMonster)
    {
        // e.g. If thisGroup == 0 then it's the same group as player - the are friendly

        int thisGroup = 0;
        int otherGroup = 0;

        if (otherMonster != null)
        {
            if (Group != 0 && otherMonster.Group != 0 && Group == otherMonster.Group)
            {
                return 0;
            }
        }

        if (BuffMap[MonsterBuffType.Berserk].IsActive())
        {
            return 4;
        }

        if (BuffMap[MonsterBuffType.Enslaved].IsActive() || Ally == 9999)
        {
            thisGroup = 0;
        }
        else if (Ally > 0)
        {
            thisGroup = Ally;
        }
        else
        {
            thisGroup = (MonsterData.Id - 1) / 3 + 1;
        }

        if (otherMonster != null)
        {
            if (otherMonster.BuffMap[MonsterBuffType.Berserk].IsActive())
            {
                return 4;
            }

            if (otherMonster.BuffMap[MonsterBuffType.Enslaved].IsActive() || otherMonster.Ally == 9999)
            {
                otherGroup = 0;
            }
            else if (otherMonster.Ally > 0)
            {
                otherGroup = otherMonster.Ally;
            }
            else
            {
                otherGroup = (otherMonster.MonsterData.Id - 1) / 3 + 1;
            }
        }
        else
        {
            // Player?
            otherGroup = 0;
        }

        if ((BuffMap[MonsterBuffType.Charm].IsActive() && otherGroup == 0) ||
            (otherMonster != null && otherMonster.BuffMap[MonsterBuffType.Charm].IsActive() && thisGroup == 0))
        {
            return 0;
        }

        if (!BuffMap[MonsterBuffType.Enslaved].IsActive() && IsEnemy() && otherGroup == 0)
        {
            return 4;
        }

        if (thisGroup >= 89 || otherGroup >= 89)
        {
            return 0;
        }

        if (thisGroup == 0)
        {
            // Other monster is either player party or this monster is enslaved and other monster is friendly
            // and other monster is not friendly with player party
            if ((otherMonster == null || BuffMap[MonsterBuffType.Enslaved].IsActive() && otherMonster.IsFriend()) &&
                MonsterRelationDb.GetRelation(otherGroup, 0) > 0)
            {
                return MonsterRelationDb.GetRelation(0, otherGroup);
            }
            else
            {
                return 4;
            }
        }
        else
        {
            return MonsterRelationDb.GetRelation(thisGroup, otherGroup);
        }
    }

    public bool IsFriend()
    {
        return !IsEnemy();
    }

    public bool IsEnemy()
    {
        return Flags.HasFlag(MonsterFlags.Aggressor);
    }

    public Transform SelectTarget(bool canTargetParty)
    {
        Transform target = null;
        float lowestRadius = float.MaxValue;

        foreach (Monster otherMonster in GameCore.Instance.MonsterList)
        {
            int actorRelation = 0;
            if (otherMonster == this ||
                otherMonster.AIState == MonsterState.Dead ||
                otherMonster.AIState == MonsterState.Dying ||
                otherMonster.AIState == MonsterState.Removed ||
                otherMonster.AIState == MonsterState.Summoned ||
                otherMonster.AIState == MonsterState.Disabled)
            {
                continue;
            }

            if (LastObjectHit == null)
            {
                actorRelation = GetRelationTo(otherMonster);
            }
            else if (!IsTargetable())
            {
                LastObjectHit = null;
                actorRelation = GetRelationTo(otherMonster);
            }
            else
            {
                bool isOtherMonsterSameGroup = 
                    Group != 0 && otherMonster.Group != 0 && Group == otherMonster.Group;
                if (isOtherMonsterSameGroup)
                {
                    continue;
                }
                actorRelation = 4;
            }

            // Friendly with other monster - cannot target
            if (actorRelation == 0)
            {
                continue;
            }

            if (HostilityType != HostilityType.Friendly)
            {
                actorRelation = (int)HostilityType;
            }

            int hostilityRadius = GetHostilityRange((HostilityType)actorRelation);

            Vector3 distanceOffset = transform.position - otherMonster.transform.position;
            float distanceSqr = distanceOffset.sqrMagnitude;

            // TODO: hostilityRadius to unity units
            if ((distanceSqr < hostilityRadius) && (distanceSqr < lowestRadius))
            {
                lowestRadius = distanceSqr;
                target = otherMonster.transform;
            }
        }

        PlayerParty party = GameCore.GetParty();
        if (canTargetParty && !party.IsInvisible())
        {
            int actorRelation = 0;
            if (IsEnemy() &&
                !BuffMap[MonsterBuffType.Enslaved].IsActive() &&
                !BuffMap[MonsterBuffType.Charm].IsActive() &&
                !BuffMap[MonsterBuffType.Summoned].IsActive())
            {
                actorRelation = 4;
            }
            else
            {
                actorRelation = GetRelationTo(null);
            }

            if (actorRelation > 0)
            {
                int hostilityRadius;
                if (HostilityType != HostilityType.Friendly)
                {
                    // ??
                    hostilityRadius = GetHostilityRange(HostilityType.HostileLong);
                }
                else
                {
                    hostilityRadius = GetHostilityRange((HostilityType)actorRelation);
                }

                Vector3 distanceOffset = transform.position - party.transform.position;
                float distanceSqr = distanceOffset.sqrMagnitude;

                // TODO: hostilityRadius to unity units
                if ((distanceSqr < hostilityRadius) && (distanceSqr < lowestRadius))
                {
                    target = party.transform;
                }
            }
        }


        return target;
    }

    public bool CanCastSpell(SpellType spellType)
    {
        switch (spellType)
        {
            case SpellType.Body_PowerCure:
                if (CurrentHp >= MonsterData.HitPoints)
                {
                    return false;
                }
                return true;

            case SpellType.Light_DispelMagic:
                foreach (SpellEffect buff in GameCore.GetParty().PartyBuffMap.Values)
                {
                    if (buff.IsActive())
                    {
                        return true;
                    }
                }

                foreach (Character chr in GameCore.GetParty().Characters)
                {
                    foreach (SpellEffect buff in chr.PlayerBuffMap.Values)
                    {
                        if (buff.IsActive())
                        {
                            return true;
                        }
                    }
                }

                return false;

            case SpellType.Light_DayOfProtection:
                return !BuffMap[MonsterBuffType.DayOfProtection].IsActive();
            case SpellType.Light_HourOfPower:
                return !BuffMap[MonsterBuffType.HourOfPower].IsActive();
            case SpellType.Dark_PainReflection:
                return !BuffMap[MonsterBuffType.PainReflection].IsActive();
            case SpellType.Body_Hammerhands:
                return !BuffMap[MonsterBuffType.Hammerhands].IsActive();
            case SpellType.Fire_Haste:
                return !BuffMap[MonsterBuffType.Haste].IsActive();
            case SpellType.Air_Shield:
                return !BuffMap[MonsterBuffType.Shield].IsActive();
            case SpellType.Earth_Stoneskin:
                return !BuffMap[MonsterBuffType.Stoneskin].IsActive();
            case SpellType.Spirit_Bless:
                return !BuffMap[MonsterBuffType.Bless].IsActive();
            case SpellType.Spirit_Fate:
                return !BuffMap[MonsterBuffType.Fate].IsActive();
            case SpellType.Spirit_Heroism:
                return !BuffMap[MonsterBuffType.Heroism].IsActive();
        }

        return true;
    }

    public bool ShouldPlayFidgetAttackAnim(SpellType spellType)
    {
        switch (spellType)
        {
            case SpellType.Fire_Haste:
            case SpellType.Air_Shield:
            case SpellType.Spirit_Bless:
            case SpellType.Spirit_Fate:
            case SpellType.Spirit_Heroism:
            case SpellType.Body_Hammerhands:
            case SpellType.Body_PowerCure:
            case SpellType.Light_DispelMagic:
            case SpellType.Light_DayOfProtection:
            case SpellType.Light_HourOfPower:
            case SpellType.Dark_PainReflection:
                return false;
        }

        return true;
    }

    public MonsterAttackType RandomizeUsedAbility()
    {
        // TODO: Summon

        bool canCastSpell1 = CanCastSpell((SpellType)MonsterData.Spell1_SpellType);
        bool canCastSpell2 = CanCastSpell((SpellType)MonsterData.Spell2_SpellType);

        if (canCastSpell1 && MonsterData.Spell1_UseChance > 0 &&
            UnityEngine.Random.Range(0, 100) < MonsterData.Spell1_UseChance)
        {
            return MonsterAttackType.Spell1;
        }
        if (canCastSpell2 && MonsterData.Spell2_UseChance > 0 &&
            UnityEngine.Random.Range(0, 100) < MonsterData.Spell2_UseChance)
        {
            return MonsterAttackType.Spell2;
        }
        if (MonsterData.Attack2_UseChance > 0 &&
            UnityEngine.Random.Range(0, 100) < MonsterData.Attack2_UseChance)
        {
            return MonsterAttackType.Attack2;
        }

        return MonsterAttackType.Attack1;
    }

    //=============================================================================================


    public bool IsWalking()
    {
        if (!NavMeshAgent.enabled)
        {
            return false;
        }

        if (!NavMeshAgent.pathPending)
        {
            if (NavMeshAgent.remainingDistance <= NavMeshAgent.stoppingDistance)
            {
                NavMeshAgent.SetDestination(transform.position);
                //GetComponent<Rigidbody>().velocity = Vector3.zero;
                return false;
            }
        }

        return true;
    }

    public void StopMoving()
    {
        SetNavMeshAgentEnabled(true);
        NavMeshAgent.ResetPath();
    }

    public void SetNavMeshAgentEnabled(bool enabled)
    {
        if (NavMeshAgent.enabled == enabled && NavMeshObstacle.enabled == !enabled)
        {
            return;
        }

        // This is to supress warnings
        if (enabled)
        {
            NavMeshObstacle.enabled = false;
            NavMeshAgent.enabled = true;
        }
        else
        {
            NavMeshAgent.enabled = false;
            NavMeshObstacle.enabled = true;
        }
    }

    public void TurnToObject(GameObject go)
    {
        if (go == null)
        {
            Debug.LogError("Cannot turn to null object !");
            return;
        }

        if (go.CompareTag("Player"))
        {
            transform.LookAt(transform.position + go.transform.rotation * Vector3.back, go.transform.rotation * Vector3.up);
            SpriteLookRotator.OnLookDirectionChanged(LookDirection.Front);
        }
        else
        {
            transform.LookAt(go.transform);
            SpriteLookRotator.AlignRotation();
        }
    }

    private void Update()
    {
        /*Transform partyTransform = GameCore.GetParty().transform;

        Vector3 heading = partyTransform.position - transform.position;
        Vector3 targetDir = heading.normalized;
        float distanceToTargetSqr = heading.sqrMagnitude;

        Debug.Log("Distance: [" + distanceToTargetSqr + "], targetDir: [" + targetDir + "]");*/
    }

    public bool CanDirectlySeeTarget(Transform target, string layerToIgnore = "")
    {
        if (target.GetComponent<PlayerParty>() != null)
        {
            layerToIgnore = "NPC";
        }
        else if (target.GetComponent<Monster>() != null)
        {
            layerToIgnore = "Player";
        }

        int layerMask = int.MaxValue;
        if (layerToIgnore != "")
        {
            layerMask &= ~(1 << LayerMask.NameToLayer(layerToIgnore));
        }

        RaycastHit hitInfo;
        if (Physics.Linecast(transform.position, target.position, out hitInfo, layerMask))
        {
            return hitInfo.transform == target;
        }

        return false;
    }

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
        /*if (direction == Vector3.zero)
        {
            
        }*/

        /*string ignoredLayer = "";
        // Ignore other monsters standing in the way
        if (target.GetComponent<PlayerParty>() != null)
        {
            ignoredLayer = "NPC";
        }
        else if (target.GetComponent<Monster>() != null)
        {
            ignoredLayer = "Player";
        }
        bool canSee = CanDirectlySeeTarget(target, ignoredLayer);
        Debug.Log("CanSee: " + canSee);*/


        // TODO: This has to be resolved sooner or later...
        bool canSee = true;
        if (canSee)
        {
            Vector3 direction = (target.position - transform.position).normalized;

            CurrentActionLength = AnimationShoot.TotalAnimationLengthSeconds;
            CurrentActionTime = 0.0f;
            RecoveryTimeLeft = MonsterData.RecoveryTime;

            transform.rotation = Quaternion.LookRotation(direction);
            AIState = MonsterState.AttackingRanged1;
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
        
        // TODO: This has to be resolved sooner or later...
        bool canSee = true;
        if (CanDirectlySeeTarget(target))
        {
            Vector3 direction = (target.position - transform.position).normalized;

            CurrentActionLength = AnimationAttack.TotalAnimationLengthSeconds;
            CurrentActionTime = 0.0f;
            RecoveryTimeLeft = MonsterData.RecoveryTime + CurrentActionLength;

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

        // TODO: This has to be resolved sooner or later...
        bool canSee = true;
        if (CanDirectlySeeTarget(target))
        {
            Vector3 direction = (target.position - transform.position).normalized;

            CurrentActionLength = AnimationAttack.TotalAnimationLengthSeconds;
            CurrentActionTime = 0.0f;
            RecoveryTimeLeft = MonsterData.RecoveryTime + CurrentActionLength;

            SpellType spell = MonsterData.Spell1_SpellType;
            AIState = MonsterState.AttackingRanged3;
            if (spellAttackType == MonsterAttackType.Spell2)
            {
                spell = MonsterData.Spell2_SpellType;
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
                if (CurrentHp > MonsterData.HitPoints)
                {
                    CurrentHp = MonsterData.HitPoints;
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
                        SpellFxRenderer.SetPlayerBuffAnim("sp108", chr);
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
        if (AIState == MonsterState.AttackingRanged1)
        {
            missileName = MonsterData.Attack1_Missile;
        }
        else if (AIState == MonsterState.AttackingRanged2)
        {
            missileName = MonsterData.Attack2_Missile;
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

        Projectile.Spawn(projectileInfo);
    }

    //=============================================================================================
    // Static methods
    //=============================================================================================



    static public int GetHostilityRange(HostilityType type)
    {
        switch (type)
        {
            case HostilityType.Friendly:
                return 0;
            case HostilityType.HostileClose:
                return 512;
            case HostilityType.HostileShort:
                return 1280;
            case HostilityType.HostileMedium:
                return 2560;
            case HostilityType.HostileLong:
                return 5120;
            default:
                break;
        }

        return 0;
    }

    static public void BuildActorList_ODM()
    {
        List<Monster> nearbyMonsters = GameCore.Instance.NearbyMonsterList;
        nearbyMonsters.Clear();

        PlayerParty party = GameCore.GetParty();
        Vector3 partyPosition = party.transform.position;
        foreach (Monster monster in GameCore.Instance.MonsterList)
        {
            monster.Flags &= ~MonsterFlags.InUpdateRange;
            if (!monster.CanAct())
            {
                //
                continue;
            }

            float distanceSqr = (partyPosition - monster.transform.position).sqrMagnitude;
            if (distanceSqr < 2816)
            {
                monster.Flags &= ~MonsterFlags.Hostile;
                if (monster.IsEnemy() || monster.GetRelationTo(null) > 0)
                {
                    monster.Flags |= MonsterFlags.Hostile;
                    if (distanceSqr < MAX_MELEE_DISTANCE_SQR)
                    {
                        // Red alert
                    }
                    else if (distanceSqr < MAX_RANGE_DISTANCE_SQR)
                    {
                        // Yellow alert
                    }
                }

                monster.Flags |= MonsterFlags.InUpdateRange;
                nearbyMonsters.Add(monster);
                //Debug.Log("Added: " + monster.name);
            }
        }

        // Limit number of updated units ? Should not be necessary these days 
    }

    static public void UpdateMonsters(float timeDelta)
    {
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
        sw.Start();

        BuildActorList_ODM();
        List<Monster> nearbyMonsters = GameCore.Instance.NearbyMonsterList;

        PlayerParty party = GameCore.GetParty();
        

        // Update monsters far away
        foreach (Monster monster in GameCore.Instance.MonsterList)
        {
            if (monster.AIState == MonsterState.Dead ||
                monster.AIState == MonsterState.Removed ||
                monster.AIState == MonsterState.Disabled ||
                monster.Flags.HasFlag(MonsterFlags.InUpdateRange))
            {
                continue;
            }

            // TODO...


            monster.CurrentActionTime += timeDelta;
            if (monster.CurrentActionTime < monster.CurrentActionLength)
            {
                continue;
            }

            if (monster.AIState == MonsterState.Dying)
            {
                monster.AIState = MonsterState.Dead;
            }
            else if (monster.AIState == MonsterState.Summoned)
            {
                monster.AIState = MonsterState.Standing;
                monster.CurrentActionTime = 0;
                monster.CurrentActionLength = 0;
                monster.UpdateAnimation();
            }
            else
            {
                Vector3 lookDirectionToPlayer = (party.transform.position - monster.transform.position).normalized;
                monster.AI_StandOrBored(lookDirectionToPlayer, 2.0f);
            }
        }

        /*sw.Stop();
        Debug.Log(sw.ElapsedMicroSeconds());*/

        // Update monsters nearby
        foreach (Monster monster in nearbyMonsters)
        {
            Transform target = monster.SelectTarget(true);
            //Transform target = null;

            PlayerParty targetAsPlayerParty = target?.GetComponent<PlayerParty>();
            Monster targetAsMonster = target?.GetComponent<Monster>();

            if (monster.HostilityType != HostilityType.Friendly && target == null)
            {
                monster.HostilityType = HostilityType.Friendly;
            }

            // Can this be at the start ?
            if (monster.AIState == MonsterState.Dying ||
                monster.AIState == MonsterState.Dead ||
                monster.AIState == MonsterState.Removed ||
                monster.AIState == MonsterState.Disabled ||
                monster.AIState == MonsterState.Summoned)
            {
                //Debug.Log("continue1");
                continue;
            }

            float nonAttackActionTimeLength = monster.MonsterData.RecoveryTime * 2.1333f;

            float radiusMultiplier = 0.5f;
            if (targetAsPlayerParty != null)
            {
                radiusMultiplier = 1.0f;
            }

            if (monster.CurrentHp <= 0)
            {
                // Die
            }

            

            if (monster.BuffMap[MonsterBuffType.Shrink].IsAppliedAndExpired())
            {
                // Reset heigh
            }

            if (monster.BuffMap[MonsterBuffType.Charm].IsActive())
            {
                monster.HostilityType = HostilityType.Friendly;
            }
            else if (monster.BuffMap[MonsterBuffType.Charm].IsAppliedAndExpired())
            {
                monster.HostilityType = (HostilityType)monster.MonsterData.Hostility;
            }

            // If monster summoning time is up
            if (monster.BuffMap[MonsterBuffType.Summoned].IsAppliedAndExpired())
            {
                monster.AIState = MonsterState.Removed;
                //Debug.Log("continue2");
                continue;
            }

            // Reset expired buffs
            foreach (SpellEffect monsterBuff in monster.BuffMap.Values)
            {
                if (monsterBuff.IsAppliedAndExpired())
                {
                    monsterBuff.Reset();
                }
            }

            // Monster cannot act due to debuff
            if (monster.BuffMap[MonsterBuffType.Stoned].IsActive() ||
                monster.BuffMap[MonsterBuffType.Paralyzed].IsActive())
            {
                //Debug.Log("continue3");
                continue;
            }

            monster.CurrentActionTime += timeDelta;
            monster.RecoveryTimeLeft -= timeDelta;
            if (monster.RecoveryTimeLeft < 0.0f)
            {
                monster.RecoveryTimeLeft = 0.0f;
            }

            if (monster.CurrAnimationType == MonsterAnimationType.Walking && !monster.IsWalking())
            {
                monster.CurrentActionTime = float.MaxValue;
            }

            // TODO: Handle speed here (slow)
            float monsterSpeed = monster.Speed;
            if (monster.AIState == MonsterState.Walking)
            {
                monsterSpeed /= 2.0f;
            }
            if (monster.BuffMap[MonsterBuffType.Slowed].IsActive())
            {
                monsterSpeed /= 2.0f;
            }
            monster.NavMeshAgent.speed = monsterSpeed;

            // Direction and square distance from target
            Vector3 targetDir = Vector3.zero;
            float distanceToTargetSqr = 0.0f;

            if (target != null)
            {
                Vector3 heading = target.position - monster.transform.position;
                targetDir = heading.normalized;
                distanceToTargetSqr = heading.sqrMagnitude;
            }

            if (monster.HostilityType == HostilityType.Friendly ||
                monster.RecoveryTimeLeft > 0.0f ||
                radiusMultiplier * 307.2f < distanceToTargetSqr ||
                monster.AIState != MonsterState.Pursuing &&
                monster.AIState != MonsterState.Standing &&
                monster.AIState != MonsterState.Walking &&
                monster.AIState != MonsterState.Fidgeting &&
                monster.AIState != MonsterState.Stunned)
            {
                // This condition body handles actual spawning of projectile/spell/doing melee damage

                // If attack animation is not finished, dont do anything, else invoke attack
                if (monster.CurrentActionTime < monster.CurrentActionLength)
                {
                    //Debug.Log("continue4");
                    continue;
                }
                else if (monster.AIState == MonsterState.AttackingMelee)
                {

                }
                else if (monster.AIState == MonsterState.AttackingRanged1)
                {
                    monster.AI_SpawnMissile(target);
                }
                else if (monster.AIState == MonsterState.AttackingRanged2)
                {
                    monster.AI_SpawnMissile(target);
                }
                else if (monster.AIState == MonsterState.AttackingRanged3)
                {
                    SpellType spellType = (SpellType)monster.MonsterData.Spell1_SpellType;
                    int skillLevel = monster.MonsterData.Spell1_SkillLevel;
                    SkillMastery skillMastery = monster.MonsterData.Spell1_SkillMastery;

                    monster.AI_CastSpell(target, spellType, skillLevel, skillMastery);
                }
                else if (monster.AIState == MonsterState.AttackingRanged4)
                {
                    SpellType spellType = (SpellType)monster.MonsterData.Spell2_SpellType;
                    int skillLevel = monster.MonsterData.Spell2_SkillLevel;
                    SkillMastery skillMastery = monster.MonsterData.Spell2_SkillMastery;

                    monster.AI_CastSpell(target, spellType, skillLevel, skillMastery);
                }
            }

            // Wasnt actor relation distance already resolved before ... ?
            if (monster.HostilityType == HostilityType.Friendly)
            {
                int monsterRelation;
                if (targetAsMonster != null)
                {
                    int monsterRelationId1 = GetMonsterRelationIndex(monster.MonsterId);
                    int monsterRelationId2 = GetMonsterRelationIndex(targetAsMonster.MonsterId);
                    monsterRelation = MonsterRelationDb.GetRelation(monsterRelationId1, monsterRelationId2);
                }
                else
                {
                    monsterRelation = 4;
                }

                float testDistanceSqr = 0.0f;
                if (monsterRelation == 2)
                {
                    testDistanceSqr = 1024.0f / 2;
                }
                else if (monsterRelation == 3)
                {
                    testDistanceSqr = 2560.0f / 2;
                }
                else if (monsterRelation == 4)
                {
                    testDistanceSqr = 5120.0f / 2;
                }

                if (monsterRelation >= 1 && monsterRelation <= 4 &&
                    distanceToTargetSqr < testDistanceSqr)
                {
                    monster.HostilityType = HostilityType.HostileLong;
                }

                //Debug.Log("Relation: " + monsterRelation);
            }

            if (monster.BuffMap[MonsterBuffType.Afraid].IsActive())
            {
                // If too far away, just move
                if (distanceToTargetSqr >= 10240.0f)
                {
                    // TODO: Actor::AI_RandomMove(actor_id, target_pid, 1024, 0);
                }
                else
                {
                    // Flee from target
                    monster.AI_Flee(target);
                }

                // No further action, he is afraid
                Debug.Log("Afraid - continue");
                continue;
            }

            
            /*if (target != null)
            {
                Debug.Log("Target: " + target.name);
            }
            else
            {
                Debug.Log("No target");
            }

            Debug.Log("HostilityType: " + monster.HostilityType);*/
            // Monster has target (To attack, to flee from, etc)
            if (monster.HostilityType == HostilityType.HostileLong && target != null)
            {
                if (monster.MonsterData.Agressivity == MonsterAggresivityType.Wimp)
                {
                    if (monster.MonsterData.MoveType == MonsterMoveType.Stationary)
                    {
                        // monster.AI_Stand(target, nonAttackActionTimeLength, targetDir, distanceToTargetSqr);
                    }
                    else
                    {
                        monster.AI_Flee(target);
                        continue;
                    }
                }

                if (!monster.Flags.HasFlag(MonsterFlags.Fleeing))
                {
                    if (monster.MonsterData.Agressivity == MonsterAggresivityType.Normal ||
                        monster.MonsterData.Agressivity == MonsterAggresivityType.Agressive)
                    {
                        float fleeHpThreshold = 0.0f;
                        if (monster.MonsterData.Agressivity == MonsterAggresivityType.Normal)
                        {
                            fleeHpThreshold = monster.MonsterData.HitPoints * 0.2f;
                        }
                        else if (monster.MonsterData.Agressivity == MonsterAggresivityType.Agressive)
                        {
                            fleeHpThreshold = monster.MonsterData.HitPoints * 0.1f;
                        }


                        if (monster.CurrentHp < fleeHpThreshold && distanceToTargetSqr < 10240.0f)
                        {
                            Debug.Log("Flee due to low hp");
                            monster.AI_Flee(target);
                            continue;
                        }
                    }
                }

                //distanceToTargetSqr -= monster.DisplayData.Radius;
                float distance = distanceToTargetSqr;
                //Debug.Log("Distance: " + distance);
                if (distance < MAX_RANGE_DISTANCE_SQR)
                {
                    Vector3 targetLookDirection = (target.transform.position - monster.transform.position).normalized;

                    MonsterAttackType chosenAttackType = monster.RandomizeUsedAbility();
                    //Debug.Log("Chosen: " + chosenAttackType);
                    if (chosenAttackType == MonsterAttackType.Attack1 ||
                        chosenAttackType == MonsterAttackType.Attack2)
                    {
                        bool isMissileAttack = false;
                        if (chosenAttackType == MonsterAttackType.Attack1)
                        {
                            isMissileAttack = monster.MonsterData.Attack1_Missile != null &&
                                monster.MonsterData.Attack1_Missile != "0";
                        }
                        else if (chosenAttackType == MonsterAttackType.Attack2)
                        {
                            isMissileAttack = monster.MonsterData.Attack2_Missile != null &&
                                monster.MonsterData.Attack2_Missile != "0"; ;
                        }
                        Debug.Log("Chosen: " + chosenAttackType + ", " + monster.MonsterData.Name);
                        Debug.Log("[" + monster.MonsterData.Attack1_Missile + ", " + monster.MonsterData.Attack2_Missile + "]");
                        Debug.Log("Bonus: " + monster.MonsterData.Attack1_DamageBonus);

                        if (isMissileAttack)
                        {
                            //Debug.Log("Recovery time left: " + monster.RecoveryTimeLeft);
                            if (monster.RecoveryTimeLeft <= 0.0f)
                            {
                                // Play attack animation
                                monster.AI_MissileAttack(target, chosenAttackType);
                            }
                            else if (monster.MonsterData.MoveType == MonsterMoveType.Stationary)
                            {
                                monster.AI_Stand(targetLookDirection, monster.MonsterData.RecoveryTime);
                            }
                            else if (distance < MAX_MELEE_DISTANCE_SQR)
                            {
                                monster.AI_Stand(targetLookDirection, monster.MonsterData.RecoveryTime);
                            }
                            else
                            {
                                monster.AI_PursueKiteRanged(target, monster.MonsterData.RecoveryTime);
                            }
                        }
                        else // Melee attack
                        {
                            if (distance > MAX_MELEE_DISTANCE_SQR)
                            {
                                if (monster.MonsterData.MoveType == MonsterMoveType.Stationary)
                                {
                                    monster.AI_Stand(targetLookDirection, monster.MonsterData.RecoveryTime);
                                }
                                else if (distance >= 512.0f)
                                {
                                    monster.AI_PursueToMeleeFarAway(target, 2.0f);
                                }
                                else
                                {
                                    //Debug.Log("Pursue 2");
                                    // Close to target - direct route
                                    monster.AI_PursueToMeleeDirect(target, 2.0f);
                                }
                            }
                            else if (monster.RecoveryTimeLeft > 0.0f)
                            {
                                monster.AI_Stand(targetLookDirection, 2.0f);
                            }
                            else
                            {
                                // Play attack animation
                                monster.AI_MeleeAttack(target);
                            }
                        }
                    }
                    else if (chosenAttackType == MonsterAttackType.Spell1 ||
                             chosenAttackType == MonsterAttackType.Spell2)
                    {
                        SpellType spellType = SpellType.None;
                        if (chosenAttackType == MonsterAttackType.Spell1)
                        {
                            spellType = (SpellType)monster.MonsterData.Spell1_SpellType;
                        }
                        else
                        {
                            spellType = (SpellType)monster.MonsterData.Spell2_SpellType;
                        }

                        if (spellType != SpellType.None)
                        {
                            if (monster.RecoveryTimeLeft <= 0.0f)
                            {
                                monster.AI_SpellAttack(target, chosenAttackType);
                            }
                            else if (distance > MAX_MELEE_DISTANCE_SQR)
                            {
                                monster.AI_PursueKiteRanged(target, monster.MonsterData.RecoveryTime);
                            }
                            else
                            {
                                monster.AI_Stand(targetLookDirection, monster.MonsterData.RecoveryTime);
                            }
                        }
                        else
                        {
                            Debug.LogError("Should have casted spell but no spelltype for: " + chosenAttackType);
                        }
                    }

                    // We are done here
                    continue;
                }
            }

            // Monster does not have target or too far away or ...
            if (monster.HostilityType != HostilityType.HostileLong ||
                target == null ||
                distanceToTargetSqr > MAX_RANGE_DISTANCE_SQR)
            {
                if (monster.MonsterData.MoveType == MonsterMoveType.Short)
                {
                    // dist = 512.0f;
                    monster.AI_RandomMove(target, 10, 0.0f);
                }
                else if (monster.MonsterData.MoveType == MonsterMoveType.Medium)
                {
                    // dist = 1280.0f;
                    monster.AI_RandomMove(target, 20, 0.0f);
                }
                else if (monster.MonsterData.MoveType == MonsterMoveType.Long)
                {
                    // dist = 2560.0f;
                    monster.AI_RandomMove(target, 40, 0.0f);
                }
                else if (monster.MonsterData.MoveType == MonsterMoveType.Free)
                {
                    // dist = 5120.0f;
                    monster.AI_RandomMove(target, 80, 0.0f);
                }
                else if (monster.MonsterData.MoveType == MonsterMoveType.Stationary)
                {
                    // Direction to player
                    // monster.AI_Stand(DirectionToPlayer, nonAttackActionTimeLength)
                    Vector3 lookDirectionToPlayer = (party.transform.position - monster.transform.position).normalized;
                    monster.AI_Stand(lookDirectionToPlayer, nonAttackActionTimeLength);
                }
            }

        }

        /*sw.Stop();
        Debug.Log(sw.ElapsedMicroSeconds());*/
    }

    public static int GetMonsterRelationIndex(int monsterId)
    {
        return (monsterId - 1) / 3 + 1;
    }
}
