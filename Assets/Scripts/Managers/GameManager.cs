using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{

   public GameState State;

    public static event Action<GameState> onGameStateChanged;

    public bool _DebuggerMode;


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
            case GameState.PlaceStartingUnits:
                //UnitManager.Instance.SpawnTestUnit();//This is a test
                //UnitManager.Instance.SpawnHeroes();
                break;
            case GameState.GameplayLoop:
                //UnitManager.Instance.SpawnEnemies();
                break;
            case GameState.EndGame:
                break;
        }

        onGameStateChanged?.Invoke(newState); //The "?.Invoke" is used to protect against null ref acception, incase no event is listening

    }

}

//game states: generate map, 
public enum GameState
{
    GenerateGrid = 0,
    PlaceStartingUnits = 1,
    GameplayLoop = 2,
    EndGame = 3,
}
