using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Attacks/MeleeDaggerAttack")]
public class MeleeDaggerAttack : Attack
{
    [Header("Dagger Attack Settings")]
    public float daggerMoveDistance = 1f; // Distance the dagger moves along the Y-axis
    public float daggerMoveSpeed = 10f; // Speed at which the dagger moves
    public float daggerRetractSpeed = 5f; // Speed at which the dagger retracts

    public override void PerformAttack(Character attacker, HurtBox target)
    {
        if (attacker.isAttacking || Time.time < attacker.nextAutoAttackTime)
        {
            Debug.LogWarning($"{attacker.UnitName} is already attacking or on cooldown.");
            return; // Skip if already attacking or on cooldown
        }

        if (ATBManager.Instance.GetATBAmount() < attacker._ATBCombatCost)
        {
            Debug.LogWarning($"{attacker.UnitName} does not have enough ATB to attack.");
            return; // Skip if not enough ATB
        }

        if (target == null)
        {
            Debug.LogError("Target HurtBox is null.");
            return; // Skip if target is null
        }

        // Start the attack coroutine
        attacker.StartCoroutine(MeleeAttackRoutine(attacker, target));
    }

    private IEnumerator MeleeAttackRoutine(Character attacker, HurtBox target)
    {
        Debug.Log($"{attacker.UnitName} is starting melee attack routine.");

        attacker.isAttacking = true;

        // 1) Pay the ATB cost for attacking
        ATBManager.Instance.PayATBCost(attacker._ATBCombatCost);

        // 2) Rotate towards the target
        yield return attacker.RotateUnitTowards(target.transform.position);

        // 3) Get the dagger GameObject (assume it's a child of the attacker)
        Transform dagger = attacker.transform.Find("Dagger"); // Replace "Dagger" with the actual name of the dagger GameObject
        if (dagger == null)
        {
            Debug.LogError("Dagger GameObject not found on the attacker.");
            yield break;
        }

        // 4) Ensure the dagger's HitBox does not despawn
        HitBox hitBox = dagger.GetComponent<HitBox>();
        if (hitBox != null)
        {
            hitBox.shouldDespawn = false; // Prevent the dagger from despawning
            Debug.Log($"Set shouldDespawn to false for {dagger.name}.");
        }
        else
        {
            Debug.LogError("Dagger GameObject does not have a HitBox component.");
        }

        // 5) Move the dagger along the Y-axis (reduce distance by 1/3)
        Vector3 initialPosition = dagger.localPosition;
        Vector3 targetPosition = initialPosition + (Vector3.up * (daggerMoveDistance * 2f / 3f)); // Reduce movement by 1/3

        float elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            dagger.localPosition = Vector3.Lerp(initialPosition, targetPosition, elapsedTime);
            elapsedTime += Time.deltaTime * daggerMoveSpeed;
            yield return null;
        }

        // 6) Check for collision with the target
        if (Vector3.Distance(dagger.position, target.transform.position) <= 0.5f)
        {
            Debug.Log($"{attacker.UnitName} hit {target.ownerUnit.UnitName}!");
            target.OnHit(1, null); // Deal damage to the target
        }

        // 7) Retract the dagger
        elapsedTime = 0f;
        while (elapsedTime < 1f)
        {
            dagger.localPosition = Vector3.Lerp(targetPosition, initialPosition, elapsedTime);
            elapsedTime += Time.deltaTime * daggerRetractSpeed;
            yield return null;
        }

        // 8) Set the cooldown time
        attacker.nextAutoAttackTime = Time.time + attackCooldown; // Use the inherited attackCooldown field
        attacker.isAttacking = false;

        Debug.Log($"{attacker.UnitName} has finished melee attack routine.");
    }
}