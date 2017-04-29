using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Helicopter : MonoBehaviour {

	public float max = 5;
	public float offset = 0;
	public ParticleSystem explodeEffect;
	float tolerence = 0.02f;
	bool hit = false;
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (!hit) {
			float x = Mathf.PingPong (Time.time + offset, max);
			transform.position = new Vector3 (x, transform.position.y, transform.position.z);
			if (x >= max - tolerence) {
				transform.rotation = new Quaternion (0, 180, 0, 1);
			}
			if (x <= tolerence) {
				transform.rotation = new Quaternion (0, 0, 0, 1);

			}
		}
	}

	void OnCollisionEnter(Collision c)
	{
		c.gameObject.GetComponent<Rigidbody> ().AddExplosionForce (10000, transform.position, 5);
		gameObject.GetComponent<Rigidbody> ().AddExplosionForce (10000, transform.position, 5);
		Debug.Log (c.gameObject);
		hit = true;
		if (explodeEffect){
			ParticleSystem effect = Instantiate (explodeEffect, transform.position, Quaternion.identity);
			effect.Play ();
		}
	}
}
