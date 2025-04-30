using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnAnimationHelper : MonoBehaviour
{
    //This Script is intended to be a helper for the animation events trigger

    public Pawn _PawnScript;

    public void ShootEvent()
    {
        _PawnScript.ShootEvent();
    }
}
