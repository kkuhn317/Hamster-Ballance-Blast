using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class marbleRoll : MonoBehaviour
{
    private float jumpHeight;
    public float jumpForce;
    private int jumpDelay = 10;
    private int currentJumpDelay = 0;
    private Vector3 jump;
    public float speed = 5;
    public float airSpeed = 0.1f;
    public float maxVelocity = 999;
    // A cached copy of the squared max velocity. Used in FixedUpdate.
    private float sqrMaxVelocity;
    private Rigidbody rb;
    private float movementX;
    private float movementY;

    // used for mouse movement
    private float mouseMovementX;
    private float mouseMovementY;

    public bool stopInput = false;
    public bool stopJumpInput = false;
    public bool grounded = false;
    public ArrayList hitPoints = new ArrayList();

    [Header("Sound Effects")]

    public AudioClip rollSound;
    public AudioClip jumpSound;
    public AudioClip hitSound;
    public AudioSource audioSourceRoll;
    public AudioSource audioSourceOther;

    // Start is called before the first frame update
    void Start() {
        rb = GetComponent<Rigidbody>();
        SetMaxVelocity(maxVelocity);
        audioSourceRoll.clip = rollSound;
        audioSourceRoll.volume = 0;
        audioSourceRoll.Play();
    }

    public void GetInput()
	{
        // This is so that the ball doesn't move while you rotate the camera in ballance style
        // ALSO you can manually turn off input
        if (Input.GetKey(KeyCode.LeftShift) || stopInput) {
            movementX = 0;
            movementY = 0;
            return;
        }
		movementX = Input.GetAxis("Horizontal");
		movementY = Input.GetAxis("Vertical");

        if (globalVariables.cameraStyle != 2) { // not the marble blast camera
            float sensitivity = PlayerPrefs.GetFloat("Sensitivity", .5f) * 4;
            movementX += mouseMovementX * sensitivity;
            movementY += mouseMovementY * sensitivity;
        }

        //print("x is" + movementX);
        //print("y is" + movementY);
	}

    void OnCollisionEnter(Collision other) {

        if (other.gameObject.tag == "instaKill") {
            reloadScene();
            return;
        }

        float collisionForce = other.impulse.magnitude / Time.fixedDeltaTime;
        audioSourceOther.PlayOneShot(hitSound, collisionForce / 1000);

        OnCollisionStay(other);
    }
    void OnCollisionStay(Collision collisionInfo) {
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            hitPoints.Add(contact.normal);
            Debug.DrawRay(contact.point, contact.normal, Color.white);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.tag == "instaKill")
            reloadScene();
    }

    public void breakBall() {
        reloadScene();
        // might add more here later
    }

    void reloadScene() {
        Time.timeScale = 1;
        Scene loadedLevel = SceneManager.GetActiveScene();
        SceneManager.LoadScene(loadedLevel.buildIndex);
    }
    private void Update() {
        // increase the mouse movement variables every frame until it is called by fixedUpdate
        // this is so the movements are smooth
        mouseMovementX += Input.GetAxisRaw("Mouse X");
        mouseMovementY += Input.GetAxisRaw("Mouse Y");
    }

    private void resetMouseMovement() {
        mouseMovementX = 0;
        mouseMovementY = 0;
    }


    private void FixedUpdate()
	{

        //assuming we only using the single camera:
        var camera = Camera.main;

        //camera forward and right vectors:
        var forward = camera.transform.forward;
        var right = camera.transform.right;

        //project forward and right vectors on the horizontal plane (y = 0)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        if (hitPoints.Count > 0)
            grounded = true;
        else
            grounded = false;

        GetInput();
        resetMouseMovement();
        
        //this is the direction in the world space we want to move:
        Vector3 movement = forward * movementY + right * movementX;
        if (movement.magnitude > 1)
                movement.Normalize();

		//Vector3 movement = new Vector3(movementX, 0.0f, movementY);

        if (grounded)
            rb.AddForce(movement * speed);
        else
            rb.AddForce(movement * airSpeed);

        jumpHeight = Input.GetAxis("Jump");

        
        //jump = new Vector3(0.0f, jumpHeight, 0.0f);

        Vector3 average = Vector3.zero;
        foreach (Vector3 hitPoint in hitPoints)
        {
            average += hitPoint;
        }

        jump = average.normalized;
     
        if (Input.GetAxis("Jump") == 1 && grounded && currentJumpDelay == 0 && !stopJumpInput) {
            rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
            GetComponent<Rigidbody>().AddForce(jump * jumpForce, ForceMode.Impulse);
            grounded = false;
            currentJumpDelay = jumpDelay;
            audioSourceOther.PlayOneShot(jumpSound, 1);
        }

        if (currentJumpDelay > 0)
            currentJumpDelay -= 1;
        
        Vector3 velocity = rb.velocity;
        // Clamp the velocity, if necessary
        // Use sqrMagnitude instead of magnitude for performance reasons.
        if (velocity.sqrMagnitude > sqrMaxVelocity) { // Equivalent to: rigidbody.velocity.magnitude > maxVelocity, but faster.
        // Vector3.normalized returns this vector with a magnitude 
        // of 1. This ensures that we're not messing with the 
        // direction of the vector, only its magnitude.
            rb.velocity = velocity.normalized * maxVelocity;
        }

        // Rolling Sounds!!!
        if (!grounded) {
            audioSourceRoll.volume = 0;
        } else {
            audioSourceRoll.volume = rb.velocity.magnitude / 50;
        }

        hitPoints.Clear();
	}

    private void SetMaxVelocity(float maxVelocity){
        this.maxVelocity = maxVelocity;
        sqrMaxVelocity = maxVelocity * maxVelocity;
    }
    void OnDrawGizmosSelected()
    {
        // Draw a yellow sphere at the transform's position
        //Gizmos.color = Color.yellow;
        //Gizmos.DrawSphere(new Vector3(0,0,0), .5f);
        //Gizmos.color = Color.red;
        //foreach (Vector3 collidePoint in hitPoints) {
        //    Gizmos.DrawSphere(collidePoint, .1f);
        //}
    }

}