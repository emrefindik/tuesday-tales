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

	enum WHICHCAMERA{
		None,
		Front,
		Back
	}
	WHICHCAMERA whichCamera = WHICHCAMERA.None;

	// Use this for initialization
	void Start () {
		initCamera (CameraMode.None);
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
				double screenRatio = (float)Screen.height / (float)Screen.width;
				//camDisplayPlane.transform.localScale += new Vector3 (0.0f, 0.0f, (float)(ratio-1.0));
				GameObject cameraImage = camDisplayCanvas.transform.FindChild ("CameraImage").gameObject;
				if (screenRatio > ratio) {
					cameraImage.GetComponent<RectTransform> ().
					SetSizeWithCurrentAnchors (RectTransform.Axis.Vertical, (float)(Screen.height * ratio));
				} else { 
					cameraImage.GetComponent<RectTransform> ().
					SetSizeWithCurrentAnchors (RectTransform.Axis.Horizontal, (float)(Screen.width / ratio));


				}

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
		whichCamera = WHICHCAMERA.Front;
		#endif

		camDisplayCanvas.transform.FindChild ("CameraImage").gameObject.GetComponent<RawImage> ().texture = pCamera;
		pCamera.Play ();
	}

	void takePhoto()
	{
		camDisplayCanvas.SetActive (false);
		uiCanvas.SetActive (false);

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

		camDisplayCanvas.SetActive (true);
		uiCanvas.SetActive (true);
	}
	#endif

	public void switchCam()
	{	
		pCamera.Stop ();
		#if UNITY_IPHONE
		if(whichCamera == WHICHCAMERA.Front)
			pCamera = new WebCamTexture(backCamName);
		else if(whichCamera == WHICHCAMERA.Back)
			pCamera = new WebCamTexture(frontCamName);
		else{
			Debug.Log("NO Camera Loaded!");
			return;
		}
		camDisplayCanvas.transform.FindChild ("CameraImage").gameObject.GetComponent<RawImage> ().texture = pCamera;
		pCamera.Play ();
		#endif
	}
		
}
