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
        _mainMenuCanvas.enabled = true;
        _mapCanvas.enabled = false;
        _pleaseWaitCanvas.enabled = false;
        _checkedInCanvas.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onCheckIn()
    {
        _mainMenuCanvas.enabled = false;
        _pleaseWaitCanvas.enabled = true;

        // get nearby marker from spatial, if there is none, create one

        _pleaseWaitCanvas.enabled = false;
        _checkedInCanvas.enabled = true;
    }

    public void onSeeWhatsAround()
    {
        _mainMenuCanvas.enabled = false;
        _pleaseWaitCanvas.enabled = true;

        GoogleMap googleMaps = GetComponent<GoogleMap>();

        // get location data and plug it into googleMaps
        // get markers from spatial and plug them into googleMaps

        googleMaps.Refresh();

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
