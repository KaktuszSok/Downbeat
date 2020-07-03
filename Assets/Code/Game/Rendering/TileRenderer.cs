using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Visual side of the tiles the world is made up of.
public abstract class TileRenderer : MonoBehaviour
{
    protected MeshRenderer rend;
    protected MeshFilter meshFilter;

    public static Map map;
    public IntVec pos { get; protected set; }
    public Tile tile;

    private void Awake()
    {
        meshFilter = gameObject.AddComponent<MeshFilter>();
        rend = gameObject.AddComponent<MeshRenderer>();
    }

    public void CleanTile() //Destroys any previously generated mesh and/or children
    {
        //Mesh
        meshFilter.mesh.Clear();

        //Children
        List<GameObject> childrenToDestroy = new List<GameObject>();

        foreach(Transform child in transform)
        {
            childrenToDestroy.Add(child.gameObject);
        }
        while(childrenToDestroy.Count > 0)
        {
            Destroy(childrenToDestroy[0]);
            childrenToDestroy.RemoveAt(0);
        }
    }

    public virtual void LoadFromMap(Map levelMap, IntVec position)
    {
        map = levelMap;
        pos = position;
        tile = levelMap.TileAtPos(position);

        transform.localPosition = (Vector3)position;

        UpdateVisuals();
    }

    public abstract void UpdateVisuals(); //Recommend to call CleanTile in implementation of this.

    /// <summary>
    /// Returns false if that side of the tile is obstructed or OOB
    /// </summary>
    public bool IsFaceVisible(CardinalDir face)
    {
        IntVec checkPos = pos + (IntVec)face;
        if (map.IsPosOOB(checkPos))
            return false; //Out of bounds - treated as opaque, since these walls need not be rendered.

        Tile checkTile = map.TileAtPos(checkPos);
        if (checkTile == null) return true;                                     //Face is touching air
        else if(!checkTile.hidesAdjacentFaces &&                                //Or face is touching non-occluding tile
            !(face == CardinalDir.UP && checkTile is GroundTile)) return true;  //And face is not underneath a ground tile
        return false; //Otherwise, face is hidden by adjacent tile.
    }
}
