using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FanAI : MonoBehaviour

{

    public float force;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerStay(Collider other) {
        if (other.GetComponent<Rigidbody>())
        {
            other.GetComponent<Rigidbody>().AddForce(transform.right * force * Time.deltaTime);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
