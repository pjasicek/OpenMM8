using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using System;

using Assets.OpenMM8.Scripts.Gameplay;
using Assets.OpenMM8.Scripts.Gameplay.Data;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(NavMeshObstacle))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(HostilityChecker))]
[RequireComponent(typeof(SpriteLookRotator))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Damageable))]

public abstract class BaseNpc : MonoBehaviour, ITriggerListener
{
    public enum NpcState { Walking, Idle, Attacking, ReceivingDamage, Dying, Dead, None }
    public enum HostilityType { Friendly, Hostile };

    //-------------------------------------------------------------------------
    // Variables
    //-------------------------------------------------------------------------

    public NpcType NpcType;
    public NpcData NpcData;

    // Public - Editor accessible
    public float StoppingDistance = 0.5f;

    public bool DoWander = false;
    public float MinWanderIdleTime = 1.0f;
    public float MaxWanderIdleTime = 2.0f;
    public float WanderRadius = 15.0f;

    public bool DrawWaypoint = true;

    public float UpdateIntervalMs = 50.0f;

    public AudioClip AttackSound;
    public AudioClip DeathSound;
    public AudioClip AwareSound;
    public AudioClip WinceSound;

    public Sprite PreviewImage;

    /*public float m_AgroRange; // Agro on Y axis is not taken into account
    public float m_MeleeRange;*/

    public Vector3 SpawnPosition;

    // Private
    protected GameObject Player;

    protected Animator Animator;
    protected NavMeshAgent NavMeshAgent;
    protected NavMeshObstacle NavMeshObstacle;
    protected HostilityChecker HostilityResolver;
    protected SpriteLookRotator SpriteLookRotator;
    protected AudioSource AudioSource;
    
    protected Vector3 CurrentDestination;

    protected float RemainingWanderIdleTime = 2.0f;

    protected GameObject CurrentWaypoint;

    protected NpcState State = NpcState.Idle;

    protected List<GameObject> EnemiesInMeleeRange = new List<GameObject>();
    // Agro range is also Ranged range for archers/casters
    protected List<GameObject> EnemiesInAgroRange = new List<GameObject>();

    protected GameObject Target;

    // State members
    protected string Faction;
    protected int FleeHealthPercantage;
    protected bool IsFleeing = false;

    protected bool IsPlayerInMeleeRange = false;

    //-------------------------------------------------------------------------
    // Unity Overrides
    //-------------------------------------------------------------------------

    public void Awake()
    {
        SpawnPosition = transform.position;
        NavMeshAgent = GetComponent<NavMeshAgent>();
        NavMeshAgent.enabled = false; // Supress warnigns
        NavMeshObstacle = GetComponent<NavMeshObstacle>();
        NavMeshObstacle.enabled = false; // Supress warnigns
        Animator = GetComponent<Animator>();
        HostilityResolver = GetComponent<HostilityChecker>();
        SpriteLookRotator = GetComponent<SpriteLookRotator>();
        AudioSource = GetComponent<AudioSource>();
    }

    // Use this for initialization
    public void OnStart ()
    {
        Player = GameObject.FindWithTag("Player");
        if (Player == null)
        {
            Debug.LogError("Could not find \"Player\" in scene !");
        }

        // Create debug waypoint
        CurrentWaypoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        CurrentWaypoint.gameObject.transform.localScale = new Vector3(0.75f, 0.75f, 0.75f);
        CurrentWaypoint.GetComponent<Renderer>().material.color = Color.red;
        CurrentWaypoint.name = this.gameObject.name + " Waypoint";
        CurrentWaypoint.GetComponent<SphereCollider>().enabled = false;

        CurrentWaypoint.SetActive(DrawWaypoint);

        SetNavMeshAgentEnabled(true);

        NpcData = GameMgr.Instance.NpcDb.GetNpcData(NpcType);

        //m_NavMeshAgent
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
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                return false;
            }
        }

        return true;
    }

    public void WanderWithinSpawnArea(float wanderRadius)
    {
        SetNavMeshAgentEnabled(true);

        CurrentDestination = SpawnPosition + new Vector3(
            UnityEngine.Random.Range((int) - wanderRadius * 0.5f - 2, (int)wanderRadius * 0.5f + 2), 
            0,
            UnityEngine.Random.Range((int) - wanderRadius * 0.5f - 2, (int)wanderRadius * 0.5f + 2));
        NavMeshAgent.ResetPath();

        NavMeshAgent.SetDestination(CurrentDestination);

        CurrentWaypoint.transform.position = CurrentDestination;

        Vector3 direction = (CurrentDestination - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
        //transform.rotation = Quaternion.Slerp(transform.rotation, qDir, Time.deltaTime * rotSpeed);
    }

    public void WanderAwayFromEnemy(GameObject enemy)
    {
        SetNavMeshAgentEnabled(true);

        Vector3 heading = enemy.transform.position - transform.position;
        heading.Normalize();

        float randRotMod = UnityEngine.Random.Range(-20.0f, 20.0f);
        //randRotMod = 90.0f;
        heading = Quaternion.AngleAxis(randRotMod, Vector3.up) * heading;

        CurrentDestination = transform.position - heading * 6.0f;
        NavMeshAgent.ResetPath();
        NavMeshAgent.SetDestination(CurrentDestination);

        CurrentWaypoint.transform.position = CurrentDestination;

        Vector3 direction = (CurrentDestination - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    public void ChaseTarget(GameObject target)
    {
        SetNavMeshAgentEnabled(true);

        Target = target;

        CurrentDestination = target.transform.position;
        NavMeshAgent.ResetPath();

        NavMeshAgent.SetDestination(CurrentDestination);

        CurrentWaypoint.transform.position = CurrentDestination;

        Vector3 direction = (CurrentDestination - transform.position).normalized;
        transform.rotation = Quaternion.LookRotation(direction);

        Animator.SetInteger("State", (int)NpcState.Walking);
    }

    public void StopMoving()
    {
        SetNavMeshAgentEnabled(true);
        NavMeshAgent.ResetPath();
    }

    public void SetNavMeshAgentEnabled(bool enabled)
    {
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
            SpriteLookRotator.OnLookDirectionChanged(SpriteLookRotator.LookDirection.Front);
        }
        else
        {
            transform.LookAt(go.transform);
            SpriteLookRotator.AlignRotation();
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