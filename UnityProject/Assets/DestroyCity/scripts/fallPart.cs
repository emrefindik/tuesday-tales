using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fallPart : MonoBehaviour {

    float duration = .1f;
    float magnitude = .1f;

    public GameObject bloodEffect;
    public AudioClip smeshAudio;
    public AudioSource smesh;
    public LevelControl healthControl;


    void Start()
    {
        smesh = GetComponent<AudioSource>();
        smesh.clip = smeshAudio;
    }

    void OnTriggerEnter(Collider c)
    {

        smesh.Play();
        if (c.tag == "building")
        {
            Rigidbody gravity =  c.gameObject.GetComponent<Rigidbody>();
            gravity.useGravity = true;
            gravity.GetComponent<Collider>().enabled = false;
            StartCoroutine(Shake());
            healthControl.increaseProgress(1);
        }


        if(c.tag == "people")
        {
            Destroy(c.gameObject);
            Transform newPos = c.gameObject.GetComponent<Transform>();
            GameObject blood = Instantiate(bloodEffect, newPos.position, transform.rotation);
			blood.transform.parent = gameObject.transform;
            healthControl.increaseProgress(1);
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
