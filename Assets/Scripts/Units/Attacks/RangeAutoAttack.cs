using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/RangeAutoAttack")]
public class RangeAutoAttack : Attack
{
    public GameObject projectilePrefab; // Projectile prefab to shoot
    public Transform projectileSpawnPoint; // Point from which projectile is fired

    public override void PerformAttack(Character attacker, HurtBox target)
    {
        if (attacker.isAttacking || Time.time < attacker.nextAutoAttackTime)
        {
            return; // Skip if already attacking or on cooldown
        }

        if (ATBManager.Instance.GetATBAmount() < attacker._ATBCombatCost)
        {
            return; // Skip if not enough ATB
        }

        // Make sure to set the attack spawn point position
        SetAttackSpawnPoint(attacker);

        // Start the attack coroutine
        attacker.StartCoroutine(AutoAttackRoutine(attacker, target));
    }

    private void SetAttackSpawnPoint(Character attacker)
    {
        // Search for the AttackSpawnPoint component in the children of the attacker object
        AttackSpawnPoint attackSpawnPoint = attacker.GetComponentInChildren<AttackSpawnPoint>();
        if (attackSpawnPoint != null)
        {
            // Set the projectileSpawnPoint to the found AttackSpawnPoint's position
            projectileSpawnPoint = attackSpawnPoint.transform;
            Debug.Log("Attack spawn point found and set.");
        }
        else
        {
            Debug.LogWarning("No AttackSpawnPoint found in the character's children.");
        }
    }

    private IEnumerator AutoAttackRoutine(Character attacker, HurtBox target)
    {
        attacker.isAttacking = true;

        // 1) Pay the ATB cost for attacking
        ATBManager.Instance.PayATBCost(attacker._ATBCombatCost);

        // 2) Rotate the attacker towards the target
        yield return attacker.RotateUnitTowards(target.transform.position); // Ensure this is in the Character class

        // 3) Spawn the projectile
        if (projectilePrefab != null && projectileSpawnPoint != null)
        {
            GameObject projectile = Instantiate(projectilePrefab, projectileSpawnPoint.position, projectileSpawnPoint.rotation);
            ProjectileHitBox hitBox = projectile.GetComponent<ProjectileHitBox>();
            if (hitBox != null)
            {
                Vector3 direction = (target.transform.position - projectileSpawnPoint.position).normalized;
                hitBox.travelDirection = direction;
                hitBox.factionOwner = attacker.Faction;
            }
        }

        // 4) Set the cooldown time
        attacker.nextAutoAttackTime = Time.time + attacker.autoAttackCooldown;
        attacker.isAttacking = false;
    }
}
