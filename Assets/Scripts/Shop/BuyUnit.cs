using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuyUnit : MonoBehaviour
{
    public int _AssignedPlayer; // The player who is buying the unit

    public UnitType _UnitType;

    public void _OnClick()
    {
        switch (_UnitType)
        {
            case UnitType.TestUnit:
                UnitManager.Instance.GetTestUnit(); //do something with the true/false statement coming from this
                break;
        }
    }
}
