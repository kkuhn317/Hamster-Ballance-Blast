using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class setMouseSensitivity : MonoBehaviour
{
    // there is a difference between this slider script and the volume slider script
    // the other script sets the mixer volume when changed, this script only changes the playerprefs value
    // the playerprefs value for this script is used in the movement scripts directly
    // while the playerprefs value for the volume slider is just used for saving and loading

    // that means that the settings menu has to be open to change the volume, but not the sensitivity


    public string savename;

    public void setLevel (float sliderValue) {
        PlayerPrefs.SetFloat(savename, sliderValue);
    }


    // Start is called before the first frame update
    void Start()
    {
        GetComponent<Slider>().value = PlayerPrefs.GetFloat(savename, .5f);  // update the slider to the value stored in the playerprefs
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
