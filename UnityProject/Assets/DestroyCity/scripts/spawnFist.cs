using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnFist : MonoBehaviour
{
    public GameObject fistPrefab;
    //public bool isLeft;
    public bool isRight;
    RaycastHit rayInfo;
    GameObject fist;
    public float forceFactor = 100;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        /* if (Input.GetMouseButtonDown(0))
         {
             float xDist = 4.0f;
             if (isLeft)
             {
                 xDist = -xDist;
             }

             if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)), out rayInfo))
             {
                 Debug.Log("raycast sent");
                 Instantiate(fistPrefab, new Vector3(xDist, rayInfo.point.y, 0), Quaternion.identity);
             }

             //Instantiate(fistPrefab, new Vector3(xDist, Input.mousePosition.y, 0), Quaternion.identity);
             Debug.Log("You Clicked!!");
         }
         */
    }
    public void sendPunch(bool isLeft, float destposY, float destposX)
    {

            float xDist = 4.0f;
            if (isLeft)
            {
                xDist = -xDist;
            }
        GameObject fist = Instantiate(fistPrefab, new Vector3(xDist, destposY, 0), Quaternion.identity);
        fist.GetComponent<startForce>().dirCorrection = xDist * -1.0f;
        fist.GetComponent<startForce>().destPosX = destposX;


        /*if (Physics.Raycast(Camera.main.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0)), out rayInfo))
            {
                //Debug.Log("raycast sent");
                Instantiate(fistPrefab, new Vector3(xDist, rayInfo.point.y, 0), Quaternion.identity).GetComponent<startForce>().dirCorrection = xDist * -1.0f;
                //fist.transform.GetComponent<Rigidbody2D>().AddForce(new Vector2(xDist * forceFactor, xDist * forceFactor));
            }
        */


        //new Vector2(xDist * forceFactor, xDist * forceFactor
        //Instantiate(fistPrefab, new Vector3(xDist, Input.mousePosition.y, 0), Quaternion.identity);
        //Debug.Log("You Clicked!!");

    }
}
