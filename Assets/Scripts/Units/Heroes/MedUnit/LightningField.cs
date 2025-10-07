using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LightningField : MonoBehaviour
{
    //a lightining field will be spawned.
    //the number and amount of lightning strikes will be determined in the initialize value

    //public GameObject _LightningStrikeObject;
    public Tile _OccupiedTile;
    float _Delay;
    public BaseUnit _OwnerUnit;
    public HitBox _HB;
    public VisualEffect _VisualEffect;
    public GameObject _Cloud;

    private void Start()
    {
    }

    public void Initalize(Tile _tile, float Delay, BaseUnit owner)
    {
        _VisualEffect.enabled = false;
        _Cloud.SetActive(false);
        _HB.gameObject.SetActive(false);
        _OccupiedTile = _tile;
        _Delay = Delay;
        _OwnerUnit = owner;
        this.transform.position = new Vector3(_OccupiedTile.gameObject.transform.position.x, _OccupiedTile.gameObject.transform.position.y, this.transform.position.z);
        //Set its location to the occupied tile here
        //begin the countdown for the lightning strikes
        StartCoroutine(LightiningStrikeCoroutine());

    }

    //every second, check if any units are in the occupied tiles and have a random lightning strike hit each person

    private IEnumerator LightiningStrikeCoroutine()
    {
        yield return new WaitForSeconds(_Delay);

        _Cloud.SetActive(true);
        _VisualEffect.enabled = true;

        yield return new WaitForSeconds(2);

        _HB.gameObject.SetActive(true);

        yield return new WaitForSeconds(2);

        this.gameObject.SetActive(false);
        
        //TODO: Wait for the end of the onplay for the visual and disable the cloud and the visual effect
        //get the spawnning for the object set up in the mage script

    }
}
