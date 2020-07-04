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
    public EffectGUIFlash[] BackgroundSubdivisions;

    int prevBeat = 0;
    int prevBeatSubdivided = 0;
    float subdivisionDenominator = 2f;

    void Awake()
    {
        instance = this;

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
        BeatManager.OnTick += OnNewTick;
        ClearInputMarkers();
    }

    void Update()
    {
        //DEBUG
        /*if(Input.GetKeyDown(KeyCode.X))
        {
            musicOffset = offset;
        }*/

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
        subdivisionIndex = (int)Mathf.PingPong(subdivisionIndex, BackgroundSubdivisions.Length-1);
        BackgroundSubdivisions[subdivisionIndex].Flash((1f/BeatManager.ticksPerBeat) / (BeatManager.bpm / 60f)); //This time length ensures that subdivisons adjacent to the downbeats finish their animation before being activated again.
    }

    /// <summary>
    /// Update the beat background to be subdivided appropriately for a given amount of ticks per beat.
    /// </summary>
    /// <remarks>Does not support fractional tickrates (yet?).</remarks> //TODO?
    public void SetSubdivisionsAmount(int ticksPerBeat)
    {
        int amountOfSubdivsNeeded = (int)ticksPerBeat + 1;
    }

    public void OnNewTick()
    {
        FlashBeatSubdivision((int)(BeatManager.GetTickTime()));
    }
}
