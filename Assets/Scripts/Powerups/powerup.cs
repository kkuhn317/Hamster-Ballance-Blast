using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class powerup : MonoBehaviour
{
    public float rotateSpeed = 150f;
    public bool uiMode = false;
    public bool active = true;
    public int id = 0;
    // super jump: 0
    // super speed: 1
    // super bounce: 2
    // shock absorber: 3
    // time travel: 4

    public bool timeTravelMode = false; // reserved for time travel powerup

    
    void Update() {
        // constantly rotate the powerup around the y axis
        transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);

    }



    void OnTriggerStay(Collider other) {
        // if the powerup collides with the player, make it dissapear for 5 seconds
        if (other.gameObject.tag == "Player" && active && !uiMode) {
            dissapear();
            GetComponent<AudioSource>().Play();
            
            if (timeTravelMode) {
                // time travel mode
                // stop the timer for 5 seconds
                GameObject timer = GameObject.FindGameObjectWithTag("levelTimer");
                timer.GetComponent<timer>().timeTravelActivate();
                Destroy(gameObject, 5);
            
            }
            else {
                globalVariables.storedPowerup = id;
                Invoke("reappear", 5);
            }
        }
    }

    public void dissapear() {
        StopAllCoroutines();    // stop reappear coroutine
        active = false;
        // make the powerup dissapear by making the material transparent
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers) {
            MaterialExtensions.ToFadeMode(renderer.material);
            renderer.material.color = new Color(renderer.material.color.r, renderer.material.color.g, renderer.material.color.b, 0);
        }

    }

    public void reappear() {
        active = true;
        StartCoroutine(FadeTo(1, 1f));

    }

    IEnumerator FadeTo(float aValue, float aTime)   // note: can only fade from transparent
    {
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        float alpha = 0;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime)
        {
            Color newColor = new Color(1, 1, 1, Mathf.Lerp(alpha, aValue, t));
            foreach (Renderer renderer in renderers) {
                renderer.material.color = newColor;
            }
            yield return null;
        }
        foreach (Renderer renderer in renderers) {
            MaterialExtensions.ToOpaqueMode(renderer.material);
        }

    }

}
