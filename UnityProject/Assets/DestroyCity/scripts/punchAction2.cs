﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class punchAction2 : MonoBehaviour {

	public GameObject leftFist;
	public GameObject rightFist;

	public GameObject leftFistIdle;
	public GameObject rightFistIdle;
	bool isLeft;
	int leftCount;
	int rightCount;

	public GameObject monster;
	public LevelControl levelControl;
	public Material trailMat;

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

		leftCount = 0;
		rightCount = 0;
    }


	private Vector3 oldPosition;
	private Vector3 screenPoint;
	private Vector3 offset;
	public Camera cam;
	private GameObject draggingObject;
	private GameObject movingFist;
	private GameObject movingIdleFist;

	// To move the idle fist
	public Vector3 Target;
	public float RotationSpeed;
	private Quaternion _lookRotation;
	private Vector3 _direction;

	void Update () {

		if (locked) {
			locked = false;
			return;
		}

		if (levelControl.getWin ()) {
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


			//if (level2On) {
				
				if (_inputPosition.x < BUILDING_CENTER_X) {
					//leftFistIdle.SetActive (false);
					Debug.Log (leftFist);
					
					movingFist = Instantiate (leftFist, new Vector3 (IDLE_L_X, 5, PUNCH_Z), Quaternion.identity);
					leftCount ++ ;
					movingIdleFist = leftFistIdle;
				if (level2On) {
					movingFist.layer = LayerMask.NameToLayer ("Human");
					movingFist.GetComponentInChildren<SpriteRenderer> ().sortingOrder = -1;
					} 
					
				
					movingFist.GetComponent<Collider> ().enabled = true;
					isLeft = true;
					
				}
				if (_inputPosition.x >= BUILDING_CENTER_X) {
					//rightFistIdle.SetActive (false);
					Debug.Log (rightFist);
					
					movingFist = Instantiate (rightFist, new Vector3 (-IDLE_L_X, 5, PUNCH_Z), Quaternion.identity);
					rightCount ++;
					movingIdleFist = rightFistIdle;
					if (level2On) {
						movingFist.layer = LayerMask.NameToLayer ("Human");
						movingFist.GetComponentInChildren<SpriteRenderer> ().sortingOrder = -1;
					} 
					
					movingFist.GetComponent<Collider> ().enabled = true;
					isLeft = false;
					
				}			

				
				Debug.Log ("level2On");
				screenPoint = cam.WorldToScreenPoint (gameObject.transform.position);

				Ray ray = cam.ScreenPointToRay (Input.mousePosition);
				LayerMask layerMask = -1;
				if(!level2On)
					layerMask = ~(1 << LayerMask.NameToLayer ("BuildingPhysics"));
				RaycastHit hit;

				if (Physics.Raycast (ray, out hit, 100, layerMask)) {
					Debug.Log ("hit");
					Debug.Log (hit.transform.gameObject.name);
					if (hit.transform.gameObject.tag == "block" || hit.transform.gameObject.tag == "people") {
						if (hit.transform.gameObject.tag == "people") {
							CharacterController cc = hit.transform.gameObject.GetComponent<CharacterController> ();
							if (cc)
								cc.enabled = false;
						} else {
							Rigidbody[] children = hit.transform.GetComponentsInChildren<Rigidbody> ();
							foreach (Rigidbody child in children) {
								child.useGravity = true;
							}
							CharacterController [] cc = hit.transform.GetComponentsInChildren<CharacterController> ();
							foreach (CharacterController c in cc) {
								c.enabled = false;
							}
						}
						draggingObject = hit.transform.gameObject;

						// Add Trail to object
						TrailRenderer trail = draggingObject.GetComponent<TrailRenderer>();
						if( trail == null)
							trail = draggingObject.AddComponent<TrailRenderer>();
						trail.material = trailMat;
						trail.startWidth = 0.3f;
						trail.endWidth = 0.1f;
						trail.startColor = Color.white;
						trail.endColor = new Color (1, 1, 1, 0.5f);
						trail.time = 1;


						draggingObject.GetComponent<Rigidbody> ().useGravity = true;
						Debug.Log (draggingObject);
						offset = draggingObject.transform.position - cam.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
						if (draggingObject.GetComponent<Animator> () != null) {
							draggingObject.GetComponent<Animator> ().Stop ();
						}
					}
				}



			//}

		}

		if (Input.GetMouseButton (0)) {
			_inputPosition = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10f));

			// add punch strength
			if (punchStrength < PUNCH_STRENGTH_MAX) {
				punchStrength += PUNCH_STRENGTH_STEP;
				pressEffect.transform.localScale = new Vector3 (1 + punchStrength, 1 + punchStrength, 1 + punchStrength);
			}

			//if (level2On) {
				//Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);

				//Vector3 curPosition = cam.ScreenToWorldPoint (curScreenPoint) + offset;
				//Vector3 curPosition = cam.ScreenToWorldPoint (curScreenPoint) + offset;

				if (draggingObject != null) {
				if (!inBound (_inputPosition)) {
					releasePunch ();
					draggingObject = null;
					} else {
						Vector3 curPosition = new Vector3 (_inputPosition.x, _inputPosition.y, draggingObject.transform.position.z);
						draggingObject.transform.position = curPosition;
						oldPosition = curPosition;
					}

				}
				
				if(movingFist)
					movingFist.transform.position = _inputPosition;

			if (movingIdleFist) {
				Transform shoulder = movingIdleFist.transform.parent;
				Debug.Log ("rotation");
				Target = new Vector3 (_inputPosition.x, _inputPosition.y, shoulder.position.z);
				Vector3 shoulderToTarget = Target - shoulder.position;

				Vector3 newDir = Vector3.RotateTowards (shoulder.forward, shoulderToTarget, 100, 0.0f);
				shoulder.rotation = Quaternion.LookRotation (newDir);
			}
					
			//}

		}


		if(Input.GetMouseButtonUp(0))
		{
			_inputPosition = Camera.main.ScreenToWorldPoint (new Vector3 (Input.mousePosition.x, Input.mousePosition.y, 10f));

			/*
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
			*/
			//else{

				_endPos = _inputPosition;
				Debug.Log ("button up, setting null");
				//Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
				//Vector3 curPosition = cam.ScreenToWorldPoint (curScreenPoint) + offset;
				releasePunch();
				
					
			//}
		}

	}

	void releasePunch()
	{
		float magnitude = Vector3.Distance (_startPos, _endPos);

		if (draggingObject != null) {
			Vector3 curPosition = new Vector3(_inputPosition.x, _inputPosition.y, draggingObject.transform.position.z);
			Vector3 vel = curPosition - oldPosition;
			if (vel.magnitude > 0.2f) {
				vel = vel.normalized * magnitude * 0.4f;
				Rigidbody rigidBody = draggingObject.GetComponent<Rigidbody> ();
				rigidBody.AddForce (vel * rigidBody.mass * 20, ForceMode.Impulse);
				movingFist.GetComponent<Rigidbody> ().velocity = vel * 2;
				throwSound.Play ();
			}

			draggingObject = null;

		}

		oldPosition = new Vector3 (0, 0, 0);



		if(magnitude < 2f)
			StartCoroutine (_DelayDestroy (movingFist, 0.3f, isLeft));
		else
			StartCoroutine (_DelayDestroy (movingFist, 0.5f, isLeft));
		
	}

	bool inBound(Vector3 position)
	{
		return position.y < 4.7f && position.y > -4.0f;
	}

	IEnumerator _DelayDestroy(GameObject fist, float time, bool isLeft)
	{
		yield return new WaitForSeconds (time);
		if (isLeft ) {
			leftCount--;
			if(leftCount == 0)
				leftFistIdle.SetActive (true);
		} else {
			rightCount--;
			if(rightCount == 0)
				rightFistIdle.SetActive (true);
		}
		Destroy (fist);
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