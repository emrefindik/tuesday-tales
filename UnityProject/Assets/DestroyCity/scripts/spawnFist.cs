using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class spawnFist : MonoBehaviour
{
    public GameObject fistPrefab;
    private float forceFactor = 100;
	private float xDist = 4.0f;
		
    public void sendPunch(bool isLeft, float destposY, float destposX)
    {
        if (isLeft)
        {
        	xDist = -xDist;
        }
        GameObject fist = Instantiate(fistPrefab, new Vector3(xDist, destposY, 0), Quaternion.identity);
        fist.GetComponent<startForce>().dirCorrection = xDist * -1.0f;
        fist.GetComponent<startForce>().destPosX = destposX;

    }
}
