using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Base classes
public abstract class EntityAction
{
    public virtual Entity entity { get; set; }

    /// <returns>Successful?</returns>
    public abstract bool Execute();

    public EntityAction(Entity actionPerformer)
    {
        entity = actionPerformer;
    }
}
public abstract class EntityAction<EntityInterfaceType> : EntityAction where EntityInterfaceType : IEntityInterface
{
    public EntityInterfaceType entityInterface;

    public EntityAction(EntityInterfaceType actionPerformer) : base(actionPerformer.GetEntity())
    {
        entityInterface = actionPerformer;
    }
}

//Specific Actions
public class ActionMove : EntityAction
{
    public IntVec destination;

    public override bool Execute()
    {
        return LevelManager.instance.MoveEntity(entity, destination);
    }

    public ActionMove(Entity actionPerformer, IntVec toPosition) : base(actionPerformer)
    {
        destination = toPosition;
    }
}

public class ActionTurn : EntityAction<IEntityDirectional>
{
    public CardinalDir newDirection;

    public override bool Execute()
    {
        entityInterface.forward = newDirection;
        return true;
    }

    public ActionTurn(IEntityDirectional actionPerformer, CardinalDir newForward) : base(actionPerformer)
    {
        newDirection = newForward;
    }
}