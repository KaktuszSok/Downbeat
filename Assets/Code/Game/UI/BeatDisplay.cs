using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatDisplay : MonoBehaviour
{
    public static BeatDisplay instance;
    public static AudioSource music;
    public static float musicBPM = 150f;
    public static float musicOffset = 0f;

    public Slider BeatSlider;
    public Transform InputsDisplay;
    public GameObject InputMarkerPrefab;
    public Transform BackgroundFull;
    public EffectGUIFlash[] BackgroundSubdivisions;
    public float bpm = 150f;
    public float offset = 0f;

    int prevBeat = 0;
    int prevBeatSubdivided = 0;
    float subdivisionDenominator = 2f;

    void Awake()
    {
        instance = this;

        musicBPM = bpm;
        musicOffset = offset;

        BackgroundSubdivisions = new EffectGUIFlash[BackgroundFull.childCount];
        int i = 0;
        foreach (Transform child in BackgroundFull)
        {
            BackgroundSubdivisions[i] = child.GetComponent<EffectGUIFlash>();
            i++;
        }
    }

    private void Start()
    {
        music = Camera.main.GetComponent<AudioSource>();
        ClearInputMarkers();
    }

    void Update()
    {
        //DEBUG
        /*if(Input.GetKeyDown(KeyCode.X))
        {
            musicOffset = offset;
        }*/

        float beat = GetBeatTime(music.time);
        BeatSlider.value = Mathf.PingPong(beat, 1f);

        if(prevBeat < (int)beat) //New beat
        {
            prevBeat = (int)beat;
        }

        //Check if we entered a new beat subdivision
        if (prevBeatSubdivided < (int)(beat*subdivisionDenominator))
        {
            FlashBeatSubdivision((int)(beat * subdivisionDenominator));
            prevBeatSubdivided = (int)(beat * subdivisionDenominator);
            subdivisionDenominator = GetSubdivisionBeatFractionDenominator(BackgroundSubdivisions.Length);
        }
    }

    public void AddInputMarker()
    {
        AddInputMarker(music.time);
    }
    public void AddInputMarker(float time)
    {
        float metronomeValue = Mathf.PingPong(GetBeatTime(time), 1f);

        RectTransform marker = Instantiate(InputMarkerPrefab, InputsDisplay).GetComponent<RectTransform>();
        Vector2 anchoredPos = marker.anchoredPosition;
        Vector2 anchorMin = marker.anchorMin; Vector2 anchorMax = marker.anchorMax;

        anchoredPos.x = 0f;
        anchorMin.x = anchorMax.x = metronomeValue;

        marker.anchorMin = anchorMin; marker.anchorMax = anchorMax;
        marker.anchoredPosition = anchoredPos;

        marker.gameObject.AddComponent<Autodestroy>().destroyTimer = 1 / (musicBPM / 60f);
        marker.GetComponent<EffectGUIColourOverLifetime>().Initiate();
    }

    public void ClearInputMarkers()
    {
        List<GameObject> inputMarkers = new List<GameObject>();
        foreach(Transform child in InputsDisplay)
        {
            inputMarkers.Add(child.gameObject);
        }

        while(inputMarkers.Count > 0)
        {
            Destroy(inputMarkers[0]);
            inputMarkers.RemoveAt(0);
        }
    }

    /// <summary>
    /// Plays a flashing animation on one of the beat's subdivision boxes.
    /// </summary>
    public void FlashBeatSubdivision(int subdivisionIndex)
    {
        subdivisionIndex = (int)Mathf.PingPong(subdivisionIndex, BackgroundSubdivisions.Length-1);
        BackgroundSubdivisions[subdivisionIndex].Flash((1f/GetSubdivisionBeatFractionDenominator(BackgroundSubdivisions.Length)) / (musicBPM / 60f)); //This time length ensures that subdivisons adjacent to the downbeats finish their animation before being activated again.
    }

    /// <summary> Each background subdivision corresponds to 1/x beats. A full background ping-pong spans 2 beats. </summary>
    /// <remarks>
    /// For 2 subdivisions, x is 1. (each subdiv is one full beat, i.e. 1/1)
    /// For 3 subdivisions, x is 2. (each subdiv is an eighth note, i.e. 1/2 a beat)
    /// For 4 subdivisions, x is 3. (each subdiv is 1/3 of a beat)
    /// Edge case is 1 subdivision, where it would represent 2 full beats, aka 1/0.5 beats so x would be 0.5.
    /// etc. </remarks>
    float GetSubdivisionBeatFractionDenominator(int subdivisions)
    {
            if (subdivisions == 1) return 0.5f;
            else return subdivisions - 1;
    }

    public static float GetBeatTime(float time)
    {
        return (time - musicOffset) * (musicBPM / 60f); //beat = adjusted time (in seconds) * beats per second
    }
}
