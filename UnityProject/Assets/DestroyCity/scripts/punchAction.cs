using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class punchAction : MonoBehaviour {

    public GameObject leftFist;
    public GameObject rightFist;
	public GameObject monster;
    public PixelDestruction pD;
	/*
    Vector2 currentFist_l;
    Vector2 currentFist_r;
    */
	/*
    float startTimeL;
    float startTimeR;

    const float HIT_TIME_BASE = 100.0f;
    public float HitTime_left = HIT_TIME_BASE;
    public float HitTime_right = HIT_TIME_BASE;
*/
    //public bool punchNowL = false;
    //public bool punchNowR = false;

	/*
    [HideInInspector]
    public  bool hitting_left = false;
    [HideInInspector]
    public bool hitting_right = false;
    [HideInInspector]
    public bool punchReturning_l=false;
    [HideInInspector]
    public bool punchReturning_r = false;
    */

    Vector3 _inputPosition;
    float tapCheckX;
    const float TAPTOLERENCE = 10;

    //float newPosY = 0.0f;
    //float newPosX = 0.0f;

    //const float newPosY_r = -1.78f;
    //const float newPosX_r = 5;
	/*
    Vector2 newPosL;
    Vector2 newPosR;
    Vector2 destPosL;
    Vector2 destPosR;
    */
    const float IDLE_L_X = -5.0f;

    //bool currentTimeOff;

    //float _currentTime;
	/*
    float startTime_l;
    float startTime_r;
	*/
    //float j = 0;
    //float i = 0;
    //float h = 0;
    //float z = 0;
    float timeCountLX = 0;
    float timeCountRX = 0;

    const bool PUNCHINGLEFT = false;
    const bool PUNCHINGRIGHT = true;
    const float BUILDING_CENTER_X = 0.0f;

	/*
    const float STOP_PUNCHING_GAP = 0.02f;
    const float TRANSITION_Y_TIME = 20.0f;
	*/
	// one touch control for now
	float punchStrength;
	const float PUNCH_STRENGTH_MAX = 4.9f;
	const float PUNCH_STRENGTH_STEP = 0.04f;
	public ParticleSystem pressEffect;
	//ParticleSystem pressEffectInstance;
	ParticleSystem.Particle[] m_Particles;

	const float PUNCH_Z = -1;

    void Start () {
		/*
        startTimeL = 0.0f;
        startTimeR = 0.0f;
        newPosL = new Vector2(0.0f, 0.0f);
        newPosR = new Vector2(-1.78f, 5.0f);
		*/
        leftFist.GetComponent<Collider>().enabled = false;
        rightFist.GetComponent<Collider>().enabled = false;
    }

	void Update () {

        // mouse input need change to phone touch input
        if (Input.GetMouseButtonDown(0))
        {
			//Debug.Log ("Getting Mouse Button Down");
            //tapCheckX = Input.mousePosition.x;
			punchStrength = PUNCH_STRENGTH_MAX;
			//_inputPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
			pressEffect.transform.position = new Vector3 (_inputPosition.x, _inputPosition.y, -0.2f);
			pressEffect.Play ();
        }

		if (Input.GetMouseButton (0)) {
			//Debug.Log ("Getting Mouse Button ");

			// add punch strength
			if (punchStrength < PUNCH_STRENGTH_MAX) {
				punchStrength += PUNCH_STRENGTH_STEP;
				pressEffect.transform.localScale = new Vector3 (1 + punchStrength, 1 + punchStrength, 1 + punchStrength);
			}

			/*
			int numParticlesAlive = pressEffect.GetParticles(m_Particles);
			// Change only the particles that are alive
			for (int i = 0; i < numParticlesAlive; i++)
			{
				m_Particles [i].startSize = 1.5f + punchStrength;
			}

			pressEffect.SetParticles (m_Particles, numParticlesAlive);
			*/

		}

        if(Input.GetMouseButtonUp(0))
        {
			tapCheckX = Input.mousePosition.x;
			_inputPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
			pressEffect.Stop ();

            if (Mathf.Abs(Input.mousePosition.x - tapCheckX) > TAPTOLERENCE)
                return;

            //_inputPosition = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 10f));
            

            //if (_inputPosition.x < BUILDING_CENTER_X && !hitting_left)
			if (_inputPosition.x < BUILDING_CENTER_X)
			{
                initPunch(PUNCHINGLEFT);
            }
            //if (_inputPosition.x >= BUILDING_CENTER_X && !hitting_right)
			if (_inputPosition.x >= BUILDING_CENTER_X)
			{
                initPunch(PUNCHINGRIGHT);
            }
        }

        //updateTimer();
        //punch();

    }

	/*
     void updateFistLocation()
    {
        currentFist_l = new Vector2(leftFist.transform.position.x, leftFist.transform.position.y);
        currentFist_r = new Vector2(rightFist.transform.position.x, rightFist.transform.position.y);
    }
	*/
	/*
    void punch()
    {
        updateFistLocation();
        if (hitting_left)
        {

            //if (Mathf.Abs(newPosL.x - _inputPosition.x) >= .2f && punchReturning_l == false)
            if (punchReturning_l == false)
            {
                
                // Fist reaching out
                if (Mathf.Abs(newPosL.y - destPosL.y) >= STOP_PUNCHING_GAP)
                {
                    
                    // First Y

                    //newPosL.y = Mathf.Lerp(currentFist_l.y, _inputPosition.y, (Time.time - startTimeL) * 4.5f);
                    newPosL.y = Mathf.Lerp(currentFist_l.y, destPosL.y, (Time.time - startTimeL) * TRANSITION_Y_TIME);
                    leftFist.transform.position = new Vector3(leftFist.transform.position.x, newPosL.y, leftFist.transform.position.z);
                }

                //if (Mathf.Abs(newPosL.y - _inputPosition.y) < .3f)
                else
                {
                    // Then X

                    timeCountLX = timeCountLX + .1f;    
                    //i = Random.Range(-1.5f, 0.5f);  // ??? What is the range about
                    //newPosL.x = Mathf.Lerp(currentFist_l.x, i, j);
                    newPosL.x = Mathf.Lerp(currentFist_l.x, destPosL.x, timeCountLX);
                    leftFist.transform.position = new Vector3(newPosL.x, leftFist.transform.position.y, leftFist.transform.position.z);
                }

            }


            //if (Mathf.Abs(newPosL.x - destPosL.x) < .3f && punchReturning_l == false)   // Why .3f
            if (Mathf.Abs(newPosL.x - destPosL.x) < STOP_PUNCHING_GAP && punchReturning_l == false)
            {
                // Reach Location
                updateFistLocation();
                punchReturning_l = true;

                // disable collider
                leftFist.GetComponent<Collider>().enabled = false;
            }

            if (punchReturning_l == true)
            {
                // Coming back Animation
                newPosL.x = Mathf.Lerp(currentFist_l.x, IDLE_L_X, (Time.time - startTimeL) * .2f);
                leftFist.transform.position = new Vector3(newPosL.x, leftFist.transform.position.y, leftFist.transform.position.z);

            }
        }

        if (hitting_right)
        {
            //if (Mathf.Abs(newPosR.x - _inputPosition.x) >= .2f && punchReturning_r == false)
            if (punchReturning_r == false)
            {
                if (Mathf.Abs(newPosR.y - destPosR.y) >= STOP_PUNCHING_GAP)
                {
                    // First Y
                    newPosR.y = Mathf.Lerp(currentFist_r.y, _inputPosition.y, (Time.time - startTimeR) * TRANSITION_Y_TIME);
                    rightFist.transform.position = new Vector3(rightFist.transform.position.x, newPosR.y, rightFist.transform.position.z);

                }
                //if (Mathf.Abs(newPosR.y - destPosR.y) < .3f)
                else
                {
                    //z = z + .05f;   // Why this value???
                    //h = Random.Range(0.2f, 0.0f);     // Why this range???
                    //newPosR.x = Mathf.Lerp(currentFist_r.x, h, z);
                    timeCountRX = timeCountRX + .1f;
                    newPosR.x = Mathf.Lerp(currentFist_r.x, destPosR.x, timeCountRX);
                    rightFist.transform.position = new Vector3(newPosR.x, rightFist.transform.position.y, rightFist.transform.position.z);
                }

            }

            if (Mathf.Abs(newPosR.x - destPosR.x) < STOP_PUNCHING_GAP && punchReturning_r == false)
            {

                updateFistLocation();
                punchReturning_r = true;

                // Disable collider
                rightFist.GetComponent<Collider>().enabled = false;
            }

            if (punchReturning_r == true)
            {

                newPosR.x = Mathf.Lerp(currentFist_r.x, -IDLE_L_X, (Time.time - startTimeR) * .2f);
                rightFist.transform.position = new Vector3(newPosR.x, rightFist.transform.position.y, rightFist.transform.position.z);
            }
        }
    }
	*/

    /*
     * Update location and increase timer
     */
    void initPunch(bool punchDirection)
    {
		/*
        updateFistLocation();
        if(punchDirection == PUNCHINGLEFT)
        {          
			Debug.Log ("PunchLeft");
            startTimeL = Time.time;
            hitting_left = true;
            newPosL = new Vector2(currentFist_l.x, currentFist_l.y);
            //destPosL.x = Random.Range(-1.5f, 0.0f);
			destPosL.x = -1.5f + punchStrength;
            destPosL.y = _inputPosition.y;
            //leftFist.GetComponent<spawnFist>().sendPunch(true, destPosL.y, destPosL.x);
            pD.smashCity(new Vector3(destPosL.x, destPosL.y, 10.0f));
            leftFist.GetComponent<Collider>().enabled = true;
        }
        else
        {   
			Debug.Log ("PunchRight");
            startTimeR = Time.time;
            hitting_right = true;
            newPosR = new Vector2(currentFist_r.x, currentFist_r.y);
            //destPosR.x = Random.Range(0.0f, 1.5f);
			destPosR.x = 1.5f - punchStrength;
            destPosR.y = _inputPosition.y;
            //rightFist.GetComponent<spawnFist>().sendPunch(false, destPosR.y, destPosR.x);
            pD.smashCity(new Vector3(destPosR.x, destPosR.y, 10.0f));
            rightFist.GetComponent<Collider>().enabled = true;

        }
        */
		GameObject fist;
		Vector2 dest;
		if (punchDirection == PUNCHINGLEFT) {
			dest = new Vector2 (IDLE_L_X + punchStrength, _inputPosition.y);
			fist = Instantiate (leftFist, new Vector3 (IDLE_L_X, dest.y, PUNCH_Z), Quaternion.identity);

		} else {
			dest = new Vector2 (-IDLE_L_X - punchStrength, _inputPosition.y);
			fist = Instantiate (rightFist, new Vector3 (-IDLE_L_X, dest.y, PUNCH_Z), Quaternion.identity);
		}
		fist.GetComponent<Punch> ().sendPunch (dest);
		fist.transform.SetParent(monster.transform, true);
		pD.smashCity(new Vector3(dest.x, dest.y, 10.0f));
    }

	/*
	void updateTimer()
    {
        // Update Left Fist
        if (hitting_left)
        {
            HitTime_left = HitTime_left + startTimeL - Time.time;
            //Debug.Log("HittingTime: " + HitTime_left);
            if (HitTime_left <= 0)
            {
                hitting_left = false;
                //punchNowL = false;
                punchReturning_l = false;
                HitTime_left = 10;
                leftFist.transform.position = new Vector3(IDLE_L_X, leftFist.transform.position.y, leftFist.transform.position.z);
                timeCountLX = 0;
            }
        }

        // Update Right Fist
        if(hitting_right)
        {
            HitTime_right = HitTime_right + startTimeR - Time.time;
            if (HitTime_right > 0)
            {
                hitting_right = true;
            }
            else
            {
                hitting_right = false;
                //punchNowR = false;
                punchReturning_r = false;
                HitTime_right = 10f;
                rightFist.transform.position = new Vector3(-IDLE_L_X, rightFist.transform.position.y, rightFist.transform.position.z);
                timeCountRX = 0;
            }
        }




    }
    */

}
