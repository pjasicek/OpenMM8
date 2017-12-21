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

        m_Animator.SetInteger("State", (int)NPCState.Idle);
        m_State = NPCState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (!m_NavMeshAgent.enabled)
        {
            return;
        }

        m_CurrentWaypoint.SetActive(m_DrawWaypoint);

        /**** If NPC is Enemy ****/
        // 1) If it is attacking, do nothing (Waiting for AttackEnded frame event)
        // 2) If it is moving, do nothing (May be interrupted if enemy enters its melee range)
        // ----- [Event] OnAttackEnded - after attack ends, it will check if it is within melee range of any hostile unit,
        //                           if it is, then it will attack it again, if it is not, it will choose some strafe
        //                           location - e.g. Shoot - Move - Shoot - Move, etc.
        // ------ [Event] If enemy enters its attack range, it will attack immediately

        //m_State = (NPCState)m_Animator.GetInteger("State");
        if (m_State == NPCState.Attacking)
        {
            if (m_Target == null)
            {

            }
            else
            {
                Debug.Log("Turning to target");
                TurnToObject(m_Target);
                Debug.Log("Attacking: " + m_Target.name);
            }
        }
        else if (m_State == NPCState.Walking)
        {
            if (m_EnemiesInAgroRange.Count > 0)
            {
                GameObject closestEnemy = m_EnemiesInAgroRange.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
                m_Target = closestEnemy;
                if (m_Target == null)
                {
                    m_EnemiesInAgroRange.Remove(m_Target);
                    StopMoving();
                    m_Animator.SetInteger("State", (int)NPCState.Idle);
                    m_State = NPCState.Idle;
                }
                else
                {
                    ChaseTarget(m_Target);
                }
            }
        }
    }

    public override void OnObjectEnteredMeleeRange(GameObject other)
    {
        if (m_HostilityResolver.IsHostileTo(other))
        {
            if (m_State != NPCState.Attacking)
            {
                if (m_EnemiesInMeleeRange.Count == 0)
                {
                    m_Animator.SetInteger("State", (int)NPCState.Attacking);
                    m_State = NPCState.Attacking;
                    m_Target = other;
                    Debug.Log("Attacking: " + m_Target.name);
                    TurnToObject(m_Target);
                    StopMoving();
                }
            }

            Debug.Log("Entered melee !");
            m_EnemiesInMeleeRange.Add(other);
        }
    }

    public override void OnObjectLeftMeleeRange(GameObject other)
    {
        if (m_HostilityResolver.IsHostileTo(other))
        {
            Debug.Log("Left melee !");
            m_EnemiesInMeleeRange.Remove(other);

            if (m_State != NPCState.Attacking)
            {
                if (m_EnemiesInMeleeRange.Count == 0)
                {
                    if (m_EnemiesInAgroRange.Count > 0)
                    {
                        GameObject closestEnemy = m_EnemiesInAgroRange.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
                        ChaseTarget(closestEnemy);
                    }
                }
                else
                {
                    GameObject closestEnemy = m_EnemiesInMeleeRange.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
                    m_Target = closestEnemy;
                }
            }
        }
    }

    public override void OnObjectEnteredAgroRange(GameObject other)
    {
        Debug.Log("Entered: " + other.name);
        if (m_HostilityResolver.IsHostileTo(other))
        {
            Debug.Log("Entered hostile: " + other.name);
            if (m_EnemiesInAgroRange.Count == 0)
            {
                Debug.Log("Other name: " + other.name);
                m_Target = other;
                ChaseTarget(m_Target);
                m_Animator.SetInteger("State", (int)NPCState.Walking);
                m_State = NPCState.Walking;
            }
            m_EnemiesInAgroRange.Add(other);
        }
    }

    public override void OnObjectLeftAgroRange(GameObject other)
    {
        if (m_HostilityResolver.IsHostileTo(other))
        {
            m_EnemiesInAgroRange.Remove(other);

            if (m_EnemiesInAgroRange.Count == 0)
            {
                m_NavMeshAgent.isStopped = true;
                m_Animator.SetInteger("State", (int)NPCState.Idle);
                m_State = NPCState.Idle;
                m_Target = null;
            }
            else if (m_Target == other)
            {
                GameObject closestEnemy = m_EnemiesInAgroRange.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
                ChaseTarget(closestEnemy);
            }
        }
    }

    // Animator
    public void OnAttackFrame()
    {

    }

    public void OnAttackDone()
    {
        Debug.Log("END ATTACK !");

        if (m_EnemiesInMeleeRange.Count > 0)
        {
            GameObject closestEnemy = m_EnemiesInMeleeRange.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
            m_Target = closestEnemy;
            Debug.Log("Attacking: " + m_Target.name);
        }
        else
        {
            if (m_EnemiesInAgroRange.Count > 0)
            {
                GameObject closestEnemy = m_EnemiesInAgroRange.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
                ChaseTarget(closestEnemy);
                m_Animator.SetInteger("State", (int)NPCState.Walking);
                m_State = NPCState.Walking;
            }
            else
            {
                StopMoving();
                m_Animator.SetInteger("State", (int)NPCState.Idle);
                m_State = NPCState.Idle;
                m_Target = null;
            }
        }
    }
}

//============================================================
// EDITOR
//============================================================

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyNpc))]
public class OpenMM8_NPC_AI_Enemy_Editor : OpenMM8_NPC_AI_Editor
{

}
#endif