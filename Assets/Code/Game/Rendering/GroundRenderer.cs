using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundRenderer : TileRenderer
{
    public override void UpdateVisuals()
    {
        CleanTile();

        GroundTileVariant tileVariant = (GroundTileVariant)tile.variant;
        rend.material = tileVariant.mat;

        Mesh mesh = new Mesh();

        if (!tileVariant.renderUnderside || !IsFaceVisible(CardinalDir.DOWN)) //Only be a floor
        {
            //Create floor quad
            Vector3[] verts = new Vector3[4]
            {
                //when looking from above:
                new Vector3(-0.5f,0,-0.5f), //bottom left
                new Vector3(0.5f,0,-0.5f), //bottom right
                new Vector3(-0.5f,0,0.5f), //top left
                new Vector3(0.5f,0,0.5f) //top right
            };
            mesh.vertices = verts;

            int[] tris = new int[6]
            {
                //bottom left tri
                0, 2, 1,
                //top right tri
                2, 3, 1
            };
            mesh.triangles = tris;

            Vector3[] normals = new Vector3[4]
            {
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up
            };
            mesh.normals = normals;

            Vector2[] uv = new Vector2[4]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            mesh.uv = uv;
        }
        else //Be a floor and also a ceiling for the layer below
        {
            //Create floor/ceil quads
            Vector3[] verts = new Vector3[8]
            {
                //when looking from above:
                new Vector3(-0.5f,0,-0.5f), //bottom left
                new Vector3(0.5f,0,-0.5f), //bottom right
                new Vector3(-0.5f,0,0.5f), //top left
                new Vector3(0.5f,0,0.5f), //top right
                new Vector3(-0.5f,0,-0.5f), //bottom left
                new Vector3(0.5f,0,-0.5f), //bottom right
                new Vector3(-0.5f,0,0.5f), //top left
                new Vector3(0.5f,0,0.5f) //top right
            };
            mesh.vertices = verts;

            int[] tris = new int[12]
            {
                //bottom left tri
                0, 2, 1,
                //top right tri
                2, 3, 1,
                //ceiling
                5,6,4,
                5,7,6
            };
            mesh.triangles = tris;

            Vector3[] normals = new Vector3[8]
            {
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.up,
                Vector3.down,
                Vector3.down,
                Vector3.down,
                Vector3.down
            };
            mesh.normals = normals;

            Vector2[] uv = new Vector2[8]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1),
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };
            mesh.uv = uv;
        }
        meshFilter.mesh = mesh;
    }
}
