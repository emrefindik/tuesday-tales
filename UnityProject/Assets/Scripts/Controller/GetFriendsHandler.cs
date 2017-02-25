using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetFriendsHandler : MonoBehaviour {
	public string username;
	public string password;
	public string friendID;
	public string Auth;
	public string userID;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.C)) {
			StartCoroutine (SpatialClient2.single.CreateUser (username, password));
		}
		if (Input.GetKeyDown(KeyCode.L)) {
			StartCoroutine (SpatialClient2.single.LoginUser (username, password));
		}
		if (Input.GetKeyDown(KeyCode.M)) {
			StartCoroutine (SpatialClient2.single.CreateMarker (50f,50f,"test", "test marker",null));
		}
		if (Input.GetKeyDown (KeyCode.I)) {
			StartCoroutine(SpatialClient2.single.GetProjectInfo());
		}
		if (Input.GetKeyDown (KeyCode.A)) {
			StartCoroutine(SpatialClient2.single.AddFriend(friendID, Auth)); 
		}
		if (Input.GetKeyDown (KeyCode.G)) {
			StartCoroutine(SpatialClient2.single.GetFriends(userID)); 
		}
	}
}
