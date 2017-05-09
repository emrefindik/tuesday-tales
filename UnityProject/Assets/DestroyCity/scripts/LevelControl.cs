using System.Collections;
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
	const int tolerence = 2;
    int buildingDestroyedCount;
    int number_of_buildings = 1;
    bool win;   // check this status : whether has win
	public bool getWin()
	{
		return win;
	}

	// Count to know when to enable interior
	int fallenPiecesCount;		
	public punchAction2 pA;

	// Wait for Facebook Share process to finish
	static public FacebookManager.ShareStatus shareStatus;

	// Shake Action
	const float accelerometerUpdateInterval = (float)(1.0 / 60.0);
	const float lowPassKernelWidthInSeconds = 1.0f;
	float shakeDetectionThreshold = 2.0f;
	float lowPassFilterFactor = accelerometerUpdateInterval /
		lowPassKernelWidthInSeconds;
	Vector3 lowPassValue;
	bool shakeNow = false;
	//float shakeInterval = 10.0f;

	GameObject shakeText;
	GameObject ground;
	GameObject progressBar;
	MainController mainController;
	GameObject kaiju;
	Kaiju selectedKaiju;
	GameObject[] blocks;

	public AudioClip smeshAudio;
	public AudioClip[] screamAudio;

	[HideInInspector]
	public AudioSource smesh;
	[HideInInspector]
	public AudioSource scream;

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

		smesh = gameObject.AddComponent<AudioSource> ();
		smesh.clip = smeshAudio;
		smesh.playOnAwake = false;

		scream = gameObject.AddComponent<AudioSource> ();
		scream.playOnAwake = false;
    }

	// Set up information about this building
	// Should init System before init the level
    void initSystem()
    {
		GameObject city = GameObject.Find ("City");
		mainController = GameObject.Find ("MainController").GetComponent<MainController>();

		// Set up city first
		if (city) {
			// TODO: DELETE THIS this is for testing
			//city.GetComponent<BuildingCreator> ().setUpBuildingTest (3);
			// TODO: UNCOMMENT THIS
			city.GetComponent<BuildingCreator> ().setUpBuilding (mainController.currentMarkerId);
		}
		
		number_of_buildings = 1;
        progressCount = new int[1];
		num_of_pieces = GameObject.FindGameObjectsWithTag("building").Length;
		blocks = GameObject.FindGameObjectsWithTag ("block");
		num_of_blocks = blocks.Length;
		totalProgress = num_of_pieces * (int)ProgressAmount.Building + (num_of_blocks - tolerence) * (int)ProgressAmount.Block;
		progressCount [0] = totalProgress;
		Debug.Log (num_of_blocks);


		for (int i = 0; i < blocks.Length; i++) {
			;
		}

		GameObject[] humans = GameObject.FindGameObjectsWithTag ("people");
		for (int i = 0; i < humans.Length; i++) {
			Rigidbody rb = humans [i].GetComponent<Rigidbody> ();
			if (rb == null)
				rb = humans [i].AddComponent<Rigidbody> ();
			rb.useGravity = false;
			humans [i].GetComponent<SpriteRenderer> ().sortingOrder = 1;
			humans [i].layer = LayerMask.NameToLayer ("Human");
		}

		ground = GameObject.FindGameObjectWithTag ("ground");
		ground.layer = LayerMask.NameToLayer ("GamePhysics");

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

		Debug.Log ("Canvas finished");
		// Init Reward
		score = 0;
		eggIndex = -1;
		eggName = "";

		Debug.Log ("REward f");

		// Init Game States
		buildingDestroyedCount = 0;
		win = false;
		shareStatus = FacebookManager.ShareStatus.None;
		fallenPiecesCount = 0;

		// Shake Action
		shakeDetectionThreshold *= shakeDetectionThreshold;
		lowPassValue = Input.acceleration;

		Debug.Log("Shake f ");
		// To Be Used GameObjects
		shakeText = gameCanvas.transform.FindChild("ShakeText").gameObject;
		shakeText.SetActive (false);
		ground = GameObject.FindGameObjectWithTag ("ground");
		progressBar = gameCanvas.transform.FindChild ("FullImage").gameObject;

		Debug.Log ("Progress finished");

		GetComponent<SpriteControl> ().deactivateColor ();

		MainMenuScript mainMenu = mainController.GetComponent<MainMenuScript> ();
		kaiju = GameObject.Find ("Kaiju").gameObject;
		selectedKaiju = mainMenu.SelectedKaiju;
		if (selectedKaiju != null) {
			kaiju.GetComponent<MonsterCreator> ().
			setUpMonster (selectedKaiju.HeadType, selectedKaiju.HandType, selectedKaiju.BodyType, selectedKaiju.MonsterColor);
		} else {
			kaiju.GetComponent<MonsterCreator> ().
			setUpMonster (6,1,1, Color.white);
		}

		Debug.Log ("Setup Kaiju");

		StartCoroutine (closeDestroyText(2.0f));
	}

	IEnumerator closeDestroyText(float seconds)
	{
		yield return new WaitForSeconds (seconds);
		Debug.Log ("Inside close destroy");
		GameObject destroyText = gameCanvas.transform.FindChild("DestroyText").gameObject;
		destroyText.SetActive (false);
		Debug.Log ("Set active false finished");
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
			if (winCanvas && (winCoroutineEnded.Success == true)) {
			//if (winCanvas) {
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
			StartCoroutine(MainController.single.addDestoryCityReward(score, winCoroutineEnded));
			ground = GameObject.FindGameObjectWithTag("ground");
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
				Debug.Log ("egg index start: " + eggIndex.ToString ());
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

        float offset = Time.time;
        if (eggIndex != -1)
        {
			Debug.Log ("Calling Coroutine");
            yield return KaijuDatabase.instance.checkAndDownloadEggSprite(eggIndex, new CoroutineResponse());
			Debug.Log ("Coroutine Finished");
            GameObject EggImage = findEggCanvas.transform.FindChild("EggImage").gameObject;
			Debug.Log ("egg index start: " + eggIndex.ToString ());
            EggImage.GetComponent<Image>().sprite = KaijuDatabase.instance.EggSprites[eggIndex];
            //StartCoroutine (startShakeCountDown ());
            offset = Time.time - offset;
        }
        //TODO: shake according to the streak
        if (offset < FIRST_SHAKE_WAIT)
            yield return new WaitForSeconds(FIRST_SHAKE_WAIT - offset);
        
		pA.punchGround(2);
		Handheld.Vibrate ();
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
		StartCoroutine(OwnedEgg.createEggForSelf (mainController.GetComponent<MainMenuScript> ().EggMenuItemPrefab, 
			mainController.GetComponent<MainMenuScript> ().EggMenuContentPanel, eggIndex, eggName));
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

	public void playScream()
	{
		scream.clip = screamAudio[Random.Range (0, screamAudio.Length - 1)];
		scream.Play ();
	}

	public void playSmesh()
	{
		smesh.Play ();
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
