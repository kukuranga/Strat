using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Character
{
    [Header("Pawn Attributes")]
    public GameObject projectilePrefab;
    public Transform projectileSpawnParent;

    protected override void Update()
    {
        base.Update();
    }

    public override void Ability1()
    {
        Debug.Log("Pawn uses Ability1.");
    }

    public override void Ability2()
    {
        Debug.Log("Pawn uses Ability2.");
    }

    // Updated TryAutoAttack to accept a BaseUnit target
    public override void TryAutoAttack(BaseUnit target)
    {
        // Skip if currently attacking or on cooldown
        if (isAttacking || Time.time < nextAutoAttackTime) return;

        // Self-attack or same faction? Skip
        if (target == this) return;
        //if (target.Faction == this.Faction) return;
        if ((_Targets & target.Faction) == 0)
        {
            Debug.Log($"{UnitName} cannot attack {target.UnitName} (faction not in targets)");
            return;
        }

        // Check ATB
        if (ATBManager.Instance.GetATBAmount() < _ATBCombatCost) return;

        // Start the coroutine to handle rotation + projectile spawn
        StartCoroutine(AutoAttackRoutine(target));
    }

    private IEnumerator AutoAttackRoutine(BaseUnit target)
    {
        isAttacking = true;

        // 1) Pay cost
        ATBManager.Instance.PayATBCost(_ATBCombatCost);

        // 2) Rotate toward the target BEFORE spawning the projectile
        yield return RotateUnitTowards(target.transform.position);

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
                // Set the initial direction toward the target
                projHitBox.travelDirection = (target.transform.position - projectileSpawnParent.position).normalized;
                projHitBox.factionOwner = this.Faction;
                projHitBox.targetUnit = target; // Assign the target unit (optional)
            }
        }

        // 4) Apply cooldown
        nextAutoAttackTime = Time.time + autoAttackCooldown;

        isAttacking = false;
        yield break;
    }
}