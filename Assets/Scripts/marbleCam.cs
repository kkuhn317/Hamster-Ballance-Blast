using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
[AddComponentMenu("Camera-Control/Mouse Orbit with zoom")]
public class marbleCam : MonoBehaviour {
 
    public bool editorCam = false;
    public Transform target;
    private float distance = 5.0f;
    public float targetDistance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;
 
    public float yMinLimit = -70f;
    public float yMaxLimit = 90f;
 
    public float distanceMin = .5f;
    public float distanceMax = 15f;

    public bool lookOnlyMode = false;
 
    private Rigidbody rb;
 
    float x = 0.0f;
    float y = 0.0f;
 
    // Use this for initialization
    void Start () 
    {

        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
 
        rb = GetComponent<Rigidbody>();
 
        // Make the rigid body not change rotation
        if (rb != null)
        {
            rb.freezeRotation = true;
        }

        if (editorCam) {
            distance = targetDistance;
        }
    }
 
    void Update () 
    {
        if (target) 
        {

            if (lookOnlyMode) {
                lookOnly();
                return;
            }

            if (!editorCam)
                distance = targetDistance;
            
            if ((!editorCam || Input.GetMouseButton(1)) && Time.timeScale != 0) {  // if not in editor mode or if right click is held
                x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
                
            }

            if (editorCam) {
                distance += Input.GetAxis("Mouse ScrollWheel") * -2;
            }

            y = ClampAngle(y, yMinLimit, yMaxLimit);
        
            Quaternion rotation = Quaternion.Euler(y, x, 0);
            distance = Mathf.Clamp(distance, distanceMin, distanceMax);
 
            Vector3 negDistance;
            Vector3 position;

            if (!editorCam) {
                RaycastHit[] hits;

                negDistance = new Vector3(0.0f, 0.0f, -distance);
                position = rotation * negDistance + target.position;

                hits = Physics.RaycastAll (target.position, position - target.position, distance, ~0, QueryTriggerInteraction.Ignore);

                if (hits.Length > 0)
                {
                    System.Array.Sort(hits, (x,y) => x.distance.CompareTo(y.distance)); // sort by distance

                    for(int i =0; i < hits.Length; i++) {
                            if (!hits[i].transform.gameObject.CompareTag("Player"))
                            {
                                // The closest hit that is not the player
                                distance = hits[i].distance;
                                break;
                            }
                    }

                }

            }

            negDistance = new Vector3(0.0f, 0.0f, -distance);
            position = rotation * negDistance + target.position;
 
            transform.rotation = rotation;
            transform.position = position;
        }
    }

    void lookOnly() {
		//Quaternion _rot = Quaternion.Euler(new Vector3(camRotationX, camRotationY, camRotationZ));
		//transform.rotation = Quaternion.Lerp(transform.rotation, _rot, lookSpeed * Time.deltaTime);
        transform.LookAt(target);
    }
 
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}