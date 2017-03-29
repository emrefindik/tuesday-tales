using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlaytestController : MonoBehaviour {

	// frequency of queries on whether location service is enabled, in number per second
	const int LOCATION_INITIALIZED_QUERIES_PER_SECOND = 4;

	// time before giving up on enabling location service, in seconds
	const int LOCATION_INITIALIZED_QUERY_TIMEOUT = 20;

	// the scene index of the destruction scene in the build settings
	const int DESTRUCTION_SCENE_INDEX = 1;

	const string MAP_ADDRESS = "map.html";

	const string JS_INIT_MAP_METHOD_NAME = "loadMap";

	// Used to display the map
	public UniWebView _webView;

	public Canvas _mapCanvas;
	public Canvas _pleaseWaitCanvas;
	public Canvas _checkedInCanvas;
	public Canvas _errorCanvas;

	private CoroutineResponse seeWhatsAroundSuccess;
	private CoroutineResponse checkInSuccess;

	private bool init;

	public void Start()
	{

		_mapCanvas.enabled = false;
		_pleaseWaitCanvas.enabled = true;
		_checkedInCanvas.enabled = false;
		_errorCanvas.enabled = false;

		// initialize the bool wrappers
		seeWhatsAroundSuccess = new CoroutineResponse();
		checkInSuccess = new CoroutineResponse();

		_webView.url = UniWebViewHelper.streamingAssetURLForPath(MAP_ADDRESS);
		_webView.OnLoadComplete += onLoadComplete;
		_webView.OnReceivedMessage += onReceivedMessage;

	}

	public void Update()
	{
		if (!init)
		{
			StartCoroutine(submit());
			init = true;
		}
	}

	public IEnumerator submit()
	{
		CoroutineResponse response = new CoroutineResponse();
		yield return SpatialClient2.single.LoginUser(response, "hello", "hello");
		switch (response.Success)
		{
		case true:

			// indicates whether this is the user's first login
			bool firstLogin = false;

			if (SpatialClient2.single.userSession.user.metadata.eggsOwned == null)
			{
				SpatialClient2.single.userSession.user.metadata.eggsOwned = new List<OwnedEgg>();
				firstLogin = true;
			}
			if (SpatialClient2.single.userSession.user.metadata.friendsEggs == null)
			{
				SpatialClient2.single.userSession.user.metadata.friendsEggs = new List<OwnedEgg>();
				firstLogin = true;
			}
			if (firstLogin)
			{
				// update metadata on Spatial to contain empty lists
				yield return SpatialClient2.single.UpdateMeta();
			}

			Debug.Log(Input.location.isEnabledByUser);

			// start location tracking
			Input.location.Start();

			_mapCanvas.enabled = true;
			_pleaseWaitCanvas.enabled = false;

			break;
		default:
			break;
		}

	}

	public void onCheckIn()
	{
		_pleaseWaitCanvas.enabled = true;

		// get nearby marker from spatial, if there is none, create one

		_pleaseWaitCanvas.enabled = false;
		_checkedInCanvas.enabled = true;
	}

	public void onSeeWhatsAround()
	{
		_pleaseWaitCanvas.enabled = true;
		checkLocationServiceIsOn();
		//StartCoroutine(seeWhatsAround());
		Debug.Log(_webView.url);
		_webView.Load();
	}

	public void onBack()
	{
		StopAllCoroutines();
		_checkedInCanvas.enabled = false;
		_pleaseWaitCanvas.enabled = false;
		_mapCanvas.enabled = true;
		_errorCanvas.enabled = false;
	}

	IEnumerator seeWhatsAround()
	{

		GoogleMap googleMaps = GetComponent<GoogleMap>();

		// get location data and plug it into googleMaps
		yield return checkLocationService(seeWhatsAroundSuccess);
		if (seeWhatsAroundSuccess.Success != true)
		{
			_pleaseWaitCanvas.enabled = false;
			_errorCanvas.enabled = true;
			yield break; // could not get location
		}
		googleMaps.centerLocation.address = "";
		googleMaps.centerLocation.latitude = Input.location.lastData.latitude; // UNCOMMENT THIS IF TESTING ON COMPUTER AND WANT TO USE ETC LOCATION 40.432633f;
		googleMaps.centerLocation.longitude = Input.location.lastData.longitude; // UNCOMMENT THIS IF TESTING ON COMPUTER AND WANT TO USE ETC LOCATION -79.964973f;

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

	/** Called when uniwebview successfully loads the HTML page */
	void onLoadComplete(UniWebView webView, bool success, string errorMessage)
	{
		if (success)
		{
			_webView.EvaluatingJavaScript(JS_INIT_MAP_METHOD_NAME + '(' +
				Input.location.lastData.latitude.ToString() + ',' +
				Input.location.lastData.longitude.ToString() + ',' +
				SpatialClient2.baseURL + ',' +
				SpatialClient2.PROJECT_ID + ')');
			_pleaseWaitCanvas.enabled = false;
			_webView.Show();
			Debug.Log("Load Uniweb Complete.");
		}
		else
		{
			Debug.Log(errorMessage);
		}
	}

	void onReceivedMessage(UniWebView webView, UniWebViewMessage message)
	{
		Debug.Log ("Receiving Message from Uniweb");
		Debug.Log (message.path);
		switch (message.path)
		{
		case "back":
			// TODO:
			_webView.Hide();
			break;
		case "marker":
			_webView.Hide();
			// TODO get message.args and redirect to correct marker's destruction
			MainController.single.goToDestroyCity();
			break;
		default:
			break;
		}
	}

	// assigns true to result.value if location service is ready, otherwise assigns false
	IEnumerator checkLocationService(CoroutineResponse result)
	{
		// make sure result's success value is null before beginning
		result.reset();

		// base code from https://docs.unity3d.com/ScriptReference/LocationService.Start.html
		// Wait until service initializes
		int maxWait = LOCATION_INITIALIZED_QUERY_TIMEOUT * LOCATION_INITIALIZED_QUERIES_PER_SECOND;
		float timeBetweenQueries = 1.0f / LOCATION_INITIALIZED_QUERIES_PER_SECOND;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
			yield return new WaitForSeconds(timeBetweenQueries);
			maxWait--;
		}

		if (maxWait > 0)
		{
			// false if location service disabled, null if timed out, true otherwise
			result.setSuccess(!(Input.location.status == LocationServiceStatus.Failed));
		}
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