using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class editorUI : MonoBehaviour
{
    public bool basicObjectPanel = false;
    public String objectsFilepath;
    private UnityEngine.Object[] items;
    public GameObject button;
    public int panelID = 0;
    public EditorLogic editorLogic;
    public GameObject content;
    public int[] textureableBlocks;

    // Start is called before the first frame update
    void Start()
    {

        if (basicObjectPanel) {
            // Make the special ground object
            GameObject itemButton = Instantiate(button);
            itemButton.GetComponentInChildren<Text>().text = "Ground Maker";
            itemButton.GetComponentInChildren<Text>().fontStyle = FontStyle.Bold;
            itemButton.GetComponent<Button>().onClick.AddListener(delegate {
                editorLogic.chooseGroundLocation();
                editorLogic.updateGroundPreview();
            });
            itemButton.transform.SetParent(content.transform);
        }

        items = Resources.LoadAll(objectsFilepath, typeof(GameObject)); // WARNING: CAUSES LONGER LOADING TIMES
        // sort the items naturally
        items = items.OrderBy(o => int.Parse(o.name.Split('-')[0])).ToArray();

        foreach (GameObject item in items) {
            GameObject itemButton = Instantiate(button);
            itemButton.GetComponentInChildren<Text>().text = item.name.Split('-')[2];
            if (textureableBlocks.Contains(Array.IndexOf(items, item))) {
                // The Block is a special textureable block
                itemButton.GetComponent<Button>().onClick.AddListener(delegate {
                    editorLogic.placeTexturableObject(item, panelID);
                });
            } else {
                // Normal Block
                itemButton.GetComponent<Button>().onClick.AddListener(delegate {
                    editorLogic.placeObject(item, panelID);
                });
            }
            itemButton.transform.SetParent(content.transform);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
