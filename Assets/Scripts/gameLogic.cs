// DEPRECIATED: will use CustomLevelPlayer instead!


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class gameLogic : MonoBehaviour
{
    
    public int respawnHeight;
    public GameObject whiteFade;
    public GameObject MBOOB;
    public GameObject HBStartCutscene;
    public GameObject lightningBall;
    private GameObject mainBall;
    private GameObject lightningBallClone;
    public GameObject MBStartCutscene;
    private GameObject[] levelTimers;
    private GameObject[] timers;
    private AudioSource audioSource;
    public bool isFallingOff;

    public int gameStyle;
    public int cameraStyle;

    // Start is called before the first frame update
    void Start()
    {

        audioSource = GetComponent<AudioSource>();

        // lock the cursor
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = false;

        GameObject[] timers = GameObject.FindGameObjectsWithTag("Timer");

        foreach (GameObject timer in timers) {
            timer.GetComponent<timer>().timeFinish();   // pause the main timer
        }

        mainBall = GameObject.FindGameObjectWithTag("Player");

        // Starting Cutscene
        // No input at start until starting cutscene is done
        if (mainBall) {
            if (mainBall.GetComponent<ballanceRoll>())
                mainBall.GetComponent<ballanceRoll>().stopInput = true;
            else
                mainBall.GetComponent<marbleRoll>().stopInput = true;
        }

        levelTimers = GameObject.FindGameObjectsWithTag("levelTimer");
        foreach(GameObject timer in levelTimers) {
            timer.GetComponent<timer>().timeFinish();
            timer.GetComponent<timer>().timeReset();
        }

        switch (gameStyle) {
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

        globalVariables.gameStyle = gameStyle;
        globalVariables.cameraStyle = cameraStyle;
            

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
    }


    void MBStart() {
        GameObject cutscene = Instantiate(MBStartCutscene);

        Destroy(cutscene, 5);
        Invoke("cutsceneDone", 3.5f);
    }

    void cutsceneDone() {
        levelTimers = GameObject.FindGameObjectsWithTag("levelTimer");
        foreach(GameObject timer in levelTimers) {
            timer.GetComponent<timer>().timeResume();
        }

        GameObject[] timers = GameObject.FindGameObjectsWithTag("Timer");
        foreach (GameObject timer in timers) {
            timer.GetComponent<timer>().timeResume();   // resume the main timer
        }

        if (mainBall.GetComponent<ballanceRoll>())
            mainBall.GetComponent<ballanceRoll>().stopInput = false;
        else
            mainBall.GetComponent<marbleRoll>().stopInput = false;
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            restart();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            backToMainMenu();
        }
        var objects = GameObject.FindGameObjectsWithTag("Player");
        var objectCount = objects.Length;
        foreach (var obj in objects) {
            if (obj.transform.position.y < respawnHeight) {
                onFallOffLevel();
                break;
            }
        }
        
    }

    void reloadLevel()
    {
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);
    }

    void quitGame() {
        Application.Quit();
    }

    void backToMainMenu() {
        stopMusic();
        SceneManager.LoadScene(0);
    }

    void backToLevelOne() {
        stopMusic();
        globalVariables.levelTimes.Clear();
        SceneManager.LoadScene(1);
    }

    void restart() {
        if (globalVariables.oneLifeMode) {
            backToLevelOne();
        } else {
            reloadLevel();
        }
    }


    public void onFallOffLevel() {
        if (!isFallingOff) {
            isFallingOff = true;
            switch (gameStyle) {
                case 0:
                    if (Camera.main.GetComponent<ballanceCamera>()) {
                        Camera.main.GetComponent<ballanceCamera>().paused = true;
                    } else {
                        Camera.main.GetComponent<marbleCam>().target = null;
                    }
                    Invoke("restart", 1);
                    break;
                case 1:
                    GameObject whiteFadeCopy = Instantiate(whiteFade).transform.GetChild(0).gameObject;
                    whiteFadeCopy.GetComponent<RectTransform>().offsetMax = new Vector2(0,0);
                    whiteFadeCopy.GetComponent<RectTransform>().offsetMin = new Vector2(0,0);
                    whiteFadeCopy.GetComponent<fadeToWhite>().autoFadeInAndOut();
                    Invoke("restart", 1);
                    break;
                case 2:
                    Instantiate(MBOOB);
                    if (Camera.main.GetComponent<ballanceCamera>()) {
                        Camera.main.GetComponent<ballanceCamera>().lookOnlyMode = true;
                    } else {
                        Camera.main.GetComponent<marbleCam>().lookOnlyMode = true;
                    }
                    Invoke("restart", 2);
                    break;
            }
        }
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
}
