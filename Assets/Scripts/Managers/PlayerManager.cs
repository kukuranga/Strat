using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    public int _PlayerNumber;
    public bool _HasUnitInHand = false;
    public BaseUnit _UnitInHand;
    public GameObject _BaseUnitGameObject;
    public BaseUnit _SelectedUnit;
    public List<Vector2> _StartingSpawningTiles; //Tiles the player is allowed to spawn units on after buying
    //Syntax: each option in here should return the values of the tiles capable to spawn in
    //OnStart set the first 2 rows (0,1)

    public List<Tile> _SelectedUnitsTiles;

    public float fixedZPosition = -1f; // Fixed Z position for the unit in world space
    public bool _UnitHoverOverTile;

    private void Start()
    {
        //Marks the starting tiles for the starting player
        //GridManager.Instance.MakeTilesSpawnable(_StartingSpawningTiles);
        
    }

    private void Update()
    {
        //RayCastDebbuger
        if (GameManager.Instance._DebuggerMode)
        {
            if (Input.GetMouseButtonDown(0)) // Left mouse click
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray, out RaycastHit hit))
                {
                    Debug.Log($"Raycast hit: {hit.collider.name} (Layer: {hit.collider.gameObject.layer})");
                }
                else
                {
                    Debug.Log("No hit detected");
                }
            }
        }

        if(_HasUnitInHand && !_UnitHoverOverTile)
        {
            // Set the position of the unit in hand to follow the mouse cursor
            Vector3 mousePosition = Input.mousePosition; // Get mouse position in screen space
            mousePosition.z = Camera.main.WorldToScreenPoint(_UnitInHand.transform.position).z; // Maintain the current Z-depth
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition); // Convert screen space to world space
            _UnitInHand.gameObject.transform.position = new Vector3(worldPosition.x, worldPosition.y, _UnitInHand.transform.position.z); // Update position
        }
    }

    public void InitializePlayerData()
    {
        GridManager.Instance.MakeRowTilesSpawnable(1);
    }

    // To be called by the multiplayer manager to set the correct player number to this player
    public void SetPlayerNumber(int i)
    {
        _PlayerNumber = i;
    }

    //Method that clears all units selected and in hand
    public void ClearAll()
    {
        EmptyHand();
        ClearSelectedUnit();
    }

    // Sets the unit in hand and allows the player to move it to a selected square
    public void SetUnitInHand(BaseUnit _unit, GameObject _GameObj)
    {
        ClearAll();
        _UnitInHand = _unit;
        _BaseUnitGameObject = _GameObj;
        _HasUnitInHand = true;

        // Optional: Disable unit interaction while it's in hand
        _UnitInHand.ToggleInteraction(false);
        MenuManager.Instance.ShowSelectedHero(_UnitInHand);
    }

    public void SetUnitSelected(BaseUnit _unit)
    {
        ClearAll();
        EmptyHand();
        _SelectedUnit = _unit;
        _SelectedUnitsTiles = GridManager.Instance._GetAllMoveableTiles(_SelectedUnit.Moves, _SelectedUnit.OccupiedTile);
        foreach(Tile t in _SelectedUnitsTiles)
        {
            t.SetSelectedTile(true);
        }
    }

    public void ClearSelectedUnit()
    {
        foreach(Tile t in _SelectedUnitsTiles)
        {
            t.SetSelectedTile(false);
        }
        _SelectedUnitsTiles.Clear();
        _SelectedUnit = null;
    }

    // Clears the unit from the player's hand
    public void EmptyHand()
    {
        if (_UnitInHand != null)
        {
            // Optional: Re-enable unit interaction
            _UnitInHand.ToggleInteraction(true);
        }

        _UnitInHand = null;
        _BaseUnitGameObject = null;
        _HasUnitInHand = false;
        MenuManager.Instance.ClearSelectedHero();
    }
}
