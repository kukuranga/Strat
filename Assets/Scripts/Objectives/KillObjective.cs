using UnityEngine;

[CreateAssetMenu(fileName = "New Kill Objective", menuName = "Objectives/Kill Objective")]
public class KillObjective : BaseObjective
{
    public int RequiredKills;
    public GameObject SpawnerPrefab;
    private int currentKills;

    public override bool IsObjectiveComplete()
    {
        return currentKills >= RequiredKills;
    }

    public override void InitializeObjective()
    {
        base.InitializeObjective();
        //Spawn the spawnerObject
        currentKills = 0; // Reset progress
        Debug.Log($"Kill {RequiredKills} enemies to complete the objective.");
    }

    // Call this method to update kills (e.g., from combat logic)
    public void AddKill()
    {
        //ToDo: Figure out how to check this data during the games stats
        currentKills++;
        Debug.Log($"Kill progress: {currentKills}/{RequiredKills}");
    }

    public override void ObjectiveUpdate()
    {
        //this function to be called each frame during the gameloop;
    }
}
