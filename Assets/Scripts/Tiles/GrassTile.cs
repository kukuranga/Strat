using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassTile : Tile
{
    [SerializeField] private Color _baseColor, _OffsetColor;


    public override void Init(int x, int y)
    {
        base.Init(x, y);
        var isOffset = (x + y) % 2 == 1;
        //_renderer.color = isOffset ? _OffsetColor : _baseColor;
    }
}
