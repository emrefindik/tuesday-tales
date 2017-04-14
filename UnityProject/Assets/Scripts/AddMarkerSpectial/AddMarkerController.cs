using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddMarkerController : MonoBehaviour {

	public Texture2D uploadImage;
	string URL = "http://matthewestone.com/PhotoTest/";
	string send = "SendPhoto.php";
	string recv;
	bool ok;

	// Use this for initialization
	void Start () {
		ok = false;
		//StartCoroutine (UploadImage("MyLocationImage"));
		//

	}
	
	// Update is called once per frame
	void Update () {
		if (!ok) {
			StartCoroutine(SpatialClient2.single.CreateMarker(40.432882, -79.964823, "Entertainment Technology Center","700 Technology Drive"));
			ok = true;
			/*
			StartCoroutine(SpatialClient2.single.CreateUser("1", "1"));
			StartCoroutine(SpatialClient2.single.CreateUser("2", "2"));
			StartCoroutine(SpatialClient2.single.CreateUser("3", "3"));
			StartCoroutine(SpatialClient2.single.CreateUser("4", "4"));
			StartCoroutine(SpatialClient2.single.CreateUser("5
			", "5"));
			ok = true;
			*/
		}
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
