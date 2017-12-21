using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(NpcData))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HostilityChecker))]

public abstract class BaseNpc : MonoBehaviour, ITriggerListener
{
    public enum NPCState { Walking, Idle, Attacking, ReceivingDamage, Dying, Dead }
    public enum HostilityType { Friendly, Hostile };

    //-------------------------------------------------------------------------
    // Variables
    //-------------------------------------------------------------------------

    // Public - Editor accessible
    public float m_StoppingDistance = 0.5f;

    public float m_MinWanderIdleTime = 1.0f;
    public float m_MaxWanderIdleTime = 2.0f;
    public float m_WanderRadius = 15.0f;

    public bool m_DrawWaypoint = true;

    public float m_AgroRange; // Agro on Y axis is not taken into account
    public float m_MeleeRange;

    public Vector3 m_SpawnPosition;

    // Private
    protected GameObject m_Player;

    protected Animator m_Animator;
    protected NavMeshAgent m_NavMeshAgent;
    protected NpcData m_Stats;
    protected HostilityChecker m_HostilityResolver;
    
    protected Vector3 m_CurrentDestination;

    protected float m_RemainingWanderIdleTime = 2.0f;

    protected GameObject m_CurrentWaypoint;

    protected NPCState m_State = NPCState.Idle;

    protected List<GameObject> m_EnemiesInMeleeRange = new List<GameObject>();
    protected List<GameObject> m_EnemiesInAgroRange = new List<GameObject>();

    protected GameObject m_Target;

    // State members
    protected string m_Faction;
    protected int m_FleeHealthPercantage;

    protected bool m_IsPlayerInMeleeRange = false;

    protected bool m_IsWalking = false;
    protected bool m_IsFleeing = false;
    protected bool m_IsAttacking = false;

    //-------------------------------------------------------------------------
    // Unity Overrides
    //-------------------------------------------------------------------------

    // Use this for initialization
    public void OnStart ()
    {
        m_Player = GameObject.FindWithTag("Player");
        if (m_Player == null)
        {
            Debug.LogError("Could not find \"Player\" in scene !");
        }

        m_SpawnPosition = transform.position;
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_Animator = GetComponent<Animator>();
        m_HostilityResolver = GetComponent<HostilityChecker>();

        // Create debug waypoint
        m_CurrentWaypoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_CurrentWaypoint.gameObject.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        m_CurrentWaypoint.GetComponent<Renderer>().material.color = Color.red;
        m_CurrentWaypoint.name = this.gameObject.name + " Waypoint";
        m_CurrentWaypoint.GetComponent<SphereCollider>().enabled = false;
    }

    /**** If NPC is a Guard ****/
    // 1) If it is attacking, do nothing (Waiting for AttackEnded frame event)
    // 2) If it is moving, do nothing (May be interrupted if enemy enters its melee range)
    // 3) If it has hostile unit(s) in range, move to its closest one
    // 4) Else If this unit can Patrol, move to its point within patrol area
    // 5) Else do nothing (Idle)
    // ----- [Event] OnAttackEnded - after attack ends, it will check if it is within melee range of any hostile unit,
    //                           if it is, then it will attack it again, if it is not, it will choose some strafe
    //                           location - e.g. Shoot - Move - Shoot - Move, etc.
    // ------ [Event] If enemy enters its attack range, it will attack immediately
    // ------ [Event] OnDamaged - If it was attacked by a unit which was previously friendly, change this unit to Hostile
    //                            and query all nearby Guards / Villagers of the same affiliation to be hostile towards
    //                            that unit too

    

    //-------------------------------------------------------------------------
    // Methods
    //-------------------------------------------------------------------------

    public bool IsOnMove()
    {
        if (!m_NavMeshAgent.pathPending)
        {
            if (m_NavMeshAgent.remainingDistance <= m_NavMeshAgent.stoppingDistance)
            {
                m_NavMeshAgent.SetDestination(transform.position);
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                return false;
            }
        }

        return true;
    }

    public void WanderWithinSpawnArea()
    {
        m_CurrentDestination = m_SpawnPosition + new Vector3(
            UnityEngine.Random.Range((int) - m_WanderRadius * 0.5f - 2, (int)m_WanderRadius * 0.5f + 2), 
            0,
            UnityEngine.Random.Range((int) - m_WanderRadius * 0.5f - 2, (int)m_WanderRadius * 0.5f + 2));
        m_NavMeshAgent.ResetPath();

        m_NavMeshAgent.SetDestination(m_CurrentDestination);

        m_CurrentWaypoint.transform.position = m_CurrentDestination;

        Vector3 direction = (m_CurrentDestination - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.Slerp(transform.rotation, qDir, Time.deltaTime * rotSpeed);
    }

    public void WanderAwayFromEnemy(GameObject enemy)
    {
        Vector3 heading = enemy.transform.position - transform.position;
        heading.Normalize();

        m_CurrentDestination = transform.position - heading * 6.0f;
        m_NavMeshAgent.ResetPath();
        m_NavMeshAgent.SetDestination(m_CurrentDestination);

        m_CurrentWaypoint.transform.position = m_CurrentDestination;

        Vector3 direction = (m_CurrentDestination - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void ChaseTarget(GameObject target)
    {
        m_Target = target;

        m_CurrentDestination = target.transform.position;
        m_NavMeshAgent.ResetPath();

        m_NavMeshAgent.SetDestination(m_CurrentDestination);

        m_CurrentWaypoint.transform.position = m_CurrentDestination;

        Vector3 direction = (m_CurrentDestination - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void StopMoving()
    {
        m_NavMeshAgent.ResetPath();
    }

    public void TurnToObject(GameObject go)
    {
        if (go.CompareTag("Player"))
        {
            transform.LookAt(transform.position + go.transform.rotation * Vector3.back, go.transform.rotation * Vector3.up);
        }
        else
        {
            transform.LookAt(go.transform);
        }
    }

    public void OnObjectEnteredMyTrigger(GameObject other, TriggerType triggerType)
    {
        switch (triggerType)
        {
            case TriggerType.MeleeRange:
                OnObjectEnteredMeleeRange(other);
                break;

            case TriggerType.AgroRange:
                OnObjectEnteredAgroRange(other);
                break;

            default:
                Debug.LogError("Unhandled Trigger Type: " + triggerType);
                break;
        }
    }

    public void OnObjectLeftMyTrigger(GameObject other, TriggerType triggerType)
    {
        switch (triggerType)
        {
            case TriggerType.MeleeRange:
                OnObjectLeftMeleeRange(other);
                break;

            case TriggerType.AgroRange:
                OnObjectLeftAgroRange(other);
                break;

            default:
                Debug.LogError("Unhandled Trigger Type: " + triggerType);
                break;
        }
    }

    abstract public void OnObjectEnteredMeleeRange(GameObject other);
    abstract public void OnObjectEnteredAgroRange(GameObject other);

    abstract public void OnObjectLeftMeleeRange(GameObject other);
    abstract public void OnObjectLeftAgroRange(GameObject other);
}

//============================================================
// EDITOR
//============================================================

#if UNITY_EDITOR
[CustomEditor(typeof(BaseNpc))]
public class OpenMM8_NPC_AI_Editor : Editor
{
    BaseNpc m_TargetObject;

    public void OnSceneGUI()
    {
        m_TargetObject = this.target as BaseNpc;

        Handles.color = new Color(0, 1.0f, 0, 0.1f);
        if (EditorApplication.isPlaying)
        {
            Handles.DrawSolidDisc(m_TargetObject.m_SpawnPosition, Vector3.up, m_TargetObject.m_WanderRadius);
        }
        else
        {
            Handles.DrawSolidDisc(m_TargetObject.transform.position, Vector3.up, m_TargetObject.m_WanderRadius);
        }
    }
}
#endif