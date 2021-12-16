using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerupPanel : MonoBehaviour
{

    private int shownPowerupId = -1;
    // none: -1
    // super jump: 0
    // super speed: 1
    // super bounce: 2
    // shock absorber: 3

    public GameObject superJumpEffect;
    public GameObject superSpeedEffect;
    public GameObject superBounceEffect;
    public GameObject shockAbsorberEffect;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (shownPowerupId != globalVariables.storedPowerup) {
            updateDisplay(globalVariables.storedPowerup);
        }

        // if the left mouse button is pressed, use the powerup
        if (Input.GetMouseButtonDown(0)) {
            usePowerup(globalVariables.storedPowerup);
            globalVariables.storedPowerup = -1;
        }
                
    }

    void updateDisplay(int powerupId) {
        // changes the display to the correct child object with the name of the id
        shownPowerupId = globalVariables.storedPowerup;
        
        // so first disable all children
        foreach (Transform child in transform) {
            child.gameObject.SetActive(false);
        }
        // then enable the correct one
        if (powerupId != -1)
            transform.Find(powerupId.ToString()).gameObject.SetActive(true);
    }

    void usePowerup(int powerupId) {
        if (powerupId == -1) {
            return;
        }
        GameObject player = GameObject.FindWithTag("Player");
        GameObject effect;
        switch (powerupId) {
            case 0:
                effect = superJumpEffect;
            break;
            case 1:
                effect = superSpeedEffect;
            break;
            case 2:
                effect = superBounceEffect;
            break;
            case 3:
                effect = shockAbsorberEffect;
            break;
            default:
                effect = null;
            break;
        }

        

        Rigidbody rb = player.GetComponent<Rigidbody>();

        switch (powerupId) {
            case 0: // super jump
                // launch the player upwards
                rb.AddForce(new Vector3(0, rb.mass * 2000, 0));
                break;
            case 1: // super speed
                // launch the player forward
                // if the camera is Marble Blast Style, move the player in the direction of the camera
                if (globalVariables.cameraStyle == 2) {
                    Vector3 cameraDirection = Camera.main.transform.forward;
                    // make they y component of the camera direction 0
                    cameraDirection.y = 0;
                    // apply forward force to the player rigidbody
                    rb.AddForce(cameraDirection * rb.mass * 2000);
                } else {
                    // if the camera is not Marble Blast Style, move the player in the direction the player is moving
                    // first find the player's movement direction
                    Vector3 movementDirection = rb.velocity.normalized;
                    // make the y component of the movement direction 0
                    movementDirection.y = 0;
                    // apply forward force to the player rigidbody
                    rb.AddForce(movementDirection * rb.mass * 2000);
                }
                break;
            case 2: // super bounce
                resetBounciness();
                CancelInvoke();
                player.GetComponent<Collider>().material.bounciness = 1;
                player.GetComponent<Collider>().material.bounceCombine = PhysicMaterialCombine.Maximum;
                player.GetComponent<Rigidbody>().drag = 0;
                if (player.GetComponent<ballanceRoll>())
                    player.GetComponent<ballanceRoll>().noBreakMode = true;
                Invoke("resetBounciness", 5f);
                break;
            case 3: // shock absorber
                resetBounciness();
                CancelInvoke();
                player.GetComponent<Collider>().material.bounciness = 0;
                player.GetComponent<Collider>().material.bounceCombine = PhysicMaterialCombine.Minimum;
                if (player.GetComponent<ballanceRoll>())
                    player.GetComponent<ballanceRoll>().noBreakMode = true;
                Invoke("resetBounciness", 5f);
                break;
        }

        GameObject effectInstance = Instantiate(effect);
        // make the effect a child of the player
        effectInstance.transform.SetParent(player.transform);
        effectInstance.transform.localPosition = new Vector3(0, 0, 0);
    }

    public void resetBounciness() {
        GameObject player = GameObject.FindWithTag("Player");
        Collider collider = player.GetComponent<Collider>();
        Rigidbody rb = player.GetComponent<Rigidbody>();

        // make the bounciness of the player's collider back to normal
        // please update this if you change the bounciness of the player's collider
        // and yes, I know this is a bad way to do this
        collider.material.bounceCombine = PhysicMaterialCombine.Average;
        switch (int.Parse(player.name.Replace("(Clone)","").Trim())) {
            case 0: // hamsterball
                collider.material.bounciness = 0.6f;
                player.GetComponent<ballanceRoll>().noBreakMode = false;
                break;
            case 1: // wood ball
                rb.drag = 1f;
                collider.material.bounciness = 0;
                break;
            case 2: // stone ball
                rb.drag = 1f;
                collider.material.bounciness = 0;
                break;
            case 3: // paper ball
                rb.drag = 1f;
                collider.material.bounciness = 0;
                break;
            case 4: // marble
                rb.drag = 0f;
                collider.material.bounceCombine = PhysicMaterialCombine.Maximum;
                collider.material.bounciness = 0.42f;
                break;
            default:
                print("uh oh");
                break;

        }
        removeEffectsFromPlayer();
    }

    public void removeEffectsFromPlayer() {
        GameObject player = GameObject.FindWithTag("Player");
        foreach (Transform child in player.transform) {
            if (child.tag == "PowerupEffect")
                Destroy(child.gameObject);
        }
    }
}
