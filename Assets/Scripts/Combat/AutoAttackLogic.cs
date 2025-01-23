using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AutoAttackLogic : MonoBehaviour
{
    [Tooltip("Reference to the parent BaseUnit that may define ATB cost, cooldown, etc.")]
    public BaseUnit parentUnit;

    private SphereCollider _sphereCollider;
    private float _nextAutoAttackTime;

    // Whether we're currently in the middle of an attack/rotation
    private bool _isAttacking = false;

    private void Start()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        if (_sphereCollider)
        {
            _sphereCollider.isTrigger = true;

            // Match the parent's autoAttackRange visually
            if (parentUnit != null)
            {
                _sphereCollider.radius = parentUnit.autoAttackRange;
            }
        }
        else
        {
            Debug.LogWarning($"{name} requires a SphereCollider set to isTrigger for auto-attack detection.", this);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Don't start a new attack if we're still attacking or on cooldown
        if (_isAttacking || Time.time < _nextAutoAttackTime) return;

        // Look for any HurtBox
        HurtBox hurtBox = other.GetComponent<HurtBox>();
        if (hurtBox == null) return; // Not a valid target

        // Ensure we have a valid parent unit
        if (parentUnit == null) return;

        // Prevent self-attacks
        if (hurtBox.ownerUnit == parentUnit) return;

        // Prevent friendly-fire (same faction)
        if (hurtBox.ownerUnit.Faction == parentUnit.Faction) return;

        // Check if we have enough ATB to attack
        if (ATBManager.Instance.GetATBAmount() >= parentUnit._ATBCombatCost)
        {
            // Start the attack routine (which rotates first, then spawns projectile)
            StartCoroutine(AttackRoutine(hurtBox));
        }
    }

    /// <summary>
    /// A coroutine that:
    /// 1) Rotates the parentUnit to face the enemy.
    /// 2) Spawns the projectile.
    /// 3) Applies cooldown & ATB cost.
    /// </summary>
    private IEnumerator AttackRoutine(HurtBox hurtBox)
    {
        _isAttacking = true;

        // 1) Deduct ATB cost up front
        ATBManager.Instance.PayATBCost(parentUnit._ATBCombatCost);

        // 2) Rotate the parentUnit to face the target's position
        Vector3 targetPos = hurtBox.transform.position;
        yield return StartCoroutine(RotateUnitTowards(targetPos));

        // 3) Spawn the projectile after rotation completes
        if (parentUnit.projectilePrefab != null && parentUnit.projectileSpawnParent != null)
        {
            // Instantiate in world space
            GameObject newProjectile = Instantiate(
                parentUnit.projectilePrefab,
                parentUnit.projectileSpawnParent.position,
                parentUnit.projectileSpawnParent.rotation
            );

            ProjectileHitBox projHitBox = newProjectile.GetComponent<ProjectileHitBox>();
            if (projHitBox != null)
            {
                // Direction from projectile spawn to hurtbox
                Vector3 directionToTarget =
                    (hurtBox.transform.position - parentUnit.projectileSpawnParent.position).normalized;
                projHitBox.travelDirection = directionToTarget;
                projHitBox.factionOwner = parentUnit.Faction;
            }
        }

        // 4) Set the cooldown
        _nextAutoAttackTime = Time.time + parentUnit.autoAttackCooldown;

        _isAttacking = false;
    }

    /// <summary>
    /// Coroutine to rotate the parentUnit so its +Y axis faces the target position 
    /// before shooting. Adjust logic if you want the negative Y or another axis to face forward.
    /// </summary>
    private IEnumerator RotateUnitTowards(Vector3 targetPos)
    {
        if (parentUnit == null) yield break;

        // Calculate direction on 2D plane
        Vector3 direction = targetPos - parentUnit.transform.position;
        direction.z = 0f; // ignore 3D for rotation

        // If there's some distance, rotate
        if (direction.sqrMagnitude > 0.0001f)
        {
            // Calc angle so +Y axis faces target => subtract 90 deg from the usual Atan2
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle -= 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

            // Rotate in place until within a small threshold
            while (Quaternion.Angle(parentUnit.transform.rotation, targetRotation) > 0.1f)
            {
                parentUnit.transform.rotation = Quaternion.RotateTowards(
                    parentUnit.transform.rotation,
                    targetRotation,
                    parentUnit.rotateSpeed * Time.deltaTime
                );
                yield return null;
            }

            // Snap to final rotation
            parentUnit.transform.rotation = targetRotation;
        }
    }
}
