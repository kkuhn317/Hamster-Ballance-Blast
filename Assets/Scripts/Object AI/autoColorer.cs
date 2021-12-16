using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoColorer : MonoBehaviour
{

    // put in a list of materials to color based on the level
    public List<GameObject> objectsToColor;    // type 1: the wall color for hb, brown for ballance, blue for mb

    // more types can be added later



    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void recolor(Color c) {
        foreach (GameObject gameObject in objectsToColor) {
            gameObject.GetComponent<Renderer>().material.color = c;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
