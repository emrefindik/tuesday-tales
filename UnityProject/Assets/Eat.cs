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
			Destroy (c.gameObject);
			if(eatEffect)
				eatEffect.Play ();
		} 

	}
}
