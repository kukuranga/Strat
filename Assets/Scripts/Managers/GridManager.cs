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

    public Tile GetHeroSpawnTile()
    {
        return _tiles.Where(t => t.Key.x < width/2 && t.Value.walkable).OrderBy(t => Random.value).First().Value;
    }

    public Tile GetEnemySpawnTile()
    {
        return _tiles.Where(t => t.Key.x > width/2 && t.Value.walkable).OrderBy(t => Random.value).First().Value;
    }

    public Tile GetTileAtPosition(Vector2 pos)
    {
        if (_tiles.TryGetValue(pos, out var tile))
        {
            return tile;
        }
        return null;
    }

    // Returns all tiles where _Spawnable is true
    public List<Tile> GetSpawnableTiles()
    {
        return _tiles.Values.Where(tile => tile._Spawnable).ToList();
    }

    public void MakeTilesSpawnable(List<Vector2> positions)
    {
        if (_tiles == null)
        {
            Debug.LogError("Tiles dictionary is not initialized. Make sure GenerateGrid() is called before this method.");
            return;
        }

        foreach (var position in positions)
        {
            if (_tiles.TryGetValue(position, out Tile tile))
                if(tile._isWalkable)
                    tile._Spawnable = true;            
        }
    }

    public void MakeRowTilesSpawnable(int row)
    {
        if (_tiles == null)
        {
            Debug.LogError("Tiles dictionary is not initialized. Make sure GenerateGrid() is called before this method.");
            return;
        }

        var positions = new List<Vector2>();
        for (int x = 0; x < width; x++)
        {
            positions.Add(new Vector2(x, row));
        }
        MakeTilesSpawnable(positions);
    }

    // Returns all tiles the unit can move based on the specific movement offsets
    public List<Tile> _GetAllMoveableTiles(List<Vector2> movementOffsets, Tile startTile)
    {
        // Result list to store all moveable tiles
        List<Tile> moveableTiles = new List<Tile>();

        // Loop through the movement offsets
        foreach (var offset in movementOffsets)
        {
            // Calculate the target position for each offset
            Vector2 targetPos = new Vector2(startTile._coordinates.x + offset.x, startTile._coordinates.y + offset.y);

            // Check if the target position is valid and the tile is walkable
            if (_tiles.TryGetValue(targetPos, out Tile targetTile) && targetTile.walkable)
            {
                moveableTiles.Add(targetTile);
            }
        }

        return moveableTiles;
    }
}
