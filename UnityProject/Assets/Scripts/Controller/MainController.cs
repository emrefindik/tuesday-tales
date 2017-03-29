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

    // Use this for initialization
    void Start()
    {
        single = this;
		gameState = GameState.MainMenu;
    }

    // Update is called once per frame
    void Update()
    {

    }

	public void goBack()
	{
		Debug.Log ("go back to the place you were");
		switch (lastGameState) {
		case GameState.DestroyCity:
			goToDestroyCity ();
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
        menuScene.SetActive(true);
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
		menuScene.SetActive(true);
		phoneCamera.enabled = false;
		phoneCameraScene.SetActive (false);
		if (destroyCityScene)
			Destroy (destroyCityScene);
		UniWebView mapWebView = (UniWebView)(mainMenuCamera.GetComponent<PlaytestController> ()._webView);
		mapWebView.Show ();

		gameState = GameState.MapView;
	}


    public void goToDestroyCity()
    {
		lastGameState = gameState;
		Debug.Log ("go to destory city gameplay");

        mainMenuCamera.enabled = false;
        menuScene.SetActive(false);
		destroyCityScene = Instantiate (destroyCityPrefab);
		phoneCamera.enabled = false;
		phoneCameraScene.SetActive (false);

		gameState = GameState.DestroyCity;
    }

	public void goToPhoneCamera()
	{
		Debug.Log ("go to phone camera");

		mainMenuCamera.enabled = false;
		menuScene.SetActive(false);
		phoneCamera.enabled = true;
		phoneCameraScene.SetActive (true);
		if (destroyCityScene)
			Destroy (destroyCityScene);
		
		gameState = GameState.PhoneCamera;
	}
}
