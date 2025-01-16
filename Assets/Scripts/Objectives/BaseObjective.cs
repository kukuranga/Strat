using UnityEngine;

public abstract class BaseObjective : ScriptableObject
{
    [Header("Objective Info")]
    public string ObjectiveName;          // Name of the objective
    public string ObjectiveDescription;   // Description of the objective for the player

    [Header("Objective Progress")]
    public bool IsComplete = false;       // Tracks if the objective is complete

    // Abstract method to determine if the objective is complete
    public abstract bool IsObjectiveComplete();

    public abstract void ObjectiveUpdate();

    // Virtual method to initialize the objective
    public virtual void InitializeObjective()
    {
        IsComplete = false; // Reset completion status
        Debug.Log($"Objective Initialized: {ObjectiveName}");
        Debug.Log(ObjectiveDescription);
    }

    // Optional method to clean up or finalize the objective
    public virtual void FinalizeObjective()
    {
        Debug.Log($"Objective Completed: {ObjectiveName}");
    }
}
