using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : MonoBehaviour
{

    //Base item script used to detail the information on each Item
    public int Priority; //The higher the prio the more likely to be deleted

    public void DestroyItem()
    {
        Destroy(this);
    }

    //This Triggers when the unit is hit by a hitbox
    public virtual void OnHit(HitBox HB)
    {

    }

}
