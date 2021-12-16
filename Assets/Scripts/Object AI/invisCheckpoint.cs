using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class invisCheckpoint : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Destroy(GetComponent<Renderer>());
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player") {
            globalVariables.checkpointLoc = transform.position;
            globalVariables.checkpointNum = System.Array.IndexOf(GameObject.FindGameObjectsWithTag("Checkpoint"), gameObject);
            globalVariables.savedBallType = int.Parse(GameObject.FindGameObjectWithTag("Player").name.Replace("(Clone)","").Trim());

            // Make all ballance checkpoints inactive
            foreach(GameObject checkpoint in GameObject.FindGameObjectsWithTag("Checkpoint")) {
                if (checkpoint != gameObject) {
                    if (checkpoint.GetComponent<ballanceCheckpoint>())
                        checkpoint.GetComponentInChildren<ballanceCheckpoint>().setInactive();
                }
            }

        
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
