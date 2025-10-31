using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Golem : Character
{
    [Header("Pawn Attributes")]
    public string _Name;

    [Header("Ability Data")]
    public float AttackTime;

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

    public override void TryAutoAttack(BaseUnit target)
    {
        //Update the auto attack logic here 
    }

    public override void Die()
    {

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
                //FireObject.SetActive(true);
            }
            else
            {
                Debug.Log("AntiClockwiseRotation");
                //FireObject.SetActive(true);
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
        //FireObject.SetActive(true);
        yield return StartCoroutine(base.MoveUnitRoutine(targetPos));
        //FireObject.SetActive(false);
    }

    public override void TakeDamage(float damage, float Acc, bool UseSPA, BaseUnit _DamagingUnit)
    {
        base.TakeDamage(damage, Acc, UseSPA, _DamagingUnit);
        //_PawnAnimCont.TakeDamage();
    }
}
