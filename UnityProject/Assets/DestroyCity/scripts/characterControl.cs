using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterControl : MonoBehaviour {

	public int unit = 1;

	const float stepSize = 0.8f;
	float minX;
	float maxX;
	//float lastX;
	//float destX;
	float t;
	float increment;




	// Use this for initialization
	void Start () {
		minX = transform.localPosition.x;
		maxX = minX + stepSize * unit;
		//lastX = minX;
		//destX = maxX;
		t = Random.Range(0.0f, 1.0f);
		increment = 1.0f;
	}
	
	// Update is called once per frame
	void Update () {
		
		float x = Mathf.Lerp (minX, maxX, t);
		transform.localPosition = new Vector3 (x, transform.localPosition.y, transform.localPosition.z);
		//lastX = x;
		t += increment * Time.deltaTime;

		if (increment > 0) {
			if (x == maxX) {
				//lastX = destX;
				//destX = minX;
				increment = -increment;
				transform.rotation = new Quaternion (transform.rotation.x, 180, transform.rotation.z, 1);
			}
		} else {
			if (x == minX) {
				//lastX = destX;
				//destX = maxX;
				increment = -increment;
				transform.rotation = new Quaternion(transform.rotation.x, 0, transform.rotation.z, 1);
			}
		}

	}
}
