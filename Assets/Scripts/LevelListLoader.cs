using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelListLoader : MonoBehaviour
{
    public GameObject levelButtonPrefab;
    public GameObject content;
    public MainMenu menu;
    public bool editMode = false;
    private List<GameObject> levelButtons = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {

        // C:\Users\matt\AppData\LocalLow\BookwormKevin\Hamster Ballance Blast
        string levelsFolder = Application.persistentDataPath + "/levels";
        if (!Directory.Exists(levelsFolder))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/levels");
            }
        var info = new DirectoryInfo(levelsFolder);
        var fileInfo = info.GetFiles();
        foreach (FileInfo file in fileInfo) {
            GameObject levelButton = Instantiate(levelButtonPrefab);
            StreamReader reader = new StreamReader(file.FullName);
            levelButton.GetComponentInChildren<Text>().text = reader.ReadLine() + " - By " + reader.ReadLine();
            levelButton.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
            reader.Close();
            if (editMode) {
                levelButton.GetComponent<Button>().onClick.AddListener(delegate {
                    globalVariables.filePath = Path.GetFileNameWithoutExtension(file.Name);
                    if (!File.Exists(file.FullName)) {
                        //menu.showErrorBox("Could not load level: Level does not exist.");
                    } else {
                        SceneManager.LoadScene("Level Editor");
                    }
                });
            } else {
                levelButton.GetComponent<Button>().onClick.AddListener(delegate {
                    globalVariables.filePath = Path.GetFileNameWithoutExtension(file.Name);
                    if (!File.Exists(file.FullName)) {
                        //menu.showErrorBox("Could not load level: Level does not exist.");
                    } else {
                        SceneManager.LoadScene("Custom Level");
                    }
                    
                });
            }
            levelButtons.Add(levelButton);
            //levelButton.transform.SetParent(content.transform);
        }

        // sort the buttons by level name
        levelButtons = levelButtons.OrderBy(x=>x.GetComponentInChildren<Text>().text).ToList();

        foreach (GameObject button in levelButtons) {
            button.transform.SetParent(content.transform);
        }
    }

    public void refreshList() {
        foreach (Transform button in content.transform) {
            Destroy(button.gameObject);
        }
        Start();
    }

}
