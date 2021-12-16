using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trapdoor : MonoBehaviour
{
    Animator animator;
    bool activated = false;
    private AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (activated = true && animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "idle") {
            activated = false;
        }
    }

    public void CollisionDetected(Collision other) {
        if ((other.gameObject.tag == "Player" || other.gameObject.tag == "8Ball" || other.gameObject.tag == "fakePlayer") && LayerMask.LayerToName(other.gameObject.layer) != "Paper" && !activated) {
            activated = true;
            Invoke("activate", 0.1f);
        }
    }

    public void CollisionStayDetected(Collision other) {
        CollisionDetected(other);
    }

    public void activate() {
        if (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "collapse") {
            animator.SetTrigger("Activate");
            audioSource.Play();
            Invoke("makeSound", 5);
        }
    }

    public void makeSound() {
        audioSource.Play();
    }



}
