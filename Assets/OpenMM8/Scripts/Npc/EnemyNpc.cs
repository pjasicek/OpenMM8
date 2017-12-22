using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class EnemyNpc : BaseNpc
{
    // Use this for initialization
    void Start()
    {
        base.OnStart();

        m_Animator.SetInteger("State", (int)NpcState.Idle);
        m_State = NpcState.Idle;

        InvokeRepeating("EnterBestState", 0.0f, m_UpdateIntervalMs / 1000.0f);
    }

    // Update is called once per frame
    /*void Update()
    {
        if (!m_NavMeshAgent.enabled)
        {
            return;
        }

        m_CurrentWaypoint.SetActive(m_DrawWaypoint);

        EnterBestState();
    }*/

    public NpcState EnterBestState()
    {
        NpcState currState = (NpcState)m_Animator.GetInteger("State");

        // If it is attacking do not force it to do anything else
        if (currState == NpcState.Attacking)
        {
            if (m_Target != null)
            {
                TurnToObject(m_Target);
            }
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
                        m_RemainingWanderIdleTime -= Time.deltaTime;
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
                StopMoving();
                TurnToObject(m_Target);
                m_Animator.SetInteger("State", (int)NpcState.Attacking);
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
                    AttackTarget(closestTarget);

                    // Moving after shooting has to be handled in OnAttackFrame event
                }
            }
            else
            {
                GameObject closestTarget = GetClosestTarget(m_EnemiesInAgroRange);
                ChaseTarget(closestTarget);
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
            m_Target = target;

            StopMoving();
            TurnToObject(m_Target);
            m_Animator.SetInteger("State", (int)NpcState.Attacking);
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
            Debug.Log("Left melee !");
            m_EnemiesInMeleeRange.Remove(other);
            EnterBestState();
        }
    }

    public override void OnObjectEnteredAgroRange(GameObject other)
    {
        Debug.Log("Entered: " + other.name);
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
        Debug.Log("END ATTACK !");

        StopMoving();
        m_Animator.SetInteger("State", (int)NpcState.Idle);
        EnterBestState();
    }
}

//============================================================
// EDITOR
//============================================================

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyNpc))]
public class EnemyNpcEditor : BaseNpcEditor
{

}
#endif