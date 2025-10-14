using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnAnimCont : MonoBehaviour
{
    //A script that handles all the code behind the pawns animations
    public Animator _Animator;
    public GameObject DeathParticles;
    public GameObject ShipGameObject;
    private Material _shipMat1;

    private void Start()
    {
        DeathParticles.SetActive(false);
        _shipMat1 = ShipGameObject.GetComponent<MeshRenderer>().material;
    }

    #region Death
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
    #endregion

    public void TakeDamage()
    {
        StartCoroutine(TakeDamageCoroutine());
    }

    private IEnumerator TakeDamageCoroutine()
    {
        _Animator.SetTrigger("TakeDamage");

        for (int i = 0; i < 3; i++)
        {
            _shipMat1.SetFloat("_TakeDamage", 1);
            yield return new WaitForSeconds(0.2f);
            _shipMat1.SetFloat("_TakeDamage", 0);
            yield return new WaitForSeconds(0.1f);
        }
    }

}
