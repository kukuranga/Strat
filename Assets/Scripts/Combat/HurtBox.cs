using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtBox : MonoBehaviour
{
    [Tooltip("The BaseUnit that owns this HurtBox.")]
    public BaseUnit ownerUnit;


    public void OnHit(int Damage, float Acc, bool _UseSPA,  HitBox HB)
    {
        ownerUnit.TakeDamage(Damage, Acc, _UseSPA, HB._OwnerUnit);
        HB.AfterHitEffect();
    }

}
