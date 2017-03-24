#pragma strict

var objTexPhoto : GameObject = null;
var objTexCamera : GameObject = null;
var objSave : GameObject = null;

var textSave : TextMesh = null;

private var obj : GameObject = null;

function Start () {

}

function Update () {

	// Touch Event
	var touched = false;
	var touchPos = Vector2.zero;
	//
	if (Application.platform == RuntimePlatform.IPhonePlayer) {
		if (Input.touchCount > 0) {
			// TouchesScreenCapture
			for (var i=0; i<Input.touchCount; ++i) {
				var touch = Input.GetTouch(i);
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
		var hit : RaycastHit;
		var ray : Ray = Camera.main.ScreenPointToRay(touchPos);
		if (Physics.Raycast(ray, hit, 100.0f)) {
			// HIt!!
			var hitPlane : GameObject = hit.collider.gameObject;
		
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
function OnFinishedImagePicker (message : String) {
	if (LoadTextureFromImagePicker.IsLoaded()) {
		var material = obj.GetComponent(Renderer).material;
		var width : int = material.mainTexture.width;
		var height : int = material.mainTexture.height;
		var createMipmap = true;
		var texture : Texture2D = LoadTextureFromImagePicker.GetLoadedTexture(message, width, height, createMipmap);
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
function CaptureScreen() {
	yield WaitForEndOfFrame();

	// Save to PhotoLibrary
	var texScreenShot : Texture2D = ScreenCapture.Capture();
	LoadTextureFromImagePicker.SaveAsPngToPhotoLibrary(texScreenShot, this.name, "OnFinishedSaveImage");
}

function OnFinishedSaveImage (message : String) {
	if (message.Equals(LoadTextureFromImagePicker.strCallbackResultMessage_Saved)) {
		// Save Succeed
		textSave.text = "Save\nSucceed";
	} else {
		// Failed
		textSave.text = "Save\nFailed";
	}
}
#endif

