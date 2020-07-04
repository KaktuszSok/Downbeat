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
    static long musicSampleTicker; //mean error when tested ~-238 samples (falling behind) per resync (resyncs at around 60Hz as this is how often AudioSource updates elapsed time & samples)
    public static double musicTime { get { return (float)(musicSampleTicker * secondsPerSample) - offset; } }
    static long prevReportedMusicSample = 0;
    static double prevBeatTime; //beat time previous frame
    static int prevTick;
    static int prevTickWindow;

    //Callbacks
    public delegate void TimingEventHandler();
    public static TimingEventHandler OnBeat; //Exactly in time with music
    public static TimingEventHandler OnTick; //Exactly in time with music
    public delegate void WindowEventHandler(int currWindow);
    public static WindowEventHandler OnNewTickWindow; //Whenever inputs for next tick are available again (slightly before OnTick)

    public delegate void TimingChangedHandler();
    public static TimingChangedHandler OnBPMChanged;
    public static TimingChangedHandler OnTickrateChanged;

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

        OnBPMChanged?.Invoke();
        OnTickrateChanged?.Invoke();
        musicSource.Play();

        musicSampleTicker = 0;
        prevReportedMusicSample = 0;
        prevBeatTime = 0f;
        prevTick = 0;
        prevTickWindow = 0;
    }

    //Called by LevelManager
    public static void Update()
    {
        //DEBUG
        if (Input.GetKeyDown(KeyCode.X))
        {
            OnTickrateChanged?.Invoke();
        }

        if (musicSource.isPlaying)
        {
            if(musicSource.timeSamples > prevReportedMusicSample)
            {
                musicSampleTicker = prevReportedMusicSample = musicSource.timeSamples;
            }
            else
            {
                musicSampleTicker += (int)(Time.deltaTime * musicSource.pitch * samplesPerSecond);
            }

            double beatTime = GetBeatTime();
            double tickTime = GetTickTime();
            int currTickWindow = GetActiveTickWindow(musicTime);

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

            if (currTickWindow > prevTickWindow)
            {
                OnNewTickWindow.Invoke(currTickWindow);
                prevTickWindow = currTickWindow;
            }
        }
    }

    static void OnNewTickWindowStarted(int tick)
    {

    }


    //WARNING - CLUSTERFUCK!!!
    //On-Beat windows last two "window lengths", Off-Beat windows last one. The middle of the window is when the tick actually happens.
    //Visually, the beats happen at the edges of the display bar (since the indicator bounces off) but the off-beats in the middle of their subdivision.
    static int GetActiveTickWindow(double time)
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
        return tick;
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
