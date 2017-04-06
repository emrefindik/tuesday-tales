using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFallenCheck: MonoBehaviour {

	public LevelControl levelControl;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision c)
	{
		Debug.Log ("Collision enter");

		if (c.gameObject.tag == "block") {
			Destroy (c.gameObject);
			levelControl.increaseProgress (1, LevelControl.ScoreType.Blocks);
		} else {
			Destroy (c.gameObject);
		}

	}

}
