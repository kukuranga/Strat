using System.Collections;
using UnityEngine;

public class DaggerHitBox : HitBox
{
    public float moveSpeed = 10f; // Speed at which the dagger moves forward
    public float lifetime = 0.5f; // Lifetime of the dagger before it disappears
    public int damage = 1; // Damage dealt by the dagger

    public Goblin owner;
    private Character target;

    public void Initialize(Goblin owner, Character target)
    {
        this.owner = owner;
        this.target = target;
        this.factionOwner = owner.Faction; // Set the faction owner
        StartCoroutine(MoveDagger());
    }

    private IEnumerator MoveDagger()
    {
        float elapsedTime = 0f;

        while (elapsedTime < lifetime)
        {
            // Move the dagger forward
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);

            // Check for collision with the target
            if (target != null && Vector3.Distance(transform.position, target.transform.position) <= 0.5f)
            {
                // Deal damage to the target
                HurtBox hurtBox = target.GetComponentInChildren<HurtBox>();
                if (hurtBox != null)
                {
                    hurtBox.OnHit(damage, Acc, UseSPA, this); // Pass 'this' as the HitBox
                }

                // Destroy the dagger after hitting the target
                AfterHitEffect();
                yield break;
            }

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Destroy the dagger if it doesn't hit anything
        AfterHitEffect();
    }
}