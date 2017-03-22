using UnityEngine;
using System.Collections;

public class ChangePhoto : MonoBehaviour {
	
	public Material targetMaterial = null;
	public Texture defaultTexture = null;
		
	public bool bUseOriginalImageSize = false;
	public bool iPadPopover_DisableAutoClose = false;
	public bool isDefaultFrontCamera = false;
	public bool isSaveToLocalFile = true;

	int textureWidth = 512;
	int textureHeight = 512;
	bool saveAsPng = false;

	string lastMessage = "";

	#if UNITY_IPHONE
	string saveTexFileName { get { return Application.persistentDataPath+"/SaveTex.data"; } }
	#endif

	void Start () {
		if (targetMaterial == null) {
			targetMaterial = GameObject.Find("Cube").GetComponent<Renderer>().material;
		}

		#if UNITY_IPHONE
		// Resume
		if (targetMaterial != null) {
			var resume = false;
			if (isSaveToLocalFile) {
				var savedTex = LoadTextureFromImagePicker.LoadFromLocalFile(saveTexFileName);
				if (savedTex != null) {
					targetMaterial.mainTexture = savedTex;
					resume = true;
				}
			}
			if (resume == false) targetMaterial.mainTexture = defaultTexture;
		}
		#endif
	}

	void Update () {
	}

	string GetButtonLabel(string label) {
		if (Application.platform == RuntimePlatform.IPhonePlayer) return label;
		return label+"\n\n(iOS ONLY)";
	}

	void OnGUI () {
		// Swithes
		GUI.Label(new Rect(0,Screen.height*0.5f, 100,30), "Options:");
		bUseOriginalImageSize = GUI.Toggle(new Rect(0, Screen.height*0.5f+30, 400, 30), bUseOriginalImageSize, "Use Original Image Size");
		iPadPopover_DisableAutoClose = GUI.Toggle(new Rect(0, Screen.height*0.5f+60, 400, 30), iPadPopover_DisableAutoClose, "iPad Popover: Disable Auto Close");
		isDefaultFrontCamera = GUI.Toggle(new Rect(0, Screen.height*0.5f+90, 400, 30), isDefaultFrontCamera, "Default Front Camera");
		isSaveToLocalFile = GUI.Toggle(new Rect(0, Screen.height*0.5f+120, 400, 30), isSaveToLocalFile, "Save/Resume last selected image to local file");

		// Buttons
		float buttonWidth = Screen.width/3;
		float buttonHeight = Screen.height/5;
		float buttonMargine = buttonWidth/3;
		Rect buttonRect = new Rect(0, Screen.height-buttonHeight, buttonWidth, buttonHeight);
		buttonRect.x = buttonMargine;
		if (targetMaterial == null) {
			GUI.Box(buttonRect, "(Set Target Material)");
		} else if (GUI.Button(buttonRect, GetButtonLabel("Camera"))) {
			#if UNITY_IPHONE
				if (Application.platform == RuntimePlatform.IPhonePlayer) {
					LoadTextureFromImagePicker.SetDefaultFrontCamera(isDefaultFrontCamera);
					LoadTextureFromImagePicker.SetPopoverToCenter();
					LoadTextureFromImagePicker.ShowCamera(gameObject.name, "OnFinishedImagePicker");
				}
			#endif
		}
		buttonRect.x = buttonMargine + buttonWidth + buttonMargine;
		if (targetMaterial == null) {
			GUI.Box(buttonRect, "(Set Target Material)");
		} else if (GUI.Button(buttonRect, GetButtonLabel("Load Image\nfrom PhotoLibrary"))) {
			#if UNITY_IPHONE
				if (Application.platform == RuntimePlatform.IPhonePlayer) {
					LoadTextureFromImagePicker.SetPopoverAutoClose(iPadPopover_DisableAutoClose == false);
					LoadTextureFromImagePicker.SetPopoverTargetRect(buttonRect.x, buttonRect.y, buttonWidth, buttonHeight);
					LoadTextureFromImagePicker.ShowPhotoLibrary(gameObject.name, "OnFinishedImagePicker");
				}
			#endif
		}
		//
		// for Save Image
		buttonRect.width = Screen.width/4;
		buttonRect.height = Screen.height/6;
		buttonMargine = 0;
		buttonRect.y = 0;
		buttonRect.x = buttonMargine + (buttonRect.width + buttonMargine) * 1;
		if (GUI.Button(buttonRect, GetButtonLabel("Save JPG\nto PhotoLibrary"))) {
			#if UNITY_IPHONE
				if (Application.platform == RuntimePlatform.IPhonePlayer) {
					saveAsPng = false;
					StartCoroutine("CaptureScreen");
				}
			#endif
		}
		buttonRect.x = buttonMargine + (buttonRect.width + buttonMargine) * 2;
		if (GUI.Button(buttonRect, GetButtonLabel("Save PNG\nto PhotoLibrary"))) {
			#if UNITY_IPHONE
				if (Application.platform == RuntimePlatform.IPhonePlayer) {
					saveAsPng = true;
					StartCoroutine("CaptureScreen");
				}
			#endif
		}


		// Disp Texture Size
		if (targetMaterial) {
			Texture targetTexture = targetMaterial.mainTexture;
			GUI.Label(new Rect(0,0, 400,100), "Current Texture Size:\n"+"width="+targetTexture.width+", height="+targetTexture.height);
		}

		// Disp Last Message
		GUI.Label(new Rect(0,80, 200,60), "Last Result:\n"+lastMessage);
	}

	#if UNITY_IPHONE
	// For Load
	void OnFinishedImagePicker (string message) {
		lastMessage = message;
		if (targetMaterial && LoadTextureFromImagePicker.IsLoaded()) {
			int width, height;
			if (bUseOriginalImageSize) {
				width = LoadTextureFromImagePicker.GetLoadedTextureWidth();
				height = LoadTextureFromImagePicker.GetLoadedTextureHeight();
			} else {
				width = textureWidth;
				height = textureHeight;
			}
			bool mipmap = true;
			Texture2D texture = LoadTextureFromImagePicker.GetLoadedTexture(message, width, height, mipmap);
			if (texture) {
				// Load Texture
				Texture lastTexture = targetMaterial.mainTexture;
				targetMaterial.mainTexture = texture;
				Destroy(lastTexture);

				if (isSaveToLocalFile) {
					// Save to local file
					LoadTextureFromImagePicker.SaveToLocalFile(saveTexFileName, texture);
				}
			}
			LoadTextureFromImagePicker.ReleaseLoadedImage();
		} else {
			// Closed
			LoadTextureFromImagePicker.Release();
		}
	}

	// For Save
	IEnumerator CaptureScreen() {
		yield return new WaitForEndOfFrame();

		// Save to PhotoLibrary
		Texture2D screenShot = ScreenCapture.Capture();
		if (saveAsPng) {
			bool withTransparency = false;
			if (withTransparency) {
				// PNG with transparency
				LoadTextureFromImagePicker.SaveAsPngWithTransparencyToPhotoLibrary(screenShot, gameObject.name, "OnFinishedSaveImage");
			} else {
				// PNG
				LoadTextureFromImagePicker.SaveAsPngToPhotoLibrary(screenShot, gameObject.name, "OnFinishedSaveImage");
			}
		} else {
			// JPG
			LoadTextureFromImagePicker.SaveAsJpgToPhotoLibrary(screenShot, gameObject.name, "OnFinishedSaveImage");
		}
	}

	void OnFinishedSaveImage (string message) {
		lastMessage = message;
		if (message.Equals(LoadTextureFromImagePicker.strCallbackResultMessage_Saved)) {
			// Save Succeed
		} else {
			// Failed
		}
	}
	#endif
}
