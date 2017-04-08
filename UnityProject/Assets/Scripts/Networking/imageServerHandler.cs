using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class imageServerHandler : MonoBehaviour {
	public Texture2D uploadTexture;
	string URL = "http://tuesday-tales.etc.cmu.edu/Photos/";
	string send = "SendPhoto.php";
	string recv;
	public Texture2D downloadTexture;
	public GameObject setText;

	// Use this for initialization
	void Start () {

	}

	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.Space)) {
			StartCoroutine (UploadImage ());
		}
		if (Input.GetKeyDown (KeyCode.D)) {
			StartCoroutine (DownloadImage ());
		}
		setText.GetComponent<Renderer> ().material.mainTexture = downloadTexture;
	}

	IEnumerator UploadImage(){
		byte[] data = uploadTexture.EncodeToPNG ();
		WWWForm form = new WWWForm ();
		form.AddBinaryData ("Image", data, "ImageUpload.png", "image/png");
		WWW web = new WWW (URL+send, form);
		yield return web;
		if (!string.IsNullOrEmpty (web.error)) {
			print (web.error);
		} else {
			print (web.text);
			recv = web.text;
		}
	}

	IEnumerator DownloadImage(){
		WWW web = new WWW (URL+recv);
		yield return web;
		if (!string.IsNullOrEmpty (web.error)) {
			print (web.error);
		} else {
			downloadTexture = web.texture;
		}
	}
}