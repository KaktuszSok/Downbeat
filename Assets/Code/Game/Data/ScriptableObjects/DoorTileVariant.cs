using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Door Tile", menuName = "Tile Variant/Door")]
public class DoorTileVariant : TileVariant<DoorTile>
{
    public Material doorMat;
    public Material floorMat;
}