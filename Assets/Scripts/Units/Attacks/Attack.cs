using UnityEngine;

public abstract class Attack : ScriptableObject
{
    [Header("Attack Settings")]
    public float attackRange; // Attack range in units
    public float attackCooldown; // Cooldown time for attack

    // Abstract method to execute attack logic
    public abstract void PerformAttack(Character attacker, HurtBox target);
}