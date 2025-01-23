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

    private float _timer = 0f;

    private void Start()
    {
        // Normalize the travel direction just in case
        travelDirection.Normalize();

        // Point the projectile to face its travel direction
        // This sets the GameObject's forward vector to match travelDirection.
        // If you want to manually rotate it in the editor, you can remove or adjust this line.
        transform.forward = travelDirection;
    }

    private void Update()
    {
        // If you want the projectile to be able to hit multiple targets 
        // before its lifetime ends, keep the base collision logic as is.
        // If you want it to destroy immediately after the first hit, 
        // consider overriding AfterHitEffect or removing the end-of-frame coroutine in the base class.

        // Move the projectile forward (based on transform.forward) 
        transform.position += transform.forward * (speed * Time.deltaTime);

        // Track time to destroy after 'lifeTime'
        _timer += Time.deltaTime;
        if (_timer >= lifeTime)
        {
            AfterHitEffect();  // Calls Destroy(gameObject) from the base class
        }
    }
}
