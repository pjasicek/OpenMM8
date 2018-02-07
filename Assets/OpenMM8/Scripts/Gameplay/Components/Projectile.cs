using UnityEngine;
using System.Collections;
using Assets.OpenMM8.Scripts.Gameplay;

public class Projectile : MonoBehaviour
{
    public AttackInfo AttackInfo = null;
    public float Speed = 15.0f;
    public bool IsTargetPlayer = false;
    public GameObject Owner;

    public void Shoot(Vector3 fromPoint, Vector3 toPoint)
    {
        Vector3 heading = (toPoint - fromPoint).normalized;
        Vector3 speed = heading * Speed;
        GetComponent<Rigidbody>().velocity = speed;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigget enter: " + other.name);

        if (other.isTrigger)
        {
            return;
        }

        if (IsTargetPlayer && other.gameObject.layer == LayerMask.NameToLayer("NPC"))
        {
            Destroy(gameObject);
            return;
        }
        else if (!IsTargetPlayer && other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
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
