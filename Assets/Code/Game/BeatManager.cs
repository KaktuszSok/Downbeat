using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BeatManager
{
    static bool initialised = false;
    public static AudioSource musicSource;

    //Song info
    public static float bpm = 128;
    public static float offset = 0.066f;
    static int samplesPerSecond = 1;
    static double secondsPerSample = 1f;
    
    //Timing info
    public static float ticksPerBeat = 2;

    //Runtime
    static long musicSampleTicker;
    public static double musicTime { get { return (float)(musicSampleTicker * secondsPerSample) - offset; } }
    static long prevReportedMusicSample = 0;
    static long totalTickerError = 0;
    static long totalTickerResyncs = 0;
    static double prevBeatTime; //beat time previous frame
    static int prevTick;
    static double nextTickWindowStart = 0f;

    //Callbacks
    public delegate void TimingEventHandler();
    public static TimingEventHandler OnBeat; //Exactly in time with music
    public static TimingEventHandler OnTick; //Exactly in time with music
    public static TimingEventHandler OnNewTickWindow; //Whenever inputs for next tick are available again (slightly before OnTick)

    public delegate void TimingChangedHandler();
    public static TimingChangedHandler OnTimingChanged;

    public static void Initialise()
    {
        if (initialised) return;

        OnNewTickWindow += OnNewTickWindowStarted;

        initialised = true;
    }

    //TODO: Make this load a song
    public static void LoadSong()
    {
        musicSource = LevelManager.instance.GetComponent<AudioSource>();
        samplesPerSecond = musicSource.clip.frequency;
        secondsPerSample = 1.0d / samplesPerSecond;
        musicSource.Play();
        prevBeatTime = 0f;
        prevTick = 0;
        nextTickWindowStart = GetNextTickWindowTime(0);
    }

    //Called by LevelManager
    public static void Update()
    {
        if (musicSource.isPlaying)
        {
            if(musicSource.timeSamples > prevReportedMusicSample)
            {
                /*DEBUG
                long error = musicSampleTicker - musicSource.timeSamples;
                totalTickerError += error;
                totalTickerResyncs++;
                if(totalTickerResyncs >= 1000)
                {
                    totalTickerError = 0; totalTickerResyncs = 0;
                }
                Debug.Log(musicSampleTicker + " vs. real " + musicSource.timeSamples + " (diff. " + error + " samples ), avg. is " + (totalTickerError/(double)totalTickerResyncs) + " over " + totalTickerResyncs + " resyncs");
                if(error < 0)
                {
                    BeatDisplay.instance.AddInputMarker(LevelManager.ActionResult.IGNORED, (float)musicTime); //DEBUG
                }
                else
                {
                    BeatDisplay.instance.AddInputMarker(LevelManager.ActionResult.SUCCESS, (float)musicTime); //DEBUG
                }*/
                musicSampleTicker = prevReportedMusicSample = musicSource.timeSamples;
            }
            else
            {
                musicSampleTicker += (int)(Time.deltaTime * samplesPerSecond);
            }

            double beatTime = GetBeatTime();
            double tickTime = GetTickTime();

            if (prevBeatTime < (int)beatTime) //New beat
            {
                OnBeat?.Invoke();
                prevBeatTime = (int)beatTime;
            }
            if (prevTick < (int)tickTime)
            {
                OnTick?.Invoke();
                prevTick = (int)(beatTime * ticksPerBeat);
            }

            if (musicTime >= nextTickWindowStart)
            {
                OnNewTickWindow.Invoke();
                nextTickWindowStart = GetNextTickWindowTime(musicTime);
            }
        }
    }

    static void OnNewTickWindowStarted()
    {

    }

    //On-Beat windows last two "window lengths", Off-Beat windows last one. The middle of the window is when the tick actually happens.
    //Visually, the beats happen at the edges of the display bar (since the indicator bounces off) but the off-beats in the middle of their subdivision.
    static double GetNextTickWindowTime(double time)
    {
        //All time is measured in beats here, everything is adjusted to fit with the start of each window's timing rather than the actual beats and ticks.
        double windowLength = 1/(ticksPerBeat + 1); //window length as fraction of a beat = 1 / total windowlengths in a beat.
        double currentBeatAdjustedForWindow = (GetBeatTime(time) + windowLength); //Beat window starts one standard (short) window length before the beat. So our effective beat time is a window length ahead of the song.
        double beatFraction = currentBeatAdjustedForWindow % 1f; //What fraction of a beat has elapsed since the start of the current beat window.

        int tick = (int)((int)currentBeatAdjustedForWindow * ticksPerBeat);
        if(beatFraction >= windowLength*2f) //We're no longer in the on-beat hit window
        {
            tick += (int)(beatFraction / windowLength) - 1; //the -1 accounts for the extra window length the on-beat takes up
        }
        //We now know which tick's window we are in.

        return TicksToSeconds(tick) + BeatsToSeconds(tick%ticksPerBeat==0? windowLength : 0.5f*windowLength); //Return the tick's time (midpoint of window) plus half the tick's window length.
    }

    public static double GetBeatTime()
    {
        return GetBeatTime(musicTime);
    }
    public static double GetBeatTime(double time)
    {
        return time * (bpm / 60f); //beat = adjusted time (in seconds) * beats per second
    }

    public static double GetTickTime()
    {
        return GetTickTime(musicTime);
    }
    public static double GetTickTime(double time)
    {
        return GetBeatTime(time) * ticksPerBeat;
    }

    public static double BeatsToSeconds(double beats)
    {
        return beats * (60f/bpm); //beats * seconds per beat
    }
    public static double SecondsToBeats(double seconds)
    {
        return seconds * (bpm/60f); //seconds * beats per second
    }

    public static double TicksToSeconds(double ticks)
    {
        return (ticks / ticksPerBeat) * (60f/bpm); //ticks/ticksPerBeat * secondsPerBeat
    }
    public static double SecondsToTicks(double seconds)
    {
        return seconds * (bpm/60f) * ticksPerBeat; //seconds * beats per second * ticks per beat
    }
}
