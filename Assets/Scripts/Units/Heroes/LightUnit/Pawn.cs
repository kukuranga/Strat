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
    public GameObject FireObject;

    [Header("Ability Data")]
    public float _TurretModeStartupTime = 1f;
    public float _TurretTime = 5f;
    public int BombRange = 1;
    public float _TurretATBCost;
    public float _BombATBCost;
    public int _BombRedCost;

    [SerializeField] private float _animationInterval = 5f; // Time between animations in seconds
    private float _timer;

    protected override void Update()
    {
        base.Update(); // Only if you're actually overriding something

        _timer -= Time.deltaTime;

        if (_timer <= 0f)
        {
            //_Animator.SetTrigger("Idle_2");
            //if(CanMove && !isAttacking)
                //Idle(); 
            _timer = _animationInterval; // Reset the timer
        }

        //if (isMoving)
        //{
        //    //_Animator.SetBool("Walking", true);
        //    FireObject.SetActive(true);
        //}
        //else
        //{
        //    //_Animator.SetBool("Walking", false);
        //    FireObject.SetActive(false);
        //}
    }

    public override void Ability1() //Turret
    {
        base.Ability1();

        if (_TurretATBCost > ATBManager.Instance.GetATBAmount())
        {
            _CharacterTextBox.UpdateMessage("Not enough ATB");
            return;
        }

        if (Ability1CoolDown >= Ability1CoolDownTime)
        {
            Debug.Log("Pawn uses Ability1.");
            StartCoroutine(TurretModeCoroutine());
            Ability1CoolDown = 0;
            Debug.Log("Pawn Ability1 on cooldown.");

        }

    }

    public override void Ability2() //Bomb
    {
        base.Ability2();

        if (_BombATBCost > ATBManager.Instance.GetATBAmount())
        {
            _CharacterTextBox.UpdateMessage("Not enough ATB");
            return;
        }


        if (_BombRedCost < ResourceManager.Instance.GetRedResource())
        {
            _CharacterTextBox.UpdateMessage("Not Enough Tech");
            return;
        }

        if (Ability2CoolDown >= Ability2CoolDownTime)
        {
            Debug.Log("Pawn uses Ability2.");

            //Step 1: highlight tiles around the unit
            List<Tile> _bombTiles = GetAllTilesInRange(BombRange);
            foreach (Tile t in _bombTiles)
            {
                t.SetAbility(this);
            }

            List<Tile> combat = GetAllTilesInAutoAttackRange();
            foreach (Tile t in combat)
            {
                t.SetCombatTile(false);
            }

            CanMove = false;
        }
        Debug.Log("Pawn Ability2 on cooldown.");
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
        Ability2CoolDown = 0;

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
        LiftUp();

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

    //---------- Animations --------------

    public void Idle()
    {
        // wiggle side to side
        StartCoroutine(IdleAnim());
    }

   

    public void Dance()
    {
        // wiggle and rotate around
    }

    public void LiftUp()
    {
        StartCoroutine(LiftAnim());
    }

    public void DropDown()
    {

    }

    public void Attack()
    {
        //Lift up and aim down to attack
    }

    public void Walk()
    {
        //lift up and slam down after you move (might need to add an onMoveEnd animation
    }

    private IEnumerator LiftAnim()
    {
        float elapsedTime = 0f;
        float duration = 0f;
        Vector3 currentPos = transform.position;
        Vector3 targetPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 2);

        while (elapsedTime < duration)
        {
            transform.position = Vector3.Lerp(currentPos, targetPos, elapsedTime/duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        yield return null;
    }

    private IEnumerator DrowDownAnim()
    {
        yield return null;
    }

    private IEnumerator IdleAnim()
    {
        // Define rotation parameters
        float rotationAngle = 15f; // How far to rotate in degrees
        float rotationSpeed = 0.5f; // Speed of rotation
        float waitTime = 0.3f; // Time to wait between rotations

        // Get initial rotation
        Quaternion startRotation = transform.rotation;

        Quaternion targetRotation = startRotation * Quaternion.Euler(0, 0, rotationAngle);
        yield return RotateToTarget(targetRotation, rotationSpeed);
        yield return new WaitForSeconds(waitTime);
        if (!CanMove) { yield return null; }

        // Rotate back to center
        yield return RotateToTarget(startRotation, rotationSpeed);
        yield return new WaitForSeconds(waitTime);
        if (!CanMove) { yield return null; }

        // Rotate left
        targetRotation = startRotation * Quaternion.Euler(0, 0, -rotationAngle);
        yield return RotateToTarget(targetRotation, rotationSpeed);
        yield return new WaitForSeconds(waitTime);
        if (!CanMove) { yield return null; }

        // Rotate back to center
        yield return RotateToTarget(startRotation, rotationSpeed);
        yield return new WaitForSeconds(waitTime);
        if (!CanMove) { yield return null; }
    }

    //to be used only for animation rotations
    private IEnumerator RotateToTarget(Quaternion targetRotation, float duration)
    {
        Quaternion startRotation = transform.rotation;
        float elapsedTime = 0f;
        float angleToTarget = Vector3.SignedAngle(startRotation.eulerAngles, targetRotation.eulerAngles, Vector3.forward);

        while (elapsedTime < duration)
        {
            if (!CanMove) { break; }
            if (angleToTarget > 0)
            {
                Debug.Log("clockwiseRotation");
                FireObject.SetActive(true);
            }
            else
            {
                Debug.Log("AntiClockwiseRotation");
                FireObject.SetActive(true);
            }
            transform.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure exact rotation at the end
        transform.rotation = targetRotation;
    }

    protected override IEnumerator MoveUnitRoutine(Vector3 targetPos)
    {
        FireObject.SetActive(true);
        yield return StartCoroutine(base.MoveUnitRoutine(targetPos));
        FireObject.SetActive(false);
    }
}