using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum UnitType
{
    TestUnit,

}

public class UnitManager : Singleton<UnitManager>
{
    
    private List<ScriptableUnit> _units;
    public BaseHero SelectedHero;


    //todo: create an object pooling of the TestHero prefabs

    #region Test ObjectPool
    public ObjectPool _TestObjects;
    public Transform _ObjectSpawnPoints;

    public bool GetTestUnit()//(Vector3 _position, Quaternion _rotation)
    {
        if (!PlayerManager.Instance._HasUnitInHand)
        {
            if (ATBManager.Instance.PayATBCost(_TestObjects.prefab.GetComponent<BaseUnit>()._ATBCost))
            {
                //TODO: simplify this to a single line
                Vector3 _pos = new Vector3(0, 0, -1);
                GameObject _obj = _TestObjects.GetObject(_pos, Quaternion.identity);
                PlayerManager.Instance.SetUnitInHand(_obj.GetComponent<BaseUnit>(), _obj);
                return true;
            }
        }
        return false;
    }

    #endregion

    private void Awake()
    { 
        //_units = Resources.LoadAll<ScriptableUnit>("Units").ToList();
    }

    public void SpawnHeroes()
    {
        //var herocount = 1;

        //for (int i = 0; i < herocount; i++)
        //{
        //    var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
        //    var spawnedHero = Instantiate(randomPrefab);
        //    var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

        //    randomSpawnTile.SetUnit(spawnedHero);
        //}

        //GameManager.Instance.UpdateGameState(GameState.SpawnEnemies);
    }

    public void SpawnEnemies()
    {
        //var enemycount = 1;

        //for (int i = 0; i < enemycount; i++)
        //{
        //    var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
        //    var spawnedEnemy = Instantiate(randomPrefab);
        //    var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

        //    randomSpawnTile.SetUnit(spawnedEnemy);
        //}

        //GameManager.Instance.UpdateGameState(GameState.HeroesTurn);
    }

    //TODO:fIX, THIS DOES NOT WORK
    public void SpawnTestUnit()
    {
        var heroCount = 1;

        for(int i = 0; i < heroCount; i++)
        {
            var randomPrefab = GetRandomUnit<BaseUnit>(Faction.Hero);
            var spawnedUnit = Instantiate(randomPrefab);
            var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

            randomSpawnTile.SetUnit(spawnedUnit);
        }
    }

    private T GetRandomUnit<T>(Faction faction) where T: BaseUnit
    {
        return (T)_units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().unitPrefab;
    }

    public void SetSelectedHero(BaseHero hero)
    {
        SelectedHero = hero;
        //MenuManager.Instance.ShowSelectedHero(hero); //Chnage this to use base unit instead of base hero
    }
}
