using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Configurable information for tiles is stored here to allow for easy variant creation. Each instance is a different variant of a tile.
public abstract class TileVariant : ScriptableObject, IMapComponent
{
    public abstract Tile CreateTile();

    public virtual void AddToLevel(ref Level level, IntVec position)
    {
        level.map.mapLayers[position.y].tiles[position.x, position.z] = CreateTile();
    }
}

public abstract class TileVariant<TileClass> : TileVariant where TileClass : Tile, new()
{
    public override Tile CreateTile()
    {
        TileClass newTile = new TileClass();
        newTile.variant = this;
        return newTile;
    }

    public Type GetTileClassType()
    {
        return typeof(TileClass);
    }
}