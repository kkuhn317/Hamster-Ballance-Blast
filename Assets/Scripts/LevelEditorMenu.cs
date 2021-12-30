using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelEditorMenu : MonoBehaviour
{
    public EditorLogic editorLogic;
    public GameObject SavingPopUp;
    public GameObject HamsterballMenu;
    public GameObject BallanceMenu;
    public GameObject MarbleBlastMenu;

    // Start is called before the first frame update
    void Start()
    {
        // Apply values to menu options
        levelName.text = globalVariables.levelName;
        levelCreator.text = globalVariables.levelCreator;
        levelStyle.value = globalVariables.gameStyle;
        levelStartBall.value = globalVariables.startingBall;
        levelCameraStyle.value = globalVariables.cameraStyle;
        extraColor.text = "#" + ColorUtility.ToHtmlStringRGB(globalVariables.extraColor);
        if (globalVariables.deathPlaneHeightActive)
            deathPlaneHeight.text = globalVariables.deathPlaneHeight.ToString();
        levelScale.text = globalVariables.levelScale.ToString();
        
        switch (globalVariables.gameStyle) {
            case 0:
                backgroundColor.text = "#" + ColorUtility.ToHtmlStringRGB(globalVariables.backgroundColor);
                floorColor1.text = "#" + ColorUtility.ToHtmlStringRGB(globalVariables.floorColor1);
                floorColor2.text = "#" + ColorUtility.ToHtmlStringRGB(globalVariables.floorColor2);
                wallColor.text = "#" + ColorUtility.ToHtmlStringRGB(globalVariables.wallColor);
                hbMusic.value = globalVariables.song;
                break;
            case 1:
                skybox.value = globalVariables.skybox;
                bMusic.value = globalVariables.song;
                break;
            case 2:
                tileColor.value = globalVariables.floorTexture;
                mbMusic.value = globalVariables.song;
                break;
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    [Header("Main Menu Variables")]
    public InputField levelName;
    public InputField levelCreator;
    public Dropdown levelStyle;
    public Dropdown levelStartBall;
    public Dropdown levelCameraStyle;
    public InputField extraColor;
    public InputField deathPlaneHeight;
    public InputField levelScale;

    [Header("Hamsterball Menu Variables")]

    public InputField backgroundColor;
    public InputField floorColor1;
    public InputField floorColor2;
    public InputField wallColor;
    public Dropdown hbMusic;

    [Header("Ballance Menu Variables")]
    public Dropdown skybox;
    public Dropdown bMusic;

    [Header("Marble Blast Menu Variables")]
    public Dropdown tileColor;
    public Dropdown mbMusic;

    // Main Menu
    public void onLevelNameChange() {
        globalVariables.levelName = levelName.text;
    }
    public void onLevelCreatorChange() {
        globalVariables.levelCreator = levelCreator.text;
    }

    public void onGameStyleChange() {
        globalVariables.gameStyle = levelStyle.value;
        editorLogic.updateStartPlatform();
        editorLogic.updateSkybox();
        editorLogic.updateLighting();
        editorLogic.updateTextures();
    }

    public void onCustomize() {
        switch (globalVariables.gameStyle) {
            case 0:
                HamsterballMenu.SetActive(true);
                break;
            case 1:
                BallanceMenu.SetActive(true);
                break;
            case 2:
                MarbleBlastMenu.SetActive(true);
                break;
        }
    }
    public void onStartBallChange() {
        globalVariables.startingBall = levelStartBall.value;
    }

    public void onCameraStyleChange() {
        globalVariables.cameraStyle = levelCameraStyle.value;
    }

    public void onExtraColorChange() {
        ColorUtility.TryParseHtmlString(extraColor.text, out globalVariables.extraColor);
        editorLogic.updateTextures();
    }

    public void onDeathPlaneHeightChange() {
        try {
            globalVariables.deathPlaneHeight = decimal.Parse(deathPlaneHeight.text);
            globalVariables.deathPlaneHeightActive = true;
        } catch {
            globalVariables.deathPlaneHeightActive = false;
        }
    }

    public void onLevelScaleChange() {
        try {
            globalVariables.levelScale = decimal.Parse(levelScale.text);
        } catch {
            globalVariables.levelScale = 1;
        }
    }

    public void onSave() {
        // save the level, easy peasy right????
        // no, not really lol

        //Debug.Log(Application.persistentDataPath);
        // For me it's C:\Users\matt\AppData\LocalLow\BookwormKevin\Hamster Ballance Blast

        string levelsFolder = Application.persistentDataPath + "/levels";
        if (!Directory.Exists(levelsFolder))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/levels");
            }

        // This should never be the case, but is used for testing the editor with no level open
        if (String.IsNullOrEmpty(globalVariables.filePath)) {
            globalVariables.filePath = "thisfileshouldntexist";
        }

        //SavingPopUp.SetActive(true);

        try {
            string path = levelsFolder + "/" + globalVariables.filePath + ".txt";

            // Write the level data to the file
            StreamWriter writer = new StreamWriter(path, false);

            // header
            writer.WriteLine(globalVariables.levelName);
            writer.WriteLine(globalVariables.levelCreator);
            writer.WriteLine(Application.version);

            // level specific settings
            writer.WriteLine(globalVariables.gameStyle);
            writer.WriteLine(globalVariables.startingBall);
            writer.WriteLine(globalVariables.cameraStyle);
            writer.WriteLine("#" + ColorUtility.ToHtmlStringRGB(globalVariables.extraColor));
            if (globalVariables.deathPlaneHeightActive)
                writer.WriteLine("*dph " + globalVariables.deathPlaneHeight);
            writer.WriteLine(globalVariables.levelScale);

            switch (globalVariables.gameStyle) {
                case 0: // Hamsterball settings
                    writer.WriteLine("#" + ColorUtility.ToHtmlStringRGB(globalVariables.backgroundColor));
                    writer.WriteLine("#" + ColorUtility.ToHtmlStringRGB(globalVariables.floorColor1));
                    writer.WriteLine("#" + ColorUtility.ToHtmlStringRGB(globalVariables.floorColor2));
                    writer.WriteLine("#" + ColorUtility.ToHtmlStringRGB(globalVariables.wallColor));
                    break;
                case 1: // Ballance settings
                    writer.WriteLine(globalVariables.skybox);
                    break;
                case 2: // Marble Blast settings
                    writer.WriteLine(globalVariables.floorTexture);
                    break;
            }
            updateSongValue();  // fixes a song bug
            writer.WriteLine(globalVariables.song); // This applies for all 3 game styles

            // Big ground blocks
            editorLogic.saveBigBlocks(writer);

            // Individual objects
            editorLogic.saveObjects(writer);

            writer.Close();
            //SavingPopUp.SetActive(false);
        } catch {
            print("ERROR!!!! AAAAA");
            //SavingPopUp.SetActive(false);
        }
    }

    public void updateSongValue() {
        switch (globalVariables.gameStyle) {
            case 0:
                globalVariables.song = hbMusic.value;
                break;
            case 1:
                globalVariables.song = bMusic.value;
                break;
            case 2:
                globalVariables.song = mbMusic.value;
                break;
        }
    }

    public void onSaveAndTest() {
        onSave();
        stopEditor();
        SceneManager.LoadScene("Custom Level");

    }

    public void stopEditor() {
        // disable the editor gameobject so that it doesn't do random stuff while scene is loading
        // basically this is for the teleporter handler
        editorLogic.gameObject.SetActive(false);
    }

    // Hamsterball customize menu

    public void onBGColorChange() {
        ColorUtility.TryParseHtmlString(backgroundColor.text, out globalVariables.backgroundColor);
        editorLogic.updateSkybox();
    }

    public void onFloorColor1Change() {
        ColorUtility.TryParseHtmlString(floorColor1.text, out globalVariables.floorColor1);
        editorLogic.updateTextures();
    }

    public void onFloorColor2Change() {
        ColorUtility.TryParseHtmlString(floorColor2.text, out globalVariables.floorColor2);
        editorLogic.updateTextures();
    }

    public void onWallColorChange() {
        ColorUtility.TryParseHtmlString(wallColor.text, out globalVariables.wallColor);
        editorLogic.updateTextures();
    }

    public void onHBMusicChange() {
        globalVariables.song = hbMusic.value;
    }

    // Ballance Customize menu

    public void onSkyboxChange() {
        globalVariables.skybox = skybox.value;
        editorLogic.updateSkybox();
    }

    public void onBallanceMusicChange() {
        globalVariables.song = bMusic.value;
    }

    // Marble Blast Customize menu

    public void onTileColorChange() {
        globalVariables.floorTexture = tileColor.value;
        editorLogic.updateTextures();
    }

    public void onMBMusicChange() {
        globalVariables.song = mbMusic.value;
    }

    // Are you sure? menu

    public void onExit() {
        stopEditor();
        SceneManager.LoadScene(0);
    }
}
