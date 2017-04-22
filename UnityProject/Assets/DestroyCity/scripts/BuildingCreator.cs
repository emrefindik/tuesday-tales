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
		buildingDict ["58f3eb594e973b0011f5f6b6"] = 1;
		buildingDict ["58f3eb594e973b0011f5f6b0"] = 2;
		buildingDict ["58f3eb594e973b0011f5f6b4"] = 3;
		buildingDict ["58f3eb594e973b0011f5f6b7"] = 4;
		buildingDict ["58f3eb594e973b0011f5f6b3"] = 5;
		buildingDict ["58f3eb594e973b0011f5f6b5"] = 6;
		buildingDict ["58f3eb594e973b0011f5f6b2"] = 7;
		buildingDict ["58f3eb594e973b0011f5f6b1"] = 8;
		buildingDict ["58f3eb594e973b0011f5f6af"] = 9;
		buildingDict ["58f3eb594e973b0011f5f6ae"] = 10;


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
		if(buildingDict.ContainsKey(markerId)){
			index = buildingDict [markerId];
		}
		else{
			index = 0;
		}
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
