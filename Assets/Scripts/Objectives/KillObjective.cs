using UnityEngine;

[CreateAssetMenu(fileName = "New Kill Objective", menuName = "Objectives/Kill Objective")]
public class KillObjective : BaseObjective
{
    public int RequiredKills;
    public GameObject SpawnerPrefab;
    private int currentKills;

    // Example coordinate for spawner. You can expose this or randomize it.
    public Vector2 spawnerCoordinates = new Vector2(2, 2);

    public override bool IsObjectiveComplete()
    {
        return currentKills >= RequiredKills;
    }

    public override void InitializeObjective()
    {
        base.InitializeObjective();

        // 1) Pick the tile for placing the spawner
        Tile spawnTile = GridManager.Instance.GetTileAtCord(spawnerCoordinates);

        if (spawnTile == null)
        {
            Debug.LogWarning($"KillObjective: No tile found at coords {spawnerCoordinates}!");
        }
        else if (spawnTile.occupiedUnit != null)
        {
            Debug.LogWarning($"KillObjective: Tile {spawnerCoordinates} is occupied, cannot spawn spawner!");
        }
        else
        {
            // 2) Instantiate the spawner prefab at the tile's position
            GameObject spawnerObj = Instantiate(
                SpawnerPrefab,
                spawnTile.transform.position,
                Quaternion.identity
            );

            // Force the Z-axis to -1
            Vector3 newPos = spawnerObj.transform.position;
            newPos.z = -1f;
            spawnerObj.transform.position = newPos;

            // 3) Attach as a BaseUnit occupant on the tile
            BaseUnit spawnerUnit = spawnerObj.GetComponent<BaseUnit>();
            if (spawnerUnit != null)
            {
                spawnTile.SetUnit(spawnerUnit, false);

                // 4) Now that OccupiedTile is set, call InitializeSpawner()
                Spawner spawnerScript = spawnerObj.GetComponent<Spawner>();
                if (spawnerScript != null)
                {
                    spawnerScript.InitializeSpawner();
                }
                else
                {
                    Debug.LogWarning(
                        "KillObjective: SpawnerPrefab has a BaseUnit but no 'Spawner' script to initialize."
                    );
                }
            }
            else
            {
                Debug.LogWarning("KillObjective: SpawnerPrefab has no BaseUnit component! Can't attach to tile.");
            }
        }

        // Reset kill progress
        currentKills = 0;
        Debug.Log($"Kill {RequiredKills} enemies to complete the objective.");
    }

    // Call this method to update kills (e.g., from combat logic)
    public void AddKill()
    {
        currentKills++;
        Debug.Log($"Kill progress: {currentKills}/{RequiredKills}");
    }

    public override void ObjectiveUpdate()
    {
        // Called each frame or periodically
    }
}
