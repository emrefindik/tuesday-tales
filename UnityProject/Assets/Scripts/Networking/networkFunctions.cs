using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkFunctions : MonoBehaviour {
	public static NetworkFunctions instance;

	void Start(){
		instance = this;
	}

	UserData FetchUserData(string userName){
		return new UserData ();
	}

	List<FriendData> FetchFriendData(UserData user){
		return new List<FriendData> ();
	}
}
