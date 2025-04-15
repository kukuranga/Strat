using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : Singleton<MenuManager>
{
    //TODO: Make object groups fot the gameobjects (headers like in base unit)

    [SerializeField] private GameObject _selectedObjectHero, _TileObject,_TileUnitObject, _ObjectiveAnouncemetBanner;

    [Header("Ability UI")]
    [SerializeField] private GameObject _AbilityUI;
    //[SerializeField] private Button _Ability1, _Ability2;
    

    public void ShowTileInfo(Tile tile)
    {
        if (tile == null)
        {
            _TileObject.SetActive(false);
            _TileUnitObject.SetActive(false);
            return;
        }
        _TileObject.GetComponentInChildren<TextMeshProUGUI>().text = tile.TileName;
        _TileObject.SetActive(true);

        if(tile.occupiedUnit)
        {
            _TileUnitObject.GetComponentInChildren<TextMeshProUGUI>().text = tile.occupiedUnit.UnitName;
            _TileUnitObject.SetActive(true);
        }
    }

    public void ShowSelectedHero(BaseUnit _Unit)
    {
        if(_Unit == null)
        {
            _selectedObjectHero.SetActive(false);
            return;
        }
        _selectedObjectHero.GetComponentInChildren<TextMeshProUGUI>().text = _Unit.UnitName;
        _selectedObjectHero.SetActive(true);
        
        //Only displays the UI for heros
        if(_Unit.Faction == Faction.Hero)
            _AbilityUI.SetActive(true);
    }

    public void ClearSelectedHero()
    {
        _selectedObjectHero.SetActive(false);
        _AbilityUI.SetActive(false);
    }

    public void ShowObjectiveBanner()
    {
        _ObjectiveAnouncemetBanner.SetActive(true);
    }
}
