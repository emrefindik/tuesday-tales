using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
	public GameObject destroyCityScene;
	public Camera phoneCamera;
	public GameObject phoneCameraScene;
    public string currentMarkerId;

    /*public int screams;
	public float timer; */
    //private int multiplier;
    const float COUNT_DOWN_BASE = 60.0f * 5;

    // Use this for initialization
    void Start()
    {
        single = this;
		gameState = GameState.MainMenu;
		//screams = 0;
		//multiplier = 1;
		//timer = COUNT_DOWN_BASE / multiplier;
    }

    // Update is called once per frame
    void Update()
    {
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
		case GameState.PhoneCamera:
			goToPhoneCamera ();
			break;
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
		phoneCamera.enabled = false;
		phoneCameraScene.SetActive (false);
		if (destroyCityScene)
			Destroy (destroyCityScene);

		gameState = GameState.MainMenu;
    }

	public void goToMapView()
	{
		lastGameState = gameState;
		Debug.Log ("go to map");

		mainMenuCamera.enabled = true;
		//menuScene.SetActive(true);
		phoneCamera.enabled = false;
		phoneCameraScene.SetActive (false);
		if (destroyCityScene)
			Destroy (destroyCityScene);
		UniWebView mapWebView = (UniWebView)(mainMenuCamera.GetComponent<MainMenuScript> ()._webView);
        mapWebView.Stop();
        mapWebView.url = UniWebViewHelper.streamingAssetURLForPath(MainMenuScript.MAP_ADDRESS);
        mapWebView.Load();

		gameState = GameState.MapView;
	}

    public void goToDestroyCity(string markerId)
    {
        currentMarkerId = markerId;
		lastGameState = gameState;
		Debug.Log ("go to destory city gameplay");

        mainMenuCamera.enabled = false;
        //menuScene.SetActive(false);
		destroyCityScene = Instantiate (destroyCityPrefab);
		phoneCamera.enabled = false;
		phoneCameraScene.SetActive (false);

		gameState = GameState.DestroyCity;
    }

	public void goToPhoneCamera()
	{
		Debug.Log ("go to phone camera");

		mainMenuCamera.enabled = false;
		//menuScene.SetActive(false);
		phoneCamera.enabled = true;
		phoneCameraScene.SetActive (true);
		if (destroyCityScene)
			Destroy (destroyCityScene);
		
		gameState = GameState.PhoneCamera;
	}
}
