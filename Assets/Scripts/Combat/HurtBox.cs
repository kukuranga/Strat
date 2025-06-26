using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [Tooltip("The BaseUnit that owns this HurtBox.")]
    public BaseUnit ownerUnit;
    public float DamageMultiplier = 1;

    public void OnHit(int Damage, HitBox HB)
    {
        ownerUnit.TakeDamage(Damage * DamageMultiplier, HB._OwnerUnit);
        HB.AfterHitEffect();
    }

}
