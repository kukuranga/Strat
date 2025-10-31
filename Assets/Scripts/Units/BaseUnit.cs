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
    public CharacterTextBox _CharacterTextBox;
    public bool BeingPushed;

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
        PerformTileRaycast();
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
        _HealthBar.UpdateHealthBar(_CurrentHealth, _MaxHealth);
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

    #region Push Logic
    public void Push(BaseUnit PushingUnit, int Distance)
    {
        CardinalDirection dir = GetDirection(PushingUnit.OccupiedTile._coordinates, this.OccupiedTile._coordinates);

        Vector2 SearchTile = this.OccupiedTile._coordinates;
        Tile _ClosestTile = this.OccupiedTile;

        for (int i = 0; i < Distance; i++)
        {
            _ClosestTile = GetClosestCardinalTile(SearchTile, dir);

            if (_ClosestTile == null)
                return;
            if (!_ClosestTile._isWalkable)
                return;
            if (_ClosestTile.occupiedUnit = null)
                return;

            SearchTile = _ClosestTile._coordinates;
        }

        StartCoroutine(PushMovement(_ClosestTile, 5));
    }

    private Tile GetClosestCardinalTile(Vector2 CurrentCord ,CardinalDirection _Dir)
    {
        //Vector2 CurrentCord = this.OccupiedTile._coordinates;

        switch (_Dir)
        {
            case CardinalDirection.North:
                return GridManager.Instance.GetTileAtCord(CurrentCord + new Vector2(0, -1));
            case CardinalDirection.NorthEast:
                return GridManager.Instance.GetTileAtCord(CurrentCord + new Vector2(1, -1));
            case CardinalDirection.East:
                return GridManager.Instance.GetTileAtCord(CurrentCord + new Vector2(1, 0));
            case CardinalDirection.SouthEast:
                return GridManager.Instance.GetTileAtCord(CurrentCord + new Vector2(1, 1));
            case CardinalDirection.South:
                return GridManager.Instance.GetTileAtCord(CurrentCord + new Vector2(0, 1));
            case CardinalDirection.SouthWest:
                return GridManager.Instance.GetTileAtCord(CurrentCord + new Vector2(-1, 1));
            case CardinalDirection.West:
                return GridManager.Instance.GetTileAtCord(CurrentCord + new Vector2(-1, 0));
            case CardinalDirection.NorthWest:
                return GridManager.Instance.GetTileAtCord(CurrentCord + new Vector2(-1, -1));
            default:
                return GridManager.Instance.GetTileAtCord(CurrentCord);
        }
    }

    IEnumerator PushMovement(Tile TargetTile ,float moveSpeed)
    {
        BeingPushed = true;
        Vector3 targetPos = TargetTile.transform.position;
        targetPos.z = this.transform.position.z;

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                targetPos,
                moveSpeed * Time.deltaTime
            );
            yield return null;
        }

        transform.position = targetPos;
        OccupiedTile.ClearOccupiedUnit();
        OccupiedTile = TargetTile;
        TargetTile.SetUnit(this, false);
        BeingPushed = false;
        yield return null;
    }

    public enum CardinalDirection
    {
        North,
        NorthEast,
        East,
        SouthEast,
        South,
        SouthWest,
        West,
        NorthWest
    }

    public CardinalDirection GetDirection(Vector2 from, Vector2 to)
    {
        Vector2 direction = to - from;

        // Calculate angle in degrees (0 = East, 90 = North, 180 = West, 270 = South)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Convert to 0 = North, 90 = East, 180 = South, 270 = West
        angle = (angle + 90f) % 360f;
        if (angle < 0) angle += 360f;

        // Determine direction based on angle
        if (angle >= 337.5f || angle < 22.5f) return CardinalDirection.North;
        if (angle >= 22.5f && angle < 67.5f) return CardinalDirection.NorthEast;
        if (angle >= 67.5f && angle < 112.5f) return CardinalDirection.East;
        if (angle >= 112.5f && angle < 157.5f) return CardinalDirection.SouthEast;
        if (angle >= 157.5f && angle < 202.5f) return CardinalDirection.South;
        if (angle >= 202.5f && angle < 247.5f) return CardinalDirection.SouthWest;
        if (angle >= 247.5f && angle < 292.5f) return CardinalDirection.West;
        return CardinalDirection.NorthWest; // 292.5 - 337.5
    }
    #endregion


    [Header("Raycast Settings")]
    [SerializeField] private float raycastDistance = 1f;
    [SerializeField] private LayerMask tileLayerMask;
    [SerializeField] private Vector3 raycastOffset = new Vector3(0, 0, 0); // Offset to start above ground

    private void PerformTileRaycast()
    {
        // Calculate raycast origin and direction
        Vector3 rayOrigin = transform.position;// + raycastOffset;
        Vector3 rayDirection = Vector3.forward;

        // Perform the raycast
        RaycastHit hit;
        if (Physics.Raycast(rayOrigin, rayDirection, out hit, raycastDistance, tileLayerMask))
        {
            // Check if we hit a Tile object specifically
            Tile hitTile = hit.collider.GetComponent<Tile>();
            if (hitTile != null)
            {
                // We hit a tile, call the method
                OnTileHit(hitTile, hit.point);
            }
        }

        // Optional: Visualize the raycast in the editor
        Debug.DrawRay(rayOrigin, rayDirection * raycastDistance, Color.red);
    }

    /// <summary>
    /// Called when the raycast hits a Tile object
    /// </summary>
    /// <param name="tile">The tile that was hit</param>
    /// <param name="hitPoint">The world position where the ray hit</param>
    private void OnTileHit(Tile tile, Vector3 hitPoint)
    {
        Debug.Log($"Hit tile at coordinates: {tile._coordinates}, World position: {hitPoint}");

        if(OccupiedTile != tile)
        {
            OccupiedTile.ClearOccupiedUnit();
            SetTile(tile);
            tile.SetUnit(this, false);
        }
    }

    /// <summary>
    /// Optional: Method to configure raycast settings at runtime
    /// </summary>
    public void ConfigureRaycast(float distance, LayerMask mask, Vector3 offset)
    {
        raycastDistance = distance;
        tileLayerMask = mask;
        raycastOffset = offset;
    }
}