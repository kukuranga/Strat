using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public abstract class Tile : MonoBehaviour
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
                //PlayerManager.Instance.ClearAll();
            }
            if(PlayerManager.Instance._SelectedUnit != null)
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
            if (occupiedUnit != null) // If there is a unit on the tile
            {

                if (_CombatTile && PlayerManager.Instance._SelectedUnit != null) //Combat logic
                {
                    PlayerManager.Instance._SelectedUnit.CombatLogic(occupiedUnit, this, PlayerManager.Instance._SelectedUnit.OccupiedTile);
                    Debug.Log("Combat!!");
                }
                else
                {
                    // Select the unit and highlight tiles
                    PlayerManager.Instance.SetUnitSelected(occupiedUnit);
                }
            }
            else if(PlayerManager.Instance._SelectedUnit != null && _Selected) //Moves the unit to this tile
            {
                //Move the unit here
                //Steps: Set the selected unit position to this tile. clear all slected values form the playerManager
                if (ATBManager.Instance.PayATBCost(PlayerManager.Instance._SelectedUnit._ATBMoveCost))
                {
                    SetUnit(PlayerManager.Instance._SelectedUnit);
                    PlayerManager.Instance.ClearAll();
                }
            }
            else
            {
                if (PlayerManager.Instance._HasUnitInHand && _Spawnable)
                {
                    SpawnUnit(PlayerManager.Instance._UnitInHand);
                }
            }
            
        }
   }

    //Moves The Unit to this tile
    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null)
            unit.OccupiedTile.occupiedUnit = null;

        // Preserve the current Z position of the unit
        Vector3 newPosition = transform.position;
        newPosition.z = unit.transform.position.z;

        unit.transform.position = newPosition;
        occupiedUnit = unit;
        unit.OccupiedTile = this;
    }

    //Spawns the unit on this tile
    public void SpawnUnit(BaseUnit unit)
    {
        // Use ATBManager's PayATBCost method to check affordability and deduct cost
        if (ATBManager.Instance.PayATBCost(PlayerManager.Instance._UnitInHand._ATBSpawnCost))
        {
            if (unit.OccupiedTile != null)
                unit.OccupiedTile.occupiedUnit = null;

            // Preserve the current Z position of the unit
            Vector3 newPosition = transform.position;
            newPosition.z = unit.transform.position.z;

            unit.transform.position = newPosition;
            occupiedUnit = unit;
            unit.OccupiedTile = this;

            PlayerManager.Instance.EmptyHand();
        }
        else
        {
            Debug.Log("Cannot afford to place the unit.");
            // TODO: Add UI feedback or animation to notify the player.
        }
    }


    public void RemoveUnitFromHand()
    {
        // Return the unit in hand to the object pool
        UnitManager.Instance.ReturnTestUnit();

        // Clear the unit in hand reference in the PlayerManager
        PlayerManager.Instance.EmptyHand();
    }

    public void HoverUnit(BaseUnit unit)
    {
        // Preserve the current Z position of the unit
        Vector3 newPosition = transform.position;
        newPosition.z = unit.transform.position.z;

        unit.transform.position = newPosition;
    }

    public void SetSpawnableTile(bool b)
    {
        _Spawnable = b;
    }

    public void SetSelectedTile(bool b)
    {
        _Selected = b;
    }

    public void SetCombatTile(bool b)
    {
        _Combat = b;
    }
}
