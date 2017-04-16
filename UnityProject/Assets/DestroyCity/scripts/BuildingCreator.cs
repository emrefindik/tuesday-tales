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
		buildingDict ["58f3eb594e973b0011f5f6b6"] = 0;
		buildingDict ["58f3eb594e973b0011f5f6b0"] = 1;
		buildingDict ["58f3eb594e973b0011f5f6b4"] = 2;
		buildingDict ["58f3eb594e973b0011f5f6b7"] = 3;

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
			index = (int)Random.Range (0, 3);
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
