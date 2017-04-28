using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFallenCheck: MonoBehaviour {

	public LevelControl levelControl;
	GameObject ground = null;

	void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.tag == "block") {
			Destroy (c.gameObject);
			levelControl.increaseProgress (1, LevelControl.ScoreType.Blocks);
			if (ground == null) {
				ground = GameObject.FindGameObjectWithTag("ground");
			}
			ground.GetComponent<Ground>().startShake(0.1f);
		} 
		else if (c.gameObject.tag != "building")
		{
			Destroy (c.gameObject);
		}

	}

}
