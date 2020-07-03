using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEntity : LivingEntity
{
    public bool justTeleported = true; //Flag for camera to know not to smoothly change position but to do so instantaneously
}
