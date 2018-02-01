using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class VillagerNpc : BaseNpc
{
    // Use this for initialization
    void Start()
    {
        base.OnStart();
    }

    // Update is called once per frame
    void Update ()
    {
        NpcState currState = (NpcState)Animator.GetInteger("State");
        if ((currState == NpcState.Dead) || (currState == NpcState.Dying))
        {
            return;
        }

        if (!NavMeshAgent.enabled)
        {
            return;
        }

        CurrentWaypoint.SetActive(DrawWaypoint);

        /**** If NPC is a Villager ****/
        // 1) If it is moving, do nothing
        // 2) Check if a hostile unit is in range
        // 3a) If it is in range, then then move away from the closest hostile unit - Calculate angle have a minimal move distance
        // 3b) Else If it has idle timer, do nothing
        // 3c) Else move to a point within its patrol area
        // ----- [Event] OnDamaged - Start running from that unit if not already running from it
        // ----- [Event] OnEnemyEnteredAgroRange - Start running away from that unit

        if (IsPlayerInMeleeRange && !HostilityResolver.IsHostileTo(Player) && EnemiesInAgroRange.Count == 0)
        {
            transform.LookAt(transform.position + Player.transform.rotation * Vector3.back, Player.transform.rotation * Vector3.up);
            Animator.SetInteger("State", (int)NpcState.Idle);
        }
        else if (IsWalking())
        {
            // Walking - either wandering or fleeing, let it reach its destination
        }
        else if ((RemainingWanderIdleTime > 0.0f) && (EnemiesInAgroRange.Count == 0))
        {
            // Villager has some time left to be idle

            RemainingWanderIdleTime -= Time.deltaTime;
            // TODO: Change, need an "event" to tell me when he started being idle
            Animator.SetInteger("State", (int)NpcState.Idle);
        }
        else if (EnemiesInAgroRange.Count > 0)
        {
            // Find closest hostile unit
            // Move away from that unit

            GameObject closestEnemy = EnemiesInAgroRange.OrderBy(t => (t.transform.position - transform.position).sqrMagnitude).FirstOrDefault();
            WanderAwayFromEnemy(closestEnemy);

            Animator.SetInteger("State", (int)NpcState.Walking);

            // I dont want to stop when running away from enemy, so this is left commented out as a reminder
            // m_RemainingWanderIdleTime = Random.Range(m_MinWanderIdleTime, m_MaxWanderIdleTime);
        }
        else
        {
            // Wander
            WanderWithinSpawnArea(WanderRadius);
            Animator.SetInteger("State", (int)NpcState.Walking);

            RemainingWanderIdleTime = Random.Range(MinWanderIdleTime, MaxWanderIdleTime);
        }

        State = (NpcState)Animator.GetInteger("State");
    }

    // Trigger events

    public override void OnObjectEnteredMeleeRange(GameObject other)
    {
        if (other.name == "Player")
        {
            IsPlayerInMeleeRange = true;

            if (!HostilityResolver.IsHostileTo(other))
            {
                NavMeshAgent.isStopped = true;
                GetComponent<Rigidbody>().velocity = Vector3.zero;
                transform.LookAt(transform.position + other.transform.rotation * Vector3.back, other.transform.rotation * Vector3.up);
            }
        }
    }

    public override void OnObjectLeftMeleeRange(GameObject other)
    {
        if (other.name == "Player")
        {
            IsPlayerInMeleeRange = false;
            if (!HostilityResolver.IsHostileTo(other))
            {
                NavMeshAgent.ResetPath();
            }
        }
    }

    public override void OnObjectEnteredAgroRange(GameObject other)
    {
        if (HostilityResolver.IsHostileTo(other))
        {
            if (EnemiesInAgroRange.Count == 0)
            {
                WanderAwayFromEnemy(other);
                Animator.SetInteger("State", (int)NpcState.Walking);
            }
            EnemiesInAgroRange.Add(other);
        }
    }

    public override void OnObjectLeftAgroRange(GameObject other)
    {
        if (HostilityResolver.IsHostileTo(other))
        {
            EnemiesInAgroRange.Remove(other);
        }
    }
}