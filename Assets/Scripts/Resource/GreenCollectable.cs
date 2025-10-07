using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenCollectable : BaseItem
{

    //on hit this needs to produce a green resource point for the units owner.

    public Tile _OcuppiedTile;

    public void Initialize(Tile _t)
    {
        //On the spawner will call this and set the values such as occupied tile etc
        _OcuppiedTile = _t;
    }

    private void OnTriggerEnter(Collider other)
    {
        //add the green resource
        //run the on deletion method
        HurtBox BU = other.GetComponent<HurtBox>();
        if (BU == null)
            return;
        if (BU.ownerUnit.Faction != Faction.Hero)
            return;

        ResourceManager.Instance.AddGreenResource(1);
        destroyObject();

    }

    private void destroyObject()
    {
        //call the destroy method in the _occupied tile object
        //destoy the object here
        Destroy(this.gameObject);
    }
}
