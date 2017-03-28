Unity iOS native plugin

iOS Load Texture From PhotoLibrary/Camera


---------------------------------------------------------------------------
Description: 
This plugin is run on iOS native code. You can use UIImagePicker for load a texture on Unity.

New Feature:
- Added functions to Save/Load to local storage as png file.
  It's to enable to resume with the last selected image.

Features:
- Load Texture from PhotoLibrary on Device
- Take a photo by Camera
- Save Image to PhotoLibrary (JPG/PNG/PNG with transparency)


Demo Video:
http://youtu.be/HeL85WW0Y80


How to use (C# Script):

ShowCamera:
	LoadTextureFromImagePicker.ShowCamera(gameObject.name, "OnFinishedImagePicker");

Load Image from PhotoLibrary:
	LoadTextureFromImagePicker.ShowPhotoLibrary(gameObject.name, "OnFinishedImagePicker");

Callback function on finished, and get Image:
	private void OnFinishedImagePicker (string message) {
		Texture2D texture = LoadTextureFromImagePicker.GetLoadedTexture(message, 512, 512);
		if (texture) {
			Texture lastTexture = targetMaterial.mainTexture;
			targetMaterial.mainTexture = texture;
			Destroy(lastTexture);
		}
	}


Popover position setting for iPad:
	Center of screen
		LoadTextureFromImagePicker.SetPopoverToCenter();

	Set target Rect
		LoadTextureFromImagePicker.SetPopoverTargetRect(buttonPos.x, buttonPos.y, buttonWidth, buttonHeight);


Save Image to PhotoLibrary:
	LoadTextureFromImagePicker.SaveAsJpgToPhotoLibrary(image, gameObject.name, "OnFinishedSaveImage");
	LoadTextureFromImagePicker.SaveAsPngToPhotoLibrary(image, gameObject.name, "OnFinishedSaveImage");
	LoadTextureFromImagePicker.SaveAsPngWithTransparencyToPhotoLibrary(image, gameObject.name, "OnFinishedSaveImage");


If you have any questions, send email to support: whitedev.support@gmail.com


---------------------------------------------------------------------------
Version Changes:
1.8.1:
	- Added example scene for JavaScript.
1.8.0:
	- Added functions to Save/Load to local storage as png file.
	  It's to enable to resume with the last selected image.
1.7.1:
	- Added sample scene for UI Image object.
1.7:
	- Support for Unity 5 and Xcode ARC
	- Enable Default Front Camera
1.6:
	- Support for arm64 and IL2CPP
	- Fixed errors on Xcode in Unity 4.6.2 p2.
1.5.6:
	- Fixed an issue of camera crashes. 
	- Fixed some issues.
1.5.5:
	- Fixed an issue of image size when taking a picture from camera in portrait mode. 
1.5.4:
	- Fixed problem that get size of image.
1.5.3:
	- Fixed rotation issue when using camera view.
1.5.2:
	- Fixed errors when the non-iOS platform.
1.5.1:
	- Fixed Landscape Camera problem on iPad & iOS7
	- Add New Sample Scene (Simple)

	**** To support Unity3 in this version is the last. ****
1.4:
	- Fixed Landscape Camera problem on iPad & iOS7
1.3:
	- Add create mipmap option
	- Include iOS source code
1.2:
	- Add "Save to PhotoLibrary" Function (JPG/PNG/PNG with transparency)
	- Fix memory leak
1.1:
	- Support for Unity 4.2
1.0.4:
	- Support for load image with original size
	- Support for no close menu on select image (only iPad)
	- Fixed conflict my plugins.
1.0.3:
	- Support for Unity 3.5.7/4.0/4.1 or Higher
1.0.2:
	- Fix Popover Positioning
1.0:
	- Initial version.
