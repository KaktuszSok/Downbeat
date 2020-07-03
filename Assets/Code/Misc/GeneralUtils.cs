using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Badger.Libraries.Images;

public static class CubeMeshUtil
{
    public static readonly int[] faceNorth = {1,0,3,3,0,2};
    public static readonly int[] faceSouth = {11,10,7,7,10,6};
    public static readonly int[] faceEast = {22,20,21,21,20,23};
    public static readonly int[] faceWest = {18,16,17,17,16,19};
    public static readonly int[] faceUp = {9,8,5,5,8,4};
    public static readonly int[] faceDown = {14,12,13,13,12,15};

    public static readonly Vector3[] verts = new Vector3[24]
    {
        new Vector3(0.5f, -0.5f, 0.5f) + Vector3.up*0.5f,
        new Vector3(-0.5f, -0.5f, 0.5f) + Vector3.up*0.5f,
        new Vector3(0.5f, 0.5f, 0.5f) + Vector3.up*0.5f,
        new Vector3(-0.5f, 0.5f, 0.5f) + Vector3.up*0.5f,
        new Vector3(0.5f, 0.5f, -0.5f) + Vector3.up*0.5f,
        new Vector3(-0.5f, 0.5f, -0.5f) + Vector3.up*0.5f,
        new Vector3(0.5f, -0.5f, -0.5f) + Vector3.up*0.5f,
        new Vector3(-0.5f, -0.5f, -0.5f) + Vector3.up*0.5f,
        new Vector3(0.5f, 0.5f, 0.5f) + Vector3.up*0.5f,
        new Vector3(-0.5f, 0.5f, 0.5f) + Vector3.up*0.5f,
        new Vector3(0.5f, 0.5f, -0.5f) + Vector3.up*0.5f,
        new Vector3(-0.5f, 0.5f, -0.5f) + Vector3.up*0.5f,
        new Vector3(0.5f, -0.5f, -0.5f) + Vector3.up*0.5f,
        new Vector3(-0.5f, -0.5f, 0.5f) + Vector3.up*0.5f,
        new Vector3(-0.5f, -0.5f, -0.5f) + Vector3.up*0.5f,
        new Vector3(0.5f, -0.5f, 0.5f) + Vector3.up*0.5f,
        new Vector3(-0.5f, -0.5f, 0.5f) + Vector3.up*0.5f,
        new Vector3(-0.5f, 0.5f, -0.5f) + Vector3.up*0.5f,
        new Vector3(-0.5f, -0.5f, -0.5f) + Vector3.up*0.5f,
        new Vector3(-0.5f, 0.5f, 0.5f) + Vector3.up*0.5f,
        new Vector3(0.5f, -0.5f, -0.5f) + Vector3.up*0.5f,
        new Vector3(0.5f, 0.5f, 0.5f) + Vector3.up*0.5f,
        new Vector3(0.5f, -0.5f, 0.5f) + Vector3.up*0.5f,
        new Vector3(0.5f, 0.5f, -0.5f) + Vector3.up*0.5f
    };

    public static Vector2[] UVs() //can't be arsed to convert this SO code to a hard-coded array
    {
        // - set UV coordinates -
        Vector2[] uvs = new Vector2[24];

        // FRONT    2    3    0    1
        uvs[2] = new Vector2(0, 1);
        uvs[3] = new Vector2(1, 1);
        uvs[0] = new Vector2(0, 0);
        uvs[1] = new Vector2(1, 0);

        // BACK    6    7   10   11
        uvs[6] = new Vector2(0, 0);
        uvs[7] = new Vector2(1, 0);
        uvs[10] = new Vector2(0, 1);
        uvs[11] = new Vector2(1, 1);
        //^This one's upside down for whatever reason

        // LEFT   19   17   16   18
        uvs[19] = new Vector2(0, 1);
        uvs[17] = new Vector2(1, 1);
        uvs[16] = new Vector2(0, 0);
        uvs[18] = new Vector2(1, 0);

        // RIGHT   23   21   20   22
        uvs[23] = new Vector2(0, 1);
        uvs[21] = new Vector2(1, 1);
        uvs[20] = new Vector2(0, 0);
        uvs[22] = new Vector2(1, 0);

        // TOP    4    5    8    9
        uvs[4] = new Vector2(0, 1);
        uvs[5] = new Vector2(1, 1);
        uvs[8] = new Vector2(0, 0);
        uvs[9] = new Vector2(1, 0);

        // BOTTOM   15   13   12   14
        uvs[15] = new Vector2(0, 1);
        uvs[13] = new Vector2(1, 1);
        uvs[12] = new Vector2(0, 0);
        uvs[14] = new Vector2(1, 0);

        return uvs;
    }

    public static Vector3[] Normals()
    {
        Vector3[] normals = new Vector3[24];
        normals[2] = normals[3] = normals[0] = normals[1] = Vector3.forward;
        normals[6] = normals[7] = normals[10] = normals[11] = Vector3.back;
        normals[19] = normals[17] = normals[16] = normals[18] = Vector3.left;
        normals[23] = normals[21] = normals[20] = normals[22] = Vector3.right;
        normals[4] = normals[5] = normals[8] = normals[9] = Vector3.up;
        normals[15] = normals[13] = normals[12] = normals[14] = Vector3.down;

        return normals;

    }
}

public struct IntVec //Integer vector
{
    public int x, z, y; //y last because it is least important for this game

    public IntVec(int xpos, int zpos, int ypos = 0)
    {
        x = xpos; z = zpos; y = ypos;
    }

    //Arithmetic operators
    public static IntVec operator + (IntVec lhs, IntVec rhs)
    {
        return new IntVec(lhs.x + rhs.x, lhs.z + rhs.z, lhs.y + rhs.y);
    }
    public static IntVec operator -(IntVec lhs, IntVec rhs)
    {
        return new IntVec(lhs.x - rhs.x, lhs.z - rhs.z, lhs.y - rhs.y);
    }

    //Conversion operators
    public static explicit operator Vector3(IntVec p)
    {
        return new Vector3(p.x, p.y, p.z);
    }
    public static explicit operator IntVec(Vector3 v)
    {
        return new IntVec((int)v.x, (int)v.z, (int)v.y);
    }

    public static explicit operator IntVec(CardinalDir dir)
    {
        switch(dir)
        {
            case CardinalDir.NORTH:
                return new IntVec(0, 1, 0);
            case CardinalDir.SOUTH:
                return new IntVec(0, -1, 0);
            case CardinalDir.EAST:
                return new IntVec(1, 0, 0);
            case CardinalDir.WEST:
                return new IntVec(-1, 0, 0);
            case CardinalDir.UP:
                return new IntVec(0, 0, 1);
            case CardinalDir.DOWN:
                return new IntVec(0, 0, -1);
            default:
                return new IntVec(0, 0, 0);
        }
    }

    public override string ToString()
    {
        return "(" + x + "," + z + ", L" + y + ")";
    }
}

public enum CardinalDir
{
    //directions 0 through 3 can be looped through for turning around 360 degrees clockwise.
    NORTH, //Z+
    EAST, //X+
    SOUTH, //Z-
    WEST, //X-
    UP, //Y+
    DOWN //Y-
}

public static class CardinalHelper
{
    public static Vector3 ToVector3(this CardinalDir dir)
    {
        switch (dir)
        {
            case CardinalDir.NORTH:
                return new Vector3(0, 0, 1);
            case CardinalDir.SOUTH:
                return new Vector3(0, 0, -1);
            case CardinalDir.EAST:
                return new Vector3(1, 0, 0);
            case CardinalDir.WEST:
                return new Vector3(-1, 0, 0);
            case CardinalDir.UP:
                return new Vector3(0, 1, 0);
            case CardinalDir.DOWN:
                return new Vector3(0, -1, 0);
            default:
                return new Vector3(0, 0, 0);
        }
    }


    /// <param name="rightTurns">1 = turn right, 2 = turn around, -1 = turn left. Works with any integer.</param>
    /// <remarks>Does not support input directions of UP or DOWN.</remarks>
    public static CardinalDir Turn(this CardinalDir dir, int rightTurns)
    {
        int directionIndex = (int)dir;
        if (directionIndex > 3) return dir; //input is UP or DOWN, not supported.

        return (CardinalDir)Mathf.Repeat(directionIndex+rightTurns, 4);
    }
}

public static class SaveLoadUtil
{
    //Loads a PNG file and ignores sRGB and gAMA chunks which usually result in wrong colours when imported from a file (not from the editor, strangely enough) with Texture2D.LoadImage().
    public static Texture2D LoadPNGFixed(string path)
    {
        Texture2D tex = new Texture2D(2, 2, UnityEngine.Experimental.Rendering.GraphicsFormat.R8G8B8A8_SRGB, 0, UnityEngine.Experimental.Rendering.TextureCreationFlags.None);
        tex.filterMode = FilterMode.Point;

        Png png = new Png(File.ReadAllBytes(path));
        png.RemoveChunk("gAMA");
        png.RemoveChunk("sRGB");

        tex.LoadImage(png.ToBytes());
        return tex;
    }
}

public static class Texture2DUtil
{
    public static Color32[,] GetColours2D(this Texture2D texture)
    {
        Color32[] colours1D = texture.GetPixels32();
        Color32[,] colours2D = new Color32[texture.width, texture.height];
        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                colours2D[x, y] = colours1D[y * texture.width + x];
            }
        }
        return colours2D;
    }
}