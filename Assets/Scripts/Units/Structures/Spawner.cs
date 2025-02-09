using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : Structure
{
    // Prefabs that can be spawned
    public List<GameObject> UnitsToSpawn;

    // A list of Vector2 offsets from the spawner's tile
    // used by GridManager's GetAllTilesInRange
    public List<Vector2> SpawnRange;

    // How often (in seconds) we spawn
    public float Interval = 5f;

    // The maximum number of units this spawner will ever create
    [Tooltip("Maximum total units this spawner can produce. If 0 or negative, spawns infinitely.")]
    public int maxSpawnCount = 5;

    // We'll store the tile references returned by GetAllTilesInRange
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
            Debug.LogWarning($"{name} Spawner has no valid OccupiedTile. Aborting spawn logic.");
            return;
        }

        // 2) Use the GridManager method to get all tiles in range from the spawner's tile
        //    Set _Moveable to false if you don't need them to be walkable.
        SpawnLocations = GridManager.Instance.GetAllTilesInRange(
            SpawnRange,
            OccupiedTile,
            false // or true if you only want walkable tiles
        );

        if (SpawnLocations.Count == 0)
        {
            Debug.LogWarning($"{name} found no spawn locations in range of tile {OccupiedTile.TileName}!");
            return;
        }

        // 3) Begin spawning - one unit each cycle until we reach max
        StartCoroutine(SpawnRoutine());
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

            Debug.Log($"{name} reached maxSpawnCount ({maxSpawnCount}). Stopping spawner.");
        }
    }

    private void AttemptSpawn()
    {
        // Only spawn if we have valid Tiles & Prefabs
        if (SpawnLocations.Count == 0 || UnitsToSpawn.Count == 0)
        {
            Debug.LogWarning($"{name}: No spawn locations or no prefabs available!");
            return;
        }

        // Pick a random free tile
        Tile randomTile = GetRandomFreeTile();
        if (randomTile == null)
        {
            Debug.Log($"{name}: No free tile to spawn on. Skipping cycle.");
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
            Debug.LogError($"{name}: Spawned prefab '{randomPrefab.name}' has no BaseUnit! Destroying.");
            Destroy(newObj);
            return;
        }

        // Place it on the tile if it's free
        if (randomTile.occupiedUnit == null)
        {
            randomTile.SetUnit(newUnit);
            spawnedCount++;

            Debug.Log(
                $"{name} spawned '{newUnit.UnitName}' on tile '{randomTile.TileName}'. " +
                $"Total: {spawnedCount}/{maxSpawnCount}"
            );
        }
        else
        {
            // If the tile got occupied in the meantime
            Debug.LogWarning($"{name}: Tile just got occupied! Destroying new unit.");
            Destroy(newObj);
        }
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
