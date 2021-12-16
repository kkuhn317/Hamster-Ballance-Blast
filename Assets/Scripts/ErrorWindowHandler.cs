using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ErrorWindowHandler : MonoBehaviour
{
    private bool render = false;
    private Rect windowRect = new Rect (Screen.width / 2 - 250, Screen.height / 2 -250,500,500);

    private string message = "Unknown Error";


    public void ShowWindow(string message) {
        this.message = message;
        render = true;
    }
    public void HideWindow() {
        render = false;
    }
    public void OnGUI() {
        if (render) {
            windowRect = GUI.ModalWindow(0, windowRect, DoMyWindow, "Error!");
        }
    }
    public void DoMyWindow(int windowID) {
        GUI.Label(new Rect(0, 20, windowRect.width, windowRect.height), message);
        GUI.Label(new Rect(0, 50, windowRect.width, windowRect.height), "Also sorry for this horrible looking error window, I'll improve it eventually");
        if (GUI.Button (new Rect (windowRect.width / 2 - 25, windowRect.height / 2 - 25, 50, 50), "OK"))
            HideWindow();
    }
}
