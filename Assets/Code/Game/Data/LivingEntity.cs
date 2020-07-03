using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : Entity, IEntityDirectional
{
    CardinalDir direction = CardinalDir.NORTH;
    public CardinalDir forward { get { return direction; } set { direction = value; } }
}
