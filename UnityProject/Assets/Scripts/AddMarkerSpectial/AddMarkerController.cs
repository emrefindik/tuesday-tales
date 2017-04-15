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

	/*
	 * The fence:  40.442199, -79.943468
		Carnegie Museum of Art: 40.443992, -79.949329
		Dippy the Dinosaur: 40.443569, -79.951436
		Cathedral of Learning: 40.444218, -79.953204
		Phipps Conservatory: 40.439157, -79.947426
		Peace Garden (cool if they had to stay there awhile): 40.440970, -79.943119
		Centerfield Gesling Stadium: 40.443185, -79.940265
		Mattress Factory: 40.456874, -80.012222
		The ETC: 40.432791, -79.964793
		Schell Games Studio: 40.433727, -80.004363

	 */
	
	// Update is called once per frame
	void Update () {
		if (!ok) {			
			//MarkerMetadata mMete = MarkerMetadata.newCheckInLocationMetadata ();
			MarkerMetadata mMete = MarkerMetadata.newBuildingMetadata ("http://tuesday-tales.etc.cmu.edu/Photos/building1.png", 
				"http://tuesday-tales.etc.cmu.edu/Photos/building1destroyed.png", new ImageBounds (40.4329, 40.4325, -79.9646, -79.9650));

			StartCoroutine(SpatialClient2.single.CreateMarker(40.432791, -79.964793, "ETC", "", mMete));
			StartCoroutine (SpatialClient2.single.DeleteMarkerById ("58f18bdb6ad13600117881d0"));
			/*
			StartCoroutine(SpatialClient2.single.CreateMarker(40.442199, -79.943468, "The Fence", "", mMete));
			StartCoroutine(SpatialClient2.single.CreateMarker(40.443992, -79.949329, "Carnegie Museum of Art", "", mMete));
			StartCoroutine(SpatialClient2.single.CreateMarker(40.443569, -79.951436, "Dippy the Dinosaur", "", mMete));
			StartCoroutine(SpatialClient2.single.CreateMarker(40.444218, -79.953204, "Cathedral of Learning", "", mMete));
			StartCoroutine(SpatialClient2.single.CreateMarker(40.439157, -79.947426, "Phipps Conservatory", "", mMete));
			StartCoroutine(SpatialClient2.single.CreateMarker(40.440970, -79.943119, "Peace Garden", "", mMete));
			StartCoroutine(SpatialClient2.single.CreateMarker(40.443185, -79.940265, "Centerfield Gesling Stadium", "", mMete));
			StartCoroutine(SpatialClient2.single.CreateMarker(40.456874, -80.012222, "Mattress Factory", "", mMete));
			StartCoroutine(SpatialClient2.single.CreateMarker(40.432791, -79.964793, "ETC", "", mMete));
			StartCoroutine(SpatialClient2.single.CreateMarker(40.433727, -80.004363, "Schell Games Studio", "", 
			*/
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
