﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LevelControl : MonoBehaviour {
    private const float FIRST_SHAKE_WAIT = 2.0f;

    public CoroutineResponse winCoroutineEnded;    

	// Canvas
	public GameObject winCanvas;
	public GameObject gameCanvas;
	public GameObject findEggCanvas;
	public GameObject editEggNameCanvas;
	public GameObject shareSucceedCanvas;

	public int score;
    const  int scoreBase = 100;

	// Init STates : this should be set by building creator
    private int num_of_pieces;
    private int num_of_people;
	private int num_of_blocks;
    //float progressStep;
    
    //float tolerance = 300;
    int level = 0;

    //float currentTime = 0;
    //bool currentTimeOff = true;

	// Game States
	//float[] progress;
	int[] progressCount;
	int totalProgress;
    int buildingDestroyedCount;
    int number_of_buildings = 1;
    bool win;   // check this status : whether has win

	// Count to know when to enable interior
	int fallenPiecesCount;		
	public punchAction2 pA;

	// Wait for Facebook Share process to finish
	static public FacebookManager.ShareStatus shareStatus;

	// Shake Action
	const float accelerometerUpdateInterval = (float)(1.0 / 60.0);
	const float lowPassKernelWidthInSeconds = 1.0f;
	float shakeDetectionThreshold = 6.0f;
	float lowPassFilterFactor = accelerometerUpdateInterval /
		lowPassKernelWidthInSeconds;
	Vector3 lowPassValue;
	bool shakeNow = false;
	//float shakeInterval = 10.0f;

	GameObject shakeText;
	GameObject ground;
	GameObject progressBar;
	GameObject mainController; 

	// REWARD:
	int eggIndex;
	string eggName;

	// FOR TEST
	bool inited = false;

	public enum ScoreType
	{
		Building,
		Human,
		Blocks
	}

	public enum ScoreAmount
	{
		Building = 2,
		Human = 1,
		Block = 3
	}

	public enum ProgressAmount
	{
		Human = 0,
		Building = 1,
		Block = 2
	}

	/**********************************
	 * 
	 * 			Initialization
	 * 
	 * ********************************/

    void Start() {
        initSystem();
		initLevel();
        winCoroutineEnded = new CoroutineResponse();
    }

	// Set up information about this building
	// Should init System before init the level
    void initSystem()
    {
		GameObject city = GameObject.Find ("City");
		mainController = GameObject.Find ("MainController");

		// Set up city first
		if (city) {
			// TODO: DELETE THIS this is for testing
			city.GetComponent<BuildingCreator> ().setUpBuildingTest (3);
			// TODO: UNCOMMENT THIS
			//city.GetComponent<BuildingCreator> ().setUpBuilding (mainController.GetComponent<MainController>().currentMarkerId);
		}
		
		number_of_buildings = 1;
        progressCount = new int[1];
		num_of_pieces = GameObject.FindGameObjectsWithTag("building").Length;
		num_of_blocks = GameObject.FindGameObjectsWithTag ("block").Length;
		Debug.Log ("Pieces " + num_of_pieces);
		Debug.Log ("Blocks" + num_of_blocks);
		totalProgress = num_of_pieces * (int)ProgressAmount.Building + num_of_blocks * (int)ProgressAmount.Block;
		progressCount [0] = totalProgress;
    }

	// Init all the game states
	void initLevel ()
	{
		// Init Cnavas
		gameCanvas.SetActive (true);
		winCanvas.SetActive(false);
		shareSucceedCanvas.SetActive(false);
		findEggCanvas.SetActive (false);
		editEggNameCanvas.SetActive (false);

		// Init Reward
		score = 0;
		eggIndex = -1;
		eggName = "";

		// Init Game States
		buildingDestroyedCount = 0;
		win = false;
		shareStatus = FacebookManager.ShareStatus.None;
		fallenPiecesCount = 0;

		// Shake Action
		shakeDetectionThreshold *= shakeDetectionThreshold;
		lowPassValue = Input.acceleration;

		// To Be Used GameObjects
		shakeText = gameCanvas.transform.FindChild("ShakeText").gameObject;
		shakeText.SetActive (false);
		ground = GameObject.FindGameObjectWithTag ("ground");
		progressBar = gameCanvas.transform.FindChild ("FullImage").gameObject;
	}

	/**********************************
	 * 
	 * 			Update
	 * 
	 * ********************************/

    void OnGUI()
    {
		switch (shareStatus) {
		case FacebookManager.ShareStatus.Init:
			{
				SendScreenshotToFacebook ();
				return;
			}
		case FacebookManager.ShareStatus.Sending:
			return;
		case FacebookManager.ShareStatus.Recieved:
			{
				shareStatus = FacebookManager.ShareStatus.None;
				// Should show a dialog to show status
				shareSucceedCanvas.SetActive(true);
				return;
			}
		default:
			break;
		}
		
		if (!win) {
			progressBar.transform.localScale = new Vector3 (0.53f * ((float)(progressCount [level]) / (totalProgress)), 0.53f, 0.53f);

		} else {
			if (findEggCanvas.activeSelf)
				return;
			// TODO: Change this back
			//if (winCanvas && (winCoroutineEnded.Success == true)) {
			if (winCanvas) {
				GameObject scoreText = winCanvas.transform.FindChild ("ScoreText").gameObject;
				scoreText.GetComponent<Text> ().text = "x " + score.ToString ();
				winCanvas.SetActive (true);
			}
		}
    }

	void Update()
	{

		// Check win
		if(buildingDestroyedCount == number_of_buildings)
		{
			shakeNow = true;
			shakeText.SetActive (true);
			buildingDestroyedCount = -1;
			// TODO: UNCOMMENT THIS
			//StartCoroutine(MainController.single.addDestoryCityReward(score, winCoroutineEnded));
			//ground = GameObject.FindGameObjectWithTag("ground");
			ground.GetComponent<Ground>().startShake(0.5f);
		}

		if (shakeNow) {
			Vector3 acceleration = Input.acceleration;
			lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
			Vector3 deltaAcceleration = acceleration - lowPassValue;

			if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
			{
				shakeNow  = false;
				shakeText.SetActive (false);
				eggIndex = KaijuDatabase.instance.generateEgg ();
				StartCoroutine (showReward ());
			}
		}


		/* // OLD VERSION: FOR SWIPE
		if (switchNow)
		{
			goToLevel(level);
		}
		//pD.switchCity(level);
		*/
	}
		
	/**********************************
	 * 
	 * 			Triggered Events
	 * 
	 * ********************************/

	public void increaseProgress(int unit, ScoreType type)
    {
		if (progressCount [level] < 0)
			return;
		
		ScoreAmount scoreAmount = 0;
		ProgressAmount progressAmount = 0;
		switch (type) {
		case ScoreType.Building:

			fallenPiecesCount++;
			if (fallenPiecesCount == num_of_pieces) {
				if (pA) {
					pA.level2On = true;
					//StartCoroutine (pA.startShakeCountDown ());
					GetComponent<SpriteControl> ().activateColor ();
				}
			}
			progressAmount = ProgressAmount.Building;
			scoreAmount = ScoreAmount.Building;
			break;

		case ScoreType.Blocks:
			progressAmount = ProgressAmount.Block;
			scoreAmount = ScoreAmount.Block;
			break;

		case ScoreType.Human:
			progressAmount = ProgressAmount.Human;
			scoreAmount = ScoreAmount.Human;
			break;
		default:
			break;
		}

		progressCount [level] -= unit * (int)progressAmount;
		increaseScore(scoreBase * unit * (int)scoreAmount);
        
        if (progressCount[level] == 0)
        {
            //structures[level].GetComponent<PolygonCollider2D>().enabled = true;
            progressCount[level]--;
            buildingDestroyedCount++;
        }

		//Debug.Log (progressCount [level]);
    }

    public void increaseScore(int increment)
    {
        score += increment;
    }
		
	IEnumerator showReward()
	{
		// TODO: UNCOMMENT THIS DISABLED FOR TEST
		/*
        float offset = Time.time;
        if (eggIndex != -1)
        {
            yield return KaijuDatabase.instance.checkAndDownloadEggSprite(eggIndex, new CoroutineResponse());
            GameObject EggImage = findEggCanvas.transform.FindChild("EggImage").gameObject;
            EggImage.GetComponent<Image>().sprite = KaijuDatabase.instance.eggSprites[eggIndex];
            //StartCoroutine (startShakeCountDown ());
            offset = Time.time - offset;
        }
        //TODO: shake according to the streak
        if (offset < FIRST_SHAKE_WAIT)
            yield return new WaitForSeconds(FIRST_SHAKE_WAIT - offset);
        */
		pA.punchGround(2);
		yield return new WaitForSeconds (.3f);
		if(eggIndex != -1)
			findEggCanvas.SetActive(true);
		win = true;
	}
		
	/**********************************
	 * 
	 * 			Button Callbacks
	 * 
	 * ********************************/

	public void closeEggCanvas()
	{
		findEggCanvas.SetActive (false);
		editEggNameCanvas.SetActive (false);
		GameObject inputName = editEggNameCanvas.transform.FindChild ("InputEggName").gameObject;
		eggName = inputName.GetComponent<InputField> ().text;
		Debug.Log (eggName);
        // QUESTION FROM EMRE - IS THIS WHERE WE SHOULD CALL CREATEEGGFORSELF?
	}

	public void BackToMainMenu()
	{
		MainController.single.goBack();
	}

	public void initShareToFacebook()
	{
		shareStatus = FacebookManager.ShareStatus.Init;
		winCanvas.SetActive (false);
		gameCanvas.SetActive (false);
	}

	void SendScreenshotToFacebook()
	{
		shareStatus = FacebookManager.ShareStatus.Sending;
		FacebookManager.single.ShareScreenshotToFacebook();
	}

	public void openEditEggNameCanvas()
	{
		editEggNameCanvas.SetActive (true);
	}
		


	/**********************************
	 * 
	 * 		Saved for Older Version
	 * 
	 * ********************************/

	/* Old Version
	//public GameObject[] realBuilding = new GameObject[1];
	//public GameObject[] realBuildingbuildingCover = new GameObject[1];
	//public GameObject[] structures = new GameObject[1];
	//public bool switchNow=false;
	//PixelDestruction pD;
	//float[] levelPositions;
	//Vector2 fingerStart;
	//Vector2 fingerEnd;
	*/

	/*
    void goToLevel(int level)
    {
        float currentX = 0;
        currentX = buildings.transform.position.x;
        if (Mathf.Abs(currentX - levelPositions[level]) >= .02f)
        {
            GetcurrentTime();
            float newPos = 0;
            newPos = Mathf.Lerp(currentX, levelPositions[level], (Time.time - currentTime) * .1f);
            buildings.transform.position = new Vector3(newPos, buildings.transform.position.y, buildings.transform.position.z);
        }
        else
        {
            for(int i=0; i<realBuilding.Length; i++)
            {
                realBuilding[i].SetActive(false);
                realBuildingbuildingCover[i].SetActive(true);
            }
            realBuilding[level].SetActive(true);
            realBuildingbuildingCover[level].SetActive(false);
            switchNow = false;
            currentTimeOff = true;
        }
    }
    */

	/*
    private bool CheckForPatternMove(int startIndex, int lengthOfPattern, List<Movement> movementToCheck)
    {
        if (switchNow == true)
        {
            return false;
        }
        //If the currently stored movements are fewer than the length of the pattern to be detected
        //it can never match the pattern. So, let's get out
        if (lengthOfPattern > movements.Count)
            return false;

        //In case the start index for the check plus the length of the pattern
        //exceeds the movement list's count, it'll throw an exception, so lets get out
        if (startIndex + lengthOfPattern > movements.Count)
            return false;

        //Populate a temporary list with the respective elements
        //from the movement list
        List<Movement> tMovements = new List<Movement>();
        for (int i = startIndex; i < startIndex + lengthOfPattern; i++)
            tMovements.Add(movements[i]);

        //Now check whether the sequence of movements is the same as the pattern you want to check for
        //The SequenceEqual method is in the System.Linq namespace
        return tMovements.SequenceEqual(movementToCheck);

    }
	*/
	/*
    void GetcurrentTime()
    {
        if (currentTimeOff==true)
        {
            currentTimeOff = false;
            currentTime = Time.time;
        }
    }
	*/

}
