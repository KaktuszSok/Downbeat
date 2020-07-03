using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ground Tile", menuName = "Tile Variant/Ground")]
public class GroundTileVariant : TileVariant<GroundTile>
{
    public Material mat;
    public bool renderUnderside = false;
}
