/************************************************
 *
 *	This is the control class of the camera scene
 *
 ************************************************/


using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneImageController : MonoBehaviour {
	
	public WebCamTexture pCamera = null;
	public GameObject camDisplayPlane;

	public GameObject KaijuSelfieModel;
	public GameObject BuildingSelfieModel;
	public GameObject EggSelfieModel;


	string frontCamName;
	string backCamName;

	string lastMessage = "";
	bool takingPhoto;
	bool GUION;
	bool CAMREADY;

	enum CameraMode{
		BuildingDestruction,
		EggHatching,
		Kaiju,
		None
	}

	// Use this for initialization
	void Start () {
		initCamera (CameraMode.Kaiju);
	}

	void initCamera(CameraMode mode)
	{
		startCamera ();
		takingPhoto = false;
		GUION = true;
		CAMREADY = false;

		KaijuSelfieModel.SetActive (false);
		BuildingSelfieModel.SetActive (false);
		EggSelfieModel.SetActive (false);

		Debug.Log (mode);
		switch (mode) {
		case CameraMode.BuildingDestruction:
			BuildingSelfieModel.SetActive (true);
			break;
		case CameraMode.EggHatching:
			EggSelfieModel.SetActive (true);
			break;
		case CameraMode.Kaiju:
			KaijuSelfieModel.SetActive (true);
			// TODO: get monster information from spatial
			KaijuSelfieModel.GetComponent<MonsterCreator> ().setUpMonster (2, 1, 1, Color.blue);
			break;
		default:
			break;	
		}

	}

	void Update()
	{
		if (pCamera.width < 100 && !CAMREADY) {
			return;
		} else {
			// Means getting image
			if (!CAMREADY) {
				double ratio = (float)pCamera.height / (float)pCamera.width;
				Debug.Log (Screen.width);
				Debug.Log (Screen.height);
				camDisplayPlane.transform.localScale += new Vector3 (0.0f, 0.0f, (float)(ratio-1.0));
				CAMREADY = true;
			}
		}

	}


	string GetButtonLabel(string label) {
		if (Application.platform == RuntimePlatform.IPhonePlayer) return label;
		return label+"\n\n(iOS ONLY)";
	}

	void OnGUI () {
		
		//GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), pCamera, ScaleMode.ScaleToFit, true, 0);
		if(!CAMREADY) return;

		if (!GUION) {
			return;
		}

		if (takingPhoto) {
			// should just do it once
			takePhoto ();
			return;
		}

		/*
		float buttonWidth = Screen.width/3;
		float buttonHeight = Screen.height/5;
		float buttonMargine = buttonWidth/3;
		Rect buttonRect = new Rect(0, Screen.height-buttonHeight, buttonWidth, buttonHeight);

		// for Save Image
		buttonRect.width = Screen.width/4;
		buttonRect.height = Screen.height/6;
		buttonMargine = 0;
		buttonRect.y = 0;
		buttonRect.x = buttonMargine + (buttonRect.width + buttonMargine) * 1;
		if (GUI.Button(buttonRect, GetButtonLabel("Save JPG\nto PhotoLibrary"))) {
			takingPhoto = true;
		}
		buttonRect.y = 0;
		buttonRect.x = buttonMargine + (buttonRect.width + buttonMargine) * 2;
		if (GUI.Button(buttonRect, GetButtonLabel("Back"))) {
			MainController.single.goToMainMenu ();
		}
		*/
	}

	public void initTakingPhoto()
	{
		takingPhoto = true;
	}

	void startCamera()
	{
		pCamera = new WebCamTexture ();

		#if UNITY_IPHONE
		WebCamDevice[] devices = WebCamTexture.devices;
		for(int i = 0; i < devices.Length; i++){
			if (devices[i].isFrontFacing) {
				frontCamName = devices[i].name;
			} else {
				backCamName = devices[i].name;
			}
		}
		pCamera = new WebCamTexture(frontCamName);
		#endif

		camDisplayPlane.GetComponent<Renderer>().material.mainTexture = pCamera;
		pCamera.Play ();
	}

	void takePhoto()
	{
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			StartCoroutine("CaptureScreen");
		}
		#endif
		GUION = false;
	}
		
	#if UNITY_IPHONE
	// For Save
	IEnumerator CaptureScreen() {
		yield return new WaitForEndOfFrame();

		Texture2D screenShot = ScreenCapture.Capture();
		LoadTextureFromImagePicker.SaveAsJpgToPhotoLibrary(screenShot, gameObject.name, "OnFinishedSaveImage");
	}

	void OnFinishedSaveImage (string message) {
		lastMessage = message;
		if (message.Equals(LoadTextureFromImagePicker.strCallbackResultMessage_Saved)) {
			// Save Succeed
		} else {
			// Failed
		}
		takingPhoto = false;
		GUION = true;
	}
	#endif

}
