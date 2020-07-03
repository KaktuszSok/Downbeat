using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EffectGUIFlash : MonoBehaviour
{
    Image image;
    public Gradient FlashGradient;
    public float defaultFlashTime = 0.75f;

    Coroutine flashCoroutine = null;

    private void Awake()
    {
        image = GetComponent<Image>();
    }

    public void Flash()
    {
        Flash(defaultFlashTime);
    }
    public void Flash(float time)
    {
        if (flashCoroutine != null) StopCoroutine(flashCoroutine);
        flashCoroutine = StartCoroutine(FlashCoroutine(time));
    }

    IEnumerator FlashCoroutine(float time)
    {
        float timer = 0;
        while(timer <= time)
        {
            image.color = FlashGradient.Evaluate(timer / time);

            timer += Time.deltaTime;
            yield return null;
        }
        image.color = FlashGradient.Evaluate(1);
    }
}
