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

	const string MAP_ADDRESS = "playtest.html";

	const string JS_INIT_MAP_METHOD_NAME = "loadMap";
	const string JS_UPDATE_CURRENT_LOCATION_NAME = "updateCurrentLocation";
	const string JS_CHECKIN_LOCATION = "addPointToPath";

	// Used to display the map
	public UniWebView _webView;

	public Canvas _pleaseWaitCanvas;
	public Canvas _checkedInCanvas;
	public Canvas _errorCanvas;

	private CoroutineResponse seeWhatsAroundSuccess;
	private CoroutineResponse checkInSuccess;

	private bool init;
	private bool mapLoaded;

	private Path destroyPath;
	private LocationCoord currentMarker;

	public void Start()
	{
		mapLoaded = false;
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
			MainController.single.gameState = MainController.GameState.MapView;
			StartCoroutine(submit());
			init = true;
		}
		if(Input.location.status == LocationServiceStatus.Running){
			updateCurrentLocation();
		}

	}

	public IEnumerator submit()
	{
		// start location tracking
		Debug.Log("GPS ON: " + Input.location.isEnabledByUser);
		Input.location.Start();
		CoroutineResponse response = new CoroutineResponse();
		yield return checkLocationService(response);
		if (response.Success != true)
		{
			_pleaseWaitCanvas.enabled = false;
			yield break; // could not turn location service on
		}

		response.reset();
		yield return SpatialClient2.single.LoginUser(response, "hello", "hello");
		Debug.Log ("Login User Return");
		switch (response.Success)
		{
		case true:
			/*yield return SpatialClient2.single.checkFirstLogin();

			// TODO delete this
			//List<Location> locations = new List<Location>();
			//Location loc = new Location(41.5, -76.5);
			//locations.Add(loc); */

			// TODO create the buttons in _friendsCanvas
			Debug.Log("Spatial Login Succeed");
			onSeeWhatsAround();
			break;
		case false:
			// wrong credentials
			Debug.Log("Wrong User or Password");
			break;
		case null:
			// connection error (possible timeout)
			Debug.Log("Connection Error");
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

		Debug.Log("OnSeeWhatsAround:" + _webView.url);
		_webView.Load();
	}

	public void onBack()
	{
		StopAllCoroutines();
		_checkedInCanvas.enabled = false;
		_pleaseWaitCanvas.enabled = false;
		_errorCanvas.enabled = false;
	}

	/** Called when uniwebview successfully loads the HTML page */
	void onLoadComplete(UniWebView webView, bool success, string errorMessage)
	{
		if (success && !mapLoaded)
		{
			_webView.EvaluatingJavaScript(JS_INIT_MAP_METHOD_NAME + '(' +
				Input.location.lastData.latitude.ToString() + ',' +
				Input.location.lastData.longitude.ToString() + ",\"" +
				SpatialClient2.baseURL + "\",\"" +
				SpatialClient2.PROJECT_ID + "\")");
			_pleaseWaitCanvas.enabled = false;
			_webView.Show();
			Debug.Log("uniwebview is showing");
			mapLoaded = true;
		}
		else
		{
			Debug.Log(errorMessage);
		}
	}

	void updateCurrentLocation()
	{
		_webView.EvaluatingJavaScript(JS_UPDATE_CURRENT_LOCATION_NAME + '(' +
			Input.location.lastData.latitude.ToString() + ',' +
			Input.location.lastData.longitude.ToString() + ")");
	}

	public void addCheckedLocation()
	{
		Debug.Log ("Adding Checked Location");
		_webView.EvaluatingJavaScript(JS_CHECKIN_LOCATION + '(' +
			currentMarker.lat + ',' +
			currentMarker.lon + ")");
	}

	void onReceivedMessage(UniWebView webView, UniWebViewMessage message)
	{
		Debug.Log ("hi");
		Debug.Log (message.path);
		switch (message.path)
		{
		case "back":
			_webView.Hide();
			break;
		case "marker":
			// check for distance
			Debug.Log ("Receive marker message: " + message.rawMessage);
			double markerLat;
			Double.TryParse (message.args ["lat"], out markerLat);
			//Debug.Log ("markerLat: " + markerLat);
			double markerLon;
			Double.TryParse (message.args ["lon"], out markerLon);
			//Debug.Log ("markerLon:" + markerLon);
			currentMarker = new LocationCoord ();
			currentMarker.lat = markerLat;
			currentMarker.lon = markerLon;
			//Debug.Log ("Device Lat:" + Input.location.lastData.latitude);
			//Debug.Log ("Device Lon:" + Input.location.lastData.longitude);
			//if(Geography.withinDistance(Input.location.lastData.latitude, Input.location.lastData.longitude, markerLat, markerLon, 100)){
				_webView.Hide();
                // TODO get message.args and redirect to correct marker's destruction
                MainController.single.goToDestroyCity(message.args["id"]);
			//}
			//else{
				// Load Error Canvas
			//}
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