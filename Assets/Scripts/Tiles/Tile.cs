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

    // A* Pathfinding properties
    public int GCost; // Cost from start to this tile
    public int HCost; // Heuristic cost to target
    public int FCost => GCost + HCost; // Total cost
    public Tile Parent; // For pathfinding

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
            // CASE 1: There's already a unit on this tile
            if (occupiedUnit != null)
            {
                if (occupiedUnit is Character characterOccupant)
                {
                    OnMouseDown_CharacterOccupied(characterOccupant);
                }
                else if (occupiedUnit is Structure structureOccupant)
                {
                    OnMouseDown_StructureOccupied(structureOccupant);
                }
            }
            // CASE 2: Tile is empty, but a unit is selected
            else if (PlayerManager.Instance._SelectedUnit != null && _Selected)
            {
                if (PlayerManager.Instance._SelectedUnit is Character characterSelected)
                {
                    OnMouseDown_EmptyWithCharacter(characterSelected);
                }
                else if (PlayerManager.Instance._SelectedUnit is Structure structureSelected)
                {
                    OnMouseDown_EmptyWithStructure(structureSelected);
                }
            }
            // CASE 3: If the player has a unit in hand and tile is spawnable
            else
            {
                if (PlayerManager.Instance._HasUnitInHand && _Spawnable)
                {
                    SpawnUnit(PlayerManager.Instance._UnitInHand);
                }
            }
        }
    }

    // ------------------------------------------------------------------------
    // OCCUPIED TILE: CHARACTER
    // ------------------------------------------------------------------------
    private void OnMouseDown_CharacterOccupied(Character occupant)
    {
        // If occupant is mid-action, disallow selection
        if (occupant.isMoving)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.Log("Character is currently moving; cannot select yet.");
            return;
        }
        if (occupant.isAttacking)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.Log("Character is currently attacking; cannot select yet.");
            return;
        }

        // If tile is flagged as a combat tile
        if (_Combat && PlayerManager.Instance._SelectedUnit != null)
        {
            if (GameManager.Instance._DebuggerMode)
                Debug.Log("Combat tile clicked.");
        }
        else
        {
            // Select the occupant (a Character)
            PlayerManager.Instance.SetUnitSelected(occupant);
        }
    }

    // ------------------------------------------------------------------------
    // OCCUPIED TILE: STRUCTURE
    // ------------------------------------------------------------------------
    private void OnMouseDown_StructureOccupied(Structure occupant)
    {
        // For structures, we typically don’t have isMoving/isAttacking checks,
        // but you could add your own "isConstructing" or some other status.

        // Example: Just select the structure
        PlayerManager.Instance.SetUnitSelected(occupant);
    }

    // ------------------------------------------------------------------------
    // EMPTY TILE BUT SELECTED UNIT: CHARACTER
    // ------------------------------------------------------------------------
    private void OnMouseDown_EmptyWithCharacter(Character selectedCharacter)
    {
        // Attempt to pay movement cost, then place the character
        if (ATBManager.Instance.PayATBCost(selectedCharacter._ATBMoveCost))
        {
            selectedCharacter.MoveToDestination(this);
        }
        else
        {
            Debug.Log("Cannot afford movement cost.");
        }
    }

    // ------------------------------------------------------------------------
    // EMPTY TILE BUT SELECTED UNIT: STRUCTURE
    // ------------------------------------------------------------------------
    private void OnMouseDown_EmptyWithStructure(Structure selectedStructure)
    {
        // Typically, structures can’t move
        // So maybe do nothing or just show a message
        Debug.Log("Structures cannot move.");
    }

    // ------------------------------------------------------------------------
    // SETTING & SPAWNING
    // ------------------------------------------------------------------------
    public void SetUnit(BaseUnit unit, bool init)
    {
        // Clear old occupant
        if (unit.OccupiedTile != null)
            unit.OccupiedTile.occupiedUnit = null;

        // Mark this tile as occupied
        occupiedUnit = unit;
        unit.SetTile(this);

        // If it's a Character, call the movement logic
        if (unit is Character character && init)
        {
            if (character.OccupiedTile == null)
            {
                if (GameManager.Instance._DebuggerMode) 
                    Debug.LogError("Character's OccupiedTile is null. Cannot move.");
                return;
            }

            // Ensure the destination tile is walkable
            if (this.walkable)
            {
                character.MoveToDestination(this);
            }
            else
            {
                if(GameManager.Instance._DebuggerMode)
                    Debug.LogWarning($"Destination tile at {this._coordinates} is not walkable. Unit will not move.");
            }
        }
    }

    public void SpawnUnit(BaseUnit unit)
    {
        if (ATBManager.Instance.PayATBCost(unit._ATBSpawnCost))
        {
            // Clear old occupant
            if (unit.OccupiedTile != null)
                unit.OccupiedTile.occupiedUnit = null;

            // Mark occupant
            occupiedUnit = unit;
            unit.SetTile(this);

            // Move if it's a Character
            if (unit is Character character)
            {
                character.MoveToDestination(this); // Use MoveToDestination instead of MoveToTile
            }

            PlayerManager.Instance.EmptyHand();
        }
        else
        {
            Debug.Log("Cannot afford to place the unit.");
        }
    }

    // ------------------------------------------------------------------------
    // OTHER UTILITY
    // ------------------------------------------------------------------------
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