using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingCreator : MonoBehaviour {

	public GameObject[] Grounds;
	public GameObject[] Buildings;
	public GameObject[] Backgrounds;

	Dictionary <string, int> buildingDict;
	private int index;

	// Use this for initialization
	void Start () {
		// prepare objects
		//index = 0;
		//loadObjects ();
	}

	public void setUpBuilding(string markerId)
	{
		index = buildingDict [markerId];
		loadObjects();
	}

	private void loadObjects()
	{
		Instantiate (Grounds [index], new Vector3 (0, 0, 0), Quaternion.identity, transform);
		Instantiate (Buildings [index], new Vector3 (0, 0, 0), Quaternion.identity, transform);
		Instantiate (Backgrounds [index], new Vector3 (0, 0, 0), Quaternion.identity, transform);
	}

	// FOR TEST
	public void setUpBuilding(int idx)
	{
		index = idx;
		loadObjects ();
	}

}
