using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fadeToWhite : MonoBehaviour
{
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void autoFadeInAndOut() {
        animator = GetComponent<Animator>();
        GameObject.DontDestroyOnLoad(transform.parent.gameObject);
        GetComponent<AudioSource>().Play();
        animator.SetBool("fadeWhite", true);
        animator.SetBool("fadeOut", true);
        Invoke("stopFadingWhite", 1);
        Invoke("removeMe", 3);
        
    }

    void stopFadingWhite() {
        animator.SetBool("fadeWhite", false);
    }

    void removeMe() {
        Destroy(transform.parent.gameObject);
    }

    public void toWhite() {
        animator.SetBool("fadeWhite", true);
        animator.SetBool("fadeOut", false);
    }

    public void toTransparent() {
        animator.SetBool("fadeWhite", false);
        animator.SetBool("fadeOut", true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
