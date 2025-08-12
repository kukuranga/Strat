using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage : Character
{
    [Header("Mage Attributes")]
    public GameObject _staff;
    public float _HealAmount = 1;

    [Header("Ability Data")]
    public float _HealRate;

    [SerializeField] private float _animationInterval = 5f; // Time between animations in seconds
    private float _timer;

    protected override void Update()
    {
        base.Update(); // Only if you're actually overriding something

    }

    public override void Ability1() //Turret
    {
        base.Ability1();
    }

    public override void Ability2() //Bomb
    {
        base.Ability2();

    }

    public override void UseAbility(Tile tile)
    {
        Debug.Log("Ability Used");
    }


    //----------------------Combat-----------------------------------
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
       // LiftUp();

        // 2) Rotate toward the target BEFORE spawning the projectile
        yield return RotateUnitTowards(target.transform.position);
        //transform.rotation = Quaternion.Euler(0, 0, -90);

        //ShootEvent();
        target.HealDamage(_HealAmount);

        // 4) Apply cooldown
        nextAutoAttackTime = Time.time + autoAttackCooldown;

        isAttacking = false;
        yield break;
    }
}
