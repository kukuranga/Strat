using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Enemies
{
    [Header("Goblin Settings")]
    public float aggroRange = 5f; // Range within which the Goblin will detect hero units
    public float moveCooldown = 2f; // Cooldown between random movements
    //public MeleeDaggerAttack daggerAttack; // Reference to the dagger attack ScriptableObject
    public GameObject Dagger;

    [Header("Variables to set")]
    [SerializeField] GameObject AggroIndicator;

    private HitBox DaggerHitBox;
    private float lastMoveTime;
    private Character targetHero;

    protected override void Start()
    {
        base.Start();
        lastMoveTime = Time.time;
        _Interactable = true;
        AutoAttackRange = 1; // Set attack range to 1 tile
        DaggerHitBox = Dagger.GetComponent<HitBox>();
    }

    protected override void Update()
    {
        base.Update();

        CheckForHeroes();

        if(targetHero != null && Time.time - lastMoveTime > moveCooldown)
        {
            FollowTarget();
            lastMoveTime = Time.time;
        }
        else if(Time.time - lastMoveTime > moveCooldown)
        {
            MoveRandomly();
            lastMoveTime = Time.time;
        }

        //Updates the visuals for the aggro if there is a target
        CheckAggro();

        //if (!isAggro)
        //{
        //    // Random movement if no hero is in range
        //    if (Time.time - lastMoveTime > moveCooldown)
        //    {
        //        MoveRandomly();
        //        lastMoveTime = Time.time;
        //    }

        //    // Check for hero units in range
        //    CheckForHeroes();
        //}
        //else
        //{
        //    // Aggro behavior: follow the target hero and attack
        //    if (targetHero != null)
        //    {
        //        // Follow the target hero
        //        FollowTarget();

        //        // Rotate towards the target and attack if in range
        //        RotateTowardsTarget();
        //        if (Vector3.Distance(transform.position, targetHero.transform.position) <= AutoAttackRange)
        //        {
        //            TryAutoAttack(targetHero);
        //        }
        //    }
        //    else
        //    {
        //        // Reset aggro if the target is lost
        //        isAggro = false;
        //    }
        //}
    }

    private void CheckAggro()
    {
        //ToDo: add any more info for aggro that needs to be set here
        //eg. Draw a line to the target or draw the path the goblin will take to get to it
        if(_AttackTarget != null)
        {
            AggroIndicator.SetActive(true);
        }
        else
        {
            AggroIndicator.SetActive(false);
        }
    }

    private void MoveRandomly()
    {
        // Check if the Goblin is assigned to a tile
        if (OccupiedTile == null)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.LogWarning("Goblin is not assigned to a tile. Cannot move randomly.");
            return;
        }

        // Get a random adjacent tile
        Tile randomTile = GetRandomAdjacentTile();
        if (randomTile != null && randomTile.walkable)
        {
            MoveToDestination(randomTile);
        }
        else
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.LogWarning("No valid walkable tile found for random movement.");
        }
    }

    private Tile GetRandomAdjacentTile()
    {
        // Check if the Goblin is assigned to a tile
        if (OccupiedTile == null)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.LogWarning("Goblin is not assigned to a tile. Cannot find adjacent tiles.");
            return null;
        }

        List<Tile> adjacentTiles = new List<Tile>();

        // Check all four directions (North, South, East, West)
        Vector2[] directions = new Vector2[]
        {
            new Vector2(0, 1),  // North
            new Vector2(0, -1), // South
            new Vector2(1, 0),  // East
            new Vector2(-1, 0)  // West
        };

        foreach (var direction in directions)
        {
            Vector2 checkPos = new Vector2(OccupiedTile._coordinates.x + direction.x, OccupiedTile._coordinates.y + direction.y);

            // Ensure the coordinates are within the grid bounds
            if (checkPos.x >= 0 && checkPos.x < GridManager.Instance.width &&
                checkPos.y >= 0 && checkPos.y < GridManager.Instance.height)
            {
                Tile tile = GridManager.Instance.GetTileAtCord(checkPos);
                if (tile != null && tile.walkable)
                {
                    adjacentTiles.Add(tile);
                }
            }
        }

        if (adjacentTiles.Count > 0)
        {
            return adjacentTiles[Random.Range(0, adjacentTiles.Count)];
        }

        return null;
    }

    private void CheckForHeroes()
    {
        List<Tile> tilesInRange = GridManager.Instance.GetTilesInRadius(OccupiedTile, Mathf.FloorToInt(aggroRange));
        float closestDistance = float.MaxValue;
        Character closestHero = null;

        foreach (Tile tile in tilesInRange)
        {
            if (tile.occupiedUnit != null && tile.occupiedUnit.Faction == Faction.Hero)
            {
                float distance = Vector3.Distance(transform.position, tile.occupiedUnit.transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestHero = tile.occupiedUnit as Character;
                }
            }
        }

        if (closestHero != null)
        {
            targetHero = closestHero;
        }
        else
        {
            targetHero = null;
        }
    }

    private void FollowTarget()
    {
        if (targetHero == null) return;

        // Check if the Goblin is already on the target's tile
        if (OccupiedTile == targetHero.OccupiedTile)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.Log($"{UnitName} is already on the target's tile.");
            return;
        }

        // Get the closest tile in the direction of the target
        Tile closestTile = GridManager.Instance.GetClosestTileInCardinalDirection(OccupiedTile, targetHero.OccupiedTile);
        if (closestTile != null && closestTile.walkable)
        {
            // Check if the Goblin is already moving towards the closest tile
            if (!isMoving)
            {
                MoveToDestination(closestTile);
                //RotateTowardsTarget();
            }
            if(OccupiedTile == closestTile)
            {
                Debug.Log("Trying Auto Attck");
                //TryAutoAttack(targetHero);
            }
        }
        else
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.LogWarning($"No valid closest tile found in the direction of the target.");
        }
    }

    private void RotateTowardsTarget()
    {
        if (targetHero == null) return;

        // Calculate the direction to the target
        Vector3 direction = (targetHero.transform.position - transform.position).normalized;

        // Calculate the angle in degrees
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Adjust the angle to account for the Goblin's forward direction (assuming the Goblin's "forward" is along the Y-axis)
        angle -= 90f; // Subtract 90 degrees to align with the Goblin's sprite or model orientation

        // Create a target rotation using the calculated angle
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);

        // Smoothly rotate towards the target rotation
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    //public override void TryAutoAttack(BaseUnit target)
    //{

    //    //if (isAttacking || Time.time < nextAutoAttackTime)
    //    //{
    //    //    if (GameManager.Instance._DebuggerMode)
    //    //        Debug.LogWarning($"{UnitName} is already attacking or on cooldown.");
    //    //    return;
    //    //}

    //    //// Perform the dagger attack
    //    //HurtBox hurtBox = target.GetComponentInChildren<HurtBox>();
    //    //if (hurtBox != null)
    //    //{
    //    //    if (GameManager.Instance._DebuggerMode)
    //    //        Debug.Log($"{UnitName} is attacking {target.UnitName}.");
    //    //}
    //    //else
    //    //{
    //    //    Debug.LogError($"No HurtBox found on target {target.UnitName}.");
    //    //}
    //}
    public override void TryAutoAttack(BaseUnit target)
    {
        if (isAttacking || Time.time < nextAutoAttackTime)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.LogWarning($"{UnitName} is already attacking or on cooldown.");
            return;
        }
        // Start the attack coroutine
        StartCoroutine(PerformDaggerAttack(target));
    }

    private IEnumerator PerformDaggerAttack(BaseUnit target)
    {
        //RotateTowardsTarget();
        Vector3 direction = target.transform.position - transform.position;
        direction.z = 0f;

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

        // Set the attacking state
        isAttacking = true;


        // Activate the Dagger GameObject
        if (Dagger != null)
        {
            Dagger.SetActive(true);
            if (GameManager.Instance._DebuggerMode)
                Debug.Log($"{UnitName} is attacking {target.UnitName} with a dagger.");
        }
        else
        {
            Debug.LogError("Dagger GameObject is not assigned.");
        }

        // Wait for a few seconds (simulating the attack animation)
        yield return new WaitForSeconds(1f); // Adjust the delay as needed

        // Deactivate the Dagger GameObject
        if (Dagger != null)
        {
            Dagger.SetActive(false);
            DaggerHitBox.ResetHitTargets();
        }

        isAttacking = false;
    }
}