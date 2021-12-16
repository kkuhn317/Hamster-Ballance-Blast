using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cloudMoving : MonoBehaviour
{
    public float xSpeed, ySpeed;
    Renderer rend;

    void Start() {
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        rend.material.mainTextureOffset += new Vector2(xSpeed * Time.deltaTime, ySpeed * Time.deltaTime);
    }
}
