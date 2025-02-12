using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Character
{
    public GameObject projectilePrefab;
    public Transform projectileSpawnParent;

    public override void Ability1()
    {
        Debug.Log("Pawn uses Ability1.");
    }

    public override void Ability2()
    {
        Debug.Log("Pawn uses Ability2.");
    }

    public override void TryAutoAttack(HurtBox hurtBox)
    {
        // Skip if currently attacking or on cooldown
        if (isAttacking || Time.time < nextAutoAttackTime) return;

        // Self-attack or same faction? Skip
        if (hurtBox.ownerUnit == this) return;
        if (hurtBox.ownerUnit.Faction == this.Faction) return;

        // Check ATB
        if (ATBManager.Instance.GetATBAmount() < _ATBCombatCost) return;

        // Start the coroutine to handle rotation + projectile spawn
        StartCoroutine(AutoAttackRoutine(hurtBox));
    }

    private IEnumerator AutoAttackRoutine(HurtBox hurtBox)
    {
        isAttacking = true;

        // 1) Pay cost
        ATBManager.Instance.PayATBCost(_ATBCombatCost);

        // 2) Rotate toward the target BEFORE spawning the projectile
        yield return RotateUnitTowards(hurtBox.transform.position);

        // 3) Spawn projectile
        if (projectilePrefab != null && projectileSpawnParent != null)
        {
            GameObject newProjectile = Instantiate(
                projectilePrefab,
                projectileSpawnParent.position,
                projectileSpawnParent.rotation
            );

            ProjectileHitBox projHitBox = newProjectile.GetComponent<ProjectileHitBox>();
            if (projHitBox != null)
            {
                Vector3 dirToTarget =
                    (hurtBox.transform.position - projectileSpawnParent.position).normalized;
                projHitBox.travelDirection = dirToTarget;
                projHitBox.factionOwner = this.Faction;
            }
        }

        // 4) Apply cooldown
        nextAutoAttackTime = Time.time + autoAttackCooldown;

        isAttacking = false;
        yield break;
    }

   
}
