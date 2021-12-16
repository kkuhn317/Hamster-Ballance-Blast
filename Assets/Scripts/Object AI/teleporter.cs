using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class teleporter : MonoBehaviour
{
    public bool isExit; // if it is an exit instead of an entrance1

    private TextMesh idText;

    public int id = 0;

    Camera cameraToLookAt;

    teleporterManager manager;

    private bool editorMode = false;

    private bool isEnabled = false; // turn this true when the object is placed so that it doesn't call onDisabled()



    // Start is called before the first frame update
    void Start()
    {
        idText = GetComponentInChildren<TextMesh>();
        idText.text = id.ToString();
        cameraToLookAt = Camera.main;
        manager = FindObjectOfType<teleporterManager>();

        manager.addTeleporter(this);

        editorMode = FindObjectOfType<EditorLogic>().editorMode;
        if (!editorMode) {
            // delete the child text object
            Destroy(transform.GetChild(0).gameObject);
            // remove the renderer
            Destroy(GetComponent<Renderer>());
        } else {
            // Editor mode
        }

        
        
    }

    void OnTriggerEnter(Collider other) {
        if (editorMode || isExit) return;

        //if (other.gameObject.tag == "Player" || other.gameObject.tag == "8Ball") {
        if (other.gameObject.GetComponent<Rigidbody>()) {
            // teleport the player to the corresponding exit if it exists
            teleporter exit = manager.getExit(this);
            if (exit != null) {
                other.gameObject.transform.position = exit.transform.position;
            }


        }
    }

    // Update is called once per frame
    void Update()
    {
        if (editorMode)
            idText.text = id.ToString();
    }
    
    private void OnEnable() {
        isEnabled = true;
    }

    void OnDisable()
    {
        if (isEnabled && !this.enabled)
            //Debug.Log("PrintOnDisable: script was disabled");
            manager.removeTeleporter(this);
    }

    private void LateUpdate() {
        if (editorMode)
            TextFaceCamera();
    }

    void TextFaceCamera() {
        Transform childText = transform.GetChild(0);
        childText.LookAt(cameraToLookAt.transform);
        childText.rotation = Quaternion.LookRotation(cameraToLookAt.transform.forward);
    }
}
