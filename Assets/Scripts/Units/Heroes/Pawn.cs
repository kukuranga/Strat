using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Character
{
    [Header("Pawn Attributes")]
    //public Animator _Animator;
    public GameObject projectilePrefab;
    public Transform projectileSpawnParent;
    public GameObject BombPrefab;

    [Header("Ability Data")]
    public float _TurretModeStartupTime = 1f;
    public float _TurretTime = 5f;
    public int BombRange = 1;

    [SerializeField] private float _animationInterval = 5f; // Time between animations in seconds
    private float _timer;

    protected override void Update()
    {
        base.Update(); // Only if you're actually overriding something

        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            //_Animator.SetTrigger("Idle_2");
            _timer = _animationInterval; // Reset the timer
        }

        //if(isMoving)
        //{
        //    _Animator.SetBool("Walking", true);
        //}
        //else
        //{
        //    _Animator.SetBool("Walking", false);
        //}
    }

    public override void Ability1()//Turret
    {
        Debug.Log("Pawn uses Ability1.");
        StartCoroutine(TurretModeCoroutine());

    }

    public override void Ability2() //Bomb
    {
        Debug.Log("Pawn uses Ability2.");

        //Step 1: highlight tiles around the unit
        List<Tile> _bombTiles = GetAllTilesInRange(BombRange);
        foreach(Tile t in _bombTiles)
        {
            t.SetAbility(this);
        }

        List<Tile> combat = GetAllTilesInAutoAttackRange();
        foreach(Tile t in combat)
        {
            t.SetCombatTile(false);
        }

        CanMove = false;
    }

    public override void UseAbility(Tile tile)
    {
        Debug.Log("Ability Used");

        //CanMove = true;

        StartCoroutine(PlaceBombCoroutine(tile));
        

        //Clear ability data
        List<Tile> _bombTiles = GetAllTilesInRange(BombRange);
        foreach (Tile t in _bombTiles)
        {
            t.ClearAbility();
        }
    }

    private IEnumerator TurretModeCoroutine()
    {
        //disable movement/interaction, small startup time, increase attack rate and attack damage, wait for the time taken for turret mode to activate, eneable interaction/movement, end turret mode

        ToggleAutoAttackRangeVisual(false);
        _Interactable = false;
        CanMove = false;
        _CharacterTextBox.UpdateMessage("Loading Turret Mode");

        yield return new WaitForSeconds(_TurretModeStartupTime);

        float tempAA = autoAttackCooldown;
        autoAttackCooldown = 0.5f;
        //TODO: Increase attack damage if needed

        yield return new WaitForSeconds(_TurretTime);

        autoAttackCooldown = tempAA;
        _Interactable = true;
        CanMove = true;

        yield return null;
    }

    private IEnumerator PlaceBombCoroutine(Tile tile)
    {
        //TODO: Pay atb cost

        yield return RotateUnitTowards(tile.transform.position);

        yield return new WaitForSeconds(1f);

        //Spawn the bomb prefab here
        if (BombPrefab != null)
        {
            GameObject newBomb = Instantiate(
                BombPrefab,
                this.transform.position,
                Quaternion.identity
            );

            newBomb.GetComponentInChildren<Bomb>().Initialize(this);
            BaseItem BI = newBomb.GetComponentInChildren<BaseItem>();
            tile.SpawnItem(BI);

        }


        CanMove = true;
    }

    // Updated TryAutoAttack to accept a BaseUnit target
    public override void TryAutoAttack(BaseUnit target)
    {
        // Skip if currently attacking or on cooldown
        if (isAttacking || Time.time < nextAutoAttackTime) return;

        // Self-attack or same faction? Skip
        if (target == this) return;
        //if (target.Faction == this.Faction) return;
        if ((_Targets & target.Faction) == 0)
        {
            Debug.Log($"{UnitName} cannot attack {target.UnitName} (faction not in targets)");
            return;
        }

        // Check ATB
        if (ATBManager.Instance.GetATBAmount() < _ATBCombatCost) return;

        // Start the coroutine to handle rotation + projectile spawn
        StartCoroutine(AutoAttackRoutine(target));
    }

    private BaseUnit _TempTarget;

    private IEnumerator AutoAttackRoutine(BaseUnit target)
    {
        isAttacking = true;
        _TempTarget = target;

        // 1) Pay cost
        ATBManager.Instance.PayATBCost(_ATBCombatCost);

        //_Animator.SetTrigger("Attack");

        // 2) Rotate toward the target BEFORE spawning the projectile
        yield return RotateUnitTowards(target.transform.position);
        //transform.rotation = Quaternion.Euler(0, 0, -90);

        ShootEvent();

        // 4) Apply cooldown
        nextAutoAttackTime = Time.time + autoAttackCooldown;

        isAttacking = false;
        yield break;
    }

    public void ShootEvent()
    {
        // 3) Spawn projectile
        if (projectilePrefab != null && projectileSpawnParent != null)
        {
            GameObject newProjectile = Instantiate(
                projectilePrefab,
                projectileSpawnParent.position,
                projectileSpawnParent.rotation
            );

            ProjectileHitBox projHitBox = newProjectile.GetComponent<ProjectileHitBox>();
            if (projHitBox != null)
            {
                // Set the initial direction toward the target
                projHitBox.travelDirection = (_TempTarget.transform.position - projectileSpawnParent.position).normalized;
                projHitBox.factionOwner = this.Faction;
                projHitBox._OwnerUnit = this;
                projHitBox.targetUnit = _TempTarget; // Assign the target unit (optional)
            }
        }
    }
}