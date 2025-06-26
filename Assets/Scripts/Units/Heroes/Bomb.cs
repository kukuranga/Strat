using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : BaseItem
{
    public GameObject _ParentGameObject;
    public BaseUnit _Owner;
    public GameObject _HitBox;
    public float _BombTickTime = 0.5f;
    public MeshRenderer _Mesh;
    public bool _AutoBlow;

    private bool _AlreadyDetonated = false;
    private HitBox _HB;


    private void Start()
    {
        _Mesh = GetComponent<MeshRenderer>();
        _HB = _HitBox.GetComponent<HitBox>();
        if(_HB != null)
        {
            _HB.factionOwner = _Owner.Faction;
        }

        if(_AutoBlow && !_AlreadyDetonated) StartCoroutine(Detonaite());

        _HitBox.transform.position = this.transform.position;
    }

    public void Initialize(BaseUnit Owner)
    {
        _Owner = Owner;
    }

    private void FixedUpdate()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        HurtBox hurtBox = other.GetComponent<HurtBox>();
        if (hurtBox != null && hurtBox.ownerUnit != null)
        {
            // Skip if the hurtBox belongs to the same faction as this hitbox
            if (hurtBox.ownerUnit == _Owner) return;

            if(!_AutoBlow && !_AlreadyDetonated) StartCoroutine(Detonaite());
        }    
    }

    private IEnumerator Detonaite()
    {
        _AlreadyDetonated = true;

        ReverseMaterials();
        yield return new WaitForSeconds(_BombTickTime);
        ReverseMaterials();
        yield return new WaitForSeconds(_BombTickTime);
        ReverseMaterials();
        yield return new WaitForSeconds(_BombTickTime);
        ReverseMaterials();
        yield return new WaitForSeconds(_BombTickTime);
        ReverseMaterials();
        yield return new WaitForSeconds(_BombTickTime);

        _HitBox.SetActive(true);

        //yield return new WaitForSeconds(_BombTickTime * 4);
        //EndExplosion();
    }

    public void EndExplosion()
    {
        Destroy(_ParentGameObject);
    }

    public override void OnHit(HitBox HB)
    {
        if (!_AlreadyDetonated) StartCoroutine(Detonaite());
    }

    private void ReverseMaterials()
    {
        if (_Mesh != null)
        {
            // Get the current materials array
            Material[] materials = _Mesh.materials;

            // Reverse the array
            System.Array.Reverse(materials);

            // Assign the reversed array back
            _Mesh.materials = materials;
        }
    }
}