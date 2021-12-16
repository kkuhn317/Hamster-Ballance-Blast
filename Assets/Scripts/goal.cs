using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class goal : MonoBehaviour
{

    private void OnCollisionEnter(Collision other) {
        if (other.gameObject.tag == "Player") {
            nextLevel();
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            nextLevel();
        }
    }

    private void nextLevel() {
        Time.timeScale = 1;

        // Reset Checkpoints
        globalVariables.checkpointLoc = new Vector3(0,0,0);
        globalVariables.checkpointNum = -1;
        globalVariables.savedBallType = -1;

        GameObject[] timers = GameObject.FindGameObjectsWithTag("Timer");
        foreach (GameObject timer in timers) {
            timer.GetComponent<timer>().timeFinish();    // stop the main timer temporarily
        }

        GameObject[] levelTimers = GameObject.FindGameObjectsWithTag("levelTimer");
        foreach (GameObject timer in levelTimers) {
            globalVariables.levelTimes.Add(timer.GetComponent<timer>().timeFinish());
            Destroy(timer.transform.parent.gameObject); // In case the timer is set with Don't destroy on load
        }

        // Delete all music and ambience objects so that they don't play in the next level
        GameObject[] musics = GameObject.FindGameObjectsWithTag ("musicPlayer");
        foreach(GameObject music in musics) {
            Destroy(music);
        }

        GameObject[] ambiences = GameObject.FindGameObjectsWithTag ("ambiencePlayer");
        foreach(GameObject ambience in ambiences) {
            Destroy(ambience);
        }



        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex + 1);
    }
}
