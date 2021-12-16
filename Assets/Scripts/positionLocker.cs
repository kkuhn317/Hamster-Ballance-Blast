using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class positionLocker : MonoBehaviour
{

    // This script is to MAKE SURE THOSE STUPID RIGIDBODIES DONT GET PUSHED AWAY FROM WHERE THEY NEED TO GO!!!
    public Vector3 correctPos;
    public Quaternion correctRot;

    // Start is called before the first frame update
    void Start()
    {

    }

    void Stop() {
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = correctPos;
        transform.rotation = correctRot;
        Stop();
    }
}
