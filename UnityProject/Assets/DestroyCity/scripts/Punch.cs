using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Punch : MonoBehaviour {

	// init is setup by isLeft
	public bool isLeft;

	float startTime;

	const float HIT_DURATION_BASE = 100.0f;
	float hitDuration;
	const float TRANSITION_Y_TIMESTEP = 20.0f;

	bool punchReturning;
	Vector3 _inputPosition;
	float tapCheckX;
	const float TAPTOLERENCE = 10000;

	Vector2 curPos;
	Vector2 newPos;
	Vector2 destPos;
	float IDLE_X = -5.0f;
	float IDLE_Y = 10f;
	const float BUILDING_CENTER_X = 0;

	float timeCount;
	const float STOP_PUNCHING_GAP = 0.02f;

	float punchStrength;
	bool sending = false;

	public enum PunchDirection
	{
		X,
		Y
	}
	PunchDirection punchDirection;

	// Use this for initialization
	void Start () {
		//init ();
	}

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
	
	// Update is called once per frame
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
