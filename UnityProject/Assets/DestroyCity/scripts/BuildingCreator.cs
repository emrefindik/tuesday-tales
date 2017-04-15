using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* This Script is Called to Initialize the City */
public class BuildingCreator : MonoBehaviour {

	public GameObject[] Scenes;

	private Dictionary <string, int> buildingDict;
	private int index;

	void initDict()
	{
		buildingDict = new Dictionary<string, int> ();
		buildingDict ["58f18c776ad13600117881d1"] = 1;
	}

	public void setUpBuildingTest(int _idx)
	{
		initDict ();
		index = _idx;
		loadScene ();
	}

	public void setUpBuilding(string markerId)
	{
		initDict ();
		index = buildingDict [markerId];
		loadScene();
	}

	/*
	 *  Initialize the selected 
	 */
	private void loadScene()
	{
		Instantiate (Scenes [index], new Vector3 (0, 0, 0), Quaternion.identity, transform);
	}

}
