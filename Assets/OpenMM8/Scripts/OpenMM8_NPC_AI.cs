using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

using System.Linq;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(BoxCollider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(OpenMM8_NPC_Stats))]

public class OpenMM8_NPC_AI : MonoBehaviour
{
    public enum NPCType { Villager, Guard, Enemy }
    public enum NPCState { Idle, Walking, MeleeAttacking, RangedAttacking, Stunned, Dying, Dead, Fidgeting }

    //-------------------------------------------------------------------------
    // Variables
    //-------------------------------------------------------------------------

    // Public - Editor accessible
    public float m_StoppingDistance = 0.5f;

    public float m_MinWanderIdleTime = 1.0f;
    public float m_MaxWanderIdleTime = 2.0f;
    public float m_WanderRadius = 15.0f;

    bool m_DrawWaypoint = true;

    public float m_AgroRange; // Agro on Y axis is not taken into account
    public float m_MeleeRange;

    public NPCType m_NPCType = NPCType.Villager;

    // Private
    private OpenMM8_NPC_Stats m_Stats;

    public Vector3 m_SpawnPosition;
    private NavMeshAgent m_NavMeshAgent;
    private Vector3 m_CurrentDestination;

    private float m_RemainingWanderIdleTime = 2.0f;

    private GameObject m_CurrentWaypoint;

    private NPCState m_State = NPCState.Idle;

    private List<GameObject> m_EnemiesInMeleeRange = new List<GameObject>();
    private List<GameObject> m_EnemiesInAgroRange = new List<GameObject>();

    private GameObject m_Target;

    // State members
    string m_Faction;
    int m_FleeHealthPercantage;

    bool m_CanPatrol;
    bool m_CanAttack;

    //-------------------------------------------------------------------------
    // Unity Overrides
    //-------------------------------------------------------------------------

    // Use this for initialization
    void Start ()
    {
        m_SpawnPosition = transform.position;
        m_NavMeshAgent = GetComponent<NavMeshAgent>();

        // Create debug waypoint
        m_CurrentWaypoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        m_CurrentWaypoint.gameObject.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        m_CurrentWaypoint.GetComponent<Renderer>().material.color = Color.red;
        m_CurrentWaypoint.name = this.gameObject.name + " Waypoint";
        m_CurrentWaypoint.GetComponent<SphereCollider>().enabled = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!m_NavMeshAgent.enabled)
        {
            return;
        }

        m_CurrentWaypoint.SetActive(m_DrawWaypoint);

        if (m_NPCType == NPCType.Villager)
        {
            /**** If NPC is a Villager ****/
            // 1) If it is moving, do nothing
            // 2) Check if a hostile unit is in range
            // 3a) If it is in range, then then move away from the closest hostile unit - Calculate angle have a minimal move distance
            // 3b) Else If it has idle timer, do nothing
            // 3c) Else move to a point within its patrol area
            // ----- [Event] OnDamaged - Start running from that unit if not already running from it
            // ----- [Event] OnEnemyEnteredAgroRange - Start running away from that unit

            // 1)
            if (IsOnMove())
            {
                return;
            }

            if (m_RemainingWanderIdleTime > 0.0f)
            {
                m_RemainingWanderIdleTime -= Time.deltaTime;
                return;
            }

            if (m_EnemiesInAgroRange.Count > 0)
            {
                // Find closest hostile unit
                // Move away from that unit

                GameObject closestEnemy = m_EnemiesInAgroRange.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
                WanderAwayFromEnemy(closestEnemy);

                // I dont want to stop when running away from enemy, so this is left commented out as a reminder
                // m_RemainingWanderIdleTime = Random.Range(m_MinWanderIdleTime, m_MaxWanderIdleTime);
            }
            else
            {
                // Wander
                WanderWithinSpawnArea();
                m_RemainingWanderIdleTime = Random.Range(m_MinWanderIdleTime, m_MaxWanderIdleTime);
            }
        }
        else if (m_NPCType == NPCType.Guard)
        {
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
        }
        else if (m_NPCType == NPCType.Enemy)
        {
            /**** If NPC is Enemy ****/
            // 1) If it is attacking, do nothing (Waiting for AttackEnded frame event)
            // 2) If it is moving, do nothing (May be interrupted if enemy enters its melee range)
            // ----- [Event] OnAttackEnded - after attack ends, it will check if it is within melee range of any hostile unit,
            //                           if it is, then it will attack it again, if it is not, it will choose some strafe
            //                           location - e.g. Shoot - Move - Shoot - Move, etc.
            // ------ [Event] If enemy enters its attack range, it will attack immediately
        }
	}

    //-------------------------------------------------------------------------
    // Methods
    //-------------------------------------------------------------------------

    private bool IsOnMove()
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

    private void WanderWithinSpawnArea()
    {
        m_CurrentDestination = m_SpawnPosition + new Vector3(Random.Range((int) - m_WanderRadius * 0.5f - 2, (int)m_WanderRadius * 0.5f + 2), 0, Random.Range((int) - m_WanderRadius * 0.5f - 2, (int)m_WanderRadius * 0.5f + 2));
        m_NavMeshAgent.ResetPath();

        m_NavMeshAgent.SetDestination(m_CurrentDestination);

        m_CurrentWaypoint.transform.position = m_CurrentDestination;
    }

    private void WanderAwayFromEnemy(GameObject enemy)
    {
        // TODO
    }
}
