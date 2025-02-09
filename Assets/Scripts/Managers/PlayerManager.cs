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
    public List<Vector2> _StartingSpawningTiles; // Tiles the player is allowed to spawn units on after buying

    public List<Tile> _SelectedUnitsTilesMovement;

    public float fixedZPosition = -1f; // Fixed Z position for the unit in world space
    public bool _UnitHoverOverTile;

    private void Start()
    {
        // Example: GridManager.Instance.MakeRowTilesSpawnable(1);
    }

    private void Update()
    {
        // Debugger Raycast
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

        // If the player is holding a unit in hand and not hovering over a tile
        if (_HasUnitInHand && !_UnitHoverOverTile)
        {
            // Make the unit follow the mouse
            Vector3 mousePosition = Input.mousePosition;
            mousePosition.z = Camera.main.WorldToScreenPoint(_UnitInHand.transform.position).z;
            Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
            _UnitInHand.gameObject.transform.position = new Vector3(
                worldPosition.x,
                worldPosition.y,
                _UnitInHand.transform.position.z
            );
        }

        //TODO: Move this away from the Update method
        if (_SelectedUnit != null)
        {
            // Show the selected unit in UI
            MenuManager.Instance.ShowSelectedHero(_SelectedUnit);
        }
        else
        {
            MenuManager.Instance.ClearSelectedHero();
        }
    }

    public void InitializePlayerData()
    {
        // Example: GridManager.Instance.MakeRowTilesSpawnable(1);
    }

    public void SetPlayerNumber(int i)
    {
        _PlayerNumber = i;
    }

    /// <summary>
    /// Clears both the unit in hand and the selected unit.
    /// </summary>
    public void ClearAll()
    {
        EmptyHand();
        ClearSelectedUnit();
    }

    public void SetUnitInHand(BaseUnit unit, GameObject unitGameObj)
    {
        ClearAll();
        _UnitInHand = unit;
        _BaseUnitGameObject = unitGameObj;
        _HasUnitInHand = true;

        // Optional: Disable unit interaction while it's in hand
        _UnitInHand.ToggleInteraction(false);
        MenuManager.Instance.ShowSelectedHero(_UnitInHand);
    }

    public void SetUnitSelected(BaseUnit unit)
    {
        ClearAll();
        EmptyHand();
        _SelectedUnit = unit;
        _SelectedUnit.OnSelectiion();

        //TODO: Move the logic here to the individual character and structure scripts

        if (unit._Type == BaseUnitType.Character)
        {
            
        }
        else if (unit._Type == BaseUnitType.Structure)
        {
            //TODO: Create logic when structure is selected
        }        
    }

    public void ClearSelectedUnit()
    {
        if(_SelectedUnit != null)
            _SelectedUnit.ClearSelection();

        _SelectedUnit = null;
    }

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
