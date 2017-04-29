using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class punchAction2 : MonoBehaviour {

	public GameObject leftFist;
	public GameObject rightFist;
	public GameObject monster;

	Vector3 _inputPosition;
	Vector3 _startPos;
	Vector3 _endPos;
	float startTime;
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

	public AudioClip throwClip;
	AudioSource throwSound;

    void Start () {
        leftFist.GetComponent<Collider>().enabled = false;
        rightFist.GetComponent<Collider>().enabled = false;

		locked = false;
		level2On = false;

		throwSound = gameObject.AddComponent<AudioSource> ();
		throwSound.clip = throwClip;
		throwSound.playOnAwake = false;

    }


	private Vector3 oldPosition;
	private Vector3 screenPoint;
	private Vector3 offset;
	public Camera cam;
	private GameObject draggingObject;
	private GameObject movingFist;

	void Update () {

		if (locked) {
			locked = false;
			return;
		}
			

		// mouse input need change to phone touch input
		if (Input.GetMouseButtonDown(0))
		{
			_inputPosition = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10f));
			startTime = Time.time;
			_startPos = _inputPosition;
			Debug.Log ("GetMouseButtonDown");
			punchStrength = 0;
			pressEffect.transform.position = new Vector3 (_inputPosition.x, _inputPosition.y, -1f);
			pressEffect.Play ();

			if (level2On) {
				
				if (_inputPosition.x < BUILDING_CENTER_X) {
					Debug.Log (leftFist);
					movingFist = Instantiate (leftFist, new Vector3 (IDLE_L_X, 5, PUNCH_Z), Quaternion.identity);
					movingFist.layer = LayerMask.NameToLayer ("Human");
					movingFist.GetComponent<Collider> ().enabled = true;
				}
				if (_inputPosition.x >= BUILDING_CENTER_X) {
					Debug.Log (rightFist);
					movingFist = Instantiate (rightFist, new Vector3 (-IDLE_L_X, 5, PUNCH_Z), Quaternion.identity);
					movingFist.layer = LayerMask.NameToLayer ("Human");
					movingFist.GetComponent<Collider> ().enabled = true;

				}			

				
				Debug.Log ("level2On");
				screenPoint = cam.WorldToScreenPoint (gameObject.transform.position);

				Ray ray = cam.ScreenPointToRay (Input.mousePosition);
				RaycastHit hit;

				if (Physics.Raycast (ray, out hit, 100)) {
					Debug.Log ("hit");
					Debug.Log (hit.transform.gameObject.name);
					if (hit.transform.gameObject.tag == "block" || hit.transform.gameObject.tag == "people") {
						draggingObject = hit.transform.gameObject;
						draggingObject.GetComponent<Rigidbody> ().useGravity = true;
						Debug.Log (draggingObject);
						offset = draggingObject.transform.position - cam.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
						if (draggingObject.GetComponent<Animator> () != null) {
							draggingObject.GetComponent<Animator> ().Stop ();
						}
					}
				}



			}

		}

		if (Input.GetMouseButton (0)) {
			_inputPosition = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10f));

			// add punch strength
			if (punchStrength < PUNCH_STRENGTH_MAX) {
				punchStrength += PUNCH_STRENGTH_STEP;
				pressEffect.transform.localScale = new Vector3 (1 + punchStrength, 1 + punchStrength, 1 + punchStrength);
			}

			if (level2On) {
				Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

				Vector3 curPosition = cam.ScreenToWorldPoint (curScreenPoint) + offset;

				if (draggingObject != null) {
					draggingObject.transform.position = curPosition;
				}

				oldPosition = curPosition;
				movingFist.transform.position = _inputPosition;
			}

		}


		if(Input.GetMouseButtonUp(0))
		{
			_inputPosition = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10f));
			if (!level2On) {
				tapCheckX = Input.mousePosition.x;
				pressEffect.Stop ();

				if (Mathf.Abs (Input.mousePosition.x - tapCheckX) > TAPTOLERENCE)
					return;
				if (_inputPosition.x < BUILDING_CENTER_X) {
					initPunch (PUNCHINGLEFT);
				}
				if (_inputPosition.x >= BUILDING_CENTER_X) {
					initPunch (PUNCHINGRIGHT);
				}
			}

			else{
				_endPos = _inputPosition;
				Debug.Log ("button up, setting null");
				Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
				Vector3 curPosition = cam.ScreenToWorldPoint (curScreenPoint) + offset;
				Vector3 vel = curPosition - oldPosition;
				float magnitude = Vector3.Distance (_startPos, _endPos);
				vel = vel.normalized * magnitude * 0.2f;
				if (draggingObject != null) {
					Rigidbody rigidBody = draggingObject.GetComponent<Rigidbody> ();
					rigidBody.AddForce (vel * rigidBody.mass * 20, ForceMode.Impulse);
					draggingObject = null;
					throwSound.Play ();
				}

				oldPosition = new Vector3 (0, 0, 0);
				movingFist.GetComponent<Rigidbody> ().velocity = vel * 2;
				if(magnitude < 2f)
					StartCoroutine (_DelayDestroy (movingFist, 0.1f));
				else
					StartCoroutine (_DelayDestroy (movingFist, 0.5f));
			}
		}

	}

	IEnumerator _DelayDestroy(GameObject g, float time)
	{
		yield return new WaitForSeconds (time);
		Destroy (g);
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