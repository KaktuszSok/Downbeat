using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Wall Tile", menuName = "Tile Variant/Wall")]
public class WallTileVariant : TileVariant<WallTile>
{
    public Material mat;
    public bool transparent = false;
}
