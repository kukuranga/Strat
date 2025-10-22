using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Mage : Character
{
    [Header("Mage Attributes")]
    public GameObject HealGameObject;
    public GameObject _Crystal;
    public GameObject _LightningField;
    public float _HealAmount = 1;
    public int LightningRange;
    public int LightningCount;

    [Header("Ability Data")]
    public float _HealRate;

    [SerializeField] private float _animationInterval = 5f; // Time between animations in seconds
    private float _timer;

    private bool _Ab1Selected;
    private bool _Ab2Selected;


    protected override void Update()
    {
        base.Update(); // Only if you're actually overriding something

    }

    public override void Ability1() //Create Crystal
    {
        //same logic as the place bomb mechanic in the pawn
        //just replace the bomb with the crystal

        base.Ability1();
        ResetAbilities();
        if (Ability1CoolDown >= Ability1CoolDownTime)
        {
            Debug.Log("Mage uses Ability1.");

            //Step 1: highlight tiles around the unit
            List<Tile> _CrystalTiles = GetAllTilesInRange(1);
            foreach (Tile t in _CrystalTiles)
            {
                t.SetAbility(this);
            }

            List<Tile> combat = GetAllTilesInAutoAttackRange();
            foreach (Tile t in combat)
            {
                t.SetCombatTile(false);
            }

            CanMove = false;
            Ability1CoolDown = 0;
            _Ab1Selected = true;
        }
        Debug.Log("Mage Ability1 on cooldown.");
    }

    public override void Ability2() //Lightning
    {
        base.Ability2();
        ResetAbilities();
        //if (Ability2CoolDown >= Ability2CoolDownTime)
        {
            //Debug.Log("Mage uses Ability2.");

            ////Step 1: highlight tiles around the unit
            //List<Tile> _CrystalTiles = GetAllTilesInRange(3); //TODO: change the code to select all the tiles around the hovered tile rather than just highlighting the selected one
            //foreach (Tile t in _CrystalTiles)
            //{
            //    t.SetAbility(this);
            //}

            //List<Tile> combat = GetAllTilesInAutoAttackRange();
            //foreach (Tile t in combat)
            //{
            //    t.SetCombatTile(false);
            //}

            //CanMove = false;
            //Ability2CoolDown = 0;
            //_Ab2Selected = true;


            //new version

            //get all tiles in range
            List<Tile> _LightningTiles = GridManager.Instance.GetTilesInRadius(OccupiedTile, LightningRange);
            //get 3 separtat tiles in range
            // Create a copy of the list
            List<Tile> shuffled = new List<Tile>(_LightningTiles);

            // Shuffle using Fisher-Yates algorithm
            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int randomIndex = UnityEngine.Random.Range(0, i + 1);
                Tile temp = shuffled[i];
                shuffled[i] = shuffled[randomIndex];
                shuffled[randomIndex] = temp;
            }

            // Return the first 'count' elements
            List<Tile> _fin = shuffled.GetRange(0, LightningCount);
            foreach(Tile t in _fin)
            {
                //spawn the lighting element here
                //set variable for the lightningfield object
                //call initialize on it
                GameObject Cry = Instantiate(
                    _LightningField,
                    this.transform.position,
                    Quaternion.identity
                );

                
                LightningField LI= Cry.GetComponent<LightningField>();
                LI.Initalize(t, 2, this);
            }
        }
        Debug.Log("Mage Ability2 on cooldown.");


        //List<Tile> _TilesInRange = GridManager.Instance.GetTilesInRadius(this.OccupiedTile, LightingRange);

        //Select several random tiles in the list of tiles



        // for each one create a lightning field object and set its tile to this one

    }

    private void ResetAbilities()
    {
        _Ab1Selected = false;
        _Ab2Selected = false;
    }

    public override void UseAbility(Tile tile)
    {
        Debug.Log("Ability Used");

        //if it is ability 1 then it will set the crystal
        //if its ab 2 it will set the the lightning field
        if(_Ab1Selected && !_Ab2Selected)
        {
            //Spawn Crystal
            StartCoroutine(PlaceCrystalCoroutine(tile));
        }

        if(_Ab2Selected && !_Ab1Selected)
        {
            //spawn Lightning field
        }
    }

    private IEnumerator PlaceCrystalCoroutine(Tile tile)
    {
        //TODO: Pay atb cost

        yield return RotateUnitTowards(tile.transform.position);

        yield return new WaitForSeconds(1f);

        if (_Crystal != null)
        {
            GameObject Cry = Instantiate(
                _Crystal,
                this.transform.position,
                Quaternion.identity
            );

            Cry.transform.position = new Vector3(tile.gameObject.transform.position.x, tile.gameObject.transform.position.y, this.transform.position.z);
            BaseUnit BI = Cry.GetComponent<BaseUnit>();
            Cry.GetComponent<Crystal>().Initalize(this);
            tile.SpawnUnit(BI);
        }


        CanMove = true;
    }


    //----------------------Combat-----------------------------------
    public override void TryAutoAttack(BaseUnit target)
    {   
        // Early exit conditions
        if (isAttacking || Time.time < nextAutoAttackTime) return;
        //if (target == this) return;
        if (ATBManager.Instance.GetATBAmount() < _ATBCombatCost) return;

        // Get all attackable units in range
        var attackableUnits = GridManager.Instance.GetTilesInRadius(OccupiedTile, AutoAttackRange)
            .Where(tile => tile.occupiedUnit != null)
            .Select(tile => tile.occupiedUnit)
            .ToList();

        // Start attack routines for all valid targets
        foreach (var unit in attackableUnits)
        {
            if (unit.Faction != this.Faction)
            {
                Debug.Log($"{UnitName} cannot attack {target.UnitName} (faction not in targets)");
                return;
            }
        }


        StartCoroutine(AutoAttackRoutine(attackableUnits));
        //ToDo: add the ability to stop the unit from attacking, to save on atb costs
    }

    private IEnumerator AutoAttackRoutine(List<BaseUnit> targets)
    {
        isAttacking = true;

        // 1) Pay cost
        ATBManager.Instance.PayATBCost(_ATBCombatCost);

        HealGameObject.SetActive(true);

        // 2) Rotate toward the target
        foreach (BaseUnit target in targets)
        {
            //yield return RotateUnitTowards(target.transform.position);

            // 3) Apply heal
            target.HealDamage(_HealAmount);
        }
        // 4) Apply cooldown
        nextAutoAttackTime = Time.time + autoAttackCooldown;

        isAttacking = false;

        yield return new WaitForSeconds(2);
        HealGameObject.SetActive(false);

        yield break;
    }
}
