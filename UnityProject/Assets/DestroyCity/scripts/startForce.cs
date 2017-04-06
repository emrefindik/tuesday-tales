using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class startForce : MonoBehaviour
{
    public float dirCorrection = 1;
    public float speed = 6000;
    public float destPosX;
    // Use this for initialization
	float damping = 0.5f;
    void Start()
    {
        transform.GetComponent<Rigidbody>().AddForce(new Vector3(speed * dirCorrection, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        if(dirCorrection > 0)
        {
            if (transform.position.x > destPosX + damping)
            {
                Debug.Log("Reached DESTINATION");
                Destroy(this.gameObject);
            }
        }
        else if (dirCorrection < 0)
        {
            if (transform.position.x < destPosX - damping)
            {
                Debug.Log("Reached DESTINATION");
                Destroy(this.gameObject);
            }
        }


    }
    void OnMouseDown()
    {
        // transform.GetComponent<Rigidbody2D>().AddForce(transform.up * 100);
        //Debug.Log("click click boom");


    }
    void OnCollisionEnter()
    {
        //Debug.Log("Collided");
        Destroy(this.gameObject);
    }
}
