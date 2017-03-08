using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuScript : MonoBehaviour {

    // frequency of queries on whether location service is enabled, in number per second
    const int LOCATION_INITIALIZED_QUERIES_PER_SECOND = 4;

    // time before giving up on enabling location service, in seconds
    const int LOCATION_INITIALIZED_QUERY_TIMEOUT = 20;

    public Canvas _mainMenuCanvas;
    public Canvas _mapCanvas;
    public Canvas _pleaseWaitCanvas;
    public Canvas _checkedInCanvas;
    public Canvas _errorCanvas;
    public Canvas _eggsCanvas;

    private BoolWrapper seeWhatsAroundSuccess;
    private BoolWrapper checkInSuccess;

	// Use this for initialization
	void Start () {
        EggController.instance.updateEggs();

        _mainMenuCanvas.enabled = true;
        _mapCanvas.enabled = false;
        _pleaseWaitCanvas.enabled = false;
        _checkedInCanvas.enabled = false;
        _errorCanvas.enabled = false;
        _eggsCanvas.enabled = false;

        // initialize the bool wrappers
        seeWhatsAroundSuccess = new BoolWrapper();
        checkInSuccess = new BoolWrapper();

        Debug.Log(Input.location.isEnabledByUser);

        // start location tracking
        Input.location.Start();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void onEggs()
    {
        _mainMenuCanvas.enabled = false;
        _eggsCanvas.enabled = true;
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
        checkLocationServiceIsOn();
        StartCoroutine(seeWhatsAround());
    }

    public void onBack()
    {
        StopAllCoroutines();
        _checkedInCanvas.enabled = false;
        _eggsCanvas.enabled = false;
        _pleaseWaitCanvas.enabled = false;
        _mapCanvas.enabled = false;
        _errorCanvas.enabled = false;
        _mainMenuCanvas.enabled = true;
    }

    IEnumerator seeWhatsAround()
    {

        GoogleMap googleMaps = GetComponent<GoogleMap>();

        // get location data and plug it into googleMaps
        seeWhatsAroundSuccess.value = false;
        yield return checkLocationService(seeWhatsAroundSuccess);
        if (!seeWhatsAroundSuccess.value)
        {
            _pleaseWaitCanvas.enabled = false;
            _errorCanvas.enabled = true;
            yield break; // could not get location
        }
        googleMaps.centerLocation.address = "";
        googleMaps.centerLocation.latitude = 40.432633f;//Input.location.lastData.latitude; TODO uncomment this
        googleMaps.centerLocation.longitude = -79.964973f;// Input.location.lastData.longitude; TODO uncomment this

        // get markers from spatial and plug them into googleMaps
        yield return SpatialClient2.single.GetMarkersByDistance(
            Input.location.lastData.longitude, Input.location.lastData.latitude);
        // TODO handle error from spatial

        Debug.Log(SpatialClient2.single.markers.Count);
        googleMaps.swapMarkersAndRefresh(SpatialClient2.single.markers);
        /* googleMaps.markers = new GoogleMapMarker[1]; // this will change when we have different types of markers
        googleMaps.markers[0] = new GoogleMapMarker();
        googleMaps.markers[0].locations = new GoogleMapLocation[SpatialClient2.single.markers.Count];
        googleMaps.markers[0].size = GoogleMapMarker.GoogleMapMarkerSize.Small;
        for (int index = 0; index < googleMaps.markers[0].locations.Length; index++)
        {
            googleMaps.markers[0].locations[index] = new GoogleMapLocation("",
                (float)(SpatialClient2.single.markers[index].loc.coordinates[0]),
                (float)(SpatialClient2.single.markers[index].loc.coordinates[1]));
        }
        googleMaps.Refresh(); */

        _pleaseWaitCanvas.enabled = false;
        _mapCanvas.enabled = true;

    }

    // assigns true to result.value if location service is ready, otherwise assigns false
    IEnumerator checkLocationService(BoolWrapper result)
    {
        // base code from https://docs.unity3d.com/ScriptReference/LocationService.Start.html
        // Wait until service initializes
        int maxWait = LOCATION_INITIALIZED_QUERY_TIMEOUT * LOCATION_INITIALIZED_QUERIES_PER_SECOND;
        float timeBetweenQueries = 1.0f / LOCATION_INITIALIZED_QUERIES_PER_SECOND;
        while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
        {
            yield return new WaitForSeconds(timeBetweenQueries);
            maxWait--;
        }

        // false if location service disabled or initialization timed out, true otherwise
        result.value = !(Input.location.status == LocationServiceStatus.Failed || maxWait < 1);
    }

    private void checkLocationServiceIsOn()
    {
        if (Input.location.status == LocationServiceStatus.Failed)
        {
            Input.location.Stop();
        }
        if (Input.location.status == LocationServiceStatus.Stopped)
        {
            Input.location.Start();
        }
    }

}