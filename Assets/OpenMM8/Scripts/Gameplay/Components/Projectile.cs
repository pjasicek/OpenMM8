using UnityEngine;
using System.Collections;
using Assets.OpenMM8.Scripts.Gameplay;

// Can be spell, arrow, blaster particle, dragon breath attack
// basically anything that is moving from PointA to PointB and causes some action on impact
// For this reason, every projectile should have some "SpellType" associated with it
public class Projectile : MonoBehaviour
{
    public AttackInfo AttackInfo = null;
    public float Speed = 15.0f;
    public bool IsTargetPlayer = false;
    public GameObject Owner;

    // TODO: Properties below should be the only ones
    public SpellType SpellType = SpellType.None;
    public SkillMastery SkillMastery = SkillMastery.None;
    public int SkillLevel = 0;


    public void Shoot(/*Vector3 fromPoint, */Vector3 direction)
    {
        //Vector3 heading = (toPoint - fromPoint).normalized;
        //Vector3 speed = heading * Speed;
        Vector3 speed = direction.normalized * Speed;
        GetComponent<Rigidbody>().velocity = speed;
    }

    public void ShootFromParty(PlayerParty party, Vector3 direction)
    {
        transform.position = party.GetProjectileSpawnPos();
        transform.rotation = party.transform.rotation;
        Shoot(direction);
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter: " + other.name);

        if (other.isTrigger)
        {
            return;
        }

        if (Owner != null && Owner.Equals(other.gameObject))
        {
            return;
        }

        // I am trying to shoot player, but some other NPC intercepted my projectile
        if (IsTargetPlayer && other.gameObject.GetComponent<BaseNpc>() != null)
        {
            Destroy(gameObject);
            return;
        }

        Damageable damageable = other.gameObject.GetComponent<Damageable>();
        if (damageable != null)
        {
            damageable.ReceiveAttack(AttackInfo, Owner);
        }

        Destroy(gameObject);
    }
}
