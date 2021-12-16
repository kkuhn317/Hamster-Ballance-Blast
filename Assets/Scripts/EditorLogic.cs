using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class EditorLogic : MonoBehaviour
{
    public bool editorMode = true;
    public GameObject pointer;
    public GameObject placedObjectHolder;
    public GameObject texturableObjectHolder;
    public Dropdown StyleDropdown;
    public GameObject[] ObjectMenus;
    public GameObject cam;
    public GameObject menu;

    private float movmentRate = .025f;
    private float movementRateCounter = .025f;

    private System.Type[] excludedScriptComponents = {typeof(teleporter), typeof(autoColorer), typeof(blockTexturingValues)};   // these scripts will not be deleted in the editor

    [Header("Level Settings Stuff")]
    private GameObject lightObject;
    public GameObject startspot;

    [Header("Ground Objects")]
    public Shader groundShader;
    public GameObject groundObjectHolder;
    public GameObject groundPreview;
    public GameObject groundPrefab;
    private bool placingGround = false;
    private Vector3 groundPoint;
    
    [Header("Multiselector")]
    public GameObject multiSelector;
    private Vector3 selectorPoint1, selectorPoint2;
    private bool placingSelector = false, selectorVisible = false;
    private List<GameObject> selectedGround, selectedTexturableObjects, selectedNormalObjects;

    [Header("Copy and Paste")]
    public GameObject copyAreaIndicator;
    private Vector3 copySelectionArea;
    private bool copyMode = false;

    // for use with loading copied objects
    UnityEngine.Object[] basicBlocks;
    UnityEngine.Object[] hamsterballObjects;
    UnityEngine.Object[] ballanceObjects;
    UnityEngine.Object[] marbleBlastObjects;

    // These three arrylists store the objects in a similar way to how they are stored in a file
    private ArrayList copiedGround = new ArrayList();
    private ArrayList copiedTexturableObjects = new ArrayList();
    private ArrayList copiedNormalObjects = new ArrayList();
    
    [Header("Texturing")]
    private Texture2D floorTexture, wallTexture, ceilingTexture;
    private float textureScale = 1;

    // Start is called before the first frame update
    void Start()
    {
        basicBlocks = Resources.LoadAll("Level Editor Parts/Basic Blocks", typeof(GameObject));
        hamsterballObjects = Resources.LoadAll("Level Editor Parts/Hamsterball", typeof(GameObject));
        ballanceObjects = Resources.LoadAll("Level Editor Parts/Ballance", typeof(GameObject));
        marbleBlastObjects = Resources.LoadAll("Level Editor Parts/Marble Blast", typeof(GameObject));

        
    }

    private Vector3 processMovement() {
        Vector3 movementVector = new Vector3(0,0,0);
        if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) {
            movementVector.x = 1;
        } else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S)) {
            movementVector.x = -1;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) {
            movementVector.z = 1;
        } else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) {
            movementVector.z = -1;
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            movementVector = new Vector3(0, movementVector.x, movementVector.z);
        }

        return movementVector;
    }

    private Vector3 holdKeyMovement() {
        
        if (movementRateCounter > 0) {
            movementRateCounter -= Time.deltaTime;
            return Vector3.zero;
        } 

        Vector3 movementVector = new Vector3(0,0,0);

        while (movementRateCounter < 0) {
            if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) {
                movementVector.x += 1;
            } else if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) {
                movementVector.x += -1;
            }

            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A)) {
                movementVector.z += 1;
            } else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D)) {
                movementVector.z += -1;
            }

            movementRateCounter += movmentRate;
        }

        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) {
            movementVector = new Vector3(0, movementVector.x, movementVector.z);
        }

        return movementVector;
    }

    public GameObject placeObject(GameObject item, int panelID) {
        GameObject newItem = Instantiate(item, pointer.transform.position, Quaternion.Euler(0, getForwardRotation() - 90,0));
        if (editorMode)
            removeMostComponents(newItem);
        newItem.name = panelID + ":" + item.name.Split('-')[1];
        if (newItem.GetComponent<blockTexturingValues>()) {
            // can happen from the copy and paste function
            // ...sorry
            applyTexturesToBlock(newItem);
            newItem.transform.SetParent(texturableObjectHolder.transform);
            return newItem;
        }
        newItem.transform.SetParent(placedObjectHolder.transform);
        if (newItem.GetComponent<autoColorer>()) {
            newItem.GetComponent<autoColorer>().recolor(globalVariables.extraColor);
        }
        return newItem;
    }

    public GameObject placeTexturableObject(GameObject item, int panelID) {
        GameObject newItem = Instantiate(item, pointer.transform.position, Quaternion.Euler(0, getForwardRotation(),0));
        if (editorMode)
            removeMostComponents(newItem);
        newItem.name = panelID + ":" + item.name.Split('-')[1];
        if (newItem.GetComponent<blockTexturingValues>()) {
            applyTexturesToBlock(newItem);
        } else {
            applyTexturesToSlope(newItem);
        }
        newItem.transform.SetParent(texturableObjectHolder.transform);
        return newItem;
    }

    public void removeMostComponents(GameObject obj) {
        foreach(var component in obj.GetComponentsInChildren<MonoBehaviour>())
        {
            if (!excludedScriptComponents.Contains(component.GetType())) // exclude some scripts
                Destroy(component);
        }
        foreach(var component in obj.GetComponentsInChildren<ConstantForce>())
        {
            Destroy(component);
        }
        foreach(var component in obj.GetComponentsInChildren<AudioSource>())
        {
            Destroy(component);
        }
        foreach(var component in obj.GetComponentsInChildren<Rigidbody>())
        {
            //component.Sleep();
            Destroy(component);
        }

    }

    public void rotateObject() {
        ArrayList rotatedObjects = new ArrayList();

        Collider[] collidedObjects = objectsIntersect();
        foreach(Collider col in collidedObjects) {
            Transform child = col.gameObject.transform;
            if (child.IsChildOf(placedObjectHolder.transform)) {
                // Find the specific placed object 1 level down from the object holder
                while((child.parent != placedObjectHolder.transform)) {
                    child = child.parent;
                }
                if (!rotatedObjects.Contains(child)) {
                    rotatedObjects.Add(child);
                    float prevRotation = child.rotation.eulerAngles.y;
                    child.rotation = Quaternion.Euler(new Vector3(0, prevRotation + 90, 0));
                }
            }
        }

        foreach(Transform child in placedObjectHolder.transform) {
            if (child.position == pointer.transform.position && !rotatedObjects.Contains(child)) {
                float prevRotation = child.rotation.eulerAngles.y;
                child.rotation = Quaternion.Euler(new Vector3(0, prevRotation + 90, 0));
            }
        }
        foreach(Transform child in texturableObjectHolder.transform) {
            if (child.position == pointer.transform.position && !rotatedObjects.Contains(child)) {
                float prevRotation = child.rotation.eulerAngles.y;
                child.rotation = Quaternion.Euler(new Vector3(0, prevRotation + 90, 0));
                if (child.GetComponent<blockTexturingValues>()) {
                    applyTexturesToBlock(child.gameObject);
                } else {
                    applyTexturesToSlope(child.gameObject);
                }
            }
        }
    }

    private void copySelection() {
        copiedGround.Clear();
        copiedTexturableObjects.Clear();
        copiedNormalObjects.Clear();

        // Save all selected objects
        copyMode = true;
        copySelectionArea = selectorPoint2 - selectorPoint1;
        foreach(GameObject groundObject in selectedGround) {
            Vector3 pos = groundObject.transform.position - selectorPoint1;
            Vector3 scale = groundObject.transform.localScale;
            copiedGround.Add(new Vector3[] {pos, scale});
        }
        foreach(GameObject texturableObject in selectedTexturableObjects) {
            Vector3 pos = texturableObject.transform.position - selectorPoint1;
            Quaternion rot = texturableObject.transform.rotation;
            copiedTexturableObjects.Add(new object[] {texturableObject.name, pos, rot});
        }
        foreach(GameObject normalObject in selectedNormalObjects) {
            Vector3 pos = normalObject.transform.position - selectorPoint1;
            Quaternion rot = normalObject.transform.rotation;
            copiedTexturableObjects.Add(new object[] {normalObject.name, pos, rot});
        }
    }

    private void cutSelection() {
        copySelection();
        deleteSelection();
    }

    private void pasteSelection() {
        // place all copied objects
        foreach(Vector3[] groundInfo in copiedGround) {
            GameObject ground = Instantiate(groundPrefab);
            Vector3 pos = pointer.transform.position + groundInfo[0];
            Vector3 scale = groundInfo[1];
            ground.transform.position = pos;
            ground.transform.localScale = scale;
            ground.transform.SetParent(groundObjectHolder.transform);
        }

        

        foreach(object[] texturableObjectInfo in copiedTexturableObjects) {
            placeCopiedObject(texturableObjectInfo);
        }

        foreach(object[] normalObjectInfo in copiedNormalObjects) {
            placeCopiedObject(normalObjectInfo);
        }

        updateTextures();
        
    }

    public void placeCopiedObject(object[] objectInfo) {
        // style:id, position, rotation
        string[] objectData = ((string)objectInfo[0]).Split(':');
        GameObject placedObject = null;

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
        Vector3 pos = (pointer.transform.position + (Vector3)objectInfo[1]);
        Quaternion rot = (Quaternion)objectInfo[2];
        placedObject.transform.position = pos;
        placedObject.transform.rotation = rot;
    }

    // TODO: This method is identical to the one in LevelLoader, which is probably not great
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

    private void stopCopying() {
        copyMode = false;
        copiedGround.Clear();
        copiedTexturableObjects.Clear();
        copiedNormalObjects.Clear();
    }

    public Collider[] objectsIntersect() {
        return Physics.OverlapSphere(pointer.transform.position, .49f);
    }

    public Collider[] multiselectorObjectsIntersect() {
        if (!placingSelector && selectorVisible) {
            Transform selectorTransform = multiSelector.transform;
            Vector3 selectorScale = multiSelector.transform.localScale;
            Vector3 overlapBoxScale = new Vector3(selectorScale.x / 2 - .01f, selectorScale.y / 2 - .01f, selectorScale.z / 2 - .01f);
            return Physics.OverlapBox(selectorTransform.position, overlapBoxScale, Quaternion.identity);
        }
        return new Collider[0];
    }

    // TODO: Incorporate this method into rotation and deletion methods
    public List<GameObject> findChildObjectsFromColliders(GameObject parent, Collider[] colliders) {
        List<GameObject> foundObjects = new List<GameObject>();
        foreach(Collider col in colliders) {
            Transform child = col.gameObject.transform;
            if (child.IsChildOf(parent.transform)) {
                // Find the specific placed object 1 level down from the object holder
                while((child.parent != parent.transform)) {
                    child = child.parent;
                }
                if (!foundObjects.Contains(child.gameObject))
                    foundObjects.Add(child.gameObject);
            }
        }
        return foundObjects;
    }

    public List<GameObject> findGameObjectsInMultiselector(GameObject objectHolder) {
        // First find all objects where their colliders intersect the multiselector box
        Collider[] intersectColliders = multiselectorObjectsIntersect();
        List<GameObject> intersectObjects = findChildObjectsFromColliders(objectHolder, intersectColliders);

        // Now find all objects where their position is inside the multiselector
        foreach (Transform childTransform in objectHolder.transform) {
            GameObject childObject = childTransform.gameObject;
            if ((!intersectObjects.Contains(childObject)) && (multiSelector.GetComponent<Collider>().bounds.Contains(childObject.transform.position))) {
                intersectObjects.Add(childObject);
            } 

        }
        return intersectObjects;
    }

    public void onGameStyleDropdownChange() {
        foreach(GameObject objectPanel in ObjectMenus) {
            objectPanel.SetActive(false);
        }
        ObjectMenus[StyleDropdown.value].SetActive(true);
    }

    public void deleteObject() {

        if (!placingSelector && selectorVisible) {  // multiselector active
            deleteSelection();
            return;
        }

        Collider[] collidedObjects = objectsIntersect();
        foreach(Collider col in collidedObjects) {
            Transform child = col.gameObject.transform;
            if (child.IsChildOf(placedObjectHolder.transform) || child.IsChildOf(texturableObjectHolder.transform)) {
                // Find the specific placed object 1 level down from the object holder
                while((child.parent != placedObjectHolder.transform) && (child.parent != texturableObjectHolder.transform)) {
                    child = child.parent;
                }
                Destroy(child.gameObject);
            }
        }

        foreach (Transform child in placedObjectHolder.transform) {
            if (child.position == pointer.transform.position) {
                Destroy(child.gameObject);
            }
        }
        foreach(Transform child in texturableObjectHolder.transform) {
            if (child.position == pointer.transform.position) {
                Destroy(child.gameObject);
            }
        }
        foreach(Transform child in groundObjectHolder.transform) {
            if (child.GetComponent<BoxCollider>().bounds.Contains(pointer.transform.position)) {
                Destroy(child.gameObject);
            }
        }
    }
    
    public void deleteSelection() {
        foreach(GameObject goodbyeobjectlol in selectedGround) {
                Destroy(goodbyeobjectlol);
            }
            foreach(GameObject goodbyeobjectlol in selectedTexturableObjects) {
                Destroy(goodbyeobjectlol);
            }
            foreach(GameObject goodbyeobjectlol in selectedNormalObjects) {
                Destroy(goodbyeobjectlol);
            }
            selectedGround.Clear();
            selectedTexturableObjects.Clear();
            selectedNormalObjects.Clear();
            selectorVisible = false;
    }

    public void chooseGroundLocation() {
        if (placingGround == false) {
            // Choose first corner
            groundPoint = pointer.transform.position;
        } else {
            // Place the ground
            GameObject newGround = Instantiate(groundPrefab);
            Vector3[] posandscale = makeBlockFromCoords(groundPoint, pointer.transform.position);
            newGround.transform.position = posandscale[0];
            newGround.transform.localScale = posandscale[1];
            applyTexturesToGround(newGround);
            newGround.transform.SetParent(groundObjectHolder.transform);
        }

        placingGround = !placingGround;
    }

    public void updateGroundPreview() {
        if (placingGround == true) {
            Vector3[] posandscale = makeBlockFromCoords(groundPoint, pointer.transform.position);
            groundPreview.transform.position = posandscale[0];
            groundPreview.transform.localScale = posandscale[1];
        } else {
            // hide the ground preview
            groundPreview.transform.localScale = new Vector3(0,0,0);
        }
    }
    public void chooseMultiSelectorPosition() {
        if (!placingSelector && !selectorVisible) {
            // Choose first corner
            multiSelector.GetComponent<Renderer>().material.color = Color.green;
            selectorPoint1 = pointer.transform.position;
            placingSelector = true;
            selectorVisible = true;

        } else if (placingSelector) {
            multiSelector.GetComponent<Renderer>().material.color = new Color(1, .64f, 0);
            selectorPoint2 = pointer.transform.position;
            placingSelector = false;

            // Save selected blocks using the methods I made above
            selectedGround = findGameObjectsInMultiselector(groundObjectHolder);
            selectedTexturableObjects = findGameObjectsInMultiselector(texturableObjectHolder);
            selectedNormalObjects = findGameObjectsInMultiselector(placedObjectHolder);


        } else {    // !placingSelector && selectorVisible
            selectorVisible = false;
            selectedGround.Clear();
            selectedTexturableObjects.Clear();
            selectedNormalObjects.Clear();
        }

    }

    public void updateMultiSelector() {
        if (placingSelector) {
            Vector3[] posandscale = makeBlockFromCoords(selectorPoint1, pointer.transform.position);
            multiSelector.transform.position = posandscale[0];
            multiSelector.transform.localScale = posandscale[1];
        }
        else if (!placingSelector && selectorVisible) {
            Vector3[] posandscale = makeBlockFromCoords(selectorPoint1, selectorPoint2);
            multiSelector.transform.position = posandscale[0];
            multiSelector.transform.localScale = posandscale[1];
        } else {
            // hide the multi selector
            multiSelector.transform.localScale = new Vector3(0,0,0);
        }
    }

    public void updateCopyArea() {
        if (copyMode) {
            Vector3[] posandscale = makeBlockFromCoords(pointer.transform.position, pointer.transform.position + copySelectionArea);
            copyAreaIndicator.transform.position = posandscale[0];
            copyAreaIndicator.transform.localScale = posandscale[1];
        } else {
            // hide the multi selector
            copyAreaIndicator.transform.localScale = new Vector3(0,0,0);
        }
    }

    public void updateSkybox() {
        Camera camera = cam.GetComponent<Camera>();
        switch(globalVariables.gameStyle) {
            case 0: // No skybox, solid colored sky
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = globalVariables.backgroundColor;
                break;
            case 1: // Skybox depends on specific level choice
                camera.clearFlags = CameraClearFlags.Skybox;
                RenderSettings.skybox = (Material)Resources.Load("Level Editor Parts/Backgrounds/Ballance/" + globalVariables.skybox);
                break;
            case 2: // Only 1 Marble Blast skybox available
                camera.clearFlags = CameraClearFlags.Skybox;
                RenderSettings.skybox = (Material)Resources.Load("Level Editor Parts/Backgrounds/MBG");
                break;
        }
        
    }

    public void updateStartPlatform() {
        if (startspot != null) {
            Destroy(startspot);
        }
        Object startprefab = Resources.Load("Level Editor Parts/Start Platforms/" + globalVariables.gameStyle);
        startspot = Instantiate((GameObject)startprefab);
        startspot.transform.position = Vector3.zero;
    }

    public void updateLighting() {
        // Change lighting to match game style (light object and also scene environment lighting)
        if (lightObject != null) {
            Destroy(lightObject);
        }
        Object lightprefab = Resources.Load("Lighting/" + globalVariables.gameStyle);
        lightObject = Instantiate((GameObject)lightprefab);

        switch(globalVariables.gameStyle) {
            case 0: // HB
                RenderSettings.ambientLight = new Color32(128, 128, 128, 255);
                break;
            case 1: // Ballance
                RenderSettings.ambientLight = new Color32(255, 255, 255, 255);
                break;
            case 2: // MB
                RenderSettings.ambientLight = new Color32(128, 128, 128, 255);
                break;
        }
    }

    public void updateTextures() {
        

        // Update ground and slope textures
        switch(globalVariables.gameStyle) {
            case 0: // Generate textures based on selected colors

                floorTexture = new Texture2D(2,2);
                floorTexture.filterMode = FilterMode.Point;
                Color[] cols = floorTexture.GetPixels();
                for (int i = 0; i < cols.Length; ++i)
                {
                    if (i == 0 || i == 3)
                        cols[i] = globalVariables.floorColor1;
                    else
                        cols[i] = globalVariables.floorColor2;
                }
                floorTexture.SetPixels(cols);
                floorTexture.Apply();

                wallTexture = new Texture2D(1,1);
                wallTexture.filterMode = FilterMode.Point;
                wallTexture.SetPixel(0, 0, globalVariables.wallColor);
                wallTexture.Apply();

                ceilingTexture = new Texture2D(1,1);
                ceilingTexture.filterMode = FilterMode.Point;
                ceilingTexture.SetPixel(0,0, globalVariables.wallColor);
                ceilingTexture.Apply();
                textureScale = 2;
                break;
            case 1: // TODO: make ground look nicer
                floorTexture = (Texture2D)Resources.Load("Level Editor Parts/Ground Textures/Ballance/Floor_Top_Borderless");
                wallTexture = (Texture2D)Resources.Load("Level Editor Parts/Ground Textures/Ballance/Floor_Top_Borderless");
                ceilingTexture = (Texture2D)Resources.Load("Level Editor Parts/Ground Textures/Ballance/Floor_Top_Borderless");
                textureScale = 1;
                break;
            case 2: // MB
                floorTexture = (Texture2D)Resources.Load("Level Editor Parts/Ground Textures/Marble Blast/" + globalVariables.floorTexture);
                wallTexture = (Texture2D)Resources.Load("Level Editor Parts/Ground Textures/Marble Blast/" + globalVariables.floorTexture);
                ceilingTexture = (Texture2D)Resources.Load("Level Editor Parts/Ground Textures/Marble Blast/" + globalVariables.floorTexture);
                textureScale = 5;
                break;

        }

        foreach(Transform ground in groundObjectHolder.transform) {
            applyTexturesToGround(ground.gameObject);
        }

        foreach(Transform slope in texturableObjectHolder.transform) {
            if (slope.GetComponent<blockTexturingValues>()) {
                // not actually a slope lol
                applyTexturesToBlock(slope.gameObject);
            } else {
                applyTexturesToSlope(slope.gameObject);
            }
        }

        // now color the colorable objects
        colorColorableObjects();

        
    }

    public void colorColorableObjects() {

        Color recolor = globalVariables.extraColor;

        // find all autoColorer components in the scene
        autoColorer[] autoColorers = FindObjectsOfType<autoColorer>();
        foreach(autoColorer autoColorer in autoColorers) {
            autoColorer.recolor(recolor);
        }
    }

    public void applyTexturesToGround(GameObject ground) {
        Vector3 groundScale = ground.transform.localScale;
        Vector3 groundPos = ground.transform.position;
        Vector3 cornerCoordinate = new Vector3();
        cornerCoordinate.x = groundPos.x - Mathf.Abs(groundScale.x / 2) + .5f;
        cornerCoordinate.y = groundPos.y - Mathf.Abs(groundScale.y / 2) + .5f;
        cornerCoordinate.z = groundPos.z - Mathf.Abs(groundScale.z / 2) + .5f;
        Vector3 cornerCoordinate2 = new Vector3();
        cornerCoordinate2.x = groundPos.x + Mathf.Abs(groundScale.x / 2) - .5f;
        cornerCoordinate2.y = groundPos.y + Mathf.Abs(groundScale.y / 2) - .5f;
        cornerCoordinate2.z = groundPos.z + Mathf.Abs(groundScale.z / 2) - .5f;

        foreach(Transform side in ground.transform) {

            Material material = new Material(groundShader);
            MeshRenderer sideRenderer = side.GetComponent<MeshRenderer>();

            if (side.name == "Top") {
                material.mainTexture = floorTexture;
                material.mainTextureScale = new Vector2(Mathf.Abs(groundScale.x) / textureScale, Mathf.Abs(groundScale.z) / textureScale);
                material.mainTextureOffset = new Vector2((1/textureScale) * cornerCoordinate.x, (1/textureScale) * cornerCoordinate.z);
            } else if (side.name == "Bottom") {
                material.mainTexture = ceilingTexture;
                material.mainTextureScale = new Vector2(Mathf.Abs(groundScale.x) / textureScale, Mathf.Abs(groundScale.z) / textureScale);
                material.mainTextureOffset = new Vector2((1/textureScale) * cornerCoordinate.x, (1/textureScale) * -cornerCoordinate2.z);
            } else if (side.name == "Front") {
                material.mainTexture = wallTexture;
                material.mainTextureScale = new Vector2(Mathf.Abs(groundScale.x) / textureScale, Mathf.Abs(groundScale.y) / textureScale);
                material.mainTextureOffset = new Vector2((1/textureScale) * cornerCoordinate.x, (1/textureScale) * cornerCoordinate.y);
            } else if (side.name == "Back") {
                material.mainTexture = wallTexture;
                material.mainTextureScale = new Vector2(Mathf.Abs(groundScale.x) / textureScale, Mathf.Abs(groundScale.y) / textureScale);
                material.mainTextureOffset = new Vector2((1/textureScale) * -cornerCoordinate2.x, (1/textureScale) * cornerCoordinate.y);
            } else if (side.name == "Right") {
                material.mainTexture = wallTexture;
                material.mainTextureScale = new Vector2(Mathf.Abs(groundScale.z) / textureScale, Mathf.Abs(groundScale.y) / textureScale);
                material.mainTextureOffset = new Vector2((1/textureScale) * cornerCoordinate.z, (1/textureScale) * cornerCoordinate.y);
            } else {
                material.mainTexture = wallTexture;
                material.mainTextureScale = new Vector2(Mathf.Abs(groundScale.z) / textureScale, Mathf.Abs(groundScale.y) / textureScale);
                material.mainTextureOffset = new Vector2((1/textureScale) * -cornerCoordinate2.z, (1/textureScale) * cornerCoordinate.y);
            }
                
            sideRenderer.material = material;
        }
    }

    // TODO eventually: make slopes in marble blast theme connect with the flat ground
    public void applyTexturesToSlope(GameObject slope) {
        // 0, 2, 3 are walls, 1 is top, 4 is bottom
        // Steep slope is 0:SteepSlope, shallow slope is 0:ShallowSlope, corner slope is 0:CornerSlope
        // "no you can't just hardcode those objects"
        // "haha coding go brrrrrrrrrrrrrrr"
        Vector3 slopePos = slope.transform.position;
        int i = 0;
        foreach(Material material in slope.GetComponent<Renderer>().materials) {
            material.color = Color.white;
            if (i == 1) {
                material.mainTexture = floorTexture;
                material.mainTexture = rotateTextureToAngle(floorTexture, (-(int)slope.transform.rotation.eulerAngles.y + 180) % 360);
                if (slope.name == "0:ShallowSlope") {
                    material.mainTextureScale = new Vector2(1/textureScale, 2/textureScale);
                } else {
                    material.mainTextureScale = new Vector2(1/textureScale, 1/textureScale);
                }

                
                switch ((int)slope.transform.rotation.eulerAngles.y) {
                    case 0:
                        material.mainTextureOffset = new Vector2((1/textureScale) * (-slopePos.x - 1), (1/textureScale) * (-slopePos.z - 1));
                        break;
                    case 90:
                        material.mainTextureOffset = new Vector2((1/textureScale) * slopePos.z, (1/textureScale) * (-slopePos.x - 1));
                        break;
                    case 180:
                        material.mainTextureOffset = new Vector2((1/textureScale) * slopePos.x, (1/textureScale) * (slopePos.z));
                        break;
                    case 270:
                        material.mainTextureOffset = new Vector2((1/textureScale) * (-slopePos.z - 1), (1/textureScale) * (slopePos.x));
                        break;
                }
                
            } else if (i == 4) {
                material.mainTexture = ceilingTexture;
            } else {
                material.mainTexture = wallTexture;
                material.mainTextureScale = new Vector2(1, .5f);
            }

            i++;
        }
    }

    public void applyTexturesToBlock(GameObject block) {
        // use blocktexturingvalues.cs to get the necessary info

        Vector3 blockPos = block.transform.position;
        blockTexturingValues values = block.GetComponent<blockTexturingValues>();

        int i = 0;
        foreach(Material material in block.GetComponent<Renderer>().materials) {
            material.color = Color.white;
            if (i == values.topMaterialnum) {
                material.mainTexture = rotateTextureToAngle(floorTexture, (-(int)block.transform.rotation.eulerAngles.y + (values.topMaterialRotation * 90)) % 360);
                Vector2 size = values.topMaterialSize;
                material.mainTextureScale = new Vector2(size.x/textureScale, size.y/textureScale);
                Vector2 extraoffset = values.topMaterialOffset;
                blockPos.x += extraoffset.x;
                blockPos.z += extraoffset.y;
                switch ((int)block.transform.rotation.eulerAngles.y) {
                    case 0:
                        material.mainTextureOffset = new Vector2((1/textureScale) * (-blockPos.x), (1/textureScale) * (-blockPos.z));
                        break;
                    case 90:
                        material.mainTextureOffset = new Vector2((1/textureScale) * blockPos.z, (1/textureScale) * -blockPos.x);
                        break;
                    case 180:
                        material.mainTextureOffset = new Vector2((1/textureScale) * blockPos.x, (1/textureScale) * blockPos.z);
                        break;
                    case 270:
                        material.mainTextureOffset = new Vector2((1/textureScale) * -blockPos.z, (1/textureScale) * blockPos.x);
                        break;
                }
                //material.mainTextureOffset += values.topMaterialOffset;
                
            } else if (i == values.bottomMaterialnum) {
                material.mainTexture = ceilingTexture;
            } else if (values.wallsMaterialnums.Contains(i)) {
                material.mainTexture = wallTexture;
                material.mainTextureScale = new Vector2(1, .5f);
            }

            i++;
        }
    }

    

    Texture2D rotateTexture(Texture2D originalTexture, bool clockwise)
    {
        Color32[] original = originalTexture.GetPixels32();
        Color32[] rotated = new Color32[original.Length];
        int w = originalTexture.width;
        int h = originalTexture.height;

        int iRotated, iOriginal;

        for (int j = 0; j < h; ++j)
        {
            for (int i = 0; i < w; ++i)
            {
                iRotated = (i + 1) * h - j - 1;
                iOriginal = clockwise ? original.Length - 1 - (j * w + i) : j * w + i;
                rotated[iRotated] = original[iOriginal];
            }
        }

        Texture2D rotatedTexture = new Texture2D(h, w);
        if (globalVariables.gameStyle == 0)
            rotatedTexture.filterMode = FilterMode.Point;
        rotatedTexture.SetPixels32(rotated);
        rotatedTexture.Apply();
        return rotatedTexture;
    }

    Texture2D rotateTextureToAngle(Texture2D texture, int angle) {
        // rotate the texture to the given angle
        // only supports 0, 90, 180, 270, 360 and negative versions of those

        if (angle <= 0) 
        {
            angle += 360;
        }

        switch (angle) {
            case 0:
                return texture;
            case 90:
                return rotateTexture(texture, true);
            case 180:
                Texture2D newTexture = rotateTexture(texture, true);
                return rotateTexture(newTexture, true);
            case 270:
                return rotateTexture(texture, false);
            case 360:
                return texture;
            default:
                print("rotateTextureToAngle doesn't support " + angle + " degrees");
                return texture;
        }
    }

    // These 2 methods are used for saving all objects to a file
    public void saveBigBlocks(StreamWriter writer) {
        foreach(Transform child in groundObjectHolder.transform) {
            Vector3 pos = child.position;
            Vector3 scale = child.localScale;
            writer.WriteLine(pos.x + "," + pos.y + "," + pos.z + ":" + scale.x + "," + scale.y + "," + scale.z);
        }
    }
    public void saveObjects(StreamWriter writer) {
        foreach(Transform child in texturableObjectHolder.transform) {
            Vector3 pos = child.position;
            int rot = (int)(child.rotation.eulerAngles.y / 90);
            writer.WriteLine(child.name + ":" + pos.x + "," + pos.y + "," + pos.z + ":" + rot);
        }
        foreach(Transform child in placedObjectHolder.transform) {
            Vector3 pos = child.position;
            int rot = (int)(child.rotation.eulerAngles.y / 90);
            writer.WriteLine(child.name + ":" + pos.x + "," + pos.y + "," + pos.z + ":" + rot);
        }
    }

    // Takes the 2 corners of a block and returns the position and scale of the block
    public Vector3[] makeBlockFromCoords(Vector3 point1, Vector3 point2) {
        Vector3 pos;
        Vector3 scale;

        pos = new Vector3(calcAverage(point1.x, point2.x), calcAverage(point1.y, point2.y), calcAverage(point1.z, point2.z));
        scale = new Vector3(Mathf.Abs(point1.x - point2.x) + 1, Mathf.Abs(point1.y - point2.y) + 1, Mathf.Abs(point1.z - point2.z) + 1);
        Vector3[] posandscale = {pos, scale};
        return posandscale;
    }

    private float calcAverage(float value1, float value2) {
        return (float) (value1 + value2) / 2;
    }

    private float getForwardRotation() {
        float camrotation = cam.transform.rotation.eulerAngles.y;

        if (camrotation >= 315 || camrotation < 45) {
            return 0;
        } else if (camrotation >= 45 && camrotation < 135) {
            return 90;
        } else if (camrotation >= 135 && camrotation < 225) {
            return 180;
        } else {
            return 270;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!editorMode)
            return;
    
        if (Input.GetKeyDown(KeyCode.Escape)) {
            menu.SetActive(true);
        }

        if (menu.activeSelf) {
            return;
        }

        Vector3 pointerMovement;
        if (Input.GetKey(KeyCode.LeftControl)) 
            pointerMovement = holdKeyMovement();
        else
            pointerMovement = processMovement();

        float camrotation = cam.transform.rotation.eulerAngles.y;

        if (camrotation >= 315 || camrotation < 45) {
            pointerMovement = new Vector3(-pointerMovement.z, pointerMovement.y, pointerMovement.x);
        } else if (camrotation >= 45 && camrotation < 135) {
            // do nothing
        } else if (camrotation >= 135 && camrotation < 225) {
            pointerMovement = new Vector3(pointerMovement.z, pointerMovement.y, -pointerMovement.x);
        } else {
            pointerMovement = new Vector3(-pointerMovement.x, pointerMovement.y, -pointerMovement.z);
        }

        pointer.transform.position += pointerMovement;

        if (Input.GetKeyDown(KeyCode.R)) {
            rotateObject();
        }

        if (Input.GetKeyDown(KeyCode.Delete)) {
            deleteObject();
        }

        if (Input.GetKeyDown(KeyCode.Space)) {
            if (Input.GetKey(KeyCode.LeftControl)) {
                if (copyMode)
                    stopCopying();
                else
                    chooseMultiSelectorPosition();
            }
            else
                chooseGroundLocation();
        }

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) {
            if (Input.GetKeyDown(KeyCode.V))
                pasteSelection();
            if (selectorVisible && !placingSelector) {
                if (Input.GetKeyDown(KeyCode.X))
                    cutSelection();
                if (Input.GetKeyDown(KeyCode.C))
                    copySelection();
            }
        }

        updateGroundPreview();
        updateMultiSelector();
        updateCopyArea();


    }
}
