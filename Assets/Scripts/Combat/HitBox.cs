using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [Header("HitBox Settings")]
    [Tooltip("Default damage to deal when this HitBox connects.")]
    public int defaultDamage = 1;

    [Tooltip("Which faction owns this HitBox (so we don't damage allies).")]
    public Faction factionOwner;

    // Keep track of all HurtBoxes hit during this frame 
    private HashSet<HurtBox> _hitTargets = new HashSet<HurtBox>();

    // We only want to despawn once, so use a flag to avoid multiple destroys
    private bool _shouldDespawn = false;

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
            }
        }
    }

    private void Update()
    {
        // If we have hit at least one target this frame (or past frames),
        // schedule a despawn so we don't destroy mid-frame (allowing multiple collisions).
        if (_hitTargets.Count > 0 && !_shouldDespawn)
        {
            _shouldDespawn = true;
            StartCoroutine(DespawnAtEndOfFrame());
        }
    }

    // Wait until the end of the frame before removing the HitBox
    // so all collisions can be processed in a single frame.
    private IEnumerator DespawnAtEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        AfterHitEffect();
    }

    /// <summary>
    /// Called after all hits for this frame are done. 
    /// Destroys the HitBox GameObject.
    /// </summary>
    public virtual void AfterHitEffect() // Marked as virtual
    {
        Destroy(gameObject);
    }
}