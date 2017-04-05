using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {

	public float offset;
	private float baseY;
	bool shake;
	float duration = .1f;
	float magnitude = .1f;

	// Use this for initialization
	void Start () {
		baseY = transform.position.y;
		shake = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (shake) {
			offset = Mathf.PingPong (Time.time, 0.05f);
			transform.position = new Vector3 (transform.position.x, baseY + offset *  10f, this.transform.position.z);
		}
	}

	public void startShake(){
		shake = true;
		StartCoroutine(stopShake());
		StartCoroutine(Shake());
	}

	IEnumerator stopShake(){
		yield return new WaitForSeconds (1);
		shake = false;
	}

	IEnumerator Shake()
	{

		float elapsed = 0.0f;

		Vector3 originalCamPos = Camera.main.transform.position;

		while (elapsed < duration)
		{

			elapsed += Time.deltaTime;

			float percentComplete = elapsed / duration;
			float damper = 1.0f - Mathf.Clamp(4.0f * percentComplete - 3.0f, 0.0f, 1.0f);

			// map value to [-1, 1]
			float x = Random.value * 2.0f - 1.0f;
			float y = Random.value * 2.0f - 1.0f;
			x *= magnitude * damper;
			y *= magnitude * damper;

			Camera.main.transform.position = new Vector3(x, y, originalCamPos.z);

			yield return null;
		}

		Camera.main.transform.position = originalCamPos;
	}

}
