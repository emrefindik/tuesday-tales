using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookManager : MonoBehaviour {

	static public FacebookManager single;

	public enum ShareStatus
	{
		None,
		Init,
		Sending,
		Recieved
	}

	void Awake ()
	{
		if (!FB.IsInitialized) {
			// Initialize the Facebook SDK
			FB.Init(InitCallback, OnHideUnity);
		} else {
			// Already initialized, signal an app activation App Event
			FB.ActivateApp();
		}
	}

	void Start()
	{
		single = this;
	}

	private void InitCallback ()
	{
		if (FB.IsInitialized) {
			// Signal an app activation App Event
			FB.ActivateApp();
			// Continue with Facebook SDK
			// ...
		} else {
			Debug.Log("Failed to Initialize the Facebook SDK");
		}
	}

	private void OnHideUnity (bool isGameShown)
	{
		if (!isGameShown) {
			// Pause the game - we will need to hide
			Time.timeScale = 0;
		} else {
			// Resume the game - we're getting focus again
			Time.timeScale = 1;
		}
	}


	/***************************************************/
	public void ShareScreenshotToFacebook()
	{
		screenshot ();
	}

	void screenshot()
	{
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			StartCoroutine("CaptureScreen");
		}
		#endif
	}

	#if UNITY_IPHONE
	// For Save
	IEnumerator CaptureScreen() {
		yield return new WaitForEndOfFrame();

		Texture2D screenShot = ScreenCapture.Capture();
		LoadTextureFromImagePicker.SaveAsJpgToPhotoLibrary(screenShot, gameObject.name, "OnFinishedSaveImage");
	}
	#endif

	void OnFinishedSaveImage (string message) {
		FB.ShareLink(
			new System.Uri("https://developers.facebook.com/"),
			callback: ShareCallback
		);
	}

	private void ShareCallback (IShareResult result) {



		if (result.Cancelled || !string.IsNullOrEmpty(result.Error)) {
			Debug.Log("ShareLink Error: "+result.Error);
		} else if (!string.IsNullOrEmpty(result.PostId)) {
			// Print post identifier of the shared content
			Debug.Log(result.PostId);
		} else {
			// Share succeeded without postID
			Debug.Log("ShareLink success!");
		}
		if (MainController.single.gameState == MainController.GameState.DestroyCity) {
			LevelControl.shareStatus = ShareStatus.Recieved;
		}
	}

	/***************************************************/
}
