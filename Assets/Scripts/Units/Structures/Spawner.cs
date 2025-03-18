using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Structure
{
    // Prefabs that can be spawned
    public List<GameObject> UnitsToSpawn;

    // How often (in seconds) we spawn
    public float Interval = 5f;

    // The maximum number of units this spawner will ever create
    [Tooltip("Maximum total units this spawner can produce. If 0 or negative, spawns infinitely.")]
    public int maxSpawnCount = 5;

    // We'll store the tile references returned by A* pathfinding
    private List<Tile> SpawnLocations = new List<Tile>();

    // Tracks how many units have been spawned so far
    private int spawnedCount = 0;

    /// <summary>
    /// Instead of using Start(), we create a public method
    /// that initializes the spawner AFTER the tile is assigned.
    /// Call this AFTER you do tile.SetUnit(this).
    /// </summary>
    public void InitializeSpawner()
    {
        // 1) Check that we have a valid tile
        //    'this' is a Structure -> BaseUnit, so check OccupiedTile
        if (OccupiedTile == null)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.LogWarning($"{name} Spawner has no valid OccupiedTile. Aborting spawn logic.");
            return;
        }

        // 2) Use A* pathfinding to find all reachable tiles within a certain range
        //    For simplicity, we'll assume a fixed range (e.g., 3 tiles)
        int spawnRange = 3;
        SpawnLocations = GetTilesInRange(spawnRange);

        if (SpawnLocations.Count == 0)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.LogWarning($"{name} found no spawn locations in range of tile {OccupiedTile.TileName}!");
            return;
        }

        _Interactable = true;

        // 3) Begin spawning - one unit each cycle until we reach max
        StartCoroutine(SpawnRoutine());
    }

    /// <summary>
    /// Finds all tiles within a given range using A* pathfinding.
    /// </summary>
    private List<Tile> GetTilesInRange(int range)
    {
        List<Tile> tilesInRange = new List<Tile>();
        AStarPathfinding pathfinding = new AStarPathfinding(GridManager.Instance);

        // Get all tiles from the GridManager
        Dictionary<Vector2, Tile> allTiles = GridManager.Instance.GetAllTiles();

        // Iterate through all tiles in the grid
        foreach (var tilePair in allTiles)
        {
            Tile tile = tilePair.Value;

            // Skip if the tile is not walkable or already occupied
            if (!tile.walkable || tile.occupiedUnit != null)
                continue;

            // Calculate the distance from the spawner's tile to the current tile
            int distance = Mathf.Abs(tile._coordinates.x - OccupiedTile._coordinates.x) +
                           Mathf.Abs(tile._coordinates.y - OccupiedTile._coordinates.y);

            // If the tile is within range, add it to the list
            if (distance <= range)
            {
                tilesInRange.Add(tile);
            }
        }

        return tilesInRange;
    }

    private IEnumerator SpawnRoutine()
    {
        // If maxSpawnCount <= 0, interpret that as "infinite"
        if (maxSpawnCount <= 0)
        {
            while (true)
            {
                yield return new WaitForSeconds(Interval);
                AttemptSpawn();
            }
        }
        else
        {
            for (int i = 0; i < maxSpawnCount; i++)
            {
                yield return new WaitForSeconds(Interval);
                AttemptSpawn();
            }
            if (GameManager.Instance._DebuggerMode)
                Debug.Log($"{name} reached maxSpawnCount ({maxSpawnCount}). Stopping spawner.");
        }
    }

    private void AttemptSpawn()
    {
        // Only spawn if we have valid Tiles & Prefabs
        if (SpawnLocations.Count == 0 || UnitsToSpawn.Count == 0)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.LogWarning($"{name}: No spawn locations or no prefabs available!");
            return;
        }

        // Pick a random free tile
        Tile randomTile = GetRandomFreeTile();
        if (randomTile == null || !randomTile.walkable)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.Log($"{name}: No free walkable tile to spawn on. Skipping cycle.");
            return;
        }

        // Pick a random prefab
        GameObject randomPrefab = UnitsToSpawn[Random.Range(0, UnitsToSpawn.Count)];

        // Spawn the prefab with the z-axis position set to -1
        Vector3 spawnPosition = new Vector3(randomTile.transform.position.x, randomTile.transform.position.y, -1f);
        GameObject newObj = Instantiate(
            randomPrefab,
            spawnPosition,  // Use the modified spawn position
            Quaternion.identity
        );

        // Must have a BaseUnit component
        BaseUnit newUnit = newObj.GetComponent<BaseUnit>();
        if (newUnit == null)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.LogError($"{name}: Spawned prefab '{randomPrefab.name}' has no BaseUnit! Destroying.");
            Destroy(newObj);
            return;
        }

        // Assign the Goblin to the tile
        randomTile.SetUnit(newUnit, true);
        spawnedCount++;
        if (GameManager.Instance._DebuggerMode)
                Debug.Log(
                $"{name} spawned '{newUnit.UnitName}' on tile '{randomTile.TileName}'. " +
                $"Total: {spawnedCount}/{maxSpawnCount}"
            );
    }

    /// <summary>
    /// Finds a random unoccupied tile from our SpawnLocations.
    /// </summary>
    private Tile GetRandomFreeTile()
    {
        for (int i = 0; i < SpawnLocations.Count; i++)
        {
            Tile candidate = SpawnLocations[Random.Range(0, SpawnLocations.Count)];
            if (candidate.occupiedUnit == null)
            {
                return candidate;
            }
        }
        return null; // All tiles occupied
    }
}