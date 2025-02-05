using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

   public GameState State;

    public static event Action<GameState> onGameStateChanged;

    public bool _DebuggerMode;

    public HitStop _HitStop;


    private void Start()
    {
        UpdateGameState(GameState.GenerateGrid);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.GenerateGrid:
                GridManager.Instance.GenerateGrid();
                PlayerManager.Instance.InitializePlayerData();
                break;
            case GameState.SetObjective:
                //start the object setup 
                ObjectiveManager.Instance.GetRandomObjective();
                GridManager.Instance.MakeRowTilesSpawnable(1); //Todo: Chnage this to only work with the first structure
                GridManager.Instance.MakeRowTilesSpawnable(0);
                break;
            case GameState.PlaceStartingUnits:
                GameManager.Instance.UpdateGameState(GameState.GameplayLoop);
                //UnitManager.Instance.SpawnTestUnit();//This is a test
                //UnitManager.Instance.SpawnHeroes();
                break;
            case GameState.GameplayLoop:
                ObjectiveManager.Instance.StartObjectiveLoop();
                MenuManager.Instance.ShowObjectiveBanner();//Shows the objective banner
                //UnitManager.Instance.SpawnEnemies();
                break;
            case GameState.EndGame:
                ObjectiveManager.Instance.StopObjectiveLoop();
                break;
        }

        onGameStateChanged?.Invoke(newState); //The "?.Invoke" is used to protect against null ref acception, incase no event is listening

    }

}

//game states: generate map, 
public enum GameState
{
    GenerateGrid = 0,
    SetObjective = 1,
    PlaceStartingUnits = 2,
    GameplayLoop = 3,
    EndGame = 4,
}
