﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.AI;
using Assets.OpenMM8.Scripts.Gameplay;

public class CombatNpc : BaseNpc
{
    [SerializeField]
    private float AttackReuseTime = 0.8f;
    private float AttackReuseTimeLeft = 0.0f;

    private bool IsTargetInMeleeRange = false;

    private bool IsRanged = false;
    private bool HasAltRangedAttack = false;
    private float AltRangedAttackChance;
    private float TimeSinceLastAltAttack = 0.0f;
    private float MinAltAttackRecoveryTime = 2.0f;
    private float MaxAltAttackRecoveryTime = 4.0f;
    private float CurrAltAttackRecoveryTime = 0.0f;

    private bool WasInit = false;

    private float m_TimeWanderWalking = 0.0f;

    // Use this for initialization
    void Start()
    {
        base.OnStart();

        Animator.SetInteger("State", (int)NpcState.Idle);
        State = NpcState.Idle;

        InvokeRepeating("EnterBestState", 0.0f, UpdateIntervalMs / 1000.0f);

        //Debug.unityLogger.logEnabled = false;

        //Debug.Log("Missile1: " + NpcData.Attack1.Missile);

        /*IsRanged = NpcData.Attack1.Missile != "0";
        HasAltRangedAttack = NpcData.Attack2.Missile != "0";
        AltRangedAttackChance = NpcData.ChanceAttack2;*/
        CurrAltAttackRecoveryTime = UnityEngine.Random.Range(MinAltAttackRecoveryTime, MaxAltAttackRecoveryTime);
        /*NavMeshAgent.velocity = new Vector3(0, -10, 0);
        SetNavMeshAgentEnabled(true);
        EnterBestState();*/
    }

    public NpcState EnterBestState()
    {
        NpcState currState = (NpcState)Animator.GetInteger("State");
        /*if (Animator.enabled)
        {
            Animator.enabled = false;
        }
        return currState;*/

        if ((currState == NpcState.Dead) || (currState == NpcState.Dying) || (currState == NpcState.ReceivingDamage))
        {
            return currState;
        }

        if ((currState != NpcState.Attacking) && 
            (EnemiesInAgroRange.Count == 0) && 
            (EnemiesInMeleeRange.Count == 0) && 
            !DoWander &&
            WasInit)
        {
            SetNavMeshAgentEnabled(false);
            SpriteLookRotator.OnLookDirectionChanged(LookDirection.Front);
            SpriteLookRotator.LookLocked = true;
            if (Animator.enabled)
            {
                Animator.Update(Time.deltaTime);
                Animator.enabled = false;
            }
            //Debug.Log("OK");
            return currState;
        }
        else
        {
            WasInit = true;
            SpriteLookRotator.LookLocked = false;
            if (!Animator.enabled)
            {
                Animator.enabled = true;
            }
        }

        if (IsPlayerInMeleeRange && 
            !HostilityResolver.m_IsHostileToPlayer &&
            EnemiesInMeleeRange.Count == 0 &&
            EnemiesInAgroRange.Count == 0)
        {
            TurnToObject(Player);
            currState = NpcState.Idle;
            Animator.SetInteger("State", (int)currState);
            SetNavMeshAgentEnabled(false);
            return currState;
        }

        //SetNavMeshAgentEnabled(true);

        if (currState == NpcState.Idle && AttackReuseTimeLeft > 0.0f)
        {
            AttackReuseTimeLeft -= UpdateIntervalMs / 1000.0f;
            SetNavMeshAgentEnabled(false);
            return currState;
        }
        else
        {
            SpriteLookRotator.LookLocked = false;
        }

        // If it is attacking do not force it to do anything else
        if (currState == NpcState.Attacking)
        {
            if (Target != null)
            {
                TurnToObject(Target);
            }
            //GetComponent<Rigidbody>().velocity = Vector3.zero;
            SetNavMeshAgentEnabled(false);
            //m_NavMeshAgent.isStopped = true;
            NavMeshAgent.velocity = Vector3.zero;
            //GetComponent<Rigidbody>() = Vector3.zero;
            return currState;
        }
        
        if (EnemiesInMeleeRange.Count == 0 && EnemiesInAgroRange.Count == 0)
        {
            if (Target != null)
            {
                Target = null;
                StopMoving();
            }

            if (!IsWalking())
            {
                m_TimeWanderWalking = 0.0f;
                if (DoWander)
                {
                    if (RemainingWanderIdleTime > 0.0f)
                    {
                        RemainingWanderIdleTime -= UpdateIntervalMs / 1000.0f;
                        Animator.SetInteger("State", (int)NpcState.Idle);
                    }
                    else
                    {
                        WanderWithinSpawnArea(WanderRadius);
                        Animator.SetInteger("State", (int)NpcState.Walking);
                        RemainingWanderIdleTime = UnityEngine.Random.Range(MinWanderIdleTime, MaxWanderIdleTime);
                    }
                }
            }
            else
            {
                if (DoWander)
                {
                    m_TimeWanderWalking += UpdateIntervalMs / 1000.0f;
                    if (m_TimeWanderWalking > 3.0f)
                    {
                        m_TimeWanderWalking = 0.0f;
                        StopMoving();
                    }
                }
            }
        }
        else if (IsFleeing)
        {
            // TODO
        }
        else if (EnemiesInMeleeRange.Count > 0)
        {
            // NPC is not attacking in this block

            Target = EnemiesInMeleeRange.OrderBy(
                t => (t.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
            if (Target == null)
            {
                EnemiesInMeleeRange.Remove(Target);
            }
            else
            {
                /*StopMoving();
                TurnToObject(m_Target);
                m_Animator.SetInteger("State", (int)NpcState.Attacking);*/
                AttackTarget(Target, true);
            }
        }
        else if (EnemiesInAgroRange.Count > 0)
        {
            // NPC is not attacking in this block

            if (IsRanged)
            {
                if (!IsWalking())
                {
                    // Ranged and not walking -> find and attack target
                    GameObject closestTarget = GetClosestTarget(EnemiesInAgroRange);
                    TurnToObject(closestTarget);
                    AttackTarget(closestTarget, false);

                    // Moving after shooting has to be handled in OnAttackFrame event
                }
            }
            else
            {
                TimeSinceLastAltAttack += UpdateIntervalMs / 1000.0f;
                if (HasAltRangedAttack && TimeSinceLastAltAttack > CurrAltAttackRecoveryTime)
                {
                    GameObject closestTarget = GetClosestTarget(EnemiesInAgroRange);
                    // If the target is in agro range but close, dont use alt ranged attack
                    if (closestTarget != null)
                    {
                        if (Vector3.Distance(closestTarget.transform.position, transform.position) > 6.0f)
                        {
                            AttackTarget(closestTarget, false);
                            TimeSinceLastAltAttack = 0.0f;
                            CurrAltAttackRecoveryTime = UnityEngine.Random.Range(MinAltAttackRecoveryTime, MaxAltAttackRecoveryTime);
                        }
                        else
                        {
                            ChaseTarget(closestTarget);
                        }
                    }
                }
                else
                {
                    GameObject closestTarget = GetClosestTarget(EnemiesInAgroRange);
                    ChaseTarget(closestTarget);
                }
            }
        }

        return (NpcState)Animator.GetInteger("State");
    }

    GameObject GetClosestTarget(List<GameObject> targets)
    {
        GameObject closest = targets.OrderBy(
                t => (t.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
        return closest;
    }

    bool AttackTarget(GameObject target, bool isMeleeRange)
    {
        if (target != null)
        {
            SetNavMeshAgentEnabled(false);
            Target = target;

            StopMoving();
            TurnToObject(Target);
            Animator.SetInteger("State", (int)NpcState.Attacking);
            AudioSource.clip = AttackSound;
            AudioSource.Play();

            IsTargetInMeleeRange = isMeleeRange;
        }

        return target != null;
    }

    public override void OnObjectEnteredMeleeRange(GameObject other)
    {
        if (HostilityResolver.IsHostileTo(other))
        {
            EnemiesInMeleeRange.Add(other);
            EnterBestState();
        }
        else if (other.CompareTag("Player"))
        {
            IsPlayerInMeleeRange = true;
            EnterBestState();
        }
    }

    public override void OnObjectLeftMeleeRange(GameObject other)
    {
        if (HostilityResolver.IsHostileTo(other))
        {
            EnemiesInMeleeRange.Remove(other);
            EnterBestState();
        }
        else if (other.CompareTag("Player"))
        {
            IsPlayerInMeleeRange = false;
            EnterBestState();
        }
    }

    public override void OnObjectEnteredAgroRange(GameObject other)
    {
        if (HostilityResolver.IsHostileTo(other))
        {
            EnemiesInAgroRange.Add(other);
            EnterBestState();
        }
    }

    public override void OnObjectLeftAgroRange(GameObject other)
    {
        if (HostilityResolver.IsHostileTo(other))
        {
            EnemiesInAgroRange.Remove(other);
            EnterBestState();
        }
    }

    // Animator
    public void OnAttackFrame()
    {
        //Debug.Log("OnAttackFrame");
        if (Target)
        {
            if (IsTargetInMeleeRange)
            {
                // Always primary attack ?
                Damageable damageable = Target.GetComponent<Damageable>();
                if (damageable)
                {
                    /*Debug.Log("Min damage: " + NpcData.Attack1.MinDamage + ", Max damage: " + NpcData.Attack1.MaxDamage);
                    Debug.Log("Name: " + NpcData.Name);*/
                    //damageable.ReceiveAttack(NpcData.Attack1, this.gameObject);
                }
            }
            else
            {
                // Spawn projectile
                /*if (NpcData.Attack2.Missile != "0")
                {
                    //Debug.Log("Spawn missile: " + NpcData.Attack2.Missile);
                    GameObject arrow = (GameObject)Instantiate(Resources.Load("Prefabs/Objects/ArrowPrefab"), transform.position + (transform.forward * 2), transform.rotation);
                    Projectile projectile = arrow.GetComponent<Projectile>();
                    projectile.AttackInfo = NpcData.Attack2;
                    projectile.IsTargetPlayer = Target.CompareTag("Player");
                    Vector3 add = new Vector3(0, 1.0f, 0);
                    Vector3 shootDirection = (Target.transform.position + add - arrow.transform.position).normalized;
                    //projectile.Shoot(arrow.transform.position, Target.transform.position + add);
                    projectile.Shoot(shootDirection);
                }*/
            }
        }
    }

    public void OnAttackDone()
    {
        //Debug.Log("END ATTACK !");

        IsTargetInMeleeRange = false;

        StopMoving();
        Animator.SetInteger("State", (int)NpcState.Idle);

        if (IsRanged && EnemiesInAgroRange.Count > 0 && EnemiesInMeleeRange.Count == 0)
        {
            // Kite the target
            GameObject closestTarget = GetClosestTarget(EnemiesInAgroRange);
            if (closestTarget != null)
            {
                MoveAfterRangedAttack(closestTarget);
            }
        }

        //NpcState currState = (NpcState)m_Animator.GetInteger("State");
        else if (/*currState == NpcState.Attacking && */ EnemiesInMeleeRange.Count != 0)
        {
            AttackReuseTimeLeft = AttackReuseTime;
            Animator.SetInteger("State", (int)NpcState.Idle);
            TurnToObject(GetClosestTarget(EnemiesInMeleeRange));
            SpriteLookRotator.LookLocked = true;
            SetNavMeshAgentEnabled(false);
        }
        else
        {
            EnterBestState();
        }
    }

    public void MoveAfterRangedAttack(GameObject target)
    {
        SetNavMeshAgentEnabled(true);

        Vector3 heading = target.transform.position - transform.position;
        heading.Normalize();

        float randRotMod = UnityEngine.Random.Range(-15.0f, 15.0f);
        float kitingModifier = 90.0f + randRotMod;
        heading = Quaternion.AngleAxis(kitingModifier, Vector3.up) * heading;

        CurrentDestination = transform.position - heading * UnityEngine.Random.Range(6.0f, 7.5f);
        NavMeshAgent.ResetPath();
        NavMeshAgent.SetDestination(CurrentDestination);

        if (DrawWaypoint)
        {
            CurrentWaypoint.transform.position = CurrentDestination;
        }

        Vector3 direction = (CurrentDestination - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        Animator.SetInteger("State", (int)NpcState.Walking);
    }
}