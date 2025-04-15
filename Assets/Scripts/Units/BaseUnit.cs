using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BaseUnitType { Character, Structure }

public class BaseUnit : MonoBehaviour
{
    public BaseUnitType _Type;

    [Header("Unit Data")]
    public bool InObjectPool;
    public int _AssignedPlayer;
    public string UnitName;
    public string UnitDescription;
    public Tile OccupiedTile;
    public Faction Faction;
    public BaseUnit _LastUnitThatDamaged;

    [Header("Health")]
    public float _MaxHealth;
    public float _CurrentHealth;
    [SerializeField] private HealthBar _HealthBar;

    [Header("ATB Costs")]
    public float _ATBSpawnCost; // Common spawn cost (both Characters & Structures can share if needed)

    // --------------------------------------------------
    // Basic toggles or references
    // --------------------------------------------------
    public GameObject CollisionGameObjects;
    public bool _Interactable;

    // --------------------------------------------------
    // Common setup and tear-down
    // --------------------------------------------------
    protected virtual void Awake()
    {
        _HealthBar = GetComponentInChildren<HealthBar>();
    }

    protected virtual void Start()
    {
        InitializeUnit();
    }

    protected virtual void Update()
    {
        SetHurtBox();
    }

    /// <summary>
    /// Initialize health, etc.
    /// </summary>
    public void InitializeUnit()
    {
        _CurrentHealth = _MaxHealth;
        if (_HealthBar != null)
            _HealthBar.UpdateHealthBar(_CurrentHealth, _MaxHealth);
    }

    private void SetHurtBox()
    {
        if (_Interactable)
            CollisionGameObjects?.SetActive(true);
        else
            CollisionGameObjects?.SetActive(false);
    }

    /// <summary>
    /// Assign or clear the tile.
    /// </summary>
    public void SetTile(Tile target)
    {
        OccupiedTile = target;
    }

    public void SetAssignedPlayer(int i)
    {
        _AssignedPlayer = i;
    }

    public void ToggleInteraction(bool val)
    {
        _Interactable = val;
    }

    //Triggers this to be overridden when the unit is selected
    public virtual void OnSelectiion()
    {
        if (GameManager.Instance._DebuggerMode)
            Debug.Log($"{UnitName} Selected");
    }

    public virtual void ClearSelection()
    {
        if (GameManager.Instance._DebuggerMode)
            Debug.Log($"{UnitName} Cleared Selected");
    }

    public virtual void TakeDamage(float damage)
    {
        _CurrentHealth -= damage;
        if (_HealthBar != null)
            _HealthBar.UpdateHealthBar(_CurrentHealth, _MaxHealth);

        // Optional: Add to some global ATB
        ATBManager.Instance.AddToATB(damage * 0.5f);

        if (_CurrentHealth <= 0)
            Die();
    }

    public void SetLastUnitDamaged(BaseUnit _Unit)
    {
        _LastUnitThatDamaged = _Unit;
    }


    public virtual void ResetUnit()
    {
        _CurrentHealth = _MaxHealth;
        if (_HealthBar != null)
            _HealthBar.UpdateHealthBar(_CurrentHealth, _MaxHealth);

        OccupiedTile = null;
        ToggleInteraction(false);
    }

    public virtual void Die()
    {
        ObjectiveManager.Instance.OnUnitKilled(this);

        if (PlayerManager.Instance._SelectedUnit == this)
            PlayerManager.Instance.ClearSelectedUnit();

        if (OccupiedTile != null)
        {
            OccupiedTile.occupiedUnit = null;
            OccupiedTile = null;
        }

        UnitManager.Instance.KillUnit(this);
    }
}