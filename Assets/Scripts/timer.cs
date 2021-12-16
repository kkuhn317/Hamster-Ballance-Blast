using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using System.Diagnostics;
using UnityEngine.SceneManagement;

public class timer : MonoBehaviour
{
    private Stopwatch stopwatch;
    public bool levelTimer = false;
    public TimeSpan timeElapsed { get; private set; }

    public int timeTravelsActive = 0;

    public bool playAtStart = true;

    private void Start()
    {
        stopwatch = new Stopwatch();
        if (!levelTimer) {
            removeOtherTimers();
            DontDestroyOnLoad(gameObject.transform.parent.gameObject);
        } else if (GameObject.FindGameObjectsWithTag("levelTimer").Length > 1) {
            // Destroy this if there's already a leveltimer
            Destroy(transform.parent.gameObject);
        }
        if (playAtStart)
            stopwatch.Start();
    }

    private void Update()
    {
        timeElapsed = stopwatch.Elapsed;
        if (!levelTimer) {
            globalVariables.timeElapsed = this.timeElapsed;
        }
        gameObject.GetComponent<Text>().text = getTime();

        // If you've already hit a checkpoint then make sure the timer doesn't reset
        if (globalVariables.checkpointNum != -1) {
            DontDestroyOnLoad(transform.parent.gameObject);
        }
    }

    public String getTime() {
        return timeElapsed.ToString(@"mm\:ss") + "." + timeElapsed.Milliseconds.ToString("D3");
    }

    public TimeSpan timeFinish() {
        if (stopwatch != null) {
            stopwatch.Stop();
            timeElapsed = stopwatch.Elapsed;
        }
        return timeElapsed;
    }

    public void timeResume() {
        if (stopwatch != null && timeTravelsActive == 0)
            stopwatch.Start();
    }

    public void timeTravelActivate() {
        // time travels are stacked, so after 1 completes the next one will start

        timeTravelsActive++;
        if (timeTravelsActive == 1) {
            // start the sound effect
            GetComponent<AudioSource>().Play();
            // stop the timer
            timeFinish();
            // resume the timer or use up one time travel
            Invoke("timeTravelDeactivate", 5);
        }

    }

    public void timeTravelDeactivate() {
        timeTravelsActive--;
        if (timeTravelsActive == 0) {
            // stop the sound and resume the timer
            GetComponent<AudioSource>().Stop();
            timeResume();
        }
        else
            Invoke("timeTravelDeactivate", 5);  // use the next time travel
    }



    public void timeReset() {
        if (stopwatch != null)
            stopwatch.Reset();
    }

    private void removeSelfIfOtherTimer() {
        GameObject[] timers = GameObject.FindGameObjectsWithTag("Timer");
        if (timers.Length > 1)
        {
            Destroy(gameObject.transform.parent.gameObject);
        }

    }

    private void removeOtherTimers() {
        GameObject[] timers = GameObject.FindGameObjectsWithTag("Timer");
        foreach (GameObject timer in timers) {
            if (timer != gameObject)
                Destroy(timer.transform.parent.gameObject);
        }
    }

}