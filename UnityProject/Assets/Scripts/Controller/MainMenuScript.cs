using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    // time interval between location marker updates on map, in seconds
    const float LOCATION_MARKER_UPDATE_INTERVAL = 4.0f;

    // frequency of queries on whether location service is enabled, in number per second
    const int LOCATION_INITIALIZED_QUERIES_PER_SECOND = 4;

    // time before giving up on enabling location service, in seconds
    const int LOCATION_INITIALIZED_QUERY_TIMEOUT = 20;

    // the scene index of the destruction scene in the build settings
    const int DESTRUCTION_SCENE_INDEX = 1;

    public const string MAP_ADDRESS = "map.html";

    const string JS_INIT_MAP_METHOD_NAME = "loadMap";
    const string JS_UPDATE_CURRENT_LOCATION_NAME = "updateCurrentLocation";
    const string JS_CHECKIN_LOCATION = "addPointToPath";

    private static Canvas errorCanvas;
    public static Canvas ErrorCanvas
    {
        get
        {
            return errorCanvas;
        }
        private set
        {
            errorCanvas = value;
        }
    }

    // Displays all of the player's own eggs
    private static Canvas eggsCanvas;
    public static Canvas EggsCanvas
    {
        get
        {
            return eggsCanvas;
        }
        private set
        {
            eggsCanvas = value;
        }
    }

    // Temporary. May get rid of it if we decided to get rid of sending eggs completely
    private static Canvas friendsCanvas;
    public static Canvas FriendsCanvas
    {
        get
        {
            return friendsCanvas;
        }
        private set
        {
            friendsCanvas = value;
        }
    }

    // Displays all the eggs from all the friends
    private static Canvas friendsEggsCanvas;
    public static Canvas FriendsEggsCanvas
    {
        get
        {
            return friendsEggsCanvas;
        }
        private set
        {
            friendsEggsCanvas = value;
        }
    }

    private static Canvas checkedInCanvas;
    public static Canvas CheckedInCanvas
    {
        get
        {
            return checkedInCanvas;
        }
        private set
        {
            checkedInCanvas = value;
        }
    }

    private static Canvas checkInErrorCanvas;
    public static Canvas CheckInErrorCanvas
    {
        get
        {
            return checkInErrorCanvas;
        }
        private set
        {
            checkInErrorCanvas = value;
        }
    }

    private static Text checkInErrorMessage;
    public static Text CheckInErrorMessage
    {
        get
        {
            return checkInErrorMessage;
        }
        private set
        {
            checkInErrorMessage = value;
        }
    }

    // the canvas that directed to the wait screen
    private static Canvas previousCanvas;

    private static Canvas pleaseWaitCanvas;

    private bool mapLoaded;

    private Path destroyPath;

    // Used to display the map
    [SerializeField]
    private UniWebView _webView;

    [SerializeField]
    private Canvas _mainMenuCanvas;
    [SerializeField]
    private Canvas _mapCanvas;
    [SerializeField]
    private Canvas _pleaseWaitCanvas;
    [SerializeField]
    private Canvas _checkedInCanvas;
    [SerializeField]
    private Canvas _checkInErrorCanvas;
    [SerializeField]
    private Canvas _locationStartErrorCanvas;

    // Displays all of the player's own eggs
    [SerializeField]
    private Canvas _eggsCanvas;

    // Displays all the eggs from all the friends
    [SerializeField]
    private Canvas _friendsEggsCanvas;

    // Displays your list of friends for sending an egg
    [SerializeField]
    private Canvas _friendsCanvas;
    [SerializeField]
    private Canvas _loginCanvas;
    [SerializeField]
    private Text _wrongPasswordText;
    [SerializeField]
    private Text _connectionErrorText;
    [SerializeField]
    private GameObject _eggMenuItemPrefab;
    [SerializeField]
    private Transform _eggMenuContentPanel;
    [SerializeField]
    private GameObject _friendMenuItemPrefab;
    [SerializeField]
    private Transform _friendMenuContentPanel;
    [SerializeField]
    private GameObject _friendEggMenuItemPrefab;
    [SerializeField]
    private Transform _friendEggMenuContentPanel;
    [SerializeField]
    private Text _checkInErrorMessage;
    [SerializeField]
    private InputField userNameField;
    [SerializeField]
    private InputField passwordField;

    private Coroutine _locationUpdateCoroutine;


    // Use this for initialization
    void Start()
    {
        _locationUpdateCoroutine = null;

        errorCanvas = _locationStartErrorCanvas;
        eggsCanvas = _eggsCanvas;
        checkedInCanvas = _checkedInCanvas;
        friendsCanvas = _friendsCanvas;
        friendsEggsCanvas = _friendsEggsCanvas;
        checkInErrorCanvas = _checkInErrorCanvas;
        checkInErrorMessage = _checkInErrorMessage;
        pleaseWaitCanvas = _pleaseWaitCanvas;

        _loginCanvas.enabled = true;
        _wrongPasswordText.enabled = false;
        _connectionErrorText.enabled = false;
        _mainMenuCanvas.enabled = false;
        _mapCanvas.enabled = false;
        _pleaseWaitCanvas.enabled = false;
        mapLoaded = false;
        _checkedInCanvas.enabled = false;
        _locationStartErrorCanvas.enabled = false;
        _eggsCanvas.enabled = false;
        _friendsEggsCanvas.enabled = false;
        _friendsCanvas.enabled = false;
        _checkInErrorCanvas.enabled = false;

        _webView.url = UniWebViewHelper.streamingAssetURLForPath(MAP_ADDRESS);
        _webView.OnLoadComplete += onLoadComplete;
        _webView.OnReceivedMessage += onReceivedMessage;

    }

    public void Update()
    {

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
            _locationStartErrorCanvas.enabled = true;
            yield break; // could not turn location service on
        }

        response.reset();
        yield return SpatialClient2.single.LoginUser(response, userNameField.text, passwordField.text);
        switch (response.Success)
        {
            case true:
                // initialize egg menu
                addButtons();

                // logged in, switch to main menu
                _connectionErrorText.enabled = false;
                _wrongPasswordText.enabled = false;
                _loginCanvas.enabled = false;
                _mainMenuCanvas.enabled = true;
                break;
            case false:
                // wrong credentials
                _connectionErrorText.enabled = false;
                _wrongPasswordText.enabled = true;
                Debug.Log("Wrong User or Password");
                break;
            case null:
                // connection error (possible timeout)
                _wrongPasswordText.enabled = false;
                _connectionErrorText.enabled = true;
                Debug.Log("Connection Error");
                break;
        }

    }

    public void onEggs()
    {
        _mainMenuCanvas.enabled = false;
        _eggsCanvas.enabled = true;
    }

    public void onFriendsEggs()
    {
        _mainMenuCanvas.enabled = false;
        _friendsEggsCanvas.enabled = true;
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
        _mainMenuCanvas.enabled = false;
        _pleaseWaitCanvas.enabled = true;
        checkLocationServiceIsOn();
        Debug.Log("OnSeeWhatsAround:" + _webView.url);
        _webView.Load();
    }

    public void onBack()
    {
        _checkedInCanvas.enabled = false;
        _checkInErrorCanvas.enabled = false;
        _eggsCanvas.enabled = false;
        _pleaseWaitCanvas.enabled = false;
        _mapCanvas.enabled = false;
        _friendsEggsCanvas.enabled = false;
        _friendsCanvas.enabled = false;
        _mainMenuCanvas.enabled = true;
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
                SpatialClient2.PROJECT_ID + "\"," +
                SpatialClient2.single.getScore().ToString() + ',' +
                SpatialClient2.single.getTimer().ToString() + ',' +
                SpatialClient2.single.getStreakPathAsJsonString() + ')');
            _pleaseWaitCanvas.enabled = false;
            _webView.Show();
            Debug.Log("uniwebview is showing");
            mapLoaded = true;

            // EMRE'S ADDITION
            _locationUpdateCoroutine = StartCoroutine(updateCurrentLocation());
        }
        else
        {
            Debug.Log(errorMessage);
        }
    }

    IEnumerator updateCurrentLocation()
    {
        if (Input.location.status == LocationServiceStatus.Running)
        {
            _webView.EvaluatingJavaScript(JS_UPDATE_CURRENT_LOCATION_NAME + '(' +
            Input.location.lastData.latitude.ToString() + ',' +
            Input.location.lastData.longitude.ToString() + ")");
            yield return new WaitForSeconds(LOCATION_MARKER_UPDATE_INTERVAL);
        }
    }

    public void addCheckedLocation()
    {
        /*Debug.Log("Adding Checked Location");
        _webView.EvaluatingJavaScript(JS_CHECKIN_LOCATION + '(' +
            currentMarker.lat + ',' +
            currentMarker.lon + ")"); */
        _webView.EvaluatingJavaScript(
            JS_CHECKIN_LOCATION + '(' + SpatialClient2.single.getStreakPathAsJsonString() + ')');
    }

    void onReceivedMessage(UniWebView webView, UniWebViewMessage message)
    {
        Debug.Log("hi");
        Debug.Log(message.path);
        switch (message.path)
        {
            case "back":
                StopCoroutine(_locationUpdateCoroutine);
                _mainMenuCanvas.enabled = true;
                _webView.Hide();
                break;
            case "marker":
                StopCoroutine(_locationUpdateCoroutine);
                // check for distance
                Debug.Log("Receive marker message: " + message.rawMessage);
                double markerLat;
                Double.TryParse(message.args["lat"], out markerLat);
                //Debug.Log ("markerLat: " + markerLat);
                double markerLon;
                Double.TryParse(message.args["lon"], out markerLon);
                //Debug.Log ("markerLon:" + markerLon);
                /*currentMarker.lat = markerLat;
                currentMarker.lon = markerLon; */
                //Debug.Log ("Device Lat:" + Input.location.lastData.latitude);
                //Debug.Log ("Device Lon:" + Input.location.lastData.longitude);

                // TODO REPLACE THIS AFTER THE PLAYTEST

                if(Geography.withinDistance(Input.location.lastData.latitude, Input.location.lastData.longitude, markerLat, markerLon, FriendEggMenuItem.MAX_CHECK_IN_DISTANCE)){
                    _friendsEggsCanvas.enabled = false;
                    _eggsCanvas.enabled = false;
                    _checkedInCanvas.GetComponent<Text>().text =
                        "Checked in " + "egg" + " at " + Input.location.lastData.latitude.ToString()
                        + ", " + Input.location.lastData.longitude.ToString();
                    _checkedInCanvas.enabled = true;
                    _webView.Hide();
                // TODO get message.args and redirect to correct marker's destruction

                    // TODO check whether player is checking in or destroying city
                    MainController.single.goToDestroyCity(message.args["id"]);
                }
                else{
                // Load Error Canvas
                }

                // END OF PART TO BE REPLACED AFTER THE PLAYTEST

                break;
            case "resetscore":
                StartCoroutine(SpatialClient2.single.resetStreak());
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

    /*public void onTestDestruction()
    {
        Debug.Log("multiplier: " + SpatialClient2.single.getMultiplier().ToString());
        Debug.Log("score: " + SpatialClient2.single.getScore().ToString());
        //SceneManager.LoadScene(DESTRUCTION_SCENE_INDEX);
        MainController.single.goToDestroyCity();
    } */

    public void onTestCamera()
    {
        //SceneManager.LoadScene(DESTRUCTION_SCENE_INDEX);
        MainController.single.goToPhoneCamera();
    }

    public void onBackToYourEggs()
    {
        _friendsCanvas.enabled = false;
        _eggsCanvas.enabled = true;
    }

    public void onBackFromLocationStartError()
    {
        _locationStartErrorCanvas.enabled = false;
        if (!SpatialClient2.single.isLoggedIn())
            // the user has not yet logged in
            _loginCanvas.enabled = true;
        else
            _mainMenuCanvas.enabled = true;
    }

    public void onBackFromFriendSelection()
    {
        _friendsCanvas.enabled = false;
        _eggsCanvas.enabled = true;
    }

    public void addButtons()
    {
        foreach (OwnedEgg e in SpatialClient2.single.EggsOwned)
        {
            GameObject eggMenuItem = GameObject.Instantiate(_eggMenuItemPrefab);
            eggMenuItem.transform.SetParent(_eggMenuContentPanel, false);
            eggMenuItem.GetComponent<EggMenuItem>().Egg = e; // also updates the egg menu item's view
        }
        foreach (FriendData fd in SpatialClient2.single.Friends)
        {
            GameObject friendMenuItem = GameObject.Instantiate(_friendMenuItemPrefab);
            friendMenuItem.transform.SetParent(_friendMenuContentPanel, false);
            friendMenuItem.GetComponent<FriendMenuItem>().Friend = fd;
            foreach (OwnedEgg e in fd.Friend.Metadata.EggsOwned)
            {
                GameObject friendEggMenuItem = GameObject.Instantiate(_friendEggMenuItemPrefab);
                friendEggMenuItem.transform.SetParent(_friendEggMenuContentPanel, false);
                friendEggMenuItem.GetComponent<EggMenuItem>().Egg = e; // also updates the egg menu item's view
            }
        }
    }

    public static void displayWaitScreen()
    {
        foreach (Canvas c in FindObjectsOfType<Canvas>())
        {
            if (c.enabled && c != pleaseWaitCanvas)
            {
                previousCanvas = c;
                c.enabled = false;
                pleaseWaitCanvas.enabled = true;
                return;
            }
        }
    }

    public static void displayError(string errorText)
    {
        checkInErrorMessage.text = errorText;
        foreach (Canvas c in FindObjectsOfType<Canvas>())
        {
            if (c.enabled)
            {
                c.enabled = false;
                checkInErrorCanvas.enabled = true;
                return;
            }
        }
    }

    public static void displayErrorFromWaitScreen(string errorText)
    {
        pleaseWaitCanvas.enabled = false;
        displayError(errorText);
    }

    public static void closeWaitScreen()
    {
        pleaseWaitCanvas.enabled = false;
        previousCanvas.enabled = true;
    }
}