using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour {

    public Canvas _mainMenuCanvas;
    public Canvas _mapCanvas;
    public Canvas _pleaseWaitCanvas;
    public Canvas _checkedInCanvas;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onCheckIn()
    {
        _mainMenuCanvas.enabled = false;
        _pleaseWaitCanvas.enabled = true;

        // get nearby marker, if there is none, create one

        _pleaseWaitCanvas.enabled = false;
        _checkedInCanvas.enabled = true;
    }

    public void onSeeWhatsAround()
    {
        _mainMenuCanvas.enabled = false;
        _pleaseWaitCanvas.enabled = true;

        // get the map info and the surrounding markers

        _pleaseWaitCanvas.enabled = false;
        _mapCanvas.enabled = true;
    }

    public void onBack()
    {
        _checkedInCanvas.enabled = false;
        _mapCanvas.enabled = false;
        _mainMenuCanvas.enabled = true;
    }

}
