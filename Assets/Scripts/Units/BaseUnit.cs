using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUnit : MonoBehaviour
{
    public int _AssignedPlayer;
    public string UnitName;
    public Tile OccupiedTile;
    public Faction Faction;
    public List<Vector2> Moves; //Stores all available moves this unit can make
    public int _Health;
    public float _ATBCost;
    private bool _Interactable;

    public void ToggleInteraction(bool _val)
    {
        _Interactable = _val;
    }

    public void SetTile(Tile _target)
    {
        OccupiedTile = _target;
    }

    public void SetAssignedPlayer(int i)
    {
        _AssignedPlayer = i;
    }
}

