using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapEntrance : IMapComponent
{
    int ID = 0;

    public MapEntrance(int entranceID)
    {
        ID = entranceID;
    }

    public void AddToLevel(ref Level level, IntVec position)
    {
        if(level.map.entrances.ContainsKey(ID))
        {
            Debug.LogWarning("Failed to add map entrance! Cannot add map entrance with ID " + ID + " to level " + level.name + " at position " + position.ToString() +  " as this ID is already in use.");
            return;
        }
        level.map.entrances.Add(ID, position);
    }
}

public class MapExit : IMapComponent
{
    public string level = "";
    public int entranceID = 0;

    public MapExit(string destinationLevel, int destinationEntrance)
    {
        level = destinationLevel;
        entranceID = destinationEntrance;
    }

    public void AddToLevel(ref Level level, IntVec position)
    {
        if(level.map.exits.ContainsKey(position))
        {
            Debug.LogWarning("Failed to add map exit! Cannot add map exit at position " + position + " to level " + level.name + " as there already is an exit at this position.");
            return;
        }
        level.map.exits.Add(position, this);
    }
}
