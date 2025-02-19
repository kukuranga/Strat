using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : Character
{
    [Header("Goblin Settings")]
    public float aggroRange = 5f; // Range within which the Goblin will detect hero units
    public float moveCooldown = 2f; // Cooldown between random movements
    public float attackRange = 1f; // Range for melee attack
    public MeleeDaggerAttack daggerAttack; // Reference to the dagger attack ScriptableObject

    private float lastMoveTime;
    private bool isAggro = false;
    private Character targetHero;

    protected override void Start()
    {
        base.Start();
        lastMoveTime = Time.time;
    }

    protected override void Update()
    {
        base.Update();

        if (!isAggro)
        {
            // Random movement if no hero is in range
            if (Time.time - lastMoveTime > moveCooldown)
            {
                MoveRandomly();
                lastMoveTime = Time.time;
            }

            // Check for hero units in range
            CheckForHeroes();
        }
        else
        {
            // Aggro behavior: follow the target hero and attack
            if (targetHero != null)
            {
                // Follow the target hero
                FollowTarget();

                // Rotate towards the target and attack if in range
                RotateTowardsTarget();
                if (Vector3.Distance(transform.position, targetHero.transform.position) <= attackRange)
                {
                    TryMeleeAttack(targetHero);
                }
            }
            else
            {
                // Reset aggro if the target is lost
                isAggro = false;
            }
        }
    }

    private void MoveRandomly()
    {
        // Check if the Goblin is assigned to a tile
        if (OccupiedTile == null)
        {
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
            Debug.LogWarning("No valid walkable tile found for random movement.");
        }
    }

    private Tile GetRandomAdjacentTile()
    {
        // Check if the Goblin is assigned to a tile
        if (OccupiedTile == null)
        {
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
        // Find all hero units within aggro range
        foreach (var tilePair in GridManager.Instance.GetAllTiles())
        {
            Tile tile = tilePair.Value;
            if (tile.occupiedUnit != null && tile.occupiedUnit.Faction == Faction.Hero)
            {
                float distance = Vector3.Distance(transform.position, tile.occupiedUnit.transform.position);
                if (distance <= aggroRange)
                {
                    targetHero = tile.occupiedUnit as Character;
                    isAggro = true;
                    break;
                }
            }
        }
    }

    private void FollowTarget()
    {
        if (targetHero == null) return;

        // Move towards the target hero
        Tile targetTile = targetHero.OccupiedTile;
        if (targetTile != null && targetTile.walkable)
        {
            MoveToDestination(targetTile);
        }
    }

    private void RotateTowardsTarget()
    {
        if (targetHero == null) return;

        // Rotate towards the target
        Vector3 direction = (targetHero.transform.position - transform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotateSpeed * Time.deltaTime);
    }

    private void TryMeleeAttack(Character target)
    {
        if (isAttacking || Time.time < nextAutoAttackTime)
        {
            Debug.LogWarning($"{UnitName} is already attacking or on cooldown.");
            return;
        }

        // Perform the dagger attack
        HurtBox hurtBox = target.GetComponentInChildren<HurtBox>();
        if (hurtBox != null)
        {
            Debug.Log($"{UnitName} is attacking {target.UnitName}.");
            daggerAttack.PerformAttack(this, hurtBox);
        }
        else
        {
            Debug.LogError($"No HurtBox found on target {target.UnitName}.");
        }
    }
}   