using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeographyTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
		if(GeographyMaster.withinDistance(40.432863d, -79.964845d,40.432161, -79.963566,100))
			Debug.Log("Within!");
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
