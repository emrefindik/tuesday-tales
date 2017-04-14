using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFallenCheck: MonoBehaviour {

	public LevelControl levelControl;

	void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.tag == "block") {
			Destroy (c.gameObject);
			levelControl.increaseProgress (1, LevelControl.ScoreType.Blocks);
		} else {
			Destroy (c.gameObject);
		}

	}

}
