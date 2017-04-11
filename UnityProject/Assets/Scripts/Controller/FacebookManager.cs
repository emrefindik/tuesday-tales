using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class FacebookManager : MonoBehaviour {

	static public FacebookManager single;

	// image server info
	Texture2D uploadImage;

	string URL = "http://tuesday-tales.etc.cmu.edu/Photos/";
	string send = "SendPhoto.php";
	string recv;

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
	/* Finish Everything From Screenshot To Share */
	public void ShareScreenshotToFacebook()
	{
		screenshot ();
	}

	/* Share Prepared Image*/
	public void ShareImageToFacebook(Texture2D image)
	{
		uploadImage = image;
		string imageName = generateImageName ();
		StartCoroutine (UploadImage (imageName));
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

		uploadImage = ScreenCapture.Capture();
		LoadTextureFromImagePicker.SaveAsJpgToPhotoLibrary(uploadImage, gameObject.name, "OnFinishedSaveImage");
	}
	#endif

	string generateImageName()
	{
		return Time.time + ".png";
	}

	void OnFinishedSaveImage (string message) {
		string imageName = generateImageName ();
		StartCoroutine (UploadImage(imageName));
	}

	IEnumerator UploadImage(string imageName){
		byte[] data = uploadImage.EncodeToPNG ();
		WWWForm form = new WWWForm ();
		form.AddBinaryData ("Image", data, imageName, "image/png");
		WWW web = new WWW (URL+send, form);
		Debug.Log ("Image URL: "+web.url);
		yield return web;
		if (!string.IsNullOrEmpty (web.error)) {
			print (web.error);
		} else {
			print (web.text);
			recv = web.text;

			// Share facebook link here
			FB.ShareLink(
				new System.Uri(URL+recv),
				callback: ShareCallback
			);
		}
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

		switch (MainController.single.gameState) {
		case MainController.GameState.DestroyCity:
			LevelControl.shareStatus = ShareStatus.Recieved;
			break;
		case MainController.GameState.PhoneCamera:
			PhoneImageController.shareStatus = ShareStatus.Recieved;
			break;
		default:
			break;
		}
	}

	/***************************************************/
}
