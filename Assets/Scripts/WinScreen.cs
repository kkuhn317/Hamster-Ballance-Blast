using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public Text timeTextBox;
    public bool customLevelMode = false;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        if (!customLevelMode) {
            string timeText = "Time: " + globalVariables.timeElapsed.ToString(@"mm\:ss") + "." + globalVariables.timeElapsed.Milliseconds.ToString("D3");
            foreach (TimeSpan levelTimer in globalVariables.levelTimes) {
                timeText += ("\nLevel " + (globalVariables.levelTimes.IndexOf(levelTimer) + 1) + ": " + levelTimer.ToString(@"mm\:ss") + "." + levelTimer.Milliseconds.ToString("D3"));
            }
            timeTextBox.text = timeText;

            GameObject[] timers = GameObject.FindGameObjectsWithTag("Timer");
            if (timers.Length > 0) {
                foreach (GameObject timer in timers) {
                    Destroy(timer.transform.parent.gameObject);
                }
            }

        } else {
            timeTextBox.text = ("Time: " + ((TimeSpan)globalVariables.levelTimes[0]).ToString(@"mm\:ss") + "." + ((TimeSpan)globalVariables.levelTimes[0]).Milliseconds.ToString("D3"));
        }
    }
    
    public void onRestart() {
        globalVariables.levelTimes.Clear();
        SceneManager.LoadScene("Custom Level");
    }

    public void onBack() {
        SceneManager.LoadScene(0);
    }

}
