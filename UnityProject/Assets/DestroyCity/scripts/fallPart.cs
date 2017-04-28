using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallPart : MonoBehaviour {

    float duration = .1f;
    float magnitude = .1f;

    public GameObject bloodEffect;
    public AudioClip smeshAudio;
	public AudioClip screamAudio;
    AudioSource smesh;
	AudioSource scream;
    public LevelControl healthControl;


    void Start()
    {
        smesh = GetComponent<AudioSource>();
		smesh.clip = smeshAudio;
		smesh.playOnAwake = false;
		scream = gameObject.AddComponent<AudioSource>();
		scream.clip = screamAudio;
		scream.playOnAwake = false;
    }

    void OnTriggerEnter(Collider c)
    {

        if (c.tag == "building")
        {
			smesh.Play();
            Rigidbody gravity =  c.gameObject.GetComponent<Rigidbody>();
            gravity.useGravity = true;
			gravity.GetComponent<SpriteRenderer> ().sortingOrder = 12;
            //gravity.GetComponent<Collider>().enabled = false;
			c.gameObject.layer = LayerMask.NameToLayer("Ruins");
			c.GetComponent<BoxCollider> ().isTrigger = false;
            StartCoroutine(Shake());
			healthControl.increaseProgress(1, LevelControl.ScoreType.Building);
        }


        if(c.tag == "people")
        {
			smesh.Play();
			scream.Play ();
            Destroy(c.gameObject);
            Transform newPos = c.gameObject.GetComponent<Transform>();
			GameObject blood = Instantiate(bloodEffect, newPos.position + new Vector3(0, 0, -3), transform.rotation);
			blood.transform.parent = Camera.main.transform;
			healthControl.increaseProgress(1, LevelControl.ScoreType.Human);
        }
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
