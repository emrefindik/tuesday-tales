/************************************************
 *
 *	This is the control class of the camera scene
 *
 ************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhoneImageController : MonoBehaviour {
	
	public WebCamTexture pCamera = null;
	public GameObject camDisplayCanvas;
	public GameObject uiCanvas;
	public GameObject shareCanvas;

	public GameObject KaijuSelfieModel;
	public GameObject BuildingSelfieModel;
	public GameObject EggSelfieModel;
	public GameObject EggCheckinModel;

	public Texture2D screenShotCopy;
	public GameObject photoRect;
	static public FacebookManager.ShareStatus shareStatus;

	string frontCamName = "";
	string backCamName = "";

	string lastMessage = "";
	bool takingPhoto;
	bool CAMREADY;

    public enum CameraMode{
		BuildingDestruction,
		EggHatching,
		Kaiju,
		EggCheckin,
		None
	}

	enum WHICHCAMERA{
		None,
		Front,
		Back
	}

	WHICHCAMERA whichCamera = WHICHCAMERA.None;
	MainController mainController;

	// Use this for initialization
	void Start () {
		//initCamera (CameraMode.EggCheckin);
		shareStatus = FacebookManager.ShareStatus.None;
	}

	public void startCameraWithMode(CameraMode mode)
	{
		initCamera (mode);
	}

	void initCamera(CameraMode mode)
	{
		mainController = GameObject.Find ("MainController").GetComponent<MainController>();
		startCamera ();
		takingPhoto = false;
		CAMREADY = false;

		KaijuSelfieModel.SetActive (false);
		BuildingSelfieModel.SetActive (false);
		EggSelfieModel.SetActive (false);
		EggCheckinModel.SetActive (false);

		MainMenuScript mainMenu = mainController.gameObject.GetComponent<MainMenuScript> ();

		Debug.Log (mode);
		switch (mode) {
		case CameraMode.BuildingDestruction:
			BuildingSelfieModel.SetActive (true);
			Kaiju kaiju = mainMenu.SelectedKaiju;
			BuildingSelfieModel.GetComponent<MonsterCreator> ().setUpMonster (kaiju.HeadSprite, kaiju.HandSprite, kaiju.BodySprite, kaiju.MonsterColor);
			break;
		case CameraMode.EggHatching:
			// TODO: get egg information from spatial
			EggSelfieModel.SetActive (true);
			GameObject Egg = GameObject.Find ("Egg");
			Egg.GetComponent<SpriteRenderer> ().sprite = mainController.selectedEgg.Sprite;
			break;
		case CameraMode.Kaiju:
			KaijuSelfieModel.SetActive (true);
			Kaiju selectedKaiju = mainMenu.SelectedKaiju;
			KaijuSelfieModel.GetComponent<MonsterCreator> ().setUpMonster (selectedKaiju.HeadSprite, selectedKaiju.HandSprite, selectedKaiju.BodySprite, selectedKaiju.MonsterColor);
			break;
		case CameraMode.EggCheckin:
			EggCheckinModel.SetActive (true);
			GameObject CheckinEgg = GameObject.Find ("CheckinEgg");
			CheckinEgg.GetComponent<SpriteRenderer> ().sprite = mainController.selectedEgg.Sprite;
			CheckinEgg.transform.localScale = new Vector3 (873.0f / mainController.selectedEgg.Sprite.texture.width, 878.0f / mainController.selectedEgg.Sprite.texture.height);
			break;
		default:
			break;	
		}
			
		uiCanvas.SetActive (false);
		camDisplayCanvas.SetActive (false);
		shareCanvas.SetActive (false);

	}

	void Update()
	{
		if (!pCamera)
			return;
        if (pCamera.width < 100 && !CAMREADY) {
			return;
		} else {
			// Means getting image
			if (!CAMREADY) {
                Debug.Log("Initializing Camera");
				double ratio = (float)pCamera.width / (float)pCamera.height;
				double screenRatio = (double)Screen.height / (double)Screen.width;
				//camDisplayPlane.transform.localScale += new Vector3 (0.0f, 0.0f, (float)(ratio-1.0));
				GameObject cameraImage = camDisplayCanvas.transform.FindChild ("CameraImage").gameObject;

                if(Application.platform == RuntimePlatform.Android)
                    cameraImage.transform.rotation = Quaternion.Euler(0, 0, 90);
				
				if (screenRatio < ratio) {
					camDisplayCanvas.GetComponent<CanvasScaler> ().referenceResolution = new Vector2 (Screen.width, Screen.height);
					cameraImage.GetComponent<RectTransform> ().
					SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, (float)(Screen.width * ratio));
					cameraImage.GetComponent<RectTransform> ().
					SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, (float)(Screen.width));
				} else { 
					camDisplayCanvas.GetComponent<CanvasScaler> ().referenceResolution = new Vector2 (Screen.width, Screen.height);
					cameraImage.GetComponent<RectTransform> ().
					SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, (float)(Screen.height));
					cameraImage.GetComponent<RectTransform> ().
					SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, (float)(Screen.height / ratio));
				}




				CAMREADY = true;
				camDisplayCanvas.SetActive (true);
				uiCanvas.SetActive (true);
			}

			if (takingPhoto) {
				// should just do it once
				takePhoto ();
				return;
			}
		}

		switch (shareStatus) {
		case FacebookManager.ShareStatus.Init:
			{
				SendScreenshotToFacebook ();
				return;
			}
		case FacebookManager.ShareStatus.Sending:
			return;
		case FacebookManager.ShareStatus.Recieved:
			{
				shareStatus = FacebookManager.ShareStatus.None;
				// Should show a dialog to show status
				shareCanvas.SetActive(true);
				shareCanvas.transform.Find ("ShareSucceedText").gameObject.SetActive (true);
				return;
			}
		default:
			break;
		}
	}
		
	string GetButtonLabel(string label) {
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) return label;
		return label+"\n\n(iOS / Android ONLY)";
	}

	public void initTakingPhoto()
	{
		takingPhoto = true;
	}

	void startCamera()
	{
		pCamera = new WebCamTexture ();

		//#if UNITY_IPHONE
		WebCamDevice[] devices = WebCamTexture.devices;
		for(int i = 0; i < devices.Length; i++){
			if (devices[i].isFrontFacing) {
				frontCamName = devices[i].name;
			} else {
				backCamName = devices[i].name;
			}
		}
		if (frontCamName != "") {
			pCamera = new WebCamTexture (frontCamName, 1920, 1080);
			whichCamera = WHICHCAMERA.Front;
		} else {
			pCamera = new WebCamTexture (backCamName, 1920, 1080);
			whichCamera = WHICHCAMERA.Back;
		}
		//#endif

		camDisplayCanvas.transform.FindChild ("CameraImage").gameObject.GetComponent<RawImage> ().texture = pCamera;
		pCamera.Play ();
	}

	void takePhoto()
	{
		uiCanvas.SetActive (false);
		takingPhoto = false;

		//#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.Android) {
			StartCoroutine ("CaptureScreen");
		}
		// FOR TEST
		/*
		else {
			StartCoroutine ("CaptureScreen");
		}
		*/
		//#endif
	}

	// For Save
	IEnumerator CaptureScreen() {
		yield return new WaitForEndOfFrame();

		//Get another copy to show on share screen
		Texture2D screenshot = ScreenCapture.Capture();

		Color32[] pix = screenshot.GetPixels32();
		Destroy (screenShotCopy);
		screenShotCopy = new Texture2D(screenshot.width, screenshot.height);
		screenShotCopy.SetPixels32(pix);
		screenShotCopy.Apply();

		Debug.Log ("Capture one frame");
        #if UNITY_IPHONE
		if(Application.platform == RuntimePlatform.IPhonePlayer)
		{
			LoadTextureFromImagePicker.SaveAsJpgToPhotoLibrary(screenshot, gameObject.name, "OnFinishedSaveImage");
		}
        #endif

        if (Application.platform == RuntimePlatform.Android)
			SaveImageToLibraryAndriod(screenshot);
        
        //FOR TEST 
        //OnFinishedSaveImage("");

    }

	#if UNITY_IPHONE
	void OnFinishedSaveImage (string message) {
		Debug.Log ("Save Image Finished.");
		lastMessage = message;
		if (message.Equals(LoadTextureFromImagePicker.strCallbackResultMessage_Saved)) {
			// Save Succeed
		} else {
			// Failed
			Debug.Log(" Save to library failed!");

		}

		camDisplayCanvas.SetActive (true);
		uiCanvas.SetActive (true);

		shareCanvas.SetActive (true);
		//photoRect = shareCanvas.transform.Find ("Photo").gameObject;
		photoRect.GetComponent<RawImage> ().texture = screenShotCopy;
		float ratio = (float)screenShotCopy.width / (float)screenShotCopy.height;
		Debug.Log ("Screenshot Width" + screenShotCopy.width);
		photoRect.GetComponent<RectTransform> ().
		SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, (float)(photoRect.GetComponent<RectTransform> ().rect.width / ratio));
		Canvas.ForceUpdateCanvases ();
	}
	#endif

	void SaveImageToLibraryAndriod(Texture2D Screenshot)
	{
        /*
		byte[] bytes = Screenshot.EncodeToJPG ();
		string filename = ScreenShotName();
		System.IO.File.WriteAllBytes(filename, bytes);
		Debug.Log(string.Format("Took screenshot to: {0}", filename));
		string path = Application.persistentDataPath + "/Snapshots/" + filename;
		System.IO.File.WriteAllBytes(path, bytes);
        */

        Debug.Log("Saved");
		camDisplayCanvas.SetActive (true);
		uiCanvas.SetActive (true);

		shareCanvas.SetActive (true);
		GameObject photoRect = shareCanvas.transform.Find ("Photo").gameObject;
		photoRect.GetComponent<RawImage> ().texture = screenShotCopy;
		float ratio = (float)screenShotCopy.width / (float)screenShotCopy.height;
		photoRect.GetComponent<RectTransform> ().
		SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, (float)(photoRect.GetComponent<RectTransform> ().rect.width / ratio));
	}

	string ScreenShotName()
	{
		return Time.time.ToString ();
	}

	public void switchCam()
	{	
		RawImage displayImage = camDisplayCanvas.transform.FindChild ("CameraImage").gameObject.GetComponent<RawImage> ();
		pCamera.Stop ();
		//#if UNITY_IOS
		if(whichCamera == WHICHCAMERA.Front && backCamName != ""){
			pCamera = new WebCamTexture(backCamName, 1920, 1080);
			whichCamera = WHICHCAMERA.Back;
			displayImage.transform.localScale = new Vector3(1, -1, 1);
		}
		else if(whichCamera == WHICHCAMERA.Back && frontCamName != ""){
			pCamera = new WebCamTexture(frontCamName, 1920, 1080);
			whichCamera = WHICHCAMERA.Front;
			displayImage.transform.localScale = new Vector3(1, 1, 1);

		}
		else{
			Debug.Log("Can Not Switch.");
			return;
		}


		displayImage.texture = pCamera;
		pCamera.Play ();
		//#endif
	}

	public void initShareToFacebook()
	{
		shareStatus = FacebookManager.ShareStatus.Init;

		camDisplayCanvas.SetActive (false);
		uiCanvas.SetActive (false);
		shareCanvas.SetActive (true);
	}
		
	public void backToCamera()
	{
		camDisplayCanvas.SetActive (true);
		uiCanvas.SetActive (true);
		shareCanvas.SetActive (false);
	}
		
	void SendScreenshotToFacebook()
	{
		shareStatus = FacebookManager.ShareStatus.Sending;
		Debug.Log (screenShotCopy);
		Texture2D screenShotShareCopy = new Texture2D (screenShotCopy.width, screenShotCopy.height);
		Color32[] pix = screenShotCopy.GetPixels32 ();
		screenShotShareCopy.SetPixels32 (pix);
		screenShotShareCopy.Apply ();
		FacebookManager.single.ShareImageToFacebook (screenShotShareCopy);
	}

	public void goBack()
	{
		GameObject mainController = GameObject.Find ("MainController");
		mainController.GetComponent<MainController> ().goToMapView ();
	}
}
