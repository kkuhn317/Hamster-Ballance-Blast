using System;
using System.Linq;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    public bool editorMode = false;
    public bool mainGameLevel = false;
    public StreamReader customReader;
    public string[] campaignLevelLines;
    public int lineNum = 0;
    private EditorLogic editorLogic;
    public GameObject cam;
    public GameObject normalSongPlayer;
    public GameObject ballanceSongPlayer;
    public GameObject ballanceAmbiencePlayer;
    public GameObject mainBall;

    // These are used for backwards compatibility
    string[,] ObjectIds = {
        {"SteepSlope", "ShallowSlope", "CornerSlope", null, null, null, null, null},
        {"Goal","8Ball", null, null, null, null, null, null},
        {"HBTransform","WoodTransform","StoneTransform","PaperTransform","MarbleTransform","Box","SmallBridge","HalfpipeHole"},
        {"Goal", null, null, null, null, null, null, null}
        };
    

    // Start is called before the first frame update
    void Start()
    {
        // Remove level timers if in the editor
        if (editorMode) {
            GameObject[] levelTimers = GameObject.FindGameObjectsWithTag("levelTimer");
            foreach (GameObject timer in levelTimers) {
                Destroy(timer.transform.parent.gameObject);
            }
        }

        // reset the powerup
        globalVariables.storedPowerup = -1;

        editorLogic = gameObject.GetComponent<EditorLogic>();

        // TIME TO LOAD A CUSTOM LEVEL!!!!!! yeaaaaa

        if (!mainGameLevel) {
            string levelsFolder = Application.persistentDataPath + "/levels";
            string path = levelsFolder + "/" + globalVariables.filePath + ".txt";
            customReader = new StreamReader(path);
        } else {
            TextAsset textFile = (TextAsset)Resources.Load("Campaign/" + globalVariables.filePath);
            campaignLevelLines = textFile.text.Split('\n');

        }

        

        // This first stuff is basically the opposite of the onSave method
        // (in LevelEditorMenu)

        // Read header
        globalVariables.levelName = readNextLine();
        globalVariables.levelCreator = readNextLine();
        Version version = Version.Parse(readNextLine()); // Version number
        
        // level specific settings
        globalVariables.gameStyle = Int32.Parse(readNextLine());
        globalVariables.startingBall = Int32.Parse(readNextLine());
        globalVariables.cameraStyle = Int32.Parse(readNextLine());

        string nextLine = readNextLine();

        // if nextline starts with "#" then it's a color, meaning its the new extra color field
        if (nextLine.StartsWith("#")) {
            ColorUtility.TryParseHtmlString(nextLine, out globalVariables.extraColor);
            nextLine = readNextLine();
        }

        // if nextline starts with "*" then it's a special setting (only deathplaneheight for now)
        globalVariables.deathPlaneHeightActive = false;
        if (nextLine.StartsWith("*")) {
            globalVariables.deathPlaneHeight = decimal.Parse(nextLine.Split(' ')[1]);
            globalVariables.deathPlaneHeightActive = true;
            nextLine = readNextLine();
        }

        globalVariables.levelScale = decimal.Parse(nextLine);
        

        switch (globalVariables.gameStyle) {
            case 0: // Hamsterball settings
                ColorUtility.TryParseHtmlString(readNextLine(), out globalVariables.backgroundColor);
                ColorUtility.TryParseHtmlString(readNextLine(), out globalVariables.floorColor1);
                ColorUtility.TryParseHtmlString(readNextLine(), out globalVariables.floorColor2);
                ColorUtility.TryParseHtmlString(readNextLine(), out globalVariables.wallColor);
                break;
            case 1: // Ballance settings
                globalVariables.skybox = Int32.Parse(readNextLine());
                break;
            case 2: // Marble Blast settings
                globalVariables.floorTexture = Int32.Parse(readNextLine());
                break;
        }
        globalVariables.song = Int32.Parse(readNextLine());

        // now here's where things change

        // load ground objects
        string blockInfo = readNextLine();
        while (blockInfo != null && blockInfo.Count(x => x == ':') == 1) {
            GameObject ground = Instantiate(editorLogic.groundPrefab);
            string[] posAndScale = blockInfo.Split(':');
            string[] pos = posAndScale[0].Split(',');
            string[] scale = posAndScale[1].Split(',');
            ground.transform.position = new Vector3(float.Parse(pos[0]), float.Parse(pos[1]), float.Parse(pos[2]));
            ground.transform.localScale = new Vector3(float.Parse(scale[0]), float.Parse(scale[1]), float.Parse(scale[2]));
            ground.transform.SetParent(editorLogic.groundObjectHolder.transform);
            blockInfo = readNextLine();
        }

        // load normal objects
        UnityEngine.Object[] basicBlocks = Resources.LoadAll("Level Editor Parts/Basic Blocks", typeof(GameObject));
        UnityEngine.Object[] hamsterballObjects = Resources.LoadAll("Level Editor Parts/Hamsterball", typeof(GameObject));
        UnityEngine.Object[] ballanceObjects = Resources.LoadAll("Level Editor Parts/Ballance", typeof(GameObject));
        UnityEngine.Object[] marbleBlastObjects = Resources.LoadAll("Level Editor Parts/Marble Blast", typeof(GameObject));

        while (blockInfo != null && blockInfo.Count(x => x == ':') == 3) {
            // style:id:posx,posy,posz:rotation
            string[] objectData = blockInfo.Split(':');
            GameObject placedObject = null;

            // i am very sorry for the following code

            // Is the id a number? if so, convert it to the new format
            int i = 0;
            bool result = int.TryParse(objectData[1], out i);

            if (result) {
                objectData[1] = ObjectIds[int.Parse(objectData[0]), i];
            }

            switch(int.Parse(objectData[0])) {
                case 0:
                    // yeah yeah, it's hardcoded slopes, whatever
                    if (objectData[1] == "SteepSlope" || objectData[1] == "ShallowSlope" || objectData[1] == "CornerSlope") {
                        placedObject = placeTexturableObject(getGameObjectByName(basicBlocks,objectData[1]), 0);
                    }
                    else {
                        // other texturable objects also get sent here, but they are identified as texturable later
                        // instead of fixing my code, i just make it more convoluted
                        placedObject = placeObject(getGameObjectByName(basicBlocks,objectData[1]), 0);
                    }
                    break;
                case 1:
                    placedObject = placeObject(getGameObjectByName(hamsterballObjects,objectData[1]), 1);
                    break;
                case 2:
                    placedObject = placeObject(getGameObjectByName(ballanceObjects,objectData[1]), 2);
                    break;
                case 3:
                    placedObject = placeObject(getGameObjectByName(marbleBlastObjects,objectData[1]), 3);
                    break;
            }

            // Now set position and rotation
            String[] pos = objectData[2].Split(',');
            Vector3 numPos = new Vector3(Mathf.Round(float.Parse(pos[0])), Mathf.Round(float.Parse(pos[1])), Mathf.Round(float.Parse(pos[2])));
            placedObject.transform.position = numPos;
            placedObject.transform.rotation = Quaternion.Euler(new Vector3(0, 90 * Int32.Parse(objectData[3]), 0));

            positionLocker locker;
            if (locker = placedObject.GetComponent<positionLocker>()) {
                locker.correctPos = numPos;
                locker.correctRot = Quaternion.Euler(new Vector3(0, 90 * Int32.Parse(objectData[3]), 0));
            }

            blockInfo = readNextLine();
        }

        if (!mainGameLevel)
            customReader.Close();

        // Apply song
        if (!editorMode) {
            GameObject songPlayer;
            switch (globalVariables.gameStyle) {
                case 0:
                    songPlayer = Instantiate(normalSongPlayer);
                    songPlayer.GetComponent<AudioSource>().clip = (AudioClip)Resources.Load("Music/Hamsterball/" + globalVariables.song);
                    songPlayer.GetComponent<AudioSource>().Play();
                    break;
                case 1:
                    songPlayer = Instantiate(ballanceSongPlayer);
                    int songPack = globalVariables.song;
                    AudioClip[] songClips = new AudioClip[3];
                    songClips[0] = (AudioClip)Resources.Load("Music/Ballance/" + (globalVariables.song + 1) + "-" + 1);
                    songClips[1] = (AudioClip)Resources.Load("Music/Ballance/" + (globalVariables.song + 1) + "-" + 2);
                    songClips[2] = (AudioClip)Resources.Load("Music/Ballance/" + (globalVariables.song + 1) + "-" + 3);
                    songPlayer.GetComponent<ballancemusicplayer>().audioSources = songClips;
                    Instantiate(ballanceAmbiencePlayer);
                    break;
                case 2:
                    songPlayer = Instantiate(normalSongPlayer);
                    songPlayer.GetComponent<AudioSource>().clip = (AudioClip)Resources.Load("Music/Marble Blast/" + globalVariables.song);
                    songPlayer.GetComponent<AudioSource>().Play();
                    break;
            }
        }

        editorLogic.updateTextures();   // ground and slopes need to be retextured
        editorLogic.updateStartPlatform();
        
        // Apply Scale
        if (!editorMode) {
            Vector3 scale = new Vector3((float)globalVariables.levelScale, (float)globalVariables.levelScale, (float)globalVariables.levelScale);
            editorLogic.startspot.transform.localScale = scale;
            editorLogic.groundObjectHolder.transform.localScale = scale;
            editorLogic.texturableObjectHolder.transform.localScale = scale;
            editorLogic.placedObjectHolder.transform.localScale = scale;
        }

        // Place ball and camera
        if (!editorMode) {
            cam = Instantiate((GameObject)Resources.Load("Level Editor Parts/Cameras/" + globalVariables.cameraStyle));

            // Put the ball at the checkpoint
            if (globalVariables.checkpointNum != -1) {
                mainBall = Instantiate((GameObject)Resources.Load("Level Editor Parts/Balls/" + globalVariables.savedBallType));
                GameObject checkpoint = GameObject.FindGameObjectsWithTag("Checkpoint")[globalVariables.checkpointNum];
                if (checkpoint.GetComponent<ballanceCheckpoint>())
                    checkpoint.GetComponent<ballanceCheckpoint>().setActive();
                mainBall.transform.position = globalVariables.checkpointLoc;
            } else {
                mainBall = Instantiate((GameObject)Resources.Load("Level Editor Parts/Balls/" + globalVariables.startingBall));
                if (globalVariables.gameStyle != 1)
                    mainBall.transform.position = Vector3.zero;
                else
                    mainBall.transform.position = new Vector3(0, 0.5f, 0);
            }

            // Make the camera follow the ball
            if (cam.GetComponent<ballanceCamera>() != null)
                cam.GetComponent<ballanceCamera>().objectToFollow = mainBall.transform;
            else
                cam.GetComponent<marbleCam>().target = mainBall.transform;

            editorLogic.cam = cam;
        }

        editorLogic.updateLighting();
        editorLogic.updateSkybox();

        // Place the Powerup Panel if necessary
        if (!editorMode) {
            if (GameObject.FindGameObjectsWithTag("Powerup").Length > 0) {
                GameObject powerupPanel = Instantiate((GameObject)Resources.Load("Level Editor Parts/PowerupPanel/PowerupCanvas"));
                GameObject powerupPanelCamera = Instantiate((GameObject)Resources.Load("Level Editor Parts/PowerupPanel/PowerupCanvasCamera"));
                powerupPanel.GetComponent<Canvas>().worldCamera = powerupPanelCamera.GetComponent<Camera>();
                // shift over the level timer
                GameObject timer = GameObject.FindGameObjectWithTag("levelTimer");
                timer.transform.position = new Vector3(0, timer.transform.position.y, timer.transform.position.z);
            }
        }
        
        // Find respawn height and place cloud layer if necessary
        if (!editorMode) {
            float lowestBlockHeight = -10;
            GameObject[] allObjects = UnityEngine.Object.FindObjectsOfType<GameObject>() ;
            foreach(GameObject go in allObjects) {
                if (go.activeInHierarchy) {
                    if (go.transform.position.y < lowestBlockHeight) {
                        lowestBlockHeight = go.transform.position.y;
                    }
                }

            }
            if (globalVariables.deathPlaneHeightActive) {
                globalVariables.respawnHeight = (float)globalVariables.deathPlaneHeight;
            }
            else {
                globalVariables.respawnHeight = lowestBlockHeight - 10;
            }
            if (globalVariables.gameStyle == 1) {
                if (globalVariables.skybox == 11) {
                    Instantiate((GameObject)Resources.Load("Cloud Layers/Tornado"), new Vector3(0, lowestBlockHeight + 10, 0), Quaternion.Euler(-90, 0, 0));
                } else {
                    Instantiate((GameObject)Resources.Load("Cloud Layers/Normal"), new Vector3(0, lowestBlockHeight, 0), Quaternion.Euler(-90, 0, 0));
                }
            }
        }

        // Hide the mouse cursor
        if (!editorMode) {
            Cursor.lockState = CursorLockMode.Confined;
            Cursor.visible = false;
        }

        // Tell the customlevelplayer loading is done
        if (!editorMode)
            FindObjectOfType<CustomLevelPlayer>().OnLoadingDone();
        
    }

    string readNextLine() {
        if (!mainGameLevel) {
            return customReader.ReadLine();
        } else {
            string line = campaignLevelLines[lineNum];
            lineNum++;
            return line;
        }
    }

    GameObject getGameObjectByName (UnityEngine.Object[] array, string name)
    {
    for (int i = 0; i < array.Length; i++) 
    {
        if (array[i].name.Split('-')[1] == name)
            return (GameObject)array[i];
    }
 
     Debug.Log ("No item has the name '" + name + "'.");
     return null;
    }

    public GameObject placeObject(GameObject item, int panelID) {
        GameObject newItem = Instantiate(item);

        if (editorMode) {
            bool positionLock = false;
            if (newItem.GetComponent<Rigidbody>()) {
                positionLock = true;
            }
            editorLogic.removeMostComponents(newItem);
            if (positionLock)
                newItem.AddComponent<positionLocker>();
        }
        newItem.name = panelID + ":" + item.name.Split('-')[1];
        if (newItem.GetComponent<blockTexturingValues>()) {
            // should actually be textured...
            // can happen since my stupid code is hardcoded for slopes
            newItem.transform.SetParent(editorLogic.texturableObjectHolder.transform);
        } else {
            newItem.transform.SetParent(editorLogic.placedObjectHolder.transform);
        }
        return newItem;
    }

    public GameObject placeTexturableObject(GameObject item, int panelID) {
        GameObject newItem = Instantiate(item);
        if (editorMode)
            editorLogic.removeMostComponents(newItem);
        newItem.name = panelID + ":" + item.name.Split('-')[1];
        newItem.transform.SetParent(editorLogic.texturableObjectHolder.transform);
        return newItem;
    }

}
