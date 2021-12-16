using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ballancemusicplayer : MonoBehaviour
{

    public AudioSource randomSound;
    public int mintimeBetweensounds = 30;
    public int maxtimeBetweensounds = 100;
    public AudioClip[] audioSources;


    // Start is called before the first frame update
    void Start()
    {
        CallAudioQuick();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void CallAudio()
    {
        Invoke("RandomSoundness", Random.Range(mintimeBetweensounds, maxtimeBetweensounds));
    }

    void CallAudioQuick()
    {
        Invoke("RandomSoundness", 5);
    }
 
    void RandomSoundness()
    {
        randomSound.clip = audioSources[Random.Range(0, audioSources.Length)];
        randomSound.Play ();
        CallAudio ();
    }
}
