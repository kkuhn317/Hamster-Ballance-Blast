using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public string variablename;
    public string savename;

    public void setLevel (float sliderValue) {
        //mixer.SetFloat("MusicVol", sliderValue);
        // This is the normal method, but incorrect i think

        mixer.SetFloat(variablename, Mathf.Log10 (sliderValue) * 20);
        PlayerPrefs.SetFloat(savename, sliderValue);
    }
}
