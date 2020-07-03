using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Autodestroy : MonoBehaviour
{

    public float destroyTimer = -6969f;

    void Update()
    {
        if (destroyTimer > 0 && destroyTimer != -6969f)
        {
            destroyTimer -= Time.deltaTime;
            if (destroyTimer <= 0)
            {
                Destroy(gameObject);
            }
        }
    }
}