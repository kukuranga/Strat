using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Tile : MonoBehaviour
{
    //todo: Make  changes to this script to acomidate changing the current system where the units are a separate gameObject
    public string TileName;
    public Vector2Int _coordinates;

    //[SerializeField] protected SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;
    public bool _isWalkable;

    public BaseUnit occupiedUnit;//Might change this to just contan a basic unit type or object
    public bool walkable => _isWalkable && occupiedUnit == null;

    public virtual void Init(int x, int y)
    {
        _coordinates = new Vector2Int(x, y);
    }

    void OnMouseEnter()
    {
        _highlight.SetActive(true);
        MenuManager.Instance.ShowTileInfo(this);
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
        MenuManager.Instance.ShowTileInfo(null);
    }

    private void OnMouseDown()
    {
        print(_coordinates);// ------------------------------------------------- remove

        //if (GameManager.Instance.State != GameState.HeroesTurn) return;

        //TODO: ON click, set occupied hero to selected h ero if available and highlight cells that unit can move to
        //TODO: on mouseUp, set selcted hero to occupied unit and remove highlite if tile is highlighted


        if (occupiedUnit != null)
        {
            if (occupiedUnit.Faction == Faction.Hero) UnitManager.Instance.SetSelectedHero((BaseHero)occupiedUnit);
            else
            {
                if (UnitManager.Instance.SelectedHero != null)
                {
                    var enemy = (BaseEnemy)occupiedUnit; //Should be proper logic for enemy attacking
                    Destroy(enemy.gameObject);
                    UnitManager.Instance.SetSelectedHero(null);
                }
            }
        }
        else
        {
            if(UnitManager.Instance.SelectedHero != null)
            {
                SetUnit(UnitManager.Instance.SelectedHero); //Moving the hero
                UnitManager.Instance.SetSelectedHero(null);

            }
        }
    }

    //Todo: Chnage this logic to move the unit smoothly to the selected location
    //Todo: figure our logic when two units hit the same position
    public void SetUnit(BaseUnit unit)
    {
        if (unit.OccupiedTile != null) unit.OccupiedTile.occupiedUnit = null;

        unit.transform.position = transform.position;
        occupiedUnit = unit;
        unit.OccupiedTile = this;
    }
}
