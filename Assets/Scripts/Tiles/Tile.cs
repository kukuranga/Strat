using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    public string TileName;
    public Vector2Int _coordinates;

    [SerializeField] private GameObject _highlight;
    public bool _isWalkable;

    public BaseUnit occupiedUnit;
    public bool walkable => _isWalkable && occupiedUnit == null;

    public virtual void Init(int x, int y)
    {
        _coordinates = new Vector2Int(x, y);
    }

    void OnMouseEnter()
    {
        _highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);

        if(PlayerManager.Instance._HasUnitInHand)
        {
            HoverUnit(PlayerManager.Instance._UnitInHand);
        }
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
        MenuManager.Instance.ShowTileInfo(null);
    }

    private void OnMouseDown()
    {
        if(GameManager.Instance._DebuggerMode)
            Debug.Log("Tile clicked: " + _coordinates);
        
        //TODO: when button pressed place the unit here and remove it from the player manager unit in hand
        if (occupiedUnit != null)
        {
            //TODO: this will start combat

            //if (occupiedUnit.Faction == Faction.Hero)
            //    UnitManager.Instance.SetSelectedHero((BaseHero)occupiedUnit);
            //else
            //{
            //    if (UnitManager.Instance.SelectedHero != null)
            //    {
            //        var enemy = (BaseEnemy)occupiedUnit;
            //        Destroy(enemy.gameObject);
            //        UnitManager.Instance.SetSelectedHero(null);
            //    }
            //}
            Debug.Log("Already A Unit Here");
        }
        else
        {
            //if (UnitManager.Instance.SelectedHero != null)
            //{
            //    SetUnit(UnitManager.Instance.SelectedHero);
            //    UnitManager.Instance.SetSelectedHero(null);
            //}
            if (PlayerManager.Instance._HasUnitInHand)
            {
                SetUnit(PlayerManager.Instance._UnitInHand);
                PlayerManager.Instance.EmptyHand();
            }
        }
    }

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

    public void HoverUnit(BaseUnit unit)
    {
        // Preserve the current Z position of the unit
        Vector3 newPosition = transform.position;
        newPosition.z = unit.transform.position.z;

        unit.transform.position = newPosition;
    }

}
