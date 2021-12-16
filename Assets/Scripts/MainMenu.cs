using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public InputField levelNameInputField;
    public InputField creatorNameInputField;

    public Slider volumeSlider;

    public Slider sfxSlider;

    public Text VersionText;

    public ErrorWindowHandler errorWindowHandler;

    void Start() {

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        GameObject[] timers = GameObject.FindGameObjectsWithTag("Timer");
        foreach (GameObject timer in timers) {
            Destroy(timer.transform.parent.gameObject);
        }

        GameObject[] levelTimers = GameObject.FindGameObjectsWithTag("levelTimer");
        foreach (GameObject timer in levelTimers) {
            Destroy(timer.transform.parent.gameObject);
        }

        globalVariables.levelTimes.Clear();

        volumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        sfxSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);

        VersionText.text = "Version " + Application.version;
    }

    public void onQuit() {
        Application.Quit();
    }

    public void onNormal() {
        globalVariables.levelScale = 1;
        globalVariables.oneLifeMode = false;
        SceneManager.LoadScene(1);
    }

    public void onOneLife() {
        globalVariables.levelScale = 1;
        globalVariables.oneLifeMode = true;
        SceneManager.LoadScene(1);
    }

    public void OpenFileExplorer()
    {
        string A = Application.persistentDataPath + "/levels";
        A = A.Replace("/",@"\");
        System.Diagnostics.Process.Start(A);
    }

    public void showErrorBox(string message) {
        errorWindowHandler.ShowWindow(message);
    }


    public void onCreateLevel() {
        string levelsFolder = Application.persistentDataPath + "/levels";
        if (!Directory.Exists(levelsFolder))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/levels");
            }

        string filePath;
        if (levelNameInputField.text == "")
            filePath = "UntitledLevel";
        else
            filePath = levelNameInputField.text;

        // Make sure the new level has a unique filename
        int duplicateNumber = 0;
        string path = levelsFolder + "/" + filePath + ".txt";

        while (File.Exists(path)) {
            duplicateNumber ++;
            filePath = levelNameInputField.text + "(" + duplicateNumber + ")";
            path = levelsFolder + "/" + filePath + ".txt";
        }
        
        globalVariables.filePath = filePath;

        // Write default level data to the file
        StreamWriter writer = new StreamWriter(path, false);

        // header
        writer.WriteLine(levelNameInputField.text);
        writer.WriteLine(creatorNameInputField.text);
        writer.WriteLine(Application.version);

        // level specific settings
        writer.WriteLine("0");
        writer.WriteLine("0");
        writer.WriteLine("0");
        writer.WriteLine("1");

        // Hamsterball style is default
        writer.WriteLine("#FFFFFF");
        writer.WriteLine("#FFFFFF");
        writer.WriteLine("#FFFFFF");
        writer.WriteLine("#FFFFFF");
        writer.WriteLine("0");

        writer.Close();
        SceneManager.LoadScene("Level Editor");
    }
}
