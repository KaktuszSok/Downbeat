using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectGUIColourOverLifetime : MonoBehaviour
{
    Image image;
    Autodestroy autodestroy;

    public Gradient colourOverLifetime;
    float startLifetime;
    bool started = false;

    public void Initiate()
    {
        image = GetComponent<Image>();
        autodestroy = GetComponent<Autodestroy>();
        startLifetime = autodestroy.destroyTimer;

        started = true;
    }

    private void Update()
    {
        if(started)
        {
            float currLifetimeNormalised = 1 - (autodestroy.destroyTimer / startLifetime);
            image.color = colourOverLifetime.Evaluate(currLifetimeNormalised);
        }
    }
}
