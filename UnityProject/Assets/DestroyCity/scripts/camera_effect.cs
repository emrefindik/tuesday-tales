using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class camera_effect : MonoBehaviour {

    public float offset;
    private float baseX;

    void Start()
    {
        baseX = transform.position.x;
    }

	void Update () {
        offset = Mathf.PingPong (Time.time, 1);

        transform.position = new Vector3( baseX + offset*.1f, this.transform.position.y, this.transform.position.z);
	}
}
