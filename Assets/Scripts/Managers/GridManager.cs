using UnityEngine;
using System.Collections.Generic;

public class GridManager : Singleton<GridManager>
{
    [SerializeField] private int _mountainSpawnRatio;
    [SerializeField] private int width, height;
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

    //returns all tiles that are moveable with a given range from the starting tile
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
            return null;
        }
    }

}
