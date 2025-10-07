using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTile : Tile
{
    public GameObject GreenResource;
    public float _GreenItemInterval;
    public int _MaxIntervalTime;//a random value added to the interval
    [Range(0,100)]
    public float _PercentageChanceOfSpawning;

    [SerializeField] private Color _baseColor, _OffsetColor;

    private void Start()
    {
        //SpawnGreenResource();
        StartCoroutine(CheckSpawn());
        _GreenItemInterval += Random.Range(0, _MaxIntervalTime);
    }

    public override void Init(int x, int y)
    {
        base.Init(x, y);
        var isOffset = (x + y) % 2 == 1;
        //_renderer.color = isOffset ? _OffsetColor : _baseColor;
    }

    private void FixedUpdate()
    {
        //On fixed update, spawn the prefab of a green resource unit.

        //Make an interval at which it will randomly check to spawn a green
    }

    private void SpawnGreenResource()
    {
        GameObject newGreen = Instantiate(
                GreenResource,
                this.transform.position,
                Quaternion.identity
            );
        GreenCollectable _col = newGreen.GetComponent<GreenCollectable>();

        _col.Initialize(this);
        SpawnItem(_col);
    }

    private void SpawnItem()
    {
        if (OccupiedItem != null)
            return;
        
        //random using the % chance
        float randomValue = Random.Range(0f, 100f);

        if(randomValue <= _PercentageChanceOfSpawning)
        { SpawnGreenResource(); }
    }

    private IEnumerator CheckSpawn()
    {
        while (true)
        {
            yield return new WaitForSeconds(_GreenItemInterval);
            SpawnItem();
        }
    }
}
