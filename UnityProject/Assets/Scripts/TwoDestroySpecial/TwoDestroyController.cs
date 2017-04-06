using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TwoDestroyController : MonoBehaviour {
	public enum GameState{
		MainMenu,
		DestroyCity
	}
	public GameState gameState;
	public GameState lastGameState;

	public Camera mainMenuCamera;
	public GameObject menuScene;
	public GameObject destroyCityPrefab1;
	public GameObject destroyCityPrefab2;
	public GameObject destroyCityScene;
	public GameObject mainMenuCanvas;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void goBack()
	{
		goToMainMenu ();
	}

	public void goToMainMenu()
	{
		lastGameState = gameState;
		Debug.Log ("go to main menu");

		mainMenuCamera.enabled = true;
		menuScene.SetActive(true);
		if (destroyCityScene)
			Destroy (destroyCityScene);

		gameState = GameState.MainMenu;
	}

	public void goToDestroyCityOld()
	{
		lastGameState = gameState;
		Debug.Log ("go to destory city gameplay");

		mainMenuCamera.enabled = false;
		menuScene.SetActive(false);

		destroyCityScene = Instantiate (destroyCityPrefab1);

		gameState = GameState.DestroyCity;
	}

	public void goToDestroyCityNew()
	{
		lastGameState = gameState;
		Debug.Log ("go to destory city gameplay");

		mainMenuCamera.enabled = false;
		menuScene.SetActive(false);

		destroyCityScene = Instantiate (destroyCityPrefab2);

		gameState = GameState.DestroyCity;
	}


}
