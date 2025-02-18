using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/MeleeDaggerAttack")]
public class MeleeDaggerAttack : Attack
{
    [Header("Dagger Attack Settings")]
    public GameObject daggerPrefab; // Prefab for the dagger used in the melee attack
    public Transform daggerSpawnPoint; // Point from which the dagger is spawned
    public float daggerSpeed = 10f; // Speed at which the dagger moves forward
    public float daggerLifetime = 0.5f; // Lifetime of the dagger before it disappears

    public override void PerformAttack(Character attacker, HurtBox target)
    {
        if (attacker.isAttacking || Time.time < attacker.nextAutoAttackTime)
        {
            return; // Skip if already attacking or on cooldown
        }

        if (ATBManager.Instance.GetATBAmount() < attacker._ATBCombatCost)
        {
            return; // Skip if not enough ATB
        }

        // Start the attack coroutine
        attacker.StartCoroutine(MeleeAttackRoutine(attacker, target));
    }

    private IEnumerator MeleeAttackRoutine(Character attacker, HurtBox target)
    {
        attacker.isAttacking = true;

        // 1) Pay the ATB cost for attacking
        ATBManager.Instance.PayATBCost(attacker._ATBCombatCost);

        // 2) Rotate towards the target
        yield return attacker.RotateUnitTowards(target.transform.position);

        // 3) Spawn the dagger and move it forward
        if (daggerPrefab != null && daggerSpawnPoint != null)
        {
            GameObject dagger = Instantiate(daggerPrefab, daggerSpawnPoint.position, daggerSpawnPoint.rotation);
            DaggerHitBox hitBox = dagger.GetComponent<DaggerHitBox>();
            if (hitBox != null)
            {
                hitBox.Initialize(attacker as Goblin, target.ownerUnit as Character);
            }
        }

        // 4) Set the cooldown time
        attacker.nextAutoAttackTime = Time.time + attacker.autoAttackCooldown;
        attacker.isAttacking = false;
    }
}