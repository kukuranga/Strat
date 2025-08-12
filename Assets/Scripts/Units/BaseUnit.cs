using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BaseUnitType { Character, Structure }

public class BaseUnit : MonoBehaviour
{
    public BaseUnitType _Type;

    //TODO: ADD THE NEW STAT NUMBERS HERE

    [Header("Unit Data")]
    public bool InObjectPool;
    public int _AssignedPlayer;
    public string UnitName;
    public string UnitDescription;
    public Tile OccupiedTile;
    public Faction Faction;
    public BaseUnit _LastUnitThatDamaged;
    public CharacterTextBox _CharacterTextBox;

    [Header("Combat Stats")]
    public float Atk = 0; // Basic Attack value
    public float SpAtk = 0; // Basic Special Attack value
    public float Def = 0; // basic defence value
    public float SpDef = 0; //basic special defence value
    public float Eva = 0; // The evasion value of the unit

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

    //TODO: change the combat logic here
    public virtual void TakeDamage(float damage, float Acc , bool UseSPA, BaseUnit _DamagingUnit)
    {

        float HitValue = Acc - Eva;
        bool _Crit = false;

        //To ensure attacks always have a chance to hit
        if (HitValue < 25)
            HitValue = 25;

        float _Rand = Random.Range(1, 100);

        if (HitValue == _Rand)
            _Crit = true;

        if(HitValue > _Rand)
        {

            float Pow;
            if (UseSPA)
                Pow = Atk;
            else
                Pow = SpAtk;
            if(_Crit)
                Pow *= 2;

            float DMG = ((Pow * (_DamagingUnit.Atk / Def))/ 50) + 2;

            Debug.Log(DMG + " Damage taken");

            _CurrentHealth -= DMG;
            if (_HealthBar != null)
                _HealthBar.UpdateHealthBar(_CurrentHealth, _MaxHealth);

            // Optional: Add to some global ATB
            //ATBManager.Instance.AddToATB(damage * 0.5f);

            SetLastUnitDamaged(_DamagingUnit);

            if (_CurrentHealth <= 0)
                Die();

        }
        else
        {
            //Miss
            _CharacterTextBox.UpdateMessage("Miss");
        }
    }

    public void HealDamage(float Amount)
    {
        _CurrentHealth += Amount;
        UpdateHealth();

        _CharacterTextBox.UpdateMessage("+" + Amount + " Health");
        //TODO: Add visual here
    }

    public void UpdateHealth()
    {
        if(_CurrentHealth > _MaxHealth)
        {
            _CurrentHealth = _MaxHealth;
        }
        if(_CurrentHealth < 0)
        {
            _CurrentHealth = 0;
        }
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

    public virtual void UseAbility(Tile tile)
    {
        Debug.Log("Ability used");
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