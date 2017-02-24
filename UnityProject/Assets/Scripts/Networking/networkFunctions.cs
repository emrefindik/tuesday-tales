using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkFunctions : MonoBehaviour {
	public static NetworkFunctions instance;
	public string PROJID = "58b070d3b4c96e00118b66ee";

	void Start(){
		instance = this;
	}

	UserData FetchUserData(string userName, string password){
		return new UserData ();
	}

	/*IEnumerator FetchUser(string userName, string password){

	}*/
}
