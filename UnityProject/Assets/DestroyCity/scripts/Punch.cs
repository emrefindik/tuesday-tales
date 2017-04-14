using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour 
{

	public bool isLeft;	// Whether this is a left fist. Init is setup by isLeft

	// Init State
	private float startTime;
	private Vector3 _inputPosition;

	// TAPPING? MIGHT NOT NEED
	//float tapCheckX;
	//const float TAPTOLERENCE = 10000;

	// Timer 
	const float HIT_DURATION_BASE = 100.0f;
	float hitDuration;

	// Update Rate
	const float TRANSITION_Y_TIMESTEP = 20.0f;

	// Action Status
	bool punchReturning;
	bool sending = false;
	Vector2 curPos;
	Vector2 newPos;
	Vector2 destPos;

	// Position Consts
	float IDLE_X = -5.0f;
	float IDLE_Y = 10f;
	const float BUILDING_CENTER_X = 0;

	float timeCount;
	const float STOP_PUNCHING_GAP = 0.02f;	//damping

	//float punchStrength;

	public enum PunchDirection
	{
		X,
		Y
	}
	PunchDirection punchDirection;

	void init()
	{
		hitDuration = HIT_DURATION_BASE;
		punchReturning = false;
		timeCount = 0.0f;

		GetComponent<Collider> ().enabled = false;
		if (!isLeft)
			IDLE_X = -IDLE_X;	
		sending = false;
	}

	public void sendPunch(Vector2 destination, PunchDirection pd)
	{
		init ();
		curPos = new Vector2 (transform.position.x, transform.position.y);
		startTime = Time.time;
		newPos = new Vector2 (curPos.x, curPos.y);
		destPos = new Vector2 (destination.x, destination.y);
		GetComponent<Collider> ().enabled = true;
		sending = true;
		punchDirection = pd;
	}

	/* Update Based on Punch Type */
	void Update () {
		if (punchDirection == PunchDirection.X)
			updateNormal ();
		if (punchDirection == PunchDirection.Y)
			updateBang ();
	}

	void updateNormal()
	{
		if (!sending)
			return;
		curPos = new Vector2(transform.position.x, transform.position.y);
		if (!punchReturning) {
			timeCount += .1f;
			newPos.x = Mathf.Lerp (curPos.x, destPos.x, timeCount);
			transform.position = new Vector3 (newPos.x, transform.position.y, transform.position.z);
			if (Mathf.Abs (newPos.x - destPos.x) < STOP_PUNCHING_GAP) {
				punchReturning = true;
				GetComponent<Collider> ().enabled = false;
			}

		} else {
			newPos.x = Mathf.Lerp (curPos.x, IDLE_X, (Time.time - startTime) * .2f);
			transform.position = new Vector3 (newPos.x, transform.position.y, transform.position.z);
			if (Mathf.Abs (transform.position.x - IDLE_X) < STOP_PUNCHING_GAP) {
				Destroy (this.gameObject);
			}
		}
	}

	void updateBang()
	{
		if (!sending)
			return;
		curPos = new Vector2(transform.position.x, transform.position.y);
		if (!punchReturning) {
			timeCount += .3f;
			newPos.y = Mathf.Lerp (curPos.y, destPos.y, timeCount);
			transform.position = new Vector3 (transform.position.x, newPos.y, transform.position.z);
			if (Mathf.Abs (newPos.y - destPos.y) < STOP_PUNCHING_GAP) {
				punchReturning = true;
				GetComponent<Collider> ().enabled = false;
			}

		} else {
			//if(Mathf.Abs (curPos.y - IDLE_Y) > 5)
				//newPos.y = Mathf.Lerp (curPos.y, IDLE_Y, (Time.time - startTime) * .02f);
			//else
				newPos.y = Mathf.Lerp (curPos.y, IDLE_Y, (Time.time - startTime) * .2f);
			transform.position = new Vector3 (transform.position.x, newPos.y, transform.position.z);
			if (Mathf.Abs (transform.position.y - IDLE_Y) < STOP_PUNCHING_GAP) {
				Destroy (this.gameObject);
			}
		}
	}

	void updateTimer()
	{
		hitDuration = hitDuration + startTime - Time.time;
		if (hitDuration <= 0) {
			hitDuration = 10;
			Destroy (this);
			timeCount = 0;
		}
	}
}
