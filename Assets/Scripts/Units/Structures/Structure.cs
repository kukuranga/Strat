using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Structure : BaseUnit
{
    // A Structure has no movement or attack logic,
    // so this class can remain empty.
    // It simply inherits all health, tile, spawn cost logic from BaseUnit.

    protected override void Start()
    {
        base.Start();
        _Type = BaseUnitType.Structure;

    }

    public override void OnSelectiion()
    {
        base.OnSelectiion();
        //On selection it will send the data to the menu manager to show info on the structure
    }

    public override void ClearSelection()
    {
        base.ClearSelection();

    }
}

