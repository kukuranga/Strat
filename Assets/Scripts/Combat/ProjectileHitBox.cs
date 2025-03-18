using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A specialized HitBox that moves in a set direction for a set time.
/// </summary>
public class ProjectileHitBox : HitBox
{
    [Header("Projectile Settings")]
    [Tooltip("Projectile's speed in units/second.")]
    public float speed = 5f;

    [Tooltip("How long the projectile lives before despawning (seconds).")]
    public float lifeTime = 3f;

    [Tooltip("Direction (local or world space) in which the projectile travels.")]
    public Vector3 travelDirection = Vector3.forward;

    [Tooltip("The target unit for the projectile (optional).")]
    public BaseUnit targetUnit; // Added to support targeting a specific BaseUnit

    private float _timer = 0f;

    private void Start()
    {
        // If a target unit is assigned, set the initial travel direction toward the target
        if (targetUnit != null)
        {
            travelDirection = (targetUnit.transform.position - transform.position).normalized;
        }

        // Normalize the travel direction just in case
        travelDirection.Normalize();

        // Point the projectile to face its travel direction
        transform.forward = travelDirection;
    }

    private void FixedUpdate()
    {
        // Move the projectile forward (based on transform.forward) 
        transform.position += transform.forward * (speed * Time.deltaTime);

        // Track time to destroy after 'lifeTime'
        _timer += Time.deltaTime;
        if (_timer >= lifeTime)
        {
            AfterHitEffect();  // Calls Destroy(gameObject) from the base class
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has a HurtBox
        HurtBox hurtBox = other.GetComponent<HurtBox>();
        if (hurtBox != null && hurtBox.ownerUnit != null)
        {
            // Skip if the hurtBox belongs to the same faction as this hitbox
            if (hurtBox.ownerUnit.Faction == factionOwner) return;

            // Only deal damage if we haven't hit this HurtBox yet
            if (!_hitTargets.Contains(hurtBox))
            {
                _hitTargets.Add(hurtBox);
                hurtBox.OnHit(defaultDamage, this);

                // If the projectile has a target unit and it hits that unit, destroy the projectile
                if (targetUnit != null && hurtBox.ownerUnit == targetUnit)
                {
                    AfterHitEffect();
                }
            }
        }
    }
}