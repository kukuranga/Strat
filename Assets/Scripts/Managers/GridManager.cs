using UnityEngine;
using System.Collections.Generic;

public class GridManager : Singleton<GridManager>
{
    [SerializeField] private int _mountainSpawnRatio;
    [SerializeField] public int width, height;
    [SerializeField] private Tile _grassTile, _mountainTile;
    [SerializeField] private Transform _cam;
    [SerializeField] private float cameraAngle = 50f; // Initial camera angle
    [SerializeField] private float cameraDistance = 10f; // Fixed distance from the grid
    [SerializeField] private float moveSpeed = 5f; // Speed of movement
    [SerializeField] private float zoomSpeed = 2f; // Speed of zoom
    [SerializeField] private float rotationSpeed = 30f; // Speed of rotation
    [SerializeField] private bool useOrthographicCamera = true; // Toggle between orthographic and perspective cameras
    private Dictionary<Vector2, Tile> _tiles;
    public GameObject _TileParent;

    public void GenerateGrid()
    {
        if (_TileParent == null)
        {
            _TileParent = new GameObject("TileParent");
        }

        _tiles = new Dictionary<Vector2, Tile>();
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var randomTile = Random.Range(0, _mountainSpawnRatio) == 0 ? _mountainTile : _grassTile;
                var spawnedTile = Instantiate(randomTile, new Vector3(x, y), _grassTile.gameObject.transform.rotation);

                spawnedTile.name = $"Tile {x} {y}";
                spawnedTile.Init(x, y);
                spawnedTile.transform.SetParent(_TileParent.transform);
                _tiles[new Vector2(x, y)] = spawnedTile;
            }
        }

        AdjustCamera();

        GameManager.Instance.UpdateGameState(GameState.SetObjective);
    }

    private void AdjustCamera()
    {
        // Calculate grid center
        Vector3 gridCenter = new Vector3((width - 1) / 2f, (height - 1) / 2f, 0);

        if (useOrthographicCamera)
        {
            Camera camera = _cam.GetComponent<Camera>();
            if (camera != null && camera.orthographic)
            {
                float gridAspectRatio = (float)width / height;
                float screenAspectRatio = (float)Screen.width / Screen.height;

                if (gridAspectRatio > screenAspectRatio)
                {
                    camera.orthographicSize = width / (2f * screenAspectRatio);
                }
                else
                {
                    camera.orthographicSize = height / 2f;
                }

                _cam.transform.position = new Vector3(gridCenter.x, gridCenter.y + cameraDistance, -cameraDistance);
                _cam.transform.rotation = Quaternion.Euler(new Vector3(cameraAngle, 0, 0));
            }
        }
        else
        {
            _cam.transform.position = new Vector3(gridCenter.x, gridCenter.y + cameraDistance * Mathf.Tan(Mathf.Deg2Rad * cameraAngle), -cameraDistance);
            _cam.transform.rotation = Quaternion.Euler(new Vector3(cameraAngle, 0, 0));
            _cam.transform.LookAt(gridCenter);
        }
    }

    private void Update()
    {
        HandleCameraControls();
    }

    private void HandleCameraControls()
    {
        // Horizontal and vertical movement
        float horizontal = Input.GetAxis("Horizontal"); // A/D
        float vertical = Input.GetAxis("Vertical");     // W/S
        _cam.transform.position += new Vector3(horizontal * moveSpeed * Time.deltaTime, vertical * moveSpeed * Time.deltaTime, 0);

        // Zoom in/out
        if (Input.GetKey(KeyCode.Z)) // Zoom in
        {
            if (useOrthographicCamera)
            {
                Camera camera = _cam.GetComponent<Camera>();
                if (camera != null && camera.orthographic)
                {
                    camera.orthographicSize -= zoomSpeed * Time.deltaTime;
                    camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 5f, 50f); // Clamp zoom level
                }
            }
            else
            {
                _cam.transform.position += _cam.transform.forward * zoomSpeed * Time.deltaTime;
            }
        }

        if (Input.GetKey(KeyCode.X)) // Zoom out
        {
            if (useOrthographicCamera)
            {
                Camera camera = _cam.GetComponent<Camera>();
                if (camera != null && camera.orthographic)
                {
                    camera.orthographicSize += zoomSpeed * Time.deltaTime;
                    camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 5f, 50f); // Clamp zoom level
                }
            }
            else
            {
                _cam.transform.position -= _cam.transform.forward * zoomSpeed * Time.deltaTime;
            }
        }

        // Rotate along the X-axis
        if (Input.GetKey(KeyCode.Q)) // Rotate left (clockwise around X-axis)
        {
            _cam.transform.RotateAround(GetGridCenter(), Vector3.right, rotationSpeed * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.E)) // Rotate right (counterclockwise around X-axis)
        {
            _cam.transform.RotateAround(GetGridCenter(), Vector3.right, -rotationSpeed * Time.deltaTime);
        }
    }

    private Vector3 GetGridCenter()
    {
        return new Vector3((width - 1) / 2f, (height - 1) / 2f, 0);
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
                if (tile._isWalkable)
                    tile._Spawnable = true;
        }
    }

    // Returns all tiles that are moveable with a given range from the starting tile
    public List<Tile> GetAllTilesInRange(List<Vector2> movementOffsets, Tile startTile, bool _Moveable)
    {
        List<Tile> moveableTiles = new List<Tile>();

        foreach (var offset in movementOffsets)
        {
            Vector2 targetPos = new(startTile._coordinates.x + offset.x, startTile._coordinates.y + offset.y);

            // Check walkable condition only if _Moveable is true
            if (_tiles.TryGetValue(targetPos, out Tile targetTile) && (!_Moveable || targetTile.walkable))
            {
                moveableTiles.Add(targetTile);
            }
        }

        return moveableTiles;
    }

    public Tile GetTileAtCord(Vector2 _cord)
    {
        if (_tiles.TryGetValue(_cord, out Tile tile))
        {
            return tile;
        }
        else
        {
            Debug.LogWarning($"Tile at coordinates {_cord} not found.");
            return null;
        }
    }

    // New method to get all tiles in the grid
    public Dictionary<Vector2, Tile> GetAllTiles()
    {
        if (_tiles == null)
        {
            Debug.LogError("Tiles dictionary is not initialized. Make sure GenerateGrid() is called before this method.");
            return new Dictionary<Vector2, Tile>();
        }

        return _tiles;
    }

    // New method to get a path between two tiles using A* pathfinding
    public List<Tile> GetPath(Tile startTile, Tile endTile)
    {
        if (startTile == null || endTile == null)
        {
            Debug.LogError("Start tile or end tile is null. Cannot find path.");
            return new List<Tile>();
        }

        AStarPathfinding pathfinding = new AStarPathfinding(this);
        return pathfinding.FindPath(startTile, endTile);
    }

    public List<Tile> GetTilesInRadius(Tile centerTile, int radius)
    {
        List<Tile> tilesInRadius = new List<Tile>();
        if (centerTile == null || radius < 0)
        {
            Debug.LogWarning("Invalid input: Center tile is null or radius is negative.");
            return tilesInRadius;
        }

        // Use a queue for BFS
        Queue<(Tile tile, int distance)> queue = new Queue<(Tile, int)>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        // Start with the center tile
        queue.Enqueue((centerTile, 0));
        visited.Add(centerTile._coordinates);

        while (queue.Count > 0)
        {
            var (currentTile, currentDistance) = queue.Dequeue();

            // Add the current tile to the result if it's within the radius
            if (currentDistance <= radius)
            {
                tilesInRadius.Add(currentTile);
            }

            // Stop exploring if we've reached the radius limit
            if (currentDistance >= radius)
            {
                continue;
            }

            // Explore neighboring tiles
            foreach (var neighborOffset in GetNeighborOffsets())
            {
                Vector2Int neighborCoords = new Vector2Int(
                    currentTile._coordinates.x + neighborOffset.x,
                    currentTile._coordinates.y + neighborOffset.y
                );

                // Check if the neighbor coordinates are valid and not already visited
                if (_tiles.TryGetValue(neighborCoords, out Tile neighborTile) && !visited.Contains(neighborCoords))
                {
                    visited.Add(neighborCoords);
                    queue.Enqueue((neighborTile, currentDistance + 1));
                }
            }
        }

        return tilesInRadius;
    }

    // Helper method to get offsets for 4-directional neighbors
    private List<Vector2Int> GetNeighborOffsets()
    {
        return new List<Vector2Int>
    {
        new Vector2Int(1, 0),  // Right
        new Vector2Int(-1, 0), // Left
        new Vector2Int(0, 1),  // Up
        new Vector2Int(0, -1), // Down
        new Vector2Int(1, 1),  // Up-Right
        new Vector2Int(-1, 1), // Up-Left
        new Vector2Int(1, -1), // Down-Right
        new Vector2Int(-1, -1) // Down-Left
    };
    
    }
    public Tile GetClosestTileInDirection(Tile startingTile, Tile targetTile)
    {
        if (startingTile == null || targetTile == null)
        {
            Debug.LogWarning("Starting tile or target tile is null. Cannot find closest tile.");
            return null;
        }

        // Calculate the direction from the starting tile to the target tile
        Vector2 direction = new Vector2(
            targetTile._coordinates.x - startingTile._coordinates.x,
            targetTile._coordinates.y - startingTile._coordinates.y
        ).normalized;

        // Determine the closest adjacent tile in the direction of the target
        Vector2Int closestTileCoords = new Vector2Int(
            startingTile._coordinates.x + Mathf.RoundToInt(direction.x),
            startingTile._coordinates.y + Mathf.RoundToInt(direction.y)
        );

        // Check if the closest tile coordinates are within the grid bounds
        if (closestTileCoords.x >= 0 && closestTileCoords.x < width &&
            closestTileCoords.y >= 0 && closestTileCoords.y < height)
        {
            // Get the tile at the calculated coordinates
            if (_tiles.TryGetValue(closestTileCoords, out Tile closestTile))
            {
                // Return the tile if it's walkable and unoccupied
                if (closestTile.walkable && closestTile.occupiedUnit == null)
                {
                    return closestTile;
                }
                else
                {
                    Debug.LogWarning($"Closest tile at {closestTileCoords} is not walkable or is occupied.");
                }
            }
            else
            {
                Debug.LogWarning($"Tile at {closestTileCoords} not found in the grid.");
            }
        }
        else
        {
            Debug.LogWarning($"Closest tile coordinates {closestTileCoords} are out of grid bounds.");
        }

        return null;
    }

    //returns the closest tile in a north east west south direction
    public Tile GetClosestTileInCardinalDirection(Tile startingTile, Tile targetTile)
    {
        if (startingTile == null || targetTile == null)
        {
            Debug.LogWarning("Starting tile or target tile is null. Cannot find closest tile.");
            return null;
        }

        // Calculate the direction from the starting tile to the target tile
        Vector2 direction = new Vector2(
            targetTile._coordinates.x - startingTile._coordinates.x,
            targetTile._coordinates.y - startingTile._coordinates.y
        );

        // Determine the closest cardinal direction
        Vector2 cardinalDirection = GetCardinalDirection(direction);

        // Calculate the coordinates of the closest tile in the cardinal direction
        Vector2Int closestTileCoords = new Vector2Int(
            startingTile._coordinates.x + (int)cardinalDirection.x,
            startingTile._coordinates.y + (int)cardinalDirection.y
        );

        // Check if the closest tile coordinates are within the grid bounds
        if (closestTileCoords.x >= 0 && closestTileCoords.x < width &&
            closestTileCoords.y >= 0 && closestTileCoords.y < height)
        {
            // Get the tile at the calculated coordinates
            if (_tiles.TryGetValue(closestTileCoords, out Tile closestTile))
            {
                // Return the tile if it's walkable and unoccupied
                if (closestTile.walkable && closestTile.occupiedUnit == null)
                {
                    return closestTile;
                }
                else
                {
                    if (GameManager.Instance._DebuggerMode)
                        Debug.LogWarning($"Closest tile at {closestTileCoords} is not walkable or is occupied.");
                }
            }
            else
            {
                if (GameManager.Instance._DebuggerMode)
                    Debug.LogWarning($"Tile at {closestTileCoords} not found in the grid.");
            }
        }
        else
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.LogWarning($"Closest tile coordinates {closestTileCoords} are out of grid bounds.");
        }

        return null;
    }

    private Vector2 GetCardinalDirection(Vector2 direction)
    {
        // Determine the closest cardinal direction based on the direction vector
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Normalize the angle to the range [0, 360)
        if (angle < 0)
        {
            angle += 360;
        }

        // Determine the closest cardinal direction
        if (angle >= 45 && angle < 135)
        {
            return Vector2.up; // North
        }
        else if (angle >= 135 && angle < 225)
        {
            return Vector2.left; // West
        }
        else if (angle >= 225 && angle < 315)
        {
            return Vector2.down; // South
        }
        else
        {
            return Vector2.right; // East
        }
    }

    //Returns a random tile from the list of tiles
    public Tile GetARandomTile()
    {
        if (_tiles == null || _tiles.Count == 0)
        {
            Debug.LogError("Tiles dictionary is not initialized or empty. Make sure GenerateGrid() is called before this method.");
            return null;
        }

        // Get random key from dictionary
        var keys = _tiles.Keys;
        int randomIndex = Random.Range(0, keys.Count);
        Vector2 randomKey = new List<Vector2>(keys)[randomIndex];

        return _tiles[randomKey];
    }
}