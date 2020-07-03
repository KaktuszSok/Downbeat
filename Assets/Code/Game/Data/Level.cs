using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Stores information, does not manage it.
public class Level
{
    public string name;
    public Map map;
    public HashSet<Entity> entities;
    public Color32 skyColour = Color.black;

    public bool IsPosOOB(int x, int z, int y=0) { return map.IsPosOOB(x, z, y); }
    public bool IsPosOOB(IntVec pos) { return map.IsPosOOB(pos); }
    public Tile TileAtPos(int x, int z, int y=0) { return map.TileAtPos(x, z, y); }
    public Tile TileAtPos(IntVec pos) { return map.TileAtPos(pos); }

    public Level(string levelName)
    {
        name = levelName;
        map = new Map();
        entities = new HashSet<Entity>();
    }
}

public class Map
{
    public Dictionary<int, MapLayer> mapLayers = new Dictionary<int, MapLayer>();
    public Dictionary<int, IntVec> entrances = new Dictionary<int, IntVec>();
    public Dictionary<IntVec, MapExit> exits = new Dictionary<IntVec, MapExit>();

    /// <returns>successful?</returns>
    public bool AddLayer(MapLayer layer)
    {
        if (mapLayers.ContainsKey(layer.height))
        {
            Debug.LogError("Could not add layer at height " + layer.height + " as it is already occupied.");
            return false;
        }
        mapLayers.Add(layer.height, layer); return true;
    }

    //Checks if position is out of bounds
    public bool IsPosOOB(int x, int z, int y=0)
    {
        return IsPosOOB(new IntVec(x, z, y));
    }
    public bool IsPosOOB(IntVec pos)
    {
        if(mapLayers.ContainsKey(pos.y))
        {
            MapLayer layer = mapLayers[pos.y];
            if (layer.tiles.GetLength(0) > pos.x && layer.tiles.GetLength(1) > pos.z && pos.x >= 0 && pos.z >= 0)
            {
                return false;
            }
        }
        return true;
    }

    //Gets tile at a position.
    public Tile TileAtPos(int x, int z, int y = 0)
    {
        return TileAtPos(new IntVec(x, z, y));
    }
    public Tile TileAtPos(IntVec pos)
    {
        if (IsPosOOB(pos)) return null; //Out of Bounds
        return mapLayers[pos.y].tiles[pos.x, pos.z]; //else, in bounds, return tile
    }
}

public class MapLayer
{
    public int height = 0;
    public Tile[,] tiles = new Tile[0,0];
}

public interface IMapComponent
{
    void AddToLevel(ref Level level, IntVec position);
}
