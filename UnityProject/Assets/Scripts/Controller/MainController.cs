using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using System;

public class MainController : MonoBehaviour
{
    public static MainController single;
	public enum GameState{
		//MainMenu,
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

	public GameObject phoneCameraPrefab;
	GameObject phoneCameraScene;

    public string currentMarkerId;
	public string markerIdAnalytics;

	public float destructionTime;
    public OwnedEgg selectedEgg;

    [SerializeField]
    private MainMenuScript _mainMenuScript;

    // Use this for initialization
    void Start()
    {
		markerIdAnalytics = null;
        single = this;
		gameState = GameState.MapView;

		// FOR TEST: NICKY
		//phoneCameraScene = Instantiate(phoneCameraPrefab);
		//phoneCameraScene.GetComponent<PhoneImageController>().initCamera (PhoneImageController.CameraMode.EggHatching);
    }

    // Update is called once per frame
    void Update()
    {
		if (markerIdAnalytics != null) {
			destructionTime += Time.deltaTime;
		}
    }

	public IEnumerator addDestoryCityReward(int amount, CoroutineResponse winCoroutineEnded)
	{

        // START OF EMRE'S CODE
        yield return SpatialClient2.single.updateLastRampageWithMultiplier(amount, currentMarkerId);
        //mainMenuCamera.GetComponent<MainMenuScript>().addCheckedLocation();
        winCoroutineEnded.setSuccess(true);
        // END OF EMRE'S CODE

    }
		
	public void goBack()
	{
		Debug.Log ("go back to the place you were");
		switch (lastGameState) {
		case GameState.DestroyCity:
			goToDestroyCity (currentMarkerId);
			break;
		//case GameState.MainMenu:
		//	goToMainMenu ();
			break;
		case GameState.MapView:
			goToMapView ();
			break;
		default:
			Debug.Log ("No such game state.");
		//	goToMainMenu ();
			goToMapView();
			break;
		}
	}

	/*
    public void goToMainMenu()
	{
		lastGameState = gameState;
		Debug.Log ("go to main menu");

        mainMenuCamera.enabled = true;
		if (destroyCityScene)
			Destroy (destroyCityScene);
		if (phoneCameraScene)
			Destroy (phoneCameraScene);

		gameState = GameState.MainMenu;
    }
    */

	public void goToMapView()
	{
		if (destroyCityScene)
			Destroy (destroyCityScene);

		if (phoneCameraScene)
			Destroy (phoneCameraScene);

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
        //UniWebView mapWebView = (UniWebView)(mainMenuCamera.GetComponent<MainMenuScript> ()._webView);
		MessageController.single.displayWaitScreen(null);
		UniWebView mapWebView = GetComponent<MainMenuScript>()._webView;
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
		if (phoneCameraScene) {
			phoneCameraScene.SetActive (false);
		}

		gameState = GameState.DestroyCity;
    }

	public void goToPhoneCamera(PhoneImageController.CameraMode mode)
	{
		Debug.Log ("go to phone camera");

		mainMenuCamera.enabled = false;
		GetComponent<MainMenuScript> ().disableWebview ();

		phoneCameraScene = Instantiate (phoneCameraPrefab);
		phoneCameraScene.GetComponent<PhoneImageController>().startCameraWithMode(mode);

		
		gameState = GameState.PhoneCamera;
	}

	public void eggToPhotoCamera()
	{
		MainMenuScript.EggsCanvas.enabled = false;
		goToPhoneCamera (PhoneImageController.CameraMode.EggHatching);
	}

	public void kaijuToPhoneCamera()
	{
		MainMenuScript.KaijuCanvas.enabled = false;
		goToPhoneCamera (PhoneImageController.CameraMode.Kaiju);
	}

	public void cityToPhoneCamera()
	{
		goToPhoneCamera (PhoneImageController.CameraMode.BuildingDestruction);
	}

}
