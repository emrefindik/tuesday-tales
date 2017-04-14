using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startForce : MonoBehaviour
{
	// Properties that can be set outside
    public float dirCorrection = 1;
    public float speed = 6000;
    public float destPosX;

    // Use this for initialization
	const float damping = 0.5f;
    void Start()
    {
        transform.GetComponent<Rigidbody>().AddForce(new Vector3(speed * dirCorrection, 0, 0));
    }

    void Update()
    {
        if(dirCorrection > 0)
        {
            if (transform.position.x > destPosX + damping)
            {
                Destroy(this.gameObject);
            }
        }
        else if (dirCorrection < 0)
        {
            if (transform.position.x < destPosX - damping)
            {
                Destroy(this.gameObject);
            }
        }
    }

    void OnCollisionEnter()
    {
        Destroy(this.gameObject);
    }
}
