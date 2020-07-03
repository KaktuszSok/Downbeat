using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WallRenderer : TileRenderer
{

    public override void UpdateVisuals()
    {
        CleanTile();

        rend.material = ((WallTileVariant)tile.variant).mat;

        Mesh mesh = new Mesh();

        mesh.vertices = CubeMeshUtil.verts;

        List<int> tris = new List<int>();
        if (IsFaceVisible(CardinalDir.NORTH)) tris.AddRange(CubeMeshUtil.faceNorth);
        if (IsFaceVisible(CardinalDir.SOUTH)) tris.AddRange(CubeMeshUtil.faceSouth);
        if (IsFaceVisible(CardinalDir.EAST)) tris.AddRange(CubeMeshUtil.faceEast);
        if (IsFaceVisible(CardinalDir.WEST)) tris.AddRange(CubeMeshUtil.faceWest);
        if (IsFaceVisible(CardinalDir.UP)) tris.AddRange(CubeMeshUtil.faceUp);
        if (IsFaceVisible(CardinalDir.DOWN)) tris.AddRange(CubeMeshUtil.faceDown);
        mesh.triangles = tris.ToArray();
        if (tris.Count == 0) Destroy(gameObject); //Completely obscured wall

        mesh.normals = CubeMeshUtil.Normals();

        mesh.uv = CubeMeshUtil.UVs();

        mesh.OptimizeReorderVertexBuffer();
        meshFilter.mesh = mesh;
    }
}
