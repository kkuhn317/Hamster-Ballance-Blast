using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CustomLevelPlayer : MonoBehaviour
{

    public GameObject pauseMenu;
    GameObject mainBall;
    public GameObject whiteFade;

    public GameObject MBOOB;
    public GameObject HBStartCutscene;
    public GameObject lightningBall;
    private GameObject lightningBallClone;
    public GameObject MBStartCutscene;
    private GameObject[] timers;

    private AudioSource audioSource;

    bool inStartingCutscene = false;

    bool isFallingOff = false;

    // Start is called before the first frame update
    void Start()
    {
        timers = GameObject.FindGameObjectsWithTag("levelTimer");
        audioSource = GetComponent<AudioSource>();
    }

    public void OnLoadingDone() {
        mainBall = GameObject.FindGameObjectWithTag("Player");
        bool ballanceCheck = false;
        
        if (globalVariables.checkpointNum != -1)
            ballanceCheck = GameObject.FindGameObjectsWithTag("Checkpoint")[globalVariables.checkpointNum].GetComponent<ballanceCheckpoint>();

        if (ballanceCheck) {
            BStart();
        }

        if (globalVariables.checkpointNum == -1) {
            // Starting Cutscene
            // No input at start until starting cutscene is done
            if (mainBall) {
                if (mainBall.GetComponent<ballanceRoll>())
                    mainBall.GetComponent<ballanceRoll>().stopInput = true;
                else
                    mainBall.GetComponent<marbleRoll>().stopInput = true;
            }

            inStartingCutscene = true;
            switch (globalVariables.gameStyle) {
                case 0:
                    HBStart();
                    break;
                case 1:
                    audioSource.Play();
                    BStart();
                    break;
                case 2:
                    MBStart();
                    break;
            }
            
        } else {
            // Checkpoint cutscene if needed
            
        }
    }

    void HBStart() {
        GameObject cutscene = Instantiate(HBStartCutscene);

        Destroy(cutscene, 4);
        Invoke("cutsceneDone", 4);
    }

    void BStart() {
        mainBall.SetActive(false);
        lightningBallClone = Instantiate(lightningBall, mainBall.transform.position, Quaternion.identity);
        Invoke("spawnLightningBall", .5f);
        Destroy(lightningBallClone, 4f);
        Invoke("reactivateBall", 3f);
        Invoke("cutsceneDone", 3f);
    }

    void spawnLightningBall() {
        lightningBallClone.GetComponent<Animator>().SetTrigger("Grow");
    }

    void reactivateBall() {
        mainBall.SetActive(true);
        //mainBall.transform.position = new Vector3(0, 0.5f, 0);
    }


    void MBStart() {
        GameObject cutscene = Instantiate(MBStartCutscene);

        Destroy(cutscene, 5);
        Invoke("cutsceneDone", 3.5f);
    }

    void cutsceneDone() {
        getNewTimers();
        inStartingCutscene = false;
        foreach(GameObject timer in timers) {
            timer.GetComponent<timer>().timeResume();
        }
        if (mainBall.GetComponent<ballanceRoll>())
            mainBall.GetComponent<ballanceRoll>().stopInput = false;
        else
            mainBall.GetComponent<marbleRoll>().stopInput = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
            reloadCheckpoint();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            if (pauseMenu.activeSelf)
                onResumeButton();
            else
                pauseGame();
        }

        var objects = GameObject.FindGameObjectsWithTag("Player");
        var objectCount = objects.Length;
        foreach (var obj in objects) {
            if (obj.transform.position.y < globalVariables.respawnHeight) {
                //reloadCheckpoint();
                onFallOffLevel();
            }
        }
        
    }

    public void pauseGame() {
        if (!isFallingOff) {
            getNewTimers();
            foreach (GameObject timer in timers) {
                timer.GetComponent<timer>().timeFinish();
            }
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            Time.timeScale = 0;
            pauseMenu.SetActive(true);
        }
    }

    public void getNewTimers() {
        timers = GameObject.FindGameObjectsWithTag("levelTimer");
    }

    public void onResumeButton() {
        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;
        pauseMenu.SetActive(false);
        if (!inStartingCutscene) {
            foreach (GameObject timer in timers) {
                timer.GetComponent<timer>().timeResume();
            }
        }
    }

    public void onEditButton() {
        stopMusic();
        resetCheckpoints();
        Time.timeScale = 1;
        SceneManager.LoadScene("Level Editor");
    }

    public void onQuitButton() {
        stopMusic();
        resetCheckpoints();
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }

    public void reloadLevelButton()
    {
        Time.timeScale = 1;
        resetCheckpoints();
        // Get rid of timers so they don't carry over
        foreach (GameObject timer in timers) {
            Destroy(timer.transform.parent.gameObject);
        }
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);
    }

    void reloadCheckpoint()
    {
        Time.timeScale = 1;
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);
    }

    void resetCheckpoints() {
        globalVariables.checkpointLoc = new Vector3(0,0,0);
        globalVariables.checkpointNum = -1;
        globalVariables.savedBallType = -1;
    }


    void stopMusic() {
        // Delete all music and ambience objects so that they don't play in the next scene
        GameObject[] musics = GameObject.FindGameObjectsWithTag ("musicPlayer");
        foreach(GameObject music in musics) {
            Destroy(music);
        }

        GameObject[] ambiences = GameObject.FindGameObjectsWithTag ("ambiencePlayer");
        foreach(GameObject ambience in ambiences) {
            Destroy(ambience);
        }
    }

    public void onFallOffLevel() {
        if (!isFallingOff) {
            isFallingOff = true;
            switch (globalVariables.gameStyle) {
                case 0:
                    if (Camera.main.GetComponent<ballanceCamera>()) {
                        Camera.main.GetComponent<ballanceCamera>().paused = true;
                    } else {
                        Camera.main.GetComponent<marbleCam>().target = null;
                    }
                    Invoke("reloadCheckpoint", 1);
                    break;
                case 1:
                    GameObject whiteFadeCopy = Instantiate(whiteFade).transform.GetChild(0).gameObject;
                    whiteFadeCopy.GetComponent<RectTransform>().offsetMax = new Vector2(0,0);
                    whiteFadeCopy.GetComponent<RectTransform>().offsetMin = new Vector2(0,0);
                    whiteFadeCopy.GetComponent<fadeToWhite>().autoFadeInAndOut();
                    Invoke("reloadCheckpoint", 1);
                    break;
                case 2:
                    Instantiate(MBOOB);
                    if (Camera.main.GetComponent<ballanceCamera>()) {
                        Camera.main.GetComponent<ballanceCamera>().lookOnlyMode = true;
                    } else {
                        Camera.main.GetComponent<marbleCam>().lookOnlyMode = true;
                    }
                    Invoke("reloadCheckpoint", 2);
                    break;
            }
        }
    }

    


}
