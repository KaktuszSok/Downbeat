using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New SFXCollection", menuName = "Collections/SFXCollection")]
public class SFXCollection : ScriptableObject
{
    public AudioClip[] clips;
    public UnityEngine.Audio.AudioMixerGroup mixerGroup;
    public float volume = 1f;
    public float randomiseVol = 0f;
    public float pitch = 1f;
    public float randomisePitch = 0f;
    public float rangeMin = 5f;
    public float rangeMax = 500f;
    public bool omnipresent = false; //use spatial blend 2D?

    public AudioSource PlaySoundAtPosition(Vector3 pos, int index = -1, float volMult = 1f, float pitchMult = 1f, float rangeMult = 1f)
    {
        if (index == -1) index = PickRandomIndex();
        else index = Mathf.Clamp(index, 0, clips.Length - 1);

        Transform sfx = new GameObject("sfx_" + name + "_" + index).transform;
        sfx.position = pos;
        AudioSource a = sfx.gameObject.AddComponent<AudioSource>();
        a.clip = clips[index];
        a.outputAudioMixerGroup = mixerGroup;
        a.volume = GetVolRandomised() * volMult;
        a.pitch = GetPitchRandomised() * pitchMult;
        a.minDistance = rangeMin;
        a.maxDistance = rangeMax;
        a.spatialBlend = omnipresent ? 0 : 1;
        a.Play();

        sfx.gameObject.AddComponent<Autodestroy>().destroyTimer = a.clip.length / a.pitch;

        return a;
    }
    public AudioSource PlayRandomSoundAtPosition(Vector3 pos, float volMult = 1f, float pitchMult = 1f, float rangeMult = 1f)
    {
        return PlaySoundAtPosition(pos, PickRandomIndex(), volMult, pitchMult, rangeMult);
    }

    public void PlaySoundSeriesAtPosition(Vector3 pos, float volMult = 1f, float pitchMult = 1f, float rangeMult = 1f, params int[] indices)
    {
        foreach (int index in indices)
        {
            PlaySoundAtPosition(pos, index, volMult, pitchMult, rangeMult);
        }
    }
    public void PlayRandomSoundSeriesAtPosition(Vector3 pos, int amount, float volMult = 1f, float pitchMult = 1f, float rangeMult = 1f)
    {
        int[] indices = new int[amount];
        for (int i = 0; i < amount; i++)
        {
            indices[i] = PickRandomIndex();
        }
        PlaySoundSeriesAtPosition(pos, volMult, pitchMult, rangeMult, indices);
    }

    float GetVolRandomised()
    {
        return Random.Range(volume * (1 - randomiseVol), volume * (1 + randomiseVol));
    }
    float GetPitchRandomised()
    {
        return Random.Range(pitch * (1 - randomisePitch), pitch * (1 + randomisePitch));
    }

    public int PickRandomIndex()
    {
        return Random.Range(0, clips.Length);
    }
    public AudioClip PickRandomSound()
    {
        return clips[PickRandomIndex()];
    }

}