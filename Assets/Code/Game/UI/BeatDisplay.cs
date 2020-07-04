using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BeatDisplay : MonoBehaviour
{
    public static BeatDisplay instance;

    public Slider BeatSlider;
    public Transform InputsDisplay;
    [Tooltip("Ignored, Failed, Success.")]
    public GameObject[] InputMarkerPrefabs; //Index corresponds to ActionResult enum index. 
    public Transform BackgroundFull;
    public GameObject BackgroundSubdivPrefab;
    List<EffectGUIFlash> BackgroundSubdivisions = new List<EffectGUIFlash>();

    void Awake()
    {
        instance = this;

        BackgroundSubdivisions = new List<EffectGUIFlash>();
        foreach (Transform child in BackgroundFull)
        {
            BackgroundSubdivisions.Add(child.GetComponent<EffectGUIFlash>());
        }

        BeatManager.OnTick += OnNewTick;
        BeatManager.OnTickrateChanged += OnTickrateChanged;
    }

    private void Start()
    {
        ClearInputMarkers();
    }

    void Update()
    {
        float beat = (float)BeatManager.GetBeatTime();
        BeatSlider.value = Mathf.PingPong(beat, 1f);
    }

    public void AddInputMarker(LevelManager.ActionResult markerType)
    {
        AddInputMarker(markerType, (float)BeatManager.musicTime);
    }
    public void AddInputMarker(LevelManager.ActionResult markerType, float time)
    {
        float metronomeValue = Mathf.PingPong((float)BeatManager.GetBeatTime(time), 1f);

        RectTransform marker = Instantiate(InputMarkerPrefabs[(int)markerType], InputsDisplay).GetComponent<RectTransform>();
        Vector2 anchoredPos = marker.anchoredPosition;
        Vector2 anchorMin = marker.anchorMin; Vector2 anchorMax = marker.anchorMax;

        anchoredPos.x = 0f;
        anchorMin.x = anchorMax.x = metronomeValue;

        marker.anchorMin = anchorMin; marker.anchorMax = anchorMax;
        marker.anchoredPosition = anchoredPos;

        marker.gameObject.AddComponent<Autodestroy>().destroyTimer = 1 / (BeatManager.bpm / 60f);
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
        subdivisionIndex = (int)Mathf.PingPong(subdivisionIndex, BackgroundSubdivisions.Count-1);
        BackgroundSubdivisions[subdivisionIndex].Flash(Mathf.Max(1f/BackgroundSubdivisions.Count, 0.25f) / (BeatManager.bpm / 60f));
    }

    /// <summary>
    /// Update the beat background to be subdivided appropriately for a given amount of ticks per beat.
    /// </summary>
    /// <remarks>Does not support fractional tickrates (yet?).</remarks> //TODO?
    public void SetSubdivisionsAmount(int ticksPerBeat)
    {
        int amountOfSubdivsNeeded = ticksPerBeat + 1;

        while(BackgroundSubdivisions.Count > amountOfSubdivsNeeded)
        {
            Destroy(BackgroundSubdivisions[BackgroundSubdivisions.Count - 1].gameObject);
            BackgroundSubdivisions.RemoveAt(BackgroundSubdivisions.Count - 1);
        }
        while(BackgroundSubdivisions.Count < amountOfSubdivsNeeded)
        {
            BackgroundSubdivisions.Add(Instantiate(BackgroundSubdivPrefab, BackgroundFull).GetComponent<EffectGUIFlash>());
        }

        //Assign sizes
        int index = 0;
        foreach (EffectGUIFlash subdivision in BackgroundSubdivisions)
        {
            float height = 25f;
            if (index == 0 || index == BackgroundSubdivisions.Count - 1) height = 45f; //Beats are Phat
            else if (index * 2 == BackgroundSubdivisions.Count - 1) height = 35f; //Off-Beat is HOT!

            RectTransform rect = subdivision.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, height);

            index++;
        }
    }

    public void OnNewTick()
    {
        FlashBeatSubdivision((int)(BeatManager.GetTickTime()));
    }

    public void OnTickrateChanged()
    {
        SetSubdivisionsAmount((int)BeatManager.ticksPerBeat);
    }
}
