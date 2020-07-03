using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EntityInterfaceHelper
{
    public static Entity GetEntity(this IEntityInterface entityInterface)
    {
        return (Entity)entityInterface;
    }
}

public interface IEntityInterface
{
    
}

public interface IEntityDirectional : IEntityInterface
{
    CardinalDir forward { get; set; }
}