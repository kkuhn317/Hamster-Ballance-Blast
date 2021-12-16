using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockTexturingValues : MonoBehaviour
{
    // Start is called before the first frame update

    // this is just for holding the values needed by editorLogic

    public int topMaterialnum = 0;
    public Vector2 topMaterialSize = new Vector2(-1, -1);
    public Vector2 topMaterialOffset = new Vector2(.5f, .5f);
    public int topMaterialRotation = 2;
    // multiplied by 90 degrees

    public int bottomMaterialnum = 4;
    public int[] wallsMaterialnums;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
