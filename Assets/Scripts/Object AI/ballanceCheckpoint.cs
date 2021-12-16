using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballanceCheckpoint : MonoBehaviour
{

    public ParticleSystem centerParticles;
    public ParticleSystem sideParticles1;
    public ParticleSystem sideParticles2;
    private bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Player" && !activated) {
            setActive();
            GetComponent<AudioSource>().Play();
        }
    }

    public void setActive() {
        centerParticles.Stop();
        sideParticles1.Play();
        sideParticles2.Play();
        globalVariables.checkpointLoc = transform.position + new Vector3(0,(float)globalVariables.levelScale,0);
        globalVariables.checkpointNum = System.Array.IndexOf(GameObject.FindGameObjectsWithTag("Checkpoint"), gameObject);

        // Make all the other checkpoints inactive
        foreach(GameObject checkpoint in GameObject.FindGameObjectsWithTag("Checkpoint")) {
            if (checkpoint != gameObject) {
                if (checkpoint.GetComponent<ballanceCheckpoint>())
                    checkpoint.GetComponentInChildren<ballanceCheckpoint>().setInactive();
            }
        }
        globalVariables.savedBallType = int.Parse(GameObject.FindGameObjectWithTag("Player").name.Replace("(Clone)","").Trim());
        activated = true;
    }

    public void setInactive() {
        centerParticles.Play();
        sideParticles1.Stop();
        sideParticles2.Stop();
        activated = false;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
