using UnityEngine;
using System.Collections;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class ChangeSprite : MonoBehaviour {

	const int maxTextureSize = 512;

	public void LoadTexture() {
		#if UNITY_IPHONE
		if (Application.platform == RuntimePlatform.IPhonePlayer) {
			LoadTextureFromImagePicker.SetPopoverAutoClose(true);
			LoadTextureFromImagePicker.SetPopoverToCenter();
			LoadTextureFromImagePicker.ShowPhotoLibrary(gameObject.name, "OnFinishedImagePicker");
		}
		#endif
	}

	void ReplaceSprite(Texture2D tex, float width, float height) {
		var image = GetComponent<UnityEngine.UI.Image>();
		var rectTransform = GetComponent<RectTransform>();
		var rect = new Rect(0, 0, tex.width, tex.height);
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tex.width);
		rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tex.height);
		image.sprite = Sprite.Create(tex, rect, rectTransform.pivot, image.sprite.pixelsPerUnit);
	}
	
	#if UNITY_IPHONE
	// For Load
	void OnFinishedImagePicker (string message) {
		if (LoadTextureFromImagePicker.IsLoaded()) {
			int width, height;
			var orgTexWidth = LoadTextureFromImagePicker.GetLoadedTextureWidth();
			var orgTexHeight = LoadTextureFromImagePicker.GetLoadedTextureHeight();
			if (orgTexWidth > orgTexHeight) {
				width = Mathf.Min(orgTexWidth, maxTextureSize);
				height = width * orgTexHeight / orgTexWidth;
			} else {
				height = Mathf.Min(orgTexHeight, maxTextureSize);
				width = height * orgTexWidth / orgTexHeight;
			}
			bool mipmap = false;
			Texture2D texture = LoadTextureFromImagePicker.GetLoadedTexture(message, width, height, mipmap);
			if (texture != null) {
				// Load Texture
				ReplaceSprite(texture, width, height);
			}
			LoadTextureFromImagePicker.ReleaseLoadedImage();
		} else {
			// Closed
			LoadTextureFromImagePicker.Release();
		}
	}
	#endif
}
