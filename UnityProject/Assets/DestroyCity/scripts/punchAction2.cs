using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class punchAction2 : MonoBehaviour {

	public GameObject leftFist;
	public GameObject rightFist;
	public GameObject monster;

	Vector3 _inputPosition;
	float tapCheckX;
	const float TAPTOLERENCE = 10;

	const float IDLE_L_X = -5.0f;
	const float IDLE_L_Y = 10f;
	const float BANG_L_X = -2f;

	const bool PUNCHINGLEFT = false;
	const bool PUNCHINGRIGHT = true;
	const float BUILDING_CENTER_X = 0.0f;

	float punchStrength;
	const float PUNCH_STRENGTH_MAX = 5.1f;
	const float PUNCH_STRENGTH_STEP = 0.04f;
	public ParticleSystem pressEffect;
	//ParticleSystem pressEffectInstance;
	ParticleSystem.Particle[] m_Particles;

	const float PUNCH_Z = -1;
	// For Jonathan's Destroy City
	public bool level2On;
	public GameObject ground;
	const float groundY = -5.0f;

	bool locked = false;	// ignore input lock

    void Start () {
        leftFist.GetComponent<Collider>().enabled = false;
        rightFist.GetComponent<Collider>().enabled = false;

		locked = false;
		level2On = false;

    }

	void Update () {

		if (locked) {
			locked = false;
			return;
		}
			

		// mouse input need change to phone touch input
		if (Input.GetMouseButtonDown(0))
		{
			punchStrength = PUNCH_STRENGTH_MAX;
			pressEffect.transform.position = new Vector3 (_inputPosition.x, _inputPosition.y, -1f);
			pressEffect.Play ();
		}

		if (Input.GetMouseButton (0)) {
			// add punch strength
			if (punchStrength < PUNCH_STRENGTH_MAX) {
				punchStrength += PUNCH_STRENGTH_STEP;
				pressEffect.transform.localScale = new Vector3 (1 + punchStrength, 1 + punchStrength, 1 + punchStrength);
			}

		}


		if(Input.GetMouseButtonUp(0))
		{
			tapCheckX = Input.mousePosition.x;
			_inputPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
			pressEffect.Stop ();

			if (Mathf.Abs(Input.mousePosition.x - tapCheckX) > TAPTOLERENCE)
				return;
			if (_inputPosition.x < BUILDING_CENTER_X)
			{
				initPunch(PUNCHINGLEFT);
			}
			if (_inputPosition.x >= BUILDING_CENTER_X)
			{
				initPunch(PUNCHINGRIGHT);
			}
		}

	}
		
	void initPunch(bool punchDirection)
	{
		GameObject fist;
		Vector2 dest;
		if (punchDirection == PUNCHINGLEFT) {
			//dest = new Vector2 (IDLE_L_X + punchStrength, _inputPosition.y);
			dest = new Vector2(_inputPosition.x, _inputPosition.y);
			fist = Instantiate (leftFist, new Vector3 (IDLE_L_X, dest.y, PUNCH_Z), Quaternion.identity);
			if (level2On) {
				fist.GetComponent<spawnFist> ().sendPunch (true, dest.y, dest.x);
			}

		} else {
			//dest = new Vector2 (-IDLE_L_X - punchStrength, _inputPosition.y);
			dest = new Vector2(_inputPosition.x, _inputPosition.y);
			fist = Instantiate (rightFist, new Vector3 (-IDLE_L_X, dest.y, PUNCH_Z), Quaternion.identity);
			if (level2On) {
				fist.GetComponent<spawnFist> ().sendPunch (false, dest.y, dest.x);
			}
		}
		fist.GetComponent<Punch> ().sendPunch (dest, Punch.PunchDirection.X);
		fist.transform.SetParent(monster.transform, true);

	}

	public void punchGround(int times)
	{
		ground = GameObject.FindGameObjectWithTag ("ground");
		Debug.Log ("Ground " + ground);
		ground.GetComponent<Ground> ().startShake (1.0f * times);
		Debug.Log ("punch ground");
		StartCoroutine(bangGround(times));
	}

	IEnumerator bangGround(int times)
	{
		Debug.Log ("Banging");
		GameObject fist;
		Vector2 dest;
		if (times % 2 == 0) {
			dest = new Vector2 (BANG_L_X, groundY);
			fist = Instantiate (leftFist, new Vector3 (dest.x, IDLE_L_Y, PUNCH_Z), Quaternion.identity);
			Debug.Log ("leftFist");
		} else {
			dest = new Vector2 (-BANG_L_X, groundY);
			fist = Instantiate (rightFist, new Vector3 (dest.x, IDLE_L_Y, PUNCH_Z), Quaternion.identity);
			Debug.Log ("rightFist");
		}
		fist.GetComponent<Punch> ().sendPunch (dest, Punch.PunchDirection.Y);
		fist.transform.SetParent(monster.transform, true);
		yield return new WaitForSeconds (0.2f);

		times = times-1;
		if(times > 0)
			StartCoroutine (bangGround (times));
	}

}