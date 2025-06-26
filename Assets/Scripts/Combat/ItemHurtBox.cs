using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemHurtBox : MonoBehaviour
{
    [Tooltip("The BaseItem that owns this HurtBox.")]
    public BaseItem ownerItem;
    //public float DamageMultiplier = 1;

    public void OnHit(HitBox HB)
    {
        ownerItem.OnHit(HB);
        HB.AfterHitEffect();
    }
}
