using UnityEngine;
using System.Collections;
using Assets.OpenMM8.Scripts.Gameplay;
using UnityStandardAssets.Utility;

// Can be spell, arrow, blaster particle, dragon breath attack
// basically anything that is moving from PointA to PointB and causes some action on impact
// For this reason, every projectile should have some "SpellType" associated with it
public class Projectile : MonoBehaviour
{
    public ProjectileInfo ProjectileInfo;

    public Character ShooterAsCharacter = null;
    public Monster ShooterAsMonster = null;

    public PlayerParty TargetAsPlayerParty = null;
    public Monster TargetAsMonster = null;

    static public void Spawn(ProjectileInfo projectileInfo)
    {
        GameObject projectileObject = (GameObject)Instantiate(Resources.Load("Prefabs/TestSpellProjectile"));
        Projectile projectile = projectileObject.GetComponent<Projectile>();
        projectile.ProjectileInfo = projectileInfo;

        if (projectileInfo.Shooter != null)
        {
            projectile.ShooterAsCharacter = projectileInfo.Shooter as Character;
            projectile.ShooterAsMonster = projectileInfo.Shooter as Monster;
        }
        if (projectileInfo.Target != null)
        {
            projectile.TargetAsPlayerParty = projectileInfo.Target as PlayerParty;
            projectile.TargetAsMonster = projectileInfo.Target as Monster;
        }

        projectileObject.transform.rotation = projectileInfo.ShooterTransform.rotation;
        if (projectile.ShooterAsCharacter != null)
        {
            int characterIndex = projectile.ShooterAsCharacter.GetPartyIndex();
            projectileObject.transform.position =
                projectile.ShooterAsCharacter.Party.GetProjectileSpawnPos(characterIndex);
        }
        else if (projectile.ShooterAsMonster != null)
        {
            float monsterHeight = projectile.ShooterAsMonster.GetComponent<CapsuleCollider>().height;
            projectileObject.transform.position = projectile.ShooterAsMonster.transform.position +
                new Vector3(0.0f, monsterHeight / 4.0f, 0.0f);
        }
        else
        {
            projectileObject.transform.position = projectileInfo.ShooterTransform.position;
        }

        SpriteObject projectileAnim = SpriteObjectRegistry.GetSpriteObject(projectileInfo.DisplayData.SFTLabel);
        projectileObject.GetComponent<SpriteBillboardAnimator>().SetAnimation(projectileAnim);

        Vector3 targetDirection = (projectileInfo.TargetPosition - projectileObject.transform.position).normalized;
        //Vector3 targetDirection = projectileInfo.TargetDirection;
        Vector3 speed = targetDirection * projectileInfo.DisplayData.Speed;
        projectileObject.GetComponent<Rigidbody>().velocity = speed;

        CapsuleCollider collider = projectileObject.GetComponent<CapsuleCollider>();
        collider.height = projectileInfo.DisplayData.Height;
        collider.radius = projectileInfo.DisplayData.Radius;

        if (projectileInfo.DisplayData.IsLifetimeInSFT)
        {
            float lifetime = projectileAnim.TotalAnimationLengthSeconds;
            projectile.SetLifetime(lifetime);
        }
        else if (projectileInfo.DisplayData.Lifetime > 0.0f)
        {
            projectile.SetLifetime(projectileInfo.DisplayData.Lifetime);
        }
        else
        {
            Debug.LogError("Projectile has infite lifetime");
        }
    }

    private void SetLifetime(float lifetime)
    {
        Invoke("DestroyNow", lifetime);
    }

    private void DestroyNow()
    {
        transform.DetachChildren();
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Trigger enter: " + other.name);

        if (other.isTrigger)
        {
            Debug.LogError("Other is trigger, wont collide");
            return;
        }

        // Cannot shoot self
        if (ProjectileInfo.ShooterTransform != null &&
            ProjectileInfo.ShooterTransform.gameObject.Equals(other.gameObject))
        {
            return;
        }


        Monster victimAsMonster = other.gameObject.GetComponent<Monster>();
        PlayerParty victimAsPlayer = other.gameObject.GetComponent<PlayerParty>();
        if (ShooterAsMonster != null && victimAsMonster != null)
        {
            // I am trying to shoot player, but some other NPC intercepted my projectile

            bool areMonstersFriendly = ShooterAsMonster.GetRelationTo(victimAsMonster) == 0;
            if (areMonstersFriendly)
            {
                // Monster are friendly, so just destroy projectile
                // I dont think that friendly fire was allowed in original game
                Destroy(gameObject);
                return;
            }
            else
            {
                // Damage monster
            }
        }
        else if (ShooterAsMonster != null && victimAsPlayer != null)
        {
            // Monster shot player

            // Damage player
            Debug.Log(ShooterAsMonster.Name + " -> " + victimAsPlayer.name);
            ShooterAsMonster.DealDamageToPlayer(victimAsPlayer, 
                ProjectileInfo.MonsterAttackType, 
                this);
        }
        else if (ShooterAsCharacter != null && victimAsMonster != null)
        {
            // Player shot monster

            // Damage monster
            Debug.Log(ShooterAsCharacter.Name + " -> " + victimAsMonster.Name);

            victimAsMonster.ReceiveDamageFromPlayer(ShooterAsCharacter, this);
        }

        if (ProjectileInfo.ImpactObject != null &&
            !string.IsNullOrEmpty(ProjectileInfo.ImpactObject.SFTLabel) &&
            ProjectileInfo.ImpactObject.SFTLabel.ToLower() != "null")
        {
            //Debug.LogError("Spawning impact object");
            Vector3 impactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);
            //impactPoint += new Vector3(0.0f, 0.0f, GetComponent<CapsuleCollider>().height / 2.0f);


            RaycastHit hit;
            Vector3 tmpPos = transform.position + transform.forward * -5.0f;
            if (Physics.Raycast(tmpPos, transform.forward, out hit))
            {
                impactPoint = hit.point;
            }

            /*Vector3 tmpDirection = (other.transform.position - transform.position);
            Vector3 tmpContactPoint = transform.position + tmpDirection;
            impactPoint = tmpContactPoint;*/

            impactPoint += transform.forward * -0.2f;

            OutdoorSpriteEffect.Spawn(impactPoint, transform.rotation, ProjectileInfo.ImpactObject);
        }

        Destroy(gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        foreach (ContactPoint contact in collision.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
    }
}