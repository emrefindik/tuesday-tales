using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class UserData {
	public string username;
	public string IDNum;
}

public class FriendData{
	public UserData friendData;
	public float lat;
	public float lng;
}