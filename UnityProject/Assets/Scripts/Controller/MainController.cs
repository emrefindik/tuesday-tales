using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using System;

public class MainController : MonoBehaviour
{
    public static MainController single;
	public enum GameState{
		MainMenu,
		DestroyCity,
		PhoneCamera,
		MapView
	}
	public GameState gameState;
	public GameState lastGameState;
    public Camera mainMenuCamera;
    public GameObject menuScene;
    public GameObject destroyCityPrefab;
	GameObject destroyCityScene;
	GameObject phoneCameraScene;
	public GameObject phoneCameraPrefab;
    public string currentMarkerId;
	public string markerIdAnalytics;
	public float destructionTime;
    public OwnedEgg selectedEgg;

    [SerializeField]
    private MainMenuScript _mainMenuScript;

    /*public int screams;
	public float timer; */
    //private int multiplier;
    const float COUNT_DOWN_BASE = 60.0f * 5;

    // Use this for initialization
    void Start()
    {
		markerIdAnalytics = null;
        single = this;
		gameState = GameState.MainMenu;
		//screams = 0;
		//multiplier = 1;
		//timer = COUNT_DOWN_BASE / multiplier;
    }

    // Update is called once per frame
    void Update()
    {
		if (markerIdAnalytics != null) {
			destructionTime += Time.deltaTime;
		}
		/* if (timer == 0.0) {
			// TODO:Trigger some notification
			timer = COUNT_DOWN_BASE;
			multiplier = 1;
		} */
    }

	public IEnumerator addDestoryCityReward(int amount, CoroutineResponse winCoroutineEnded)
	{
        /*screams += amount * multiplier;
		if (multiplier < 32) {
			multiplier *= 2;
			setTimer ();
		}*/

        // START OF EMRE'S CODE
        yield return SpatialClient2.single.updateLastRampageWithMultiplier(amount, currentMarkerId);
        //mainMenuCamera.GetComponent<MainMenuScript>().addCheckedLocation();
        winCoroutineEnded.setSuccess(true);
        // END OF EMRE'S CODE

        /*
        
        !!!!!

        UNCOMMENT THESE!!!

        !!!!!

        PlaytestController pcl = (PlaytestController)(mainMenuCamera.GetComponent<PlaytestController>());
		pcl.addCheckedLocation (); 
        !!!!!

        UNCOMMENT THESE!!!

        !!!!!

        !!!!! */
    }

	/*public void setTimer()
	{
		timer = COUNT_DOWN_BASE / multiplier;
	} */

	public void goBack()
	{
		Debug.Log ("go back to the place you were");
		switch (lastGameState) {
		case GameState.DestroyCity:
			goToDestroyCity (currentMarkerId);
			break;
		case GameState.MainMenu:
			goToMainMenu ();
			break;
		case GameState.MapView:
			goToMapView ();
			break;
			/*
		case GameState.PhoneCamera:
			//goToPhoneCamera ();
			break;
			*/
		default:
			Debug.Log ("No such game state.");
			goToMainMenu ();
			break;
		}
	}
		
    public void goToMainMenu()
	{
		lastGameState = gameState;
		Debug.Log ("go to main menu");

        mainMenuCamera.enabled = true;
        //menuScene.SetActive(true);
		if (destroyCityScene)
			Destroy (destroyCityScene);
		if (phoneCameraScene)
			Destroy (phoneCameraScene);

		gameState = GameState.MainMenu;
    }

	public void goToMapView()
	{
		if (destroyCityScene)
			Destroy (destroyCityScene);

		if (markerIdAnalytics != null) {
			Analytics.CustomEvent ("BuildingDestruction", new Dictionary<string,object> {
				{"BuildingId", markerIdAnalytics},
				{"PlayerId", SpatialClient2.single.userId},
				{"Time", DateTime.UtcNow.ToString()},
				{"DestructionTime", destructionTime}
			});
			markerIdAnalytics = null;
		}
		lastGameState = gameState;
		Debug.Log ("go to map");

		mainMenuCamera.enabled = true;
		//menuScene.SetActive(true);
		if (destroyCityScene)
			Destroy (destroyCityScene);
        //UniWebView mapWebView = (UniWebView)(mainMenuCamera.GetComponent<MainMenuScript> ()._webView);
        UniWebView mapWebView = _mainMenuScript._webView;
        mapWebView.Stop();
        mapWebView.url = UniWebViewHelper.streamingAssetURLForPath(MainMenuScript.MAP_ADDRESS);
        mapWebView.Load();

		gameState = GameState.MapView;
	}

    public void goToDestroyCity(string markerId)
    {
		markerIdAnalytics = markerId;
		destructionTime = 0;
        currentMarkerId = markerId;
		lastGameState = gameState;
		Debug.Log ("go to destory city gameplay");

        mainMenuCamera.enabled = false;
        //menuScene.SetActive(false);
		destroyCityScene = Instantiate (destroyCityPrefab);
		phoneCameraScene.SetActive (false);

		gameState = GameState.DestroyCity;
    }

	public void goToPhoneCamera(PhoneImageController.CameraMode mode)
	{
		Debug.Log ("go to phone camera");

		mainMenuCamera.enabled = false;
		//menuScene.SetActive(false);
		//phoneCamera.enabled = true;
		//phoneCameraScene.SetActive (true);
		phoneCameraScene = Instantiate (phoneCameraPrefab);
		phoneCameraScene.GetComponent<PhoneImageController>().initCamera(mode);

		
		gameState = GameState.PhoneCamera;
	}
}
