using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class LevelControl : MonoBehaviour {
    private const float FIRST_SHAKE_WAIT = 2.0f;

    public CoroutineResponse winCoroutineEnded;    

    public GameObject buildings;
    public GameObject[] realBuilding = new GameObject[4];
    public GameObject[] realBuildingbuildingCover = new GameObject[4];
    public GameObject[] structures = new GameObject[4];
	public GameObject winCanvas;
	public GameObject gameCanvas;
	public GameObject findEggCanvas;
	public GameObject editEggNameCanvas;
	public GameObject shareSucceedCanvas;
    public bool switchNow=false;
	public Texture2D barFull;
	public Texture2D barEmpty;
	public int score;

	//PixelDestruction pD;
    float[] levelPositions;
	//Vector2 fingerStart;
	//Vector2 fingerEnd;

    // GUI
    Vector2 barPos = new Vector2(20, 40);
    Vector2 barSize = new Vector2(Screen.width-40, Screen.height/10);
    const int scoreBase = 100;

    //float[] progress;
    int[] progressCount;
    public int num_of_pieces;
    public int num_of_people;
	public int num_of_blocks;
	int totalProgress;
    //float progressStep;
    
    //float tolerance = 300;
    int level = 0;

    float currentTime = 0;
    bool currentTimeOff = true;

    int buildingDestroyedCount;
    int number_of_buildings;
    bool win;   // check this status : whether has win

	// For Jonathan's destroy city
	int fallenPiecesCount;
	public punchAction2 pA;

	// Wait for other process to finish
	static public FacebookManager.ShareStatus shareStatus;

	// Shake Action
	const float accelerometerUpdateInterval = (float)(1.0 / 60.0);
	const float lowPassKernelWidthInSeconds = 1.0f;
	float shakeDetectionThreshold = 6.0f;
	float lowPassFilterFactor = accelerometerUpdateInterval /
		lowPassKernelWidthInSeconds;
	Vector3 lowPassValue;
	bool shakeNow = false;
	float shakeInterval = 10.0f;

	GameObject shakeText;
	GameObject ground;
	GameObject progressBar;

	// REWARD:
	int eggIndex;
	string eggName;

	// FOR TEST
	bool inited = false;

    public enum Movement
    {
        Left,
        Right,
        Up,
        Down
    };

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

    public List<Movement> movements = new List<Movement>();

    void Start() {
        initSystem();
		initLevel();
        winCoroutineEnded = new CoroutineResponse();
    }

    void initSystem()
    {
        //pD = FindObjectOfType(typeof(PixelDestruction)) as PixelDestruction;

        number_of_buildings = realBuilding.Length;
        levelPositions = new float[number_of_buildings];
        //progress = new float[number_of_buildings];
        progressCount = new int[number_of_buildings];
        for (int i = 0; i < number_of_buildings; i++)
        {
            levelPositions[i] = -realBuilding[i].transform.position.x;
            //progress[i] = 1.0f;
			//progressCount[i] = num_of_pieces + num_of_people +num_of_blocks;
			// TODO: will there only be one building? if it is, remove the list.
			totalProgress = num_of_pieces * (int)ProgressAmount.Building + num_of_blocks * (int)ProgressAmount.Block + num_of_people * (int)ProgressAmount.Human;
			progressCount [i] = totalProgress;
			structures[i].GetComponent<PolygonCollider2D>().enabled = false;
        }
        //progressStep = 1.0f / (num_of_pieces + num_of_people);
  
    }

	void initLevel ()
	{
		winCanvas.SetActive(false);
		gameCanvas.SetActive (true);
		shareSucceedCanvas.SetActive(false);
		findEggCanvas.SetActive (false);
		editEggNameCanvas.SetActive (false);


		score = 0;
		eggIndex = -1;
		buildingDestroyedCount = 0;

		win = false;
		shareStatus = FacebookManager.ShareStatus.None;

		// For Jonathan's destroy city
		fallenPiecesCount = 0;
		// Shake Action
		shakeDetectionThreshold *= shakeDetectionThreshold;
		lowPassValue = Input.acceleration;
		//StartCoroutine (startShakeCountDown());

		shakeText = GameObject.Find ("ShakeText");
		shakeText.SetActive (false);
		ground = GameObject.Find ("Ground");
		progressBar = GameObject.Find ("FullImage");
	}
		
    private void OnGUI()
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
			/*
			GUI.BeginGroup (new Rect (barPos.x, barPos.y, barSize.x, barSize.y));
			GUI.Box (new Rect (0, 0, barSize.x, barSize.y), barEmpty);
			GUI.BeginGroup (new Rect (0, 0, barSize.x * (((float)(progressCount [level]) / (totalProgress))), barSize.y));
			GUI.Box (new Rect (0, 0, barSize.x, barSize.y), barFull);
			GUI.EndGroup ();
			GUI.EndGroup ();
			*/
			progressBar.transform.localScale = new Vector3 (0.53f * ((float)(progressCount [level]) / (totalProgress)), 0.53f, 0.53f);

			//GUI.skin.label.fontSize = (int)(Screen.height * 0.05);
			//GUI.Label (new Rect (20, (int)(Screen.height * 0.85), Screen.width / 3, (int)(Screen.height * 0.1)), score.ToString ());
		} else {
			if (findEggCanvas.activeSelf)
				return;
			//if (winCanvas && (winCoroutineEnded.Success == true)) {
			if (winCanvas) {
				GameObject scoreText = winCanvas.transform.FindChild ("ScoreText").gameObject;
				scoreText.GetComponent<Text> ().text = "x " + score.ToString ();
				//Debug.Log (scoreText.GetComponent<Text> ().text);
				winCanvas.SetActive (true);
			}
		}
    }

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

    void Update()
    {
		
		if (!inited) {
			GameObject city = GameObject.Find ("City");
			Debug.Log (city);
			if(city)
				city.GetComponent<BuildingCreator> ().setUpBuilding (1);
			inited = true;
		}


        // Check win
		// Test Win Status
        //if(buildingDestroyedCount == number_of_buildings)
		if(buildingDestroyedCount == number_of_buildings)
        {
            shakeNow = true;
			shakeText.SetActive (true);
            buildingDestroyedCount = -1;
			//StartCoroutine(MainController.single.addDestoryCityReward(score, winCoroutineEnded));
			ground = GameObject.Find ("Ground");
			ground.GetComponent<Ground>().startShake(0.5f);
        }

		/*
        if (Input.GetMouseButtonDown(0))
        {
            fingerStart = Input.mousePosition;
            fingerEnd = Input.mousePosition;
        }
        */
		if (shakeNow) {
			Vector3 acceleration = Input.acceleration;
			lowPassValue = Vector3.Lerp(lowPassValue, acceleration, lowPassFilterFactor);
			Vector3 deltaAcceleration = acceleration - lowPassValue;

			if (deltaAcceleration.sqrMagnitude >= shakeDetectionThreshold)
			{
				// Perform your "shaking actions" here. If necessary, add suitable
				// guards in the if check above to avoid redundant handling during
				// the same shake (e.g. a minimum refractory period).
				//Debug.Log("Shake event detected at time "+Time.time);
				//punchGround();
				shakeNow  = false;
				shakeText.SetActive (false);
				GameObject mainController = GameObject.Find ("MainController");
				eggIndex = KaijuDatabase.instance.generateEgg ();
				StartCoroutine (showReward ());
			}


		}

        if (switchNow)
        {
            goToLevel(level);
        }
        //pD.switchCity(level);
	}

	IEnumerator showReward()
	{
		// DISABLED FOR TEST
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

	public void closeEggCanvas()
	{
		findEggCanvas.SetActive (false);
		editEggNameCanvas.SetActive (false);
		GameObject inputName = editEggNameCanvas.transform.FindChild ("InputEggName").gameObject;
		eggName = inputName.GetComponent<InputField> ().text;
		Debug.Log (eggName);
        // QUESTION FROM EMRE - IS THIS WHERE WE SHOULD CALL CREATEEGGFORSELF?
	}

	public void openEditEggNameCanvas()
	{
		editEggNameCanvas.SetActive (true);
	}
		
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
    void GetcurrentTime()
    {
        if (currentTimeOff==true)
        {
            currentTimeOff = false;
            currentTime = Time.time;
        }
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


}
