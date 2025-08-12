using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : BaseUnit
{
    [Header("Movement")]
    public float _ATBMoveCost = 5;
    public float moveSpeed = 3f;
    public float rotateSpeed = 360f;
    public bool isMoving = false;
    public bool isRotating = false;
    public bool CanMove = true;

    [Header("Attack")]
    public int AutoAttackRange;
    [HideInInspector]public bool isAttacking = false;
    public float nextAutoAttackTime = 0f;
    public int _ATBCombatCost;
    public float autoAttackCooldown;
    public BaseUnit _AttackTarget;
    public float Ability1CoolDown;
    public float Ability2CoolDown;
    public float Ability1CoolDownTime;
    public float Ability2CoolDownTime;
    public Slider _AttackSlider;

    [EnumFlags] public Faction _Targets;

    private AStarPathfinding _pathfinding;
    private List<Tile> _currentPath;
    private int _currentPathIndex;
    private bool _waitingForATB = false;

    protected override void Start()
    {
        base.Start();
        _Type = BaseUnitType.Character;
        _pathfinding = new AStarPathfinding(GridManager.Instance);
        Ability1CoolDown = Ability1CoolDownTime;
        Ability2CoolDown = Ability2CoolDownTime;
    }

    protected override void Update()
    {
        base.Update();

        // Check if the unit is ready to auto-attack
        if(nextAutoAttackTime >= autoAttackCooldown && !isMoving && !isAttacking && !isRotating)
        {
            // Get all targets in range
            List<BaseUnit> targetsInRange = GetTargetsInRange();
        
            if(targetsInRange.Count > 0)
            {
                // Sort targets based on precedence in the _Targets list
                targetsInRange.Sort((a, b) => GetTargetPriority(a.Faction).CompareTo(GetTargetPriority(b.Faction)));

                // Try to auto-attack the highest-priority target
                BaseUnit target = targetsInRange[0];
                TryAutoAttack(target);

                // Set the next auto-attack time based on the cooldown
                nextAutoAttackTime = 0;
            }
        }

        if(Ability1CoolDown < Ability1CoolDownTime)
            Ability1CoolDown += Time.deltaTime;
        if (Ability2CoolDown < Ability2CoolDownTime)
            Ability2CoolDown += Time.deltaTime;

        nextAutoAttackTime += Time.deltaTime;

        UpdateAutoAttackSlider();
    }

    #region Combat Logic
    public virtual void Ability1()
    {
        Debug.Log($"{UnitName} uses Ability1.");
    }

    public virtual void Ability2()
    {
        Debug.Log($"{UnitName} uses Ability2.");
        Ability2CoolDown = 0;
    }

    public virtual void TryAutoAttack(BaseUnit target)
    {
        if (isMoving)
        {
            Debug.Log($"{UnitName} is moving; cannot auto-attack yet.");
            return;
        }

        if (isAttacking || Time.time < nextAutoAttackTime)
        {
            Debug.Log($"{UnitName} is attacking or on cooldown; cannot auto-attack yet.");
            return;
        }

        // Perform the auto-attack logic here
        Debug.Log($"{UnitName} is auto-attacking {target.UnitName}.");

        // Example: Deal damage to the target
        //target.TakeDamage(10, this); // Replace with your damage logic

        // Set the unit to the attacking state
        isAttacking = true;

        // Reset the attacking state after a short delay (optional)
        StartCoroutine(ResetAttackingState());
    }

    // Coroutine to reset the attacking state
    private IEnumerator ResetAttackingState()
    {
        yield return new WaitForSeconds(0.5f); // Adjust the delay as needed
        isAttacking = false;
    }

    public void ToggleAutoAttackRangeVisual(bool show)
    {
        if (_Interactable)
        {
            List<Tile> tilesInRange = GetAllTilesInAutoAttackRange();
            foreach (Tile tile in tilesInRange)
            {
                if (tile != null)
                {
                    tile.SetCombatTile(show);
                }
            }
        }
        //Debug.Log($"Toggled combat visuals on {tilesInRange.Count} tiles. Show: {show}");
    }

    public List<Tile> GetAllTilesInAutoAttackRange()
    {
        List<Tile> tilesInRange = GridManager.Instance.GetTilesInRadius(OccupiedTile, AutoAttackRange);
        if (OccupiedTile != null)
        {
            tilesInRange.Remove(OccupiedTile);
        }
        return tilesInRange;
    }

    public List<Tile> GetAllTilesInRange(int range)
    {
        List<Tile> tilesInRange = GridManager.Instance.GetTilesInRadius(OccupiedTile, range);

        tilesInRange.Remove(OccupiedTile);

        return tilesInRange;
    }

    public List<BaseUnit> GetTargetsInRange()
    {
        List<BaseUnit> targetsInRange = new List<BaseUnit>();
        List<Tile> tilesInRange = GetAllTilesInAutoAttackRange();

        foreach (Tile tile in tilesInRange)
        {
            if (tile.occupiedUnit != null)
            {
                BaseUnit unit = tile.occupiedUnit;
                // Corrected faction check - should be != 0
                if ((_Targets & unit.Faction) != 0)
                {
                    targetsInRange.Add(unit);
                }
            }
        }

        return targetsInRange;
    }

    // Helper method to get the priority of a faction based on the _Targets list
    private int GetTargetPriority(Faction faction)
    {
        // Define the priority order manually
        switch (faction)
        {
            case Faction.Hero: return 0; // Highest priority
            case Faction.Enemy: return 1;
            case Faction.NPC: return 2;
            default: return int.MaxValue; // Lowest priority
        }
    }
    #endregion

    public void UpdateAutoAttackSlider()
    {
        _AttackSlider.gameObject.SetActive(nextAutoAttackTime != 0);

        _AttackSlider.value = nextAutoAttackTime / autoAttackCooldown;
    }

    public void MoveToDestination(Tile destinationTile)
    {
        if(!CanMove)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.LogError("CanMove is false. Cannot move.");
            return;
        }

        if (OccupiedTile == null)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.LogError("OccupiedTile is null. Cannot move.");
            return;
        }

        if (destinationTile == null)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.LogError("Destination tile is null. Cannot move.");
            return;
        }

        if (_pathfinding == null)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.LogError("Pathfinding is not initialized. Cannot move.");
            return;
        }

        if (isAttacking)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.Log($"{UnitName} is attacking and cannot move yet.");
            return;
        }

        // Check if the destination tile is occupied
        if (destinationTile.occupiedUnit != null)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.LogWarning($"Destination tile at {destinationTile._coordinates} is occupied. Cannot move.");
            return;
        }

        // Clear combat visuals before starting movement
        ToggleAutoAttackRangeVisual(false);

        _currentPath = _pathfinding.FindPath(OccupiedTile, destinationTile);
        if (_currentPath != null && _currentPath.Count > 0)
        {
            _currentPathIndex = 0;
            StartCoroutine(FollowPath());
        }
        else
        {
            Debug.Log("No valid path found.");
        }
    }

    private IEnumerator FollowPath()
    {

        while (_currentPathIndex < _currentPath.Count)
        {
            Tile nextTile = _currentPath[_currentPathIndex];

            // Check if the next tile is occupied
            if (nextTile.occupiedUnit != null)
            {
                Debug.LogWarning($"Tile at {nextTile._coordinates} is occupied. Stopping movement.");
                break;
            }

            // Wait until there's enough ATB to move
            while (ATBManager.Instance.GetATBAmount() < _ATBMoveCost)
            {
                _waitingForATB = true;
                yield return null; // Wait for the next frame
            }
            _waitingForATB = false;

            // Update the unit's occupied tile
            if (OccupiedTile != null)
            {
                OccupiedTile.occupiedUnit = null; // Clear the old tile's occupant
            }
            nextTile.occupiedUnit = this; // Set the new tile's occupant
            OccupiedTile = nextTile; // Update the unit's current tile

            yield return StartCoroutine(MoveUnitRoutine(nextTile.transform.position));
            _currentPathIndex++;

            // Pay ATB cost after each step
            ATBManager.Instance.PayATBCost(_ATBMoveCost);
        }

        // Check if the unit is still selected before toggling combat visuals
        if (PlayerManager.Instance._SelectedUnit == this)
        {
            ToggleAutoAttackRangeVisual(true);
        }
    }

    private IEnumerator MoveUnitRoutine(Vector3 targetPos)
    {

        Vector3 startPos = transform.position;
        float startZ = startPos.z;
        targetPos.z = startZ;

        // 1) Rotate first
        isRotating = true;
        {
            Vector3 direction = targetPos - transform.position;
            direction.z = 0f;

            if (direction.sqrMagnitude > 0.0001f)
            {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
                angle -= 90f;
                Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

                while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
                {
                    transform.rotation = Quaternion.RotateTowards(
                        transform.rotation,
                        targetRotation,
                        rotateSpeed * Time.deltaTime
                    );
                    yield return null;
                }
                transform.rotation = targetRotation;
            }
        }
        isRotating = false;

        // 2) Move second
        isMoving = true;

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
        isMoving = false;
    }

    // --------------------------------------------------
    // Attack Logic (Replaced with Scriptable Object)
    // --------------------------------------------------
    

    public override void OnSelectiion()
    {
        base.OnSelectiion();

        ToggleAutoAttackRangeVisual(true);
    }

    public override void ClearSelection()
    {
        base.ClearSelection();

        ToggleAutoAttackRangeVisual(false);
    }

    public IEnumerator RotateUnitTowards(Vector3 targetPos)
    {
        Vector3 direction = targetPos - transform.position;
        direction.z = 0f;

        if (direction.sqrMagnitude > 0.0001f)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle -= 90f;
            Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.RotateTowards(
                    transform.rotation,
                    targetRotation,
                    rotateSpeed * Time.deltaTime
                );
                yield return null;
            }
            transform.rotation = targetRotation;
        }
    }
}