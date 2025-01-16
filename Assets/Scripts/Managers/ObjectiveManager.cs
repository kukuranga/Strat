using System.Collections.Generic;
using UnityEngine;

public class ObjectiveManager : Singleton<ObjectiveManager>
{
    [Header("Objective Settings")]
    public List<BaseObjective> AllObjectives; // List of all possible objectives
    public BaseObjective _ActiveObjective;    // Currently active objective
    public bool ObjectiveLoopActive = false;

    private void Start()
    {
        if (AllObjectives == null || AllObjectives.Count == 0)
        {
            Debug.LogError("Objective list is empty. Add objectives to the list.");
            return;
        }

        // Initialize with a random objective at start (optional)
        //GetRandomObjective();
    }

    private void Update()
    {
        if(ObjectiveLoopActive)
        {
            _ActiveObjective.ObjectiveUpdate();
        }
    }

    public void StartObjectiveLoop()
    {
        ObjectiveLoopActive = true;
    }
    public void StopObjectiveLoop()
    {
        ObjectiveLoopActive = false;
    }

    // Method to get a random objective from the list
    public void GetRandomObjective()
    {
        if (AllObjectives.Count == 0)
        {
            Debug.LogWarning("No objectives available.");
            return;
        }

        // Randomize the selection
        int randomIndex = Random.Range(0, AllObjectives.Count);
        _ActiveObjective = AllObjectives[randomIndex];

        // Initialize the selected objective
        _ActiveObjective.InitializeObjective();

        Debug.Log($"New Active Objective: {_ActiveObjective.ObjectiveName}");

        GameManager.Instance.UpdateGameState(GameState.PlaceStartingUnits); //Might change this to work with the full game loop once finished
    }

    // Method to check if the active objective is complete
    public bool CheckActiveObjective()
    {
        if (_ActiveObjective == null)
        {
            Debug.LogWarning("No active objective to check.");
            return false;
        }

        return _ActiveObjective.IsObjectiveComplete();
    }

    // Method to set a specific objective (optional)
    public void SetActiveObjective(BaseObjective objective)
    {
        if (objective == null)
        {
            Debug.LogWarning("Cannot set a null objective.");
            return;
        }

        _ActiveObjective = objective;
        _ActiveObjective.InitializeObjective();

        Debug.Log($"Set Active Objective: {_ActiveObjective.ObjectiveName}");
    }

    // Called when a unit is killed
    public void OnUnitKilled(BaseUnit killedUnit)
    {
        // Check if the active objective is a KillObjective
        if (_ActiveObjective is KillObjective killObjective) //TODO: Change this to be non player decided characters
        {
            killObjective.AddKill();
        }
    }
}

