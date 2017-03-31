using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMarkerController : MonoBehaviour {

	public Texture2D uploadImage;
	string URL = "http://matthewestone.com/PhotoTest/";
	string send = "SendPhoto.php";
	string recv;

	// Use this for initialization
	void Start () {
		StartCoroutine (UploadImage("MyLocationImage"));
	}
	
	// Update is called once per frame
	void Update () {
		
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
		}
		Debug.Log (URL + recv);
	}
}
