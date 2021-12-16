using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class removeExplosion : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("remove", 1);
    }

    private void remove() {
        Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
