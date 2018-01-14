using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;
using UnityEngine.AI;

public class CombatNpc : BaseNpc
{
    [SerializeField]
    private float m_AttackReuseTime = 0.8f;
    private float m_AttackReuseTimeLeft = 0.0f;

    // Use this for initialization
    void Start()
    {
        base.OnStart();

        m_Animator.SetInteger("State", (int)NpcState.Idle);
        m_State = NpcState.Idle;

        InvokeRepeating("EnterBestState", 0.0f, m_UpdateIntervalMs / 1000.0f);

        //Debug.unityLogger.logEnabled = false;
    }

    public NpcState EnterBestState()
    {
        SetNavMeshAgentEnabled(true);

        NpcState currState = (NpcState)m_Animator.GetInteger("State");

        if (currState == NpcState.Idle && m_AttackReuseTimeLeft > 0.0f)
        {
            m_AttackReuseTimeLeft -= m_UpdateIntervalMs / 1000.0f;
            SetNavMeshAgentEnabled(false);
            return currState;
        }
        else
        {
            m_SpriteLookRotator.m_LookLocked = false;
        }

        // If it is attacking do not force it to do anything else
        if (currState == NpcState.Attacking)
        {
            if (m_Target != null)
            {
                TurnToObject(m_Target);
            }
            GetComponent<Rigidbody>().velocity = Vector3.zero;
            SetNavMeshAgentEnabled(false);
            //m_NavMeshAgent.isStopped = true;
            m_NavMeshAgent.velocity = Vector3.zero;
            //GetComponent<Rigidbody>() = Vector3.zero;
            return currState;
        }
        
        if (m_EnemiesInMeleeRange.Count == 0 && m_EnemiesInAgroRange.Count == 0)
        {
            if (m_Target != null)
            {
                m_Target = null;
                StopMoving();
            }

            if (!IsWalking())
            {
                if (m_DoWander)
                {
                    if (m_RemainingWanderIdleTime > 0.0f)
                    {
                        m_RemainingWanderIdleTime -= m_UpdateIntervalMs / 1000.0f;
                        m_Animator.SetInteger("State", (int)NpcState.Idle);
                    }
                    else
                    {
                        WanderWithinSpawnArea(m_WanderRadius);
                        m_Animator.SetInteger("State", (int)NpcState.Walking);
                        m_RemainingWanderIdleTime = UnityEngine.Random.Range(m_MinWanderIdleTime, m_MaxWanderIdleTime);
                    }
                }
            }
        }
        else if (m_IsFleeing)
        {
            // TODO
        }
        else if (m_EnemiesInMeleeRange.Count > 0)
        {
            // NPC is not attacking in this block

            m_Target = m_EnemiesInMeleeRange.OrderBy(
                t => (t.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
            if (m_Target == null)
            {
                m_EnemiesInMeleeRange.Remove(m_Target);
            }
            else
            {
                /*StopMoving();
                TurnToObject(m_Target);
                m_Animator.SetInteger("State", (int)NpcState.Attacking);*/
                AttackTarget(m_Target);
            }
        }
        else if (m_EnemiesInAgroRange.Count > 0)
        {
            // NPC is not attacking in this block

            if (m_IsRanged)
            {
                if (!IsWalking())
                {
                    // Ranged and not walking -> find and attack target
                    GameObject closestTarget = GetClosestTarget(m_EnemiesInAgroRange);
                    TurnToObject(closestTarget);
                    AttackTarget(closestTarget);

                    // Moving after shooting has to be handled in OnAttackFrame event
                }
            }
            else
            {
                m_TimeSinceLastAltAttack += m_UpdateIntervalMs / 1000.0f;
                if (m_HasAltRangedAttack && m_TimeSinceLastAltAttack > 2.0f)
                {
                    AttackTarget(GetClosestTarget(m_EnemiesInAgroRange));
                    m_TimeSinceLastAltAttack = 0.0f;
                }
                else
                {
                    GameObject closestTarget = GetClosestTarget(m_EnemiesInAgroRange);
                    ChaseTarget(closestTarget);
                }
            }
        }

        return (NpcState)m_Animator.GetInteger("State");
    }

    GameObject GetClosestTarget(List<GameObject> targets)
    {
        GameObject closest = targets.OrderBy(
                t => (t.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
        return closest;
    }

    bool AttackTarget(GameObject target)
    {
        if (target != null)
        {
            SetNavMeshAgentEnabled(false);
            m_Target = target;

            StopMoving();
            TurnToObject(m_Target);
            m_Animator.SetInteger("State", (int)NpcState.Attacking);
            m_AudioSource.clip = m_AttackSound;
            m_AudioSource.Play();
        }

        return target != null;
    }

    public override void OnObjectEnteredMeleeRange(GameObject other)
    {
        if (m_HostilityResolver.IsHostileTo(other))
        {
            m_EnemiesInMeleeRange.Add(other);
            EnterBestState();
        }
    }

    public override void OnObjectLeftMeleeRange(GameObject other)
    {
        if (m_HostilityResolver.IsHostileTo(other))
        {
            m_EnemiesInMeleeRange.Remove(other);
            EnterBestState();
        }
    }

    public override void OnObjectEnteredAgroRange(GameObject other)
    {
        if (m_HostilityResolver.IsHostileTo(other))
        {
            m_EnemiesInAgroRange.Add(other);
            EnterBestState();
        }
    }

    public override void OnObjectLeftAgroRange(GameObject other)
    {
        if (m_HostilityResolver.IsHostileTo(other))
        {
            m_EnemiesInAgroRange.Remove(other);
            EnterBestState();
        }
    }

    // Animator
    public void OnAttackFrame()
    {

    }

    public void OnAttackDone()
    {
        //Debug.Log("END ATTACK !");

        StopMoving();
        m_Animator.SetInteger("State", (int)NpcState.Idle);

        if (m_IsRanged && m_EnemiesInAgroRange.Count > 0 && m_EnemiesInMeleeRange.Count == 0)
        {
            // Kite the target
            GameObject closestTarget = GetClosestTarget(m_EnemiesInAgroRange);
            if (closestTarget != null)
            {
                MoveAfterRangedAttack(closestTarget);
            }
        }

        //NpcState currState = (NpcState)m_Animator.GetInteger("State");
        else if (/*currState == NpcState.Attacking && */ m_EnemiesInMeleeRange.Count != 0)
        {
            m_AttackReuseTimeLeft = m_AttackReuseTime;
            m_Animator.SetInteger("State", (int)NpcState.Idle);
            TurnToObject(GetClosestTarget(m_EnemiesInMeleeRange));
            m_SpriteLookRotator.m_LookLocked = true;
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

        float randRotMod = UnityEngine.Random.Range(-10.0f, 10.0f);
        float kitingModifier = 90.0f + randRotMod;
        heading = Quaternion.AngleAxis(kitingModifier, Vector3.up) * heading;

        m_CurrentDestination = transform.position - heading * 6.0f;
        m_NavMeshAgent.ResetPath();
        m_NavMeshAgent.SetDestination(m_CurrentDestination);

        m_CurrentWaypoint.transform.position = m_CurrentDestination;

        Vector3 direction = (m_CurrentDestination - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        m_Animator.SetInteger("State", (int)NpcState.Walking);
    }
}