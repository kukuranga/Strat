using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnAnimCont : MonoBehaviour
{
    //A script that handles all the code behind the pawns animations
    public Animator _Animator;
    public GameObject DeathParticles;

    private void Start()
    {
        DeathParticles.SetActive(false);
    }

    public void StartIdle()
    {
        //will start the idle effect when needed
    }

    public void StopIdle()
    {
        //Stops the idle effect
    }

    public void DeathAnim(BaseUnit unit)
    {
        _Animator.SetTrigger("Death");
        DeathParticles.SetActive(true);

        StartCoroutine(DeathAnimCoroutine(unit));
    }

    private IEnumerator DeathAnimCoroutine(BaseUnit unit)
    {
        // Now wait for the death animation to finish
        yield return new WaitForSeconds(2.2f);
        DeathParticles.SetActive(false);

        UnitManager.Instance.KillUnit(unit);
    }
}
