using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ground : MonoBehaviour {

	public float offset;
	public AudioClip buildingCollapseAudio;
	AudioSource buildingCollapse;
	private float baseY;
	bool shake;
	float duration = .3f;
	float magnitude = .1f;

	// Use this for initialization
	void Start () {
		baseY = transform.position.y;
		shake = false;
		buildingCollapse = gameObject.AddComponent<AudioSource>();
		buildingCollapse.clip = buildingCollapseAudio;
		buildingCollapse.playOnAwake = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (shake) {
			offset = Mathf.PingPong (Time.time, 0.05f);
			transform.position = new Vector3 (transform.position.x, baseY + offset * 1f, transform.position.z);
		} else {
			transform.position = new Vector3 (transform.position.x, baseY, transform.position.z);
		}
	}

	public void startShake(float seconds){
		shake = true;
		buildingCollapse.Play ();
		StartCoroutine(stopShake(seconds));
		StartCoroutine(Shake());
	}

	IEnumerator stopShake(float seconds){
		yield return new WaitForSeconds (seconds);
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
