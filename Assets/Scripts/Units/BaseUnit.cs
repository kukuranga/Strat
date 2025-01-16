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
    public float _ATBCombatCost;
    private bool _Interactable;
    [SerializeField] HealthBar _HealthBar;

    public void InitializeUnit()
    {
        _CurrentHealth = _MaxHealth;
    }

    private void Awake()
    {
        _HealthBar = GetComponentInChildren<HealthBar>();
    }

    private void Start()
    {
        InitializeUnit();
    }

    private void Update()
    {
        
    }

    public virtual bool CombatLogic(BaseUnit _TargetUnit, Tile _TargetTile, Tile _OriginTile)
    {
        // Check if there is enough ATB to perform the combat action
        if (ATBManager.Instance.GetATBAmount() >= _ATBCombatCost)
        {
            // Deduct ATB cost for performing the combat
            ATBManager.Instance.PayATBCost(_ATBCombatCost);

            // Proceed with combat logic (e.g., dealing damage)
            _TargetUnit.TakeDamage(1);  // For example, we deal 1 damage (can be adjusted)

            return true;  // Combat was successful
        }
        else
        {
            Debug.Log("Not enough ATB to perform combat.");
            return false; // Combat cannot proceed because there is not enough ATB
        }
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
        _HealthBar.UpdateHealthBar(_CurrentHealth, _MaxHealth);  // Update health bar UI
        ATBManager.Instance.AddToATB(Damage * 0.5f);  // Optional: Add ATB points based on damage taken (example multiplier)

        if (_CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void ResetUnit()
    {
        _CurrentHealth = _MaxHealth;
        _HealthBar.UpdateHealthBar(_CurrentHealth, _MaxHealth); // Ensure health bar reflects full health
        OccupiedTile = null; // Reset tile association
        ToggleInteraction(false); // Optionally reset interaction

        // Reset ATB value when unit resets (if needed)
        ATBManager.Instance.AddToATB(-ATBManager.Instance.GetATBAmount()); // Reset ATB (or set to initial value)
    }


    public void Die()
    {
        // Notify ObjectiveManager about the kill
        ObjectiveManager.Instance.OnUnitKilled(this);

        // Handle unit death
        if (OccupiedTile != null)
        {
            OccupiedTile.occupiedUnit = null; // Clear the tile
            OccupiedTile = null;
        }

        UnitManager.Instance.KillUnit(this); // Return the unit to the pool
    }
}

