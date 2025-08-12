using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public enum UnitType
{
    Pawn,
    Mage,

}
public enum Faction
{
    Hero = 0,
    Enemy = 1,
    NPC = 2
}

public class UnitManager : Singleton<UnitManager>
{
    
    private List<ScriptableUnit> _units;
    //public BaseHero SelectedHero;


    //todo: create an object pooling of the TestHero prefabs
    public Transform _ObjectSpawnPoints;

    #region Test ObjectPool
    public ObjectPool _TestObjects;

    public bool GetTestUnit()//(Vector3 _position, Quaternion _rotation)
    {
        if (!PlayerManager.Instance._HasUnitInHand)
        {
            //if (ATBManager.Instance.PayATBCost(_TestObjects.prefab.GetComponent<BaseUnit>()._ATBCost))
                //TODO: simplify this to a single line
                Vector3 _pos = new Vector3(0, 0, -1);
                GameObject _obj = _TestObjects.GetObject(_pos, Quaternion.identity);
                _obj.GetComponent<BaseUnit>().InitializeUnit();
                PlayerManager.Instance.SetUnitInHand(_obj.GetComponent<BaseUnit>(), _obj);
                return true;
        }
        return false;
    }

    public bool ReturnTestUnit()
    {
        if( PlayerManager.Instance._HasUnitInHand)
        {
            _TestObjects.ReturnObject(PlayerManager.Instance._UnitInHand.GameObject());
        }
        return false;
    }

    #endregion


    #region Mage ObjectPool
    public ObjectPool _MageObjects;

    public bool GetMageUnit()//(Vector3 _position, Quaternion _rotation)
    {
        if (!PlayerManager.Instance._HasUnitInHand)
        {
            //if (ATBManager.Instance.PayATBCost(_TestObjects.prefab.GetComponent<BaseUnit>()._ATBCost))
            //TODO: simplify this to a single line
            Vector3 _pos = new Vector3(0, 0, -1);
            GameObject _obj = _MageObjects.GetObject(_pos, Quaternion.identity);
            _obj.GetComponent<BaseUnit>().InitializeUnit();
            PlayerManager.Instance.SetUnitInHand(_obj.GetComponent<BaseUnit>(), _obj);
            return true;
        }
        return false;
    }

    public bool ReturnMageUnit()
    {
        if (PlayerManager.Instance._HasUnitInHand)
        {
            _MageObjects.ReturnObject(PlayerManager.Instance._UnitInHand.GameObject());
        }
        return false;
    }

    #endregion
    public void KillUnit(BaseUnit _unit)
    {
        if(_unit. InObjectPool)
            _TestObjects.ReturnObject(_unit.gameObject);
        else
            Destroy(_unit.gameObject);
    }

    private void Awake()
    { 
        //_units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    private T GetRandomUnit<T>(Faction faction) where T: BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().unitPrefab;
    }

    //public void SetSelectedHero(BaseHero hero)
    //{
    //    SelectedHero = hero;
    //    //MenuManager.Instance.ShowSelectedHero(hero); //Chnage this to use base unit instead of base hero
    //}
}
