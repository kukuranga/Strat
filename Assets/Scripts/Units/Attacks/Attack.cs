using UnityEngine;

public abstract class Attack : ScriptableObject
{
    [Header("Attack Settings")]
    public float attackRange; // Attack range in units
    public float attackCooldown; // Cooldown time for attack
    public float Acc = 100;

    // Abstract method to execute attack logic
    public abstract void PerformAttack(Character attacker, HurtBox target);
}