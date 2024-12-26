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
    public List<Vector2> Combat;//Stores all squares the unit can Attack
    public int _MaxHealth;
    public int _CurrentHealth;
    //public float _ATBCost;
    public float _ATBMoveCost;
    public float _ATBSpawnCost;
    private bool _Interactable;

    public void InitializeUnit()
    {
        _CurrentHealth = _MaxHealth;
    }

    private void Start()
    {
        InitializeUnit();
    }

    private void Update()
    {
        if (_CurrentHealth <= 0) Die(); //Kills unit once health is less than 0
    }

    public virtual bool CombatLogic(BaseUnit _TargetUnit, Tile _TargetTile, Tile _OriginTile) 
    {
        //ToDO: Might change this later to be pure virtual
        _TargetUnit.TakeDamage(1);//Future implimentations of this code will have different ways to change this value
        return true; 
    }

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

    public void TakeDamage(int Damage)
    {
        _CurrentHealth -= Damage;
    }

    public void Die()
    {
        InitializeUnit();
        UnitManager.Instance.KillUnit(this); //TODO: Expand on this
    }
}

