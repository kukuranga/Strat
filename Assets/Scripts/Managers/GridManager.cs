
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GridManager : Singleton<GridManager>
{  

    [SerializeField] private int _mountainSpawnRatio;
    [SerializeField] private int width, height;
    [SerializeField] private Tile _grassTile, _mountainTile; 
    [SerializeField] private Transform _cam;
    private Dictionary<Vector2, Tile> _tiles;
    public GameObject _TileParent;


    public void GenerateGrid()
    {
        // Ensure _TileParent exists
        if (_TileParent == null)
        {
            _TileParent = new GameObject("TileParent");
        }

        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++) // Biom spawning logic goes here
            {
                var randomTile = Random.Range(0, _mountainSpawnRatio) == 0 ? _mountainTile : _grassTile;
                var spawnedTile = Instantiate(randomTile, new Vector3(x, y), _grassTile.gameObject.transform.rotation);

                // Set the tile's name and initialize it
                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.Init(x, y);

                // Set the parent to _TileParent
                spawnedTile.transform.SetParent(_TileParent.transform);

                // Add to the dictionary
                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        // Adjust the camera position and rotation
        _cam.transform.position = new Vector3((float)width / 2 - 0.5f, (float)-5, (float)-7.5);
        _cam.transform.rotation = Quaternion.Euler(new Vector3(-50, 0, 0));

        // Notify the GameManager to update the game state
        GameManager.Instance.UpdateGameState(GameState.PlaceStartingUnits);
    }

    //todo: Check if this values actually get called when changing the unit manager
    public Tile GetHeroSpawnTile()
    {
        return _tiles.Where(t => t.Key.x < width/2 && t.Value.walkable).OrderBy(t => Random.value).First().Value;
    }

    public Tile GetEnemySpawnTile()
    {
        return _tiles.Where(t => t.Key.x > width/2 && t.Value.walkable).OrderBy(t => Random.value).First().Value;
    }

    public Tile GetTileAtPosition(Vector2 pos) {
        if( _tiles.TryGetValue(pos,out var tile))
        {
            return tile;
        }
        return null;
    }


    //Returns all tiles the unit can move
    public List<Tile> _GetAllMoveableTiles(List<Vector2> movementOffsets, Tile startTile)
    {
        // Result list to store all moveable tiles
        List<Tile> moveableTiles = new List<Tile>();

        // A set to keep track of visited tiles
        HashSet<Vector2> visited = new HashSet<Vector2>();

        // Start the recursive search
        RecursiveSearch(startTile, movementOffsets, movementOffsets.Count, visited, moveableTiles);

        return moveableTiles;
    }

    // Recursive helper method
    private void RecursiveSearch(
        Tile currentTile,
        List<Vector2> movementOffsets,
        int remainingSteps,
        HashSet<Vector2> visited,
        List<Tile> moveableTiles)
    {
        // Get the current tile position
        Vector2 currentPosition = new Vector2(currentTile._coordinates.x, currentTile._coordinates.y);

        // Base case: if no steps remain or the tile is already visited, exit
        if (remainingSteps <= 0 || visited.Contains(currentPosition))
            return;

        // Add the current tile to the visited set and result list
        visited.Add(currentPosition);
        moveableTiles.Add(currentTile);

        // Explore neighbors based on movement offsets
        foreach (var offset in movementOffsets)
        {
            Vector2 neighborPos = new Vector2(currentTile._coordinates.x + offset.x, currentTile._coordinates.y + offset.y);

            // Check if the neighbor is valid and walkable
            if (_tiles.TryGetValue(neighborPos, out Tile neighborTile) && neighborTile.walkable)
            {
                RecursiveSearch(neighborTile, movementOffsets, remainingSteps - 1, visited, moveableTiles);
            }
        }
    }
}
