using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class transformer : MonoBehaviour
{
    public GameObject newBall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && other.gameObject.layer != newBall.layer) {
            foreach(GameObject player in GameObject.FindGameObjectsWithTag("Player")) {
                Destroy(player);
            }
            GameObject ballInstance = Instantiate(newBall, transform.parent.position + new Vector3(0, ((float)globalVariables.levelScale / 2) + .5f, 0), Quaternion.Euler(0,0,0));
            Camera cam = Camera.main;
            if (cam.GetComponent<ballanceCamera>() != null)
                cam.GetComponent<ballanceCamera>().objectToFollow = ballInstance.transform;
            else
                cam.GetComponent<marbleCam>().target = ballInstance.transform;
            
        }
    }

}
