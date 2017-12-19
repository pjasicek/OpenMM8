using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class OpenMM8_NPC_AI_Villager : OpenMM8_NPC_AI, OpenMM8_IObjectRangeListener
{
    // Use this for initialization
    void Start()
    {
        base.OnStart();
    }

    // Update is called once per frame
    void Update ()
    {
        if (!m_NavMeshAgent.enabled)
        {
            return;
        }

        m_CurrentWaypoint.SetActive(m_DrawWaypoint);

        /**** If NPC is a Villager ****/
        // 1) If it is moving, do nothing
        // 2) Check if a hostile unit is in range
        // 3a) If it is in range, then then move away from the closest hostile unit - Calculate angle have a minimal move distance
        // 3b) Else If it has idle timer, do nothing
        // 3c) Else move to a point within its patrol area
        // ----- [Event] OnDamaged - Start running from that unit if not already running from it
        // ----- [Event] OnEnemyEnteredAgroRange - Start running away from that unit

        if (m_IsPlayerInMeleeRange && !m_EnemiesInAgroRange.Contains(m_Player))
        {
            transform.LookAt(transform.position + m_Player.transform.rotation * Vector3.back, m_Player.transform.rotation * Vector3.up);
            m_Animator.SetInteger("State", (int)NPCState.Idle);
            return;
        }

        if (IsOnMove())
        {
            return;
        }

        if ((m_RemainingWanderIdleTime > 0.0f) && (m_EnemiesInAgroRange.Count == 0))
        {
            m_RemainingWanderIdleTime -= Time.deltaTime;
            // TODO: Change, need an "event" to tell me when he started being idle
            m_Animator.SetInteger("State", (int)NPCState.Idle);
            return;
        }

        if (m_EnemiesInAgroRange.Count > 0)
        {
            // Find closest hostile unit
            // Move away from that unit

            GameObject closestEnemy = m_EnemiesInAgroRange.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
            WanderAwayFromEnemy(closestEnemy);

            m_Animator.SetInteger("State", (int)NPCState.Walking);

            // I dont want to stop when running away from enemy, so this is left commented out as a reminder
            // m_RemainingWanderIdleTime = Random.Range(m_MinWanderIdleTime, m_MaxWanderIdleTime);
        }
        else
        {
            // Wander
            WanderWithinSpawnArea();
            m_Animator.SetInteger("State", (int)NPCState.Walking);

            m_RemainingWanderIdleTime = Random.Range(m_MinWanderIdleTime, m_MaxWanderIdleTime);
        }
    }

    // OpenMM8_IEventListener implementation

    public void OnObjectEnteredMeleeRange(GameObject other)
    {
        Debug.Log("Object entered melee range: " + other.name);

        if (other.name == "Player")
        {
            m_IsPlayerInMeleeRange = true;

            if (!m_HostilityResolver.IsHostileTo(other))
            {
                m_NavMeshAgent.isStopped = true;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                transform.LookAt(transform.position + other.transform.rotation * Vector3.back, other.transform.rotation * Vector3.up);
            }
        }
    }

    public void OnObjectLeftMeleeRange(GameObject other)
    {
        //Debug.Log("Object left melee range: " + other.name);

        if (other.name == "Player")
        {
            m_IsPlayerInMeleeRange = false;
            if (!m_HostilityResolver.IsHostileTo(other))
            {
                m_NavMeshAgent.ResetPath();
            }
        }
    }

    public void OnObjectEnteredAgroRange(GameObject other)
    {
        if (m_HostilityResolver.IsHostileTo(other))
        {
            if (m_EnemiesInAgroRange.Count == 0)
            {
                WanderAwayFromEnemy(other);
                m_Animator.SetInteger("State", (int)NPCState.Walking);
            }
            m_EnemiesInAgroRange.Add(other);
        }
    }

    public void OnObjectLeftAgroRange(GameObject other)
    {
        //Debug.Log("Object left agro range: " + other.name);

        if (m_HostilityResolver.IsHostileTo(other))
        {
            m_EnemiesInAgroRange.Remove(other);
        }
    }
}

//============================================================
// EDITOR
//============================================================

#if UNITY_EDITOR
[CustomEditor(typeof(OpenMM8_NPC_AI_Villager))]
public class OpenMM8_NPC_AI_Villager_Editor : OpenMM8_NPC_AI_Editor
{

}
#endif