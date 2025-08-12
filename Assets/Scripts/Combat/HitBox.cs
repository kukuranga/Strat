using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    [Header("HitBox Settings")]
    [Tooltip("Default damage to deal when this HitBox connects.")]
    public bool UseSPA;
    public int defaultDamage = 1;
    public float Acc = 100; // percentage chance of move hitting

    [Tooltip("Which faction owns this HitBox (so we don't damage allies).")]
    public Faction factionOwner;
    public BaseUnit _OwnerUnit;

    [Tooltip("Whether the HitBox should despawn after hitting a target.")]
    public bool shouldDespawn = true; // Default to true for backward compatibility

    [Tooltip("List of target factions this HitBox can damage.")]
    [EnumFlags] public Faction _Targets;

    // Keep track of all HurtBoxes hit during this frame 
    public HashSet<HurtBox> _hitTargets = new HashSet<HurtBox>();

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collided object has a HurtBox
        HurtBox hurtBox = other.GetComponent<HurtBox>();
        if (hurtBox != null && hurtBox.ownerUnit != null)
        {
            // Skip if the hurtBox belongs to the same faction as this hitbox
            if (hurtBox.ownerUnit.Faction == factionOwner) return;

            // Skip if the hurtBox's faction is not in the _Targets bitmask
            if ((_Targets & hurtBox.ownerUnit.Faction) != hurtBox.ownerUnit.Faction) return;

            // Only deal damage if we haven't hit this HurtBox yet
            if (!_hitTargets.Contains(hurtBox))
            {
                _hitTargets.Add(hurtBox);
                hurtBox.OnHit(defaultDamage, Acc, UseSPA ,this);///------------------------------------------------Here-------------------------------------------------------
            }
        }

        ItemHurtBox itemhurtBox = other.GetComponent<ItemHurtBox>();
        if (itemhurtBox != null && itemhurtBox.ownerItem != null)
        {
            itemhurtBox.OnHit(this);
        }
    }

    private void Update()
    {
        // Optionally, you can add logic here if needed
    }

    /// <summary>
    /// Called after all hits for this frame are done. 
    /// Destroys the HitBox GameObject if shouldDespawn is true.
    /// </summary>
    public virtual void AfterHitEffect() // Marked as virtual
    {
        if (shouldDespawn)
        {
            Destroy(gameObject);
        }
    }

    public void ResetHitTargets()
    {
        _hitTargets.Clear();
    }
}