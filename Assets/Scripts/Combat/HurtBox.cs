using System.Collections;
using System.Collections.Generic;
using UnityEngine;



/// <summary>
/// Attach this script to the "HurtBox" GameObject.
/// It holds a reference to the BaseUnit that will be damaged
/// if struck by a HitBox.
/// </summary>
/// 
public class HurtBox : MonoBehaviour
{
    [Tooltip("The BaseUnit that owns this HurtBox.")]
    public BaseUnit ownerUnit;
    public float DamageMultiplier = 1;

    public void OnHit(int Damage, HitBox HB)
    {
        ownerUnit.TakeDamage(Damage * DamageMultiplier);
        HB.AfterHitEffect();
    }

}
