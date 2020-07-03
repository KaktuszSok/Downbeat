using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Game Data and Logic side of the tiles the world is made up of.
public abstract class Tile
{
    //Tile Info
    public abstract TileVariant variant { get; set; }
    public abstract Type renderer { get; }

    //Tile Parameters
    /// <summary>
    /// Should another tile's face be considered obscured if it is touching this tile? Yes for walls, no for ground, no for transparent objects.
    /// </summary>
    public virtual bool hidesAdjacentFaces { get { return true; } }
    public abstract bool isWalkable { get; }

    //Functions
    public Tile() { }
    public Tile(TileVariant tileVariant)
    {
        variant = tileVariant;
    }
}

public abstract class Tile<Variant, Renderer> : Tile where Variant : TileVariant where Renderer : TileRenderer
{
    public override TileVariant variant
    {
        get { return variant; }
        set { typedVariant = (Variant)value; }
    }
    public Variant typedVariant { get; protected set; }

    public override Type renderer
    {
        get
        {
            return typeof(Renderer);
        }
    }

    public Tile() { }
    public Tile(Variant tileVariant) : base(tileVariant)
    {

    }
}

public class GroundTile : Tile<GroundTileVariant, GroundRenderer>
{
    //Overrides
    public override bool hidesAdjacentFaces { get { return false; } }
    public override bool isWalkable { get { return true; } }

    public GroundTile() { }
    public GroundTile(GroundTileVariant tileVariant) : base(tileVariant) { }
}

public class WallTile : Tile<WallTileVariant, WallRenderer>
{
    //Overrides
    public override bool hidesAdjacentFaces { get { return !typedVariant.transparent; } }
    public override bool isWalkable { get { return false; } }

    public WallTile() { }
    public WallTile(WallTileVariant tileVariant) : base(tileVariant) { }
}

public class DoorTile : Tile<DoorTileVariant, WallRenderer> //TODO: Add Door Renderer
{
    //Class-specific code
    public bool isOpen = false;

    public void SetOpen(bool open)
    {
        isOpen = open;
    }

    //Overrides
    public override bool hidesAdjacentFaces { get { return false; } } //False as once the door is open, it acts and renders like a ground tile.
    public override bool isWalkable { get { return isOpen; } }

    public DoorTile() { }
    public DoorTile(DoorTileVariant tileVariant) : base(tileVariant) { }
}