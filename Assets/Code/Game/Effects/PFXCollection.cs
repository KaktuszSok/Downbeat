using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PFXCollection", menuName = "Collections/PFXCollection")]
public class PFXCollection : ScriptableObject
{

    public GameObject[] particles;

    public ParticleSystem PlayEffectAtPosition(Vector3 pos, Vector3 rot, int index = -1, float scale = 1f)
    {
        if (index == -1) index = PickRandomIndex();
        else index = Mathf.Clamp(index, 0, particles.Length - 1);

        Transform fx = Instantiate(particles[index]).transform;
        fx.transform.position = pos;
        fx.transform.eulerAngles = rot;
        fx.transform.localScale = Vector3.one * scale;
        ParticleSystem ps = fx.GetComponent<ParticleSystem>();
        ps.Play();

        fx.gameObject.AddComponent<Autodestroy>().destroyTimer = ps.main.duration;
        return ps;
    }
    public ParticleSystem PlayRandomEffectAtPosition(Vector3 pos, Vector3 rot, float scale = 1f)
    {
        return PlayEffectAtPosition(pos, rot, PickRandomIndex(), scale);
    }

    public int PickRandomIndex()
    {
        return Random.Range(0, particles.Length);
    }
}