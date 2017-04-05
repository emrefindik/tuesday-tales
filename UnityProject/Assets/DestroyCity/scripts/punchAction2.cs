using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class punchAction2 : MonoBehaviour {

	public GameObject leftFist;
	public GameObject rightFist;
	public GameObject monster;
	//public PixelDestruction pD;

	Vector3 _inputPosition;
	float tapCheckX;
	const float TAPTOLERENCE = 10;

	const float IDLE_L_X = -5.0f;

	float timeCountLX = 0;
	float timeCountRX = 0;

	const bool PUNCHINGLEFT = false;
	const bool PUNCHINGRIGHT = true;
	const float BUILDING_CENTER_X = 0.0f;

	// one touch control for now
	float punchStrength;
	const float PUNCH_STRENGTH_MAX = 4.9f;
	const float PUNCH_STRENGTH_STEP = 0.04f;
	public ParticleSystem pressEffect;
	//ParticleSystem pressEffectInstance;
	ParticleSystem.Particle[] m_Particles;

	const float PUNCH_Z = -1;
	// For Jonathan's Destroy City
	public bool level2On;

    void Start () {

        leftFist.GetComponent<Collider>().enabled = false;
        rightFist.GetComponent<Collider>().enabled = false;

		level2On = false;
    }

	void Update () {

		// mouse input need change to phone touch input
		if (Input.GetMouseButtonDown(0))
		{
			punchStrength = PUNCH_STRENGTH_MAX;
			pressEffect.transform.position = new Vector3 (_inputPosition.x, _inputPosition.y, -0.2f);
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
			dest = new Vector2 (IDLE_L_X + punchStrength, _inputPosition.y);
			fist = Instantiate (leftFist, new Vector3 (IDLE_L_X, dest.y, PUNCH_Z), Quaternion.identity);
			if (level2On) {
				fist.GetComponent<spawnFist> ().sendPunch (true, dest.y, dest.x);
			}

		} else {
			dest = new Vector2 (-IDLE_L_X - punchStrength, _inputPosition.y);
			fist = Instantiate (rightFist, new Vector3 (-IDLE_L_X, dest.y, PUNCH_Z), Quaternion.identity);
			if (level2On) {
				fist.GetComponent<spawnFist> ().sendPunch (false, dest.y, dest.x);
			}
		}
		fist.GetComponent<Punch> ().sendPunch (dest);
		fist.transform.SetParent(monster.transform, true);

		Debug.Log (dest.x);
		//pD.smashCity(new Vector3(dest.x, dest.y, 10.0f));
	}

}