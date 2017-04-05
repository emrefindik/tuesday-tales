using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class LevelControl : MonoBehaviour {

    public CoroutineResponse winCoroutineEnded;    

    public GameObject buildings;
    public GameObject[] realBuilding = new GameObject[4];
    public GameObject[] realBuildingbuildingCover = new GameObject[4];
    public GameObject[] structures = new GameObject[4];
	public GameObject winCanvas;
	public GameObject shareSucceedCanvas;
    public bool switchNow=false;
	public Texture2D barFull;
	public Texture2D barEmpty;
	public int score;

	PixelDestruction pD;
    float[] levelPositions;
	Vector2 fingerStart;
	Vector2 fingerEnd;

    // GUI
    Vector2 barPos = new Vector2(20, 40);
    Vector2 barSize = new Vector2(Screen.width-40, Screen.height/10);
    const int scoreBase = 100;

    //float[] progress;
    int[] progressCount;
    public int num_of_pieces;
    public int num_of_people;
    float progressStep;
    
    float tolerance = 300;
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
		Human
	}

    public List<Movement> movements = new List<Movement>();

    void Start() {
        initSystem();
		initLevel();
        winCoroutineEnded = new CoroutineResponse();
    }

    void initSystem()
    {
        pD = FindObjectOfType(typeof(PixelDestruction)) as PixelDestruction;

        number_of_buildings = realBuilding.Length;
        levelPositions = new float[number_of_buildings];
        //progress = new float[number_of_buildings];
        progressCount = new int[number_of_buildings];

        for (int i = 0; i < number_of_buildings; i++)
        {
            levelPositions[i] = -realBuilding[i].transform.position.x;
            //progress[i] = 1.0f;
            progressCount[i] = num_of_pieces + num_of_people;
            structures[i].GetComponent<PolygonCollider2D>().enabled = false;
        }
        progressStep = 1.0f / (num_of_pieces + num_of_people);
  
    }

	void initLevel ()
	{
		winCanvas.SetActive(false);
		shareSucceedCanvas.SetActive(false);

		score = 0;
		buildingDestroyedCount = 0;

		win = false;
		shareStatus = FacebookManager.ShareStatus.None;

		// For Jonathan's destroy city
		fallenPiecesCount = 0;
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
			GUI.BeginGroup (new Rect (barPos.x, barPos.y, barSize.x, barSize.y));
			GUI.Box (new Rect (0, 0, barSize.x, barSize.y), barEmpty);
			GUI.BeginGroup (new Rect (0, 0, barSize.x * (((float)progressCount [level]) / (num_of_people + num_of_pieces)), barSize.y));
			GUI.Box (new Rect (0, 0, barSize.x, barSize.y), barFull);
			GUI.EndGroup ();
			GUI.EndGroup ();

			GUI.skin.label.fontSize = (int)(Screen.height * 0.05);
			GUI.Label (new Rect (20, (int)(Screen.height * 0.85), Screen.width / 3, (int)(Screen.height * 0.1)), score.ToString ());
		} else {
			if(winCanvas && (winCoroutineEnded.Success == true))
				winCanvas.SetActive (true);
		}
    }

	public void increaseProgress(int amount, ScoreType type)
    {
        //progress[level] -= amount * progressStep;
        progressCount[level] -= amount;
        //Debug.Log(level);
        //Debug.Log(progress[level]);
        increaseScore(scoreBase * amount);
        //Debug.Log(progressCount[level]);
        if (progressCount[level] == 0)
        {
            structures[level].GetComponent<PolygonCollider2D>().enabled = true;
            //Debug.Log("setCollider");
            progressCount[level]--;
            buildingDestroyedCount++;
        }

		if (type == ScoreType.Building) {
			fallenPiecesCount++;
			//Debug.Log (fallenPiecesCount);
			if (fallenPiecesCount == num_of_pieces) {
				if (pA) {
					pA.level2On = true;
				}
			}
		}
    }

    public void increaseScore(int increment)
    {
        score += increment;
    }

    void Update()
    {
        // Check win
		// Test Win Status
        //if(buildingDestroyedCount == number_of_buildings)
		if(buildingDestroyedCount > number_of_buildings/30)
        {
            win = true;
            buildingDestroyedCount = -1;
			StartCoroutine(MainController.single.addDestoryCityReward(score, winCoroutineEnded));

        }


        if (Input.GetMouseButtonDown(0))
        {
            fingerStart = Input.mousePosition;
            fingerEnd = Input.mousePosition;
        }

		/*
        //GetMouseButton instead of TouchPhase.Moved
        //This returns true if the LMB is held down in standalone OR
        //there is a single finger touch on a mobile device
        if (Input.GetMouseButton(0))
        {
            fingerEnd = Input.mousePosition;
		
            //There was some movement! The tolerance variable is to detect some useful movement
            //i.e. an actual swipe rather than some jitter. This is the same as the value of 80
            //you used in your original code.
            if (Mathf.Abs(fingerEnd.x - fingerStart.x) > tolerance ||
               Mathf.Abs(fingerEnd.y - fingerStart.y) > tolerance)
            {

                //There is more movement on the X axis than the Y axis
                if (Mathf.Abs(fingerStart.x - fingerEnd.x) > Mathf.Abs(fingerStart.y - fingerEnd.y))
                {
                    //Right Swipe
                    if ((fingerEnd.x - fingerStart.x) > 0)
                        movements.Add(Movement.Right);
                    //Left Swipe
                    else
                        movements.Add(Movement.Left);
                }

                //More movement along the Y axis than the X axis
                else
                {
                    //Upward Swipe
                    if ((fingerEnd.y - fingerStart.y) > 0)
                        movements.Add(Movement.Up);
                    //Downward Swipe
                    else
                        movements.Add(Movement.Down);
                }

                //After the checks are performed, set the fingerStart & fingerEnd to be the same
                fingerStart = fingerEnd;

				/*
                //Now let's check if the Movement pattern is what we want
                //In this example, I'm checking whether the pattern is Left, then Right, then Left again
                // Debug.Log(CheckForPatternMove(0, 3, new List<Movement>() { Movement.Left, Movement.Right, Movement.Left }));
                if (CheckForPatternMove(0, 1, new List<Movement>() { Movement.Left }))
                {
                    switchNow = true;
                    if (level < 3 )
                    {
                        level = level + 1;
                    }
                    
                }

                if (CheckForPatternMove(0, 1, new List<Movement>() { Movement.Right }))
                {
                    switchNow = true;
                    if (level>0  )
                    level= level-1;
                }
				*/
            //}
        //}
		
		/*
        //GetMouseButtonUp(0) instead of TouchPhase.Ended
        if (Input.GetMouseButtonUp(0))
        {
            fingerStart = Vector2.zero;
            fingerEnd = Vector2.zero;
            movements.Clear();
        }
		*/

        if (switchNow)
        {
            goToLevel(level);
        }
        //pD.switchCity(level);
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
	}

	void SendScreenshotToFacebook()
	{
		shareStatus = FacebookManager.ShareStatus.Sending;
		FacebookManager.single.ShareScreenshotToFacebook();
	}

}
