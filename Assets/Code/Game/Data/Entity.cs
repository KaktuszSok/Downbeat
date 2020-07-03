using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity : IMapComponent
{
    public IntVec pos;
    public TagList tags;

    public void AddToLevel(ref Level level, IntVec position)
    {
        pos = position;
        level.entities.Add(this);
    }
}