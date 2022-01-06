using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class pauseMenu : MonoBehaviour
{
    public GameObject menu;
    public CustomLevelPlayer levelPlayer;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (menu.activeSelf)
                onResumeButton();
            else
                pauseGame();
        }
    }

    public void pauseGame() {
        if (levelPlayer.isFallingOff)
            return;
        levelPlayer.getNewTimers();
        foreach (GameObject timer in levelPlayer.timers) {
            timer.GetComponent<timer>().timeFinish();
        }
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 0;
        menu.SetActive(true);
    }

    public void onResumeButton() {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        menu.SetActive(false);
        if (!levelPlayer.inStartingCutscene) {
            foreach (GameObject timer in levelPlayer.timers) {
                timer.GetComponent<timer>().timeResume();
            }
        }
    }

    public void onEditButton() {
        levelPlayer.stopMusic();
        levelPlayer.resetCheckpoints();
        Time.timeScale = 1;
        SceneManager.LoadScene("Level Editor");
    }

    public void onQuitButton() {
        levelPlayer.stopMusic();
        levelPlayer.resetCheckpoints();
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void reloadLevelButton()
    {
        Time.timeScale = 1;
        levelPlayer.resetCheckpoints();
        // Get rid of timers so they don't carry over
        foreach (GameObject timer in levelPlayer.timers) {
            Destroy(timer.transform.parent.gameObject);
        }
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);
    }
}
