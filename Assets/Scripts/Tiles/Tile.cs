using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Tile : MonoBehaviour
{
    public string TileName;
    public Vector2Int _coordinates;

    [SerializeField] private GameObject _highlight;
    [SerializeField] private GameObject _SpawnableTile;
    [SerializeField] private GameObject _SelectedTile;
    [SerializeField] private GameObject _CombatTile;
    public bool _isWalkable;

    public BaseUnit occupiedUnit;
    public bool walkable => _isWalkable && occupiedUnit == null;
    public bool _Spawnable = false; // This shows if a tile is available for unit spawning.
    public bool _Selected = false;
    public bool _Combat = false;

    private void Update()
    {
        _SpawnableTile.SetActive(_Spawnable && PlayerManager.Instance._HasUnitInHand);
        _SelectedTile.SetActive(_Selected);
        _CombatTile.SetActive(_Combat);
    }

    public virtual void Init(int x, int y)
    {
        _coordinates = new Vector2Int(x, y);
    }

    private void OnMouseEnter()
    {
        _highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);

        if (PlayerManager.Instance._HasUnitInHand)
        {
            PlayerManager.Instance._UnitHoverOverTile = true;
            HoverUnit(PlayerManager.Instance._UnitInHand);
        }
    }

    private void OnMouseExit()
    {
        _highlight.SetActive(false);
        MenuManager.Instance.ShowTileInfo(null);

        if (PlayerManager.Instance._HasUnitInHand)
        {
            PlayerManager.Instance._UnitHoverOverTile = false;
        }
    }

    private void OnMouseOver()
    {
        // Detect right-click while the mouse is over the tile
        if (Input.GetMouseButtonDown(1)) // Right-click
        {
            if (PlayerManager.Instance._HasUnitInHand)
            {
                RemoveUnitFromHand();
            }
            if (PlayerManager.Instance._SelectedUnit != null)
            {
                PlayerManager.Instance.ClearAll();
            }
        }
    }

    private void OnMouseDown()
    {
        if (GameManager.Instance._DebuggerMode)
            Debug.Log("Tile clicked: " + _coordinates);

        if (Input.GetMouseButtonDown(0)) // Left-click
        {
            // Case 1: There's already a unit on this tile
            if (occupiedUnit != null)
            {
                // If the occupant is still moving, don't allow selection
                if (occupiedUnit.isMoving)
                {
                    Debug.Log("Unit is currently moving; cannot select yet.");
                    return;
                }

                // If we're in combat or something else
                if (_CombatTile && PlayerManager.Instance._SelectedUnit != null)
                {
                    Debug.Log("Combat!!");
                }
                else
                {
                    // Select the unit
                    PlayerManager.Instance.SetUnitSelected(occupiedUnit);
                }
            }
            // Case 2: This tile is empty, but there's a selected unit wanting to move here
            else if (PlayerManager.Instance._SelectedUnit != null && _Selected)
            {
                if (ATBManager.Instance.PayATBCost(PlayerManager.Instance._SelectedUnit._ATBMoveCost))
                {
                    SetUnit(PlayerManager.Instance._SelectedUnit);
                    PlayerManager.Instance.ClearAll();
                }
            }
            // Case 3: If the player has a unit in hand and tile is spawnable
            else
            {
                if (PlayerManager.Instance._HasUnitInHand && _Spawnable)
                {
                    SpawnUnit(PlayerManager.Instance._UnitInHand);
                }
            }
        }
    }

    public void SetUnit(BaseUnit unit)
    {
        // Clear old occupant
        if (unit.OccupiedTile != null)
            unit.OccupiedTile.occupiedUnit = null;

        // Mark this tile as occupied
        occupiedUnit = unit;
        unit.SetTile(this);

        // Move the unit to this tile's position (smoothly handled by unit)
        unit.MoveToTile(this);
    }

    public void SpawnUnit(BaseUnit unit)
    {
        if (ATBManager.Instance.PayATBCost(PlayerManager.Instance._UnitInHand._ATBSpawnCost))
        {
            // Clear old occupant
            if (unit.OccupiedTile != null)
                unit.OccupiedTile.occupiedUnit = null;

            // Mark occupant
            occupiedUnit = unit;
            unit.SetTile(this);

            // Move the unit to this tile's position
            unit.MoveToTile(this);

            PlayerManager.Instance.EmptyHand();
        }
        else
        {
            Debug.Log("Cannot afford to place the unit.");
        }
    }

    public void RemoveUnitFromHand()
    {
        UnitManager.Instance.ReturnTestUnit();
        PlayerManager.Instance.EmptyHand();
    }

    // For visual "hover" only
    public void HoverUnit(BaseUnit unit)
    {
        Vector3 newPosition = transform.position;
        newPosition.z = unit.transform.position.z;
        unit.transform.position = newPosition;
    }

    public void SetSpawnableTile(bool b) => _Spawnable = b;
    public void SetSelectedTile(bool b) => _Selected = b;
    public void SetCombatTile(bool b) => _Combat = b;

    public void ClearOccupiedUnit()
    {
        occupiedUnit = null;  // Clear the occupied unit on the tile
    }
}
