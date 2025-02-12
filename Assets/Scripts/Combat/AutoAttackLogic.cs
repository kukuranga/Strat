using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class AutoAttackLogic : MonoBehaviour
{
    [Tooltip("Reference to the parent BaseUnit.")]
    public Character parentUnit;

    private SphereCollider _sphereCollider;

    private void Start()
    {
        _sphereCollider = GetComponent<SphereCollider>();
        if (_sphereCollider)
        {
            _sphereCollider.isTrigger = true;
            if (parentUnit != null)
            {
                // Match the parent's autoAttackRange
                _sphereCollider.radius = parentUnit.attack.attackRange;
            }
        }
        else
        {
            Debug.LogWarning($"{name} needs a SphereCollider with isTrigger = true!");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Only collision check. Let BaseUnit handle the logic.
        if (parentUnit == null) return;

        // Look for a HurtBox
        HurtBox hurtBox = other.GetComponent<HurtBox>();
        if (hurtBox == null) return;

        // Pass the detection to BaseUnit
        parentUnit.TryAutoAttack(hurtBox);
    }
}
