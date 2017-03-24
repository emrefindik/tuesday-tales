using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainController : MonoBehaviour
{
    public static MainController single;
	public enum GameState{
		MainMenu,
		DestroyCity,
		PhoneCamera
	}
	public GameState gameState;
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

    public void goToMainMenu()
	{
		Debug.Log ("go to main menu");

        mainMenuCamera.enabled = true;
        menuScene.SetActive(true);
		phoneCamera.enabled = false;
		phoneCameraScene.SetActive (false);
		if (destroyCityScene)
			Destroy (destroyCityScene);

		gameState = GameState.MainMenu;
    }

    public void goToDestroyCity()
    {
        mainMenuCamera.enabled = false;
        menuScene.SetActive(false);
		destroyCityScene = Instantiate (destroyCityPrefab);
		phoneCamera.enabled = false;
		phoneCameraScene.SetActive (false);

		gameState = GameState.DestroyCity;
    }

	public void goToPhoneCamera()
	{
		mainMenuCamera.enabled = false;
		menuScene.SetActive(false);
		phoneCamera.enabled = true;
		phoneCameraScene.SetActive (true);
		if (destroyCityScene)
			Destroy (destroyCityScene);
		
		gameState = GameState.PhoneCamera;
	}
}
