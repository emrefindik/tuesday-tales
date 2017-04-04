using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class freezeRotation : MonoBehaviour {

    void FixedUpdate()
    {
        //Convert rotation into readable angles.
        Vector3 eulerRotation = transform.rotation.eulerAngles;
        float oldz = eulerRotation.z;
        //Set non essential angles to 0.
        //eulerRotation.x = 0;
        eulerRotation.z = oldz;

        //Reset the transform rotation.
        transform.rotation = Quaternion.Euler(eulerRotation);
    }
    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
