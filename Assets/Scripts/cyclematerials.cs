using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cyclematerials : MonoBehaviour
{
    public Texture2D[] textures;
    public float delay = 1.0f;
    private int textureCounter = 0;

    void CycleTextures () {
        textureCounter = ++textureCounter % textures.Length;
        GetComponent<Renderer>().material.mainTexture = textures[textureCounter];
        GetComponent<Renderer>().material.SetTexture ("_EmissionMap", textures[textureCounter]);
        
    }

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("CycleTextures", delay, delay);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
