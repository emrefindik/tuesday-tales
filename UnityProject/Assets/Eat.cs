using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Eat : MonoBehaviour {

	public LevelControl levelControl;
	public AudioClip eatClip;
	public ParticleSystem eatEffect;
	AudioSource eatSound;
	// Use this for initialization
	void Start () {
		eatSound = gameObject.AddComponent<AudioSource> ();
		eatSound.clip = eatClip;
		eatSound.playOnAwake = false;
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnCollisionEnter(Collision c)
	{
		if (c.gameObject.tag == "block") {
			levelControl.increaseProgress (1, LevelControl.ScoreType.Blocks);
			eatSound.Play();
			StartCoroutine (_Eat (c.gameObject));
			if(eatEffect)
				eatEffect.Play ();
		} 

	}

	IEnumerator _Eat(GameObject g)
	{
		Rigidbody rigidBody = g.GetComponent<Rigidbody> ();
		rigidBody.constraints = RigidbodyConstraints.None;
		rigidBody.velocity = new Vector3 (0, 0, 3.5f);
		rigidBody.useGravity = false;
		yield return new WaitForSeconds (0.5f);
		Destroy (g);
	}
}
