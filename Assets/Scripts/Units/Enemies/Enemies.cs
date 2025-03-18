using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemies : BaseUnit
{
    //To Expand on the base unit and not be selectable
    //this script will have the loic for movement and pathfinding
    // have a basic method for attacking
    //handles moving towards the player

    [Header("Movement")]
    public float moveSpeed = 3f;
    public float rotateSpeed = 360f;
    public bool isMoving = false;

    [Header("Attack")]
    public int AutoAttackRange;
    public bool isAttacking = false;
    public float nextAutoAttackTime = 0f;
    public float autoAttackCooldown;
    public BaseUnit _AttackTarget;

    [EnumFlags] public Faction _Targets;

    private AStarPathfinding _pathfinding;
    private List<Tile> _currentPath;
    private int _currentPathIndex;

    protected override void Start()
    {
        base.Start();
        _Type = BaseUnitType.Character;
        _pathfinding = new AStarPathfinding(GridManager.Instance);
    }

    protected override void Update()
    {
        base.Update();

        // Check if the unit is ready to auto-attack
        if (Time.time >= nextAutoAttackTime && !isMoving && !isAttacking)
        {
            // Get all targets in range
            List<BaseUnit> targetsInRange = GetTargetsInRange();

            if (targetsInRange.Count > 0)
            {
                // Sort targets based on precedence in the _Targets list
                targetsInRange.Sort((a, b) => GetTargetPriority(a.Faction).CompareTo(GetTargetPriority(b.Faction)));

                // Try to auto-attack the highest-priority target
                BaseUnit target = targetsInRange[0];
                TryAutoAttack(target);

                // Set the next auto-attack time based on the cooldown
                nextAutoAttackTime = Time.time + autoAttackCooldown;
            }
        }
    }

    #region Combat Logic

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
        target.TakeDamage(10); // Replace with your damage logic

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
        List<Tile> tilesInRange = GetAllTilesInAutoAttackRange();
        foreach (Tile tile in tilesInRange)
        {
            if (tile != null)
            {
                tile.SetCombatTile(show);
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

    public List<BaseUnit> GetTargetsInRange()
    {
        List<BaseUnit> targetsInRange = new List<BaseUnit>();
        List<Tile> tilesInRange = GetAllTilesInAutoAttackRange();

        foreach (Tile tile in tilesInRange)
        {
            if (tile.occupiedUnit != null)
            {
                BaseUnit unit = tile.occupiedUnit;
                if ((_Targets & unit.Faction) == unit.Faction) // Check if the faction is included in the bitmask
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

    public void MoveToDestination(Tile destinationTile)
    {
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
        isMoving = true;

        while (_currentPathIndex < _currentPath.Count)
        {
            Tile nextTile = _currentPath[_currentPathIndex];

            // Check if the next tile is occupied
            if (nextTile.occupiedUnit != null)
            {
                Debug.LogWarning($"Tile at {nextTile._coordinates} is occupied. Stopping movement.");
                break;
            }

            // Update the unit's occupied tile
            if (OccupiedTile != null)
            {
                OccupiedTile.occupiedUnit = null; // Clear the old tile's occupant
            }
            nextTile.occupiedUnit = this; // Set the new tile's occupant
            OccupiedTile = nextTile; // Update the unit's current tile

            yield return StartCoroutine(MoveUnitRoutine(nextTile.transform.position));
            _currentPathIndex++;
        }

        isMoving = false;

        // Check if the unit is still selected before toggling combat visuals
        if (PlayerManager.Instance._SelectedUnit == this)
        {
            ToggleAutoAttackRangeVisual(true);
        }
    }

    private IEnumerator MoveUnitRoutine(Vector3 targetPos)
    {
        isMoving = true;

        Vector3 startPos = transform.position;
        float startZ = startPos.z;
        targetPos.z = startZ;

        // 1) Rotate first
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

        // 2) Move second
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
