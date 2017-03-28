using UnityEngine;
using System.Collections;

public class ChangeSample : MonoBehaviour {
	
	public GameObject objTexPhoto = null;
	public GameObject objTexCamera = null;
	public GameObject objSave = null;
	
	public TextMesh textSave = null;

	GameObject obj = null;

	void Start () {
	}

	void Update () {

		// Touch Event
		bool touched = false;
		Vector2 touchPos = Vector2.zero;
		//
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			if (Input.touchCount > 0) {
				// Touches
				for (int i=0; i<Input.touchCount; ++i) {
					Touch touch = Input.GetTouch(i);
					if (touch.phase == TouchPhase.Began) {
						touchPos.x = touch.position.x;
						touchPos.y = touch.position.y;
						touched = true;
						break;	// Single Touch
					}
				}
			}
		} else {
			// Mouse
			if (Input.GetMouseButtonDown(0)) {
				touchPos.x = Input.mousePosition.x;
				touchPos.y = Input.mousePosition.y;
				touched = true;
			}
		}

		// Check Touch to Object
		if (touched) {

			// Hit Check
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(touchPos);
			if (Physics.Raycast(ray, out hit, 100.0f)) {
				// HIt!!
				GameObject hitPlane = hit.collider.gameObject;
			
				if (hitPlane.Equals(objTexPhoto)) {
					// Photo
#if UNITY_IPHONE
					if (Application.platform == RuntimePlatform.IPhonePlayer) {
						obj = objTexPhoto;
						LoadTextureFromImagePicker.ShowPhotoLibrary(this.name, "OnFinishedImagePicker");
					} else 
#endif
					{
						Debug.Log("Touched on Photo");
					}
				} else if (hitPlane.Equals(objTexCamera)) {
					// Camera
#if UNITY_IPHONE
					if (Application.platform == RuntimePlatform.IPhonePlayer) {
						obj = objTexCamera;
						LoadTextureFromImagePicker.ShowCamera(this.name, "OnFinishedImagePicker");
					} else 
#endif
					{
						Debug.Log("Touched on Camera");
					}
				} else if (hitPlane.Equals(objSave)) {
					// Save
					if (Application.platform == RuntimePlatform.IPhonePlayer) {
						textSave.text = "Saving..";
						StartCoroutine("CaptureScreen");
					} else {
						Debug.Log("Touched on Save Button");
					}
				}
			}
		}
	}

	#if UNITY_IPHONE
	// For Load
	void OnFinishedImagePicker (string message) {
		if (LoadTextureFromImagePicker.IsLoaded()) {
			var material = obj.GetComponent<Renderer>().material;
			int width = material.mainTexture.width;
			int height = material.mainTexture.height;
			const bool createMipmap = true;
			Texture2D texture = LoadTextureFromImagePicker.GetLoadedTexture(message, width, height, createMipmap);
			if (texture) {
				// Load Texture
#if false
				var lastTexture = material.mainTexture;
				material.mainTexture = texture;
				Destroy(lastTexture);	// Destroy the last texture
#else
				material.mainTexture = texture;
#endif
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
		Texture2D texScreenShot = ScreenCapture.Capture();
		LoadTextureFromImagePicker.SaveAsPngToPhotoLibrary(texScreenShot, this.name, "OnFinishedSaveImage");
	}

	void OnFinishedSaveImage (string message) {
		if (message.Equals(LoadTextureFromImagePicker.strCallbackResultMessage_Saved)) {
			// Save Succeed
			textSave.text = "Save\nSucceed";
		} else {
			// Failed
			textSave.text = "Save\nFailed";
		}
	}
	#endif
}
