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
        buildingDict["5913c84c6a3f6f0011a80751"] = 1; // Doherty Hall                       buildingDict ["58f3eb594e973b0011f5f6b6"] = 1;
        buildingDict["5913cd546a3f6f0011a80759"] = 2; // FMS Building                       buildingDict ["58f3eb594e973b0011f5f6b0"] = 2;
        buildingDict["5913cd556a3f6f0011a8075a"] = 3; // Hamerschlag Hall                   buildingDict ["58f3eb594e973b0011f5f6b4"] = 3;
        buildingDict["5913cd546a3f6f0011a80757"] = 4; // Margaret Morrison Carnegie Hall    buildingDict ["58f3eb594e973b0011f5f6b7"] = 4;
        buildingDict["5913798c58a5f10011338f8e"] = 5; // Hunt Library                       buildingDict ["58f3eb594e973b0011f5f6b3"] = 5;
        buildingDict["5913cfb96a3f6f0011a8075f"] = 6; // Purnell Center for the Arts        buildingDict ["58fd56788bd5410011d1115d"] = 6;
        buildingDict["5913cd546a3f6f0011a80756"] = 7; // Wean Hall                          buildingDict ["58f3eb594e973b0011f5f6b2"] = 7;
        buildingDict["5913cd556a3f6f0011a8075b"] = 8; // Baker Hall                         buildingDict ["58f3eb594e973b0011f5f6b1"] = 8;
        buildingDict["5913cd556a3f6f0011a8075c"] = 9; // Cohon University Center            buildingDict ["58f3eb594e973b0011f5f6af"] = 9;
        buildingDict["5913cd546a3f6f0011a80758"] = 10; // College of Fine Arts              buildingDict ["58f3eb594e973b0011f5f6ae"] = 10;


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
			index = (int)Random.Range (1, 10);
			//index = 1;
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
